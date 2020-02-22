using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FFRadarBuddy
{
    public class MemoryScanner
    {
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [Flags]
        public enum MemoryProtectionFlags : uint
        {
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_NOACCESS = 0x1,
            PAGE_READONLY = 0x2,
            PAGE_READWRITE = 0x4,
            PAGE_WRITECOPY = 0x8,
            PAGE_TARGETS_INVALID = 0x40000000,
            PAGE_TARGETS_NO_UPDATE = 0x40000000,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400,
        }

        [Flags]
        public enum MemoryStateFlags : uint
        {
            MEM_COMMIT = 0x1000,
            MEM_FREE = 0x10000,
            MEM_RESERVE = 0x2000,
        }

        [Flags]
        public enum MemoryRegionFlags : uint
        {
            Writeable = 0x1,
            Executable = 0x2,
            MainModule = 0x4,
            All = Writeable | Executable,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public MemoryStateFlags State;
            public MemoryProtectionFlags Protect;
            public uint Type;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, [In] [Out] byte[] lpBuffer, IntPtr regionSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll")]
        private static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        public struct MemoryRegionInfo
        {
            public IntPtr BaseAddress;
            public IntPtr Size;
            public MemoryRegionFlags Type;
            public byte[] CachedData;

            public bool IsInside(long Address)
            {
                return (Address >= (long)BaseAddress) && (Address < ((long)BaseAddress + (long)Size));
            }

            public bool IsValid()
            {
                return (long)Size > 0;
            }
        }

        private string cachedProcessName;
        private Process cachedProcess;
        private IntPtr cachedProcessHandle;
        private long cachedProcessBase;
        private List<MemoryRegionInfo> memoryRegions = new List<MemoryRegionInfo>();
        private bool useCachedMemory = false;

        public void OpenProcess(string procName)
        {
            cachedProcessName = procName;
            cachedProcessHandle = IntPtr.Zero;
            memoryRegions.Clear();

            Process[] match = Process.GetProcessesByName(procName);
            cachedProcess = (match.Length > 0) ? match[0] : null;

            if (cachedProcess != null)
            {
                cachedProcessHandle = OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead, false, match[0].Id);
                InitMemoryRegions();
            }
        }

        public long GetBaseAddress()
        {
            return cachedProcessBase;
        }

        public Process GetProcess()
        {
            return cachedProcess;
        }

        public IntPtr GetProcessHandle()
        {
            return cachedProcessHandle;
        }

        public bool IsValid()
        {
            if (cachedProcess == null || cachedProcessHandle == IntPtr.Zero)
            {
                return false;
            }

            if (cachedProcess.HasExited)
            {
                cachedProcess = null;
                return false;
            }

            return true;
        }

        private void InitMemoryRegions()
        {
            cachedProcessBase = (long)cachedProcess.MainModule.BaseAddress;
            MemoryRegionInfo baseModuleInfo = new MemoryRegionInfo
            {
                BaseAddress = new IntPtr(cachedProcessBase),
                Size = new IntPtr(cachedProcess.MainModule.ModuleMemorySize),
                Type = MemoryRegionFlags.Executable | MemoryRegionFlags.Writeable | MemoryRegionFlags.MainModule,
            };

            memoryRegions.Add(baseModuleInfo);
            Logger.WriteLine("base:0x{0} [{1}], size:0x{2}", baseModuleInfo.BaseAddress.ToString("x"), cachedProcess.MainModule.ModuleName, baseModuleInfo.Size.ToString("x"));
        }

        public void SaveMemoryRegions()
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open("memory.cache", FileMode.Create)))
            {
                writer.Write(memoryRegions.Count);
                foreach (MemoryRegionInfo regInfo in memoryRegions)
                {
                    writer.Write((long)regInfo.BaseAddress);
                    writer.Write((long)regInfo.Size);
                    writer.Write((uint)regInfo.Type);

                    byte[] buffer = ReadRegionMemory(regInfo);
                    writer.Write(buffer);
                }
            }
        }

        public bool LoadMemoryRegions()
        {
            useCachedMemory = false;
            using (BinaryReader reader = new BinaryReader(File.Open("memory.cache", FileMode.Open)))
            {
                int numRegions = reader.ReadInt32();

                memoryRegions.Clear();
                for (int regIdx = 0; regIdx < numRegions; regIdx++)
                {
                    long regBaseAddr = reader.ReadInt64();
                    long regSize = reader.ReadInt64();
                    MemoryRegionFlags regType = (MemoryRegionFlags)reader.ReadUInt32();

                    if (regIdx == 0)
                    {
                        regType |= MemoryRegionFlags.MainModule;
                        cachedProcessBase = regBaseAddr;
                    }

                    MemoryRegionInfo regInfo = new MemoryRegionInfo()
                    {
                        BaseAddress = new IntPtr(regBaseAddr),
                        Size = new IntPtr(regSize),
                        Type = regType
                    };

                    regInfo.CachedData = reader.ReadBytes((int)regSize);
                    memoryRegions.Add(regInfo);
                }

                useCachedMemory = true;
            }

            return useCachedMemory;
        }

        public void CacheMemoryRegions()
        {
            memoryRegions.Clear();
            InitMemoryRegions();

            MemoryProtectionFlags MemFlagsExecutable =
                MemoryProtectionFlags.PAGE_EXECUTE |
                MemoryProtectionFlags.PAGE_EXECUTE_READ |
                MemoryProtectionFlags.PAGE_EXECUTE_READWRITE |
                MemoryProtectionFlags.PAGE_EXECUTE_WRITECOPY;

            MemoryProtectionFlags MemFlagsWriteable =
                MemoryProtectionFlags.PAGE_EXECUTE_READWRITE |
                MemoryProtectionFlags.PAGE_EXECUTE_WRITECOPY |
                MemoryProtectionFlags.PAGE_READWRITE |
                MemoryProtectionFlags.PAGE_WRITECOPY;

            int regionInfoSize = Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));

            for (long scanAddress = 0; scanAddress < Int64.MaxValue;)
            {
                MEMORY_BASIC_INFORMATION regionInfo;
                int result = VirtualQueryEx(cachedProcessHandle, (IntPtr)scanAddress, out regionInfo, (uint)regionInfoSize);
                if (result != regionInfoSize)
                {
                    break;
                }

                bool bIsCommited = (regionInfo.State & MemoryStateFlags.MEM_COMMIT) != 0;
                bool bIsGuarded = (regionInfo.Protect & MemoryProtectionFlags.PAGE_GUARD) != 0;
                bool bIsWritebale = (regionInfo.Protect & MemFlagsWriteable) != 0;
                bool bIsExecutable = (regionInfo.Protect & MemFlagsExecutable) != 0;
                bool bShouldCacheRegion = bIsCommited && !bIsGuarded && (bIsWritebale || bIsExecutable);

                Logger.WriteLine("scan:0x{0}, size:0x{1}, state:[{2}], protect:[{3}], type:0x{4} => {5}",
                    scanAddress.ToString("x"), regionInfo.RegionSize.ToString("x"), regionInfo.State, regionInfo.Protect, regionInfo.Type.ToString("x"),
                    bShouldCacheRegion ? "CACHE" : "meh");

                if (bShouldCacheRegion)
                {
                    MemoryRegionFlags storeType = (bIsWritebale ? MemoryRegionFlags.Writeable : 0) | (bIsExecutable ? MemoryRegionFlags.Executable : 0);
                    MemoryRegionInfo storeInfo = new MemoryRegionInfo
                    {
                        BaseAddress = regionInfo.BaseAddress,
                        Size = regionInfo.RegionSize,
                        Type = storeType,
                    };

                    memoryRegions.Add(storeInfo);
                }

                scanAddress = (long)regionInfo.BaseAddress + (long)regionInfo.RegionSize;
            }
        }

        public byte[] ReadRegionMemory(MemoryRegionInfo regionInfo)
        {
#if DEBUG
            if (useCachedMemory)
            {
                return regionInfo.CachedData;
            }
#endif // DEBUG

            byte[] memBuffer = new byte[(long)regionInfo.Size];
            IntPtr bytesRead = IntPtr.Zero;
            ReadProcessMemory(cachedProcessHandle, regionInfo.BaseAddress, memBuffer, regionInfo.Size, out bytesRead);

            return memBuffer;
        }

        public byte[] ReadBytes(long Address, int Size)
        {
            byte[] memBuffer = new byte[Size];
#if DEBUG
            if (useCachedMemory)
            {
                MemoryRegionInfo regInfo = FindMemoryRegion(Address);
                long copyEndAddr = Math.Min((long)regInfo.BaseAddress + (long)regInfo.Size, Address + Size);
                long copySize = copyEndAddr - Address;
                if (copySize > 0)
                {
                    Array.Copy(regInfo.CachedData, Address - (long)regInfo.BaseAddress, memBuffer, 0, copySize);
                }

                return memBuffer;
            }
#endif // DEBUG

            IntPtr bytesRead = IntPtr.Zero;
            ReadProcessMemory(cachedProcessHandle, new IntPtr(Address), memBuffer, new IntPtr(Size), out bytesRead);

            return memBuffer;
        }

        public int ReadInt(long Address)
        {
            byte[] memBuffer = ReadBytes(Address, 4);
            return BitConverter.ToInt32(memBuffer, 0);
        }

        public long ReadPointer(long Address)
        {
            byte[] memBuffer = ReadBytes(Address, 8);
            return BitConverter.ToInt64(memBuffer, 0);
        }

        public MemoryRegionInfo FindMemoryRegion(long Address)
        {
            foreach (MemoryRegionInfo regInfo in memoryRegions)
            {
                if (regInfo.IsInside(Address))
                {
                    return regInfo;
                }                
            }

            return new MemoryRegionInfo() { BaseAddress = new IntPtr(0), Size = new IntPtr(0) };
        }

        public long FindPatternInBuffer(byte[] buffer, byte[] patternBytes, byte[] patternMask, long startIndex = 0)
        {
            long matchOffset = 0;

            long maxOffsetToCheck = buffer.Length - patternBytes.Length;
            for (long offset = startIndex; offset < maxOffsetToCheck; offset++)
            {
                if (buffer[offset] == patternBytes[0])
                {
                    matchOffset = offset;
                    for (int patternIdx = 0; patternIdx < patternBytes.Length; patternIdx++)
                    {
                        if ((buffer[offset + patternIdx] != patternBytes[patternIdx]) && (patternMask == null || patternMask[patternIdx] == 1))
                        {
                            matchOffset = 0;
                            break;
                        }
                    }

                    if (matchOffset != 0)
                    {
                        break;
                    }
                }
            }

            return matchOffset;
        }

        public long FindPatternInMemory(byte[] patternBytes, byte[] patternMask, MemoryRegionFlags regionFlags)
        {
            foreach (MemoryRegionInfo regionInfo in memoryRegions)
            {
                if ((regionInfo.Type & regionFlags) != 0)
                {
                    byte[] regionMemory = ReadRegionMemory(regionInfo);
                    if (regionMemory != null)
                    {
                        long matchOffset = FindPatternInBuffer(regionMemory, patternBytes, patternMask);
                        if (matchOffset != 0)
                        {
                            matchOffset += (long)regionInfo.BaseAddress;
                            return matchOffset;
                        }
                    }
                }
            }

            return 0;
        }

        public List<long> FindPatternInMemoryAll(byte[] patternBytes, byte[] patternMask, MemoryRegionFlags regionFlags)
        {
            List<long> results = new List<long>();

            foreach (MemoryRegionInfo regionInfo in memoryRegions)
            {
                if ((regionInfo.Type & regionFlags) != 0)
                {
                    byte[] regionMemory = ReadRegionMemory(regionInfo);
                    if (regionMemory != null)
                    {
                        long matchOffset = FindPatternInBuffer(regionMemory, patternBytes, patternMask);
                        while (matchOffset != 0)
                        {
                            long matchAddr = matchOffset + (long)regionInfo.BaseAddress;
                            results.Add(matchAddr);

                            matchOffset = FindPatternInBuffer(regionMemory, patternBytes, patternMask, matchOffset + patternBytes.Length);
                        }
                    }
                }
            }

            return results;
        }
    }
}
