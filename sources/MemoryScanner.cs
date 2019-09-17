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
        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool ReadProcessMemory(IntPtr processHandle, IntPtr lpBaseAddress, [In] [Out] byte[] lpBuffer, IntPtr regionSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        private struct MemoryRegionInfo
        {
            public IntPtr BaseAddress;
            public IntPtr Size;
        }

        private string cachedProcessName;
        private Process cachedProcess;
        private long cachedProcessBase;
        private List<MemoryRegionInfo> memoryRegions = new List<MemoryRegionInfo>();

        public void OpenProcess(string procName)
        {
            cachedProcessName = procName;

            Process[] match = Process.GetProcessesByName(procName);
            cachedProcess = (match.Length > 0) ? match[0] : null;

            memoryRegions.Clear();
            if (cachedProcess != null && cachedProcess.Handle != IntPtr.Zero)
            {
                UpdateMemoryRegions();
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

        public bool IsValid()
        {
            if (cachedProcess == null)
            {
                return false;
            }

            if (cachedProcess.HasExited || cachedProcess.Handle == IntPtr.Zero)
            {
                cachedProcess = null;
                return false;
            }

            return true;
        }

        private void UpdateMemoryRegions()
        {
            cachedProcessBase = (long)cachedProcess.MainModule.BaseAddress;
            MemoryRegionInfo baseModuleInfo = new MemoryRegionInfo
            {
                BaseAddress = new IntPtr(cachedProcessBase),
                Size = new IntPtr(cachedProcess.MainModule.ModuleMemorySize)
            };

            memoryRegions.Add(baseModuleInfo);
            //Console.WriteLine("base:0x" + baseModuleInfo.BaseAddress.ToString("x") + " [" + cachedProcess.MainModule.ModuleName + "], size:0x" + baseModuleInfo.Size.ToString("x"));

            bool bScanMemoryPages = false;
            if (bScanMemoryPages)
            {
                int regionInfoSize = Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));

                const uint MemProtection_PAGE_EXECUTE_READWRITE = 0x40;
                const uint MemProtection_PAGE_EXECUTE_WRITECOPY = 0x80;
                const uint MemProtection_PAGE_READWRITE = 0x04;
                const uint MemProtection_PAGE_PAGE_WRITECOPY = 0x08;
                const uint MemProtection_PAGE_GUARD = 0x100;
                const uint MemProtection_AllWriteable = MemProtection_PAGE_EXECUTE_READWRITE | MemProtection_PAGE_EXECUTE_WRITECOPY | MemProtection_PAGE_READWRITE | MemProtection_PAGE_PAGE_WRITECOPY;
                const uint MemState_MEM_COMMIT = 0x1000;

                for (long scanAddress = 0; scanAddress < Int64.MaxValue;)
                {
                    MEMORY_BASIC_INFORMATION regionInfo;
                    int result = VirtualQueryEx(cachedProcess.Handle, (IntPtr)scanAddress, out regionInfo, (uint)regionInfoSize);
                    if (result != regionInfoSize)
                    {
                        break;
                    }

                    bool bShouldCacheRegion =
                        ((regionInfo.State & MemState_MEM_COMMIT) != 0) &&
                        ((regionInfo.Protect & MemProtection_AllWriteable) != 0) &&
                        ((regionInfo.Protect & MemProtection_PAGE_GUARD) == 0);

                    //Console.WriteLine("scan:0x" + scanAddress.ToString("x") + ", state:0x" + regionInfo.State.ToString("x") + ", protect:0x" + regionInfo.Protect.ToString("x") + ", size:0x" + regionInfo.RegionSize.ToString("x"));
                    if (bShouldCacheRegion)
                    {
                        MemoryRegionInfo storeInfo = new MemoryRegionInfo
                        {
                            BaseAddress = regionInfo.BaseAddress,
                            Size = regionInfo.RegionSize
                        };

                        memoryRegions.Add(storeInfo);
                    }

                    scanAddress = (long)regionInfo.BaseAddress + (long)regionInfo.RegionSize;
                }
            }
        }

        private byte[] ReadRegionMemory(MemoryRegionInfo regionInfo)
        {
            byte[] memBuffer = new byte[(long)regionInfo.Size];
            IntPtr bytesRead = IntPtr.Zero;
            ReadProcessMemory(cachedProcess.Handle, regionInfo.BaseAddress, memBuffer, regionInfo.Size, out bytesRead);

            return memBuffer;
        }

        public byte[] ReadBytes(long Address, int Size)
        {
            byte[] memBuffer = new byte[Size];
            IntPtr bytesRead = IntPtr.Zero;
            ReadProcessMemory(cachedProcess.Handle, new IntPtr(Address), memBuffer, new IntPtr(Size), out bytesRead);

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

        public long FindPatternMatch(byte[] buffer, byte[] patternBytes, byte[] patternMask)
        {
            long matchOffset = 0;

            long maxOffsetToCheck = buffer.Length - patternBytes.Length;
            for (long offset = 0; offset < maxOffsetToCheck; offset++)
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

        public long FindPatternMatchFull(byte[] patternBytes, byte[] patternMask)
        {
            foreach (MemoryRegionInfo regionInfo in memoryRegions)
            {
                byte[] regionMemory = ReadRegionMemory(regionInfo);
                if (regionMemory != null)
                {
                    long matchOffset = FindPatternMatch(regionMemory, patternBytes, patternMask);
                    if (matchOffset != 0)
                    {
                        matchOffset += (long)regionInfo.BaseAddress;
                        return matchOffset;
                    }
                }
            }

            return 0;
        }
    }
}
