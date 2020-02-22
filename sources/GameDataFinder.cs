using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRadarBuddy
{
#if DEBUG
    public class GameDataFinder
    {
        public MemoryScanner memoryScanner = null;

        public void FindActorList()
        {
            Logger.WriteLine("FindActorList: loading stored memory regions...");
            if (memoryScanner == null || !memoryScanner.LoadMemoryRegions())
            {
                Logger.WriteLine(">> failed! Make sure to create snapshot first with [F11]");
                return;
            }

            List<long> listEntryAddr = FindActorEntries();
            if (listEntryAddr == null)
            {
                Logger.WriteLine(">> failed! Can't find actor entries, make sure you're in Inn Room");
                return;
            }

            List<long> listLists = FindActorLists(listEntryAddr);

            Logger.WriteLine("Scanning for LEA opcodes...");
            MemoryScanner.MemoryRegionInfo baseRegion = memoryScanner.FindMemoryRegion(memoryScanner.GetBaseAddress());
            byte[] baseRegionMem = memoryScanner.ReadRegionMemory(baseRegion);

            foreach (long addr in listLists)
            {
                for (int idx = 0; idx < 3; idx++)
                {
                    long useAddr = addr - (idx * 8);
                    long relativeAddr = useAddr - memoryScanner.GetBaseAddress();
                    int opCodePos = FindLEA(relativeAddr, baseRegionMem);

                    if (opCodePos >= 0)
                    {
                        string byteDesc = GetOpCodeDesc(baseRegionMem, opCodePos, 32);
                        Logger.WriteLine("  0x{0} (game+0x{1}) => {2}", useAddr.ToString("x"), relativeAddr.ToString("x"), opCodePos);
                        Logger.WriteLine("    " + byteDesc);
                        break;
                    }
                }
            }

            Logger.WriteLine("Finished");
        }

        private bool IsActorValid(MemoryLayout.ActorData actorOb)
        {
            const int MaxCoordValue = 10000;
            const int MaxRadiusValue = 1000;

            if ((byte)actorOb.Type >= 15)
            {
                return false;
            }

            if (actorOb.Radius < 0 || actorOb.Radius > MaxRadiusValue)
            {
                return false;
            }

            if (actorOb.Position.X < -MaxCoordValue && actorOb.Position.X > MaxCoordValue &&
                actorOb.Position.Y < -MaxCoordValue && actorOb.Position.Y > MaxCoordValue &&
                actorOb.Position.Z < -MaxCoordValue && actorOb.Position.Z > MaxCoordValue)
            {
                return false;
            }

            return true;
        }

        private List<long> FindActorEntries()
        {
            string[] expectedActorNames = new string[] { "Summoning Bell", "Crystal Bell", "Glamour Dresser", "Toy Chest", "The Unending Journey" };
            Logger.WriteLine("Scanning for {0} name patterns...", expectedActorNames.Length);

            Dictionary<string, List<long>> mapNameOffsets = new Dictionary<string, List<long>>();
            {
                object lockOb = new object();
                Parallel.ForEach(expectedActorNames, expectedName =>
                //foreach (string expectedName in expectedActorNames)
                {
                    byte[] namePattern = Encoding.UTF8.GetBytes(expectedName);

                    List<long> results = memoryScanner.FindPatternInMemoryAll(namePattern, null, MemoryScanner.MemoryRegionFlags.Writeable);
                    lock (lockOb)
                    {
                        mapNameOffsets.Add(expectedName, results);
                    }
                });
            }

            List<long> resultAddr = new List<long>();
            foreach (var kvp in mapNameOffsets)
            {
                Logger.WriteLine("Matches for: '{0}'", kvp.Key);
                for (int addrIdx = kvp.Value.Count - 1; addrIdx >= 0; addrIdx--)
                {
                    bool validActor = false;
                    long testActorAddr = kvp.Value[addrIdx] - MemoryLayout.ActorConsts.Name;
                    if (testActorAddr >= 0)
                    {
                        MemoryLayout.ActorData testActorOb = new MemoryLayout.ActorData();
                        byte[] memBuffer = memoryScanner.ReadBytes(testActorAddr, MemoryLayout.ActorConsts.Size);
                        testActorOb.Set(memBuffer);

                        if (testActorOb.Type == MemoryLayout.ActorType.Interaction)
                        {
                            validActor = IsActorValid(testActorOb);
                        }

                        string debugStr = !validActor ? "" :
                            string.Format(": name:{0}, type:{1}:{2}, id:{3}, radius:{4}, pos:({5},{6},{7})",
                                testActorOb.Name, testActorOb.Type, testActorOb.SubType, testActorOb.NpcId,
                                testActorOb.Radius, testActorOb.Position.X, testActorOb.Position.Y, testActorOb.Position.Z);

                        Logger.WriteLine("  0x{0} => {1}{2}", testActorAddr.ToString("x"), validActor ? "OK" : "meh", debugStr);
                    }

                    if (validActor)
                    {
                        resultAddr.Add(testActorAddr);
                    }
                    else
                    {
                        kvp.Value.RemoveAt(addrIdx);
                    }
                }

                if (kvp.Value.Count != 1)
                {
                    return null;
                }
            }

            return resultAddr;
        }
    
        private List<long> FindActorLists(List<long> listEntryAddr)
        {
            const int MaxListSize = 1024;

            Logger.WriteLine("Scanning for actor entry pointers...");
            Dictionary<long, List<long>> mapEntryPointers = new Dictionary<long, List<long>>();
            if (true)
            {
                object lockOb = new object();
                Parallel.ForEach(listEntryAddr, destAddr =>
                {
                    byte[] pointerPattern = BitConverter.GetBytes(destAddr);

                    List<long> results = memoryScanner.FindPatternInMemoryAll(pointerPattern, null, MemoryScanner.MemoryRegionFlags.MainModule);
                    lock (lockOb)
                    {
                        mapEntryPointers.Add(destAddr, results);
                    }
                });
            }

            foreach (var kvp in mapEntryPointers)
            {
                if (kvp.Value.Count == 0)
                {
                    Logger.WriteLine("Failed to find pointers to: 0x{0}", kvp.Key.ToString("x"));
                    return null;
                }
            }

            List<long> listHeaders = new List<long>();

            int numEntry0 = mapEntryPointers[listEntryAddr[0]].Count;
            for (int listIdx = 0; listIdx < numEntry0; listIdx++)
            {
                List<long> addrSet = new List<long>();
                addrSet.Add(mapEntryPointers[listEntryAddr[0]][listIdx]);

                for (int otherIdx = 1; otherIdx < listEntryAddr.Count; otherIdx++)
                {
                    long testAddr = 0;
                    if (FindAddressInRange(addrSet, mapEntryPointers[listEntryAddr[otherIdx]], MaxListSize * 8, out testAddr))
                    {
                        addrSet.Add(testAddr);
                        addrSet.Sort();
                    }
                }

                if (addrSet.Count == listEntryAddr.Count)
                {
                    long listAddr = FindActorListHeader(addrSet[0], listEntryAddr[0], MaxListSize);
                    listHeaders.Add(listAddr);
                }
            }

            return listHeaders;
        }

        private bool FindAddressInRange(List<long> sourceAddr, List<long> potentialAddr, long maxAddrDiff, out long foundAddr)
        {
            // sourceAddr is sorted, find closest match in potentialAddr, up to maxAddrDiff in either direction
            long bestDist = -1;
            foundAddr = 0;

            for (int idx = 0; idx < potentialAddr.Count; idx++)
            {
                long testDiff = Math.Abs(potentialAddr[idx] - sourceAddr[0]);
                if (testDiff < maxAddrDiff)
                {
                    bestDist = testDiff;
                    foundAddr = potentialAddr[idx];
                }
            }

            return (bestDist > 0);
        }

        private long FindActorListHeader(long testAddr, long testEntryAddr, int maxListSize)
        {
            List<MemoryLayout.ActorData> actors = new List<MemoryLayout.ActorData>();
            MemoryScanner.MemoryRegionInfo regInfoEntry = memoryScanner.FindMemoryRegion(testEntryAddr);
            if (!regInfoEntry.IsValid())
            {
                return 0;
            }

            byte[] dataBeforeHeader = null;

            long listAddr = testAddr;
            for (int prevIdx = 1; prevIdx < maxListSize; prevIdx++)
            {
                long prevAddrPtr = testAddr - (prevIdx * 8);
                long prevEntryAddr = memoryScanner.ReadPointer(prevAddrPtr);

                if (regInfoEntry.IsInside(prevEntryAddr))
                {
                    byte[] buffer = memoryScanner.ReadBytes(prevEntryAddr, MemoryLayout.ActorConsts.Size);
                    MemoryLayout.ActorData actorOb = new MemoryLayout.ActorData();
                    actorOb.Set(buffer);

                    if (IsActorValid(actorOb))
                    {
                        listAddr = prevAddrPtr;
                        actors.Insert(0, actorOb);
                        continue;
                    }
                }
                else
                {
                    dataBeforeHeader = memoryScanner.ReadBytes(prevAddrPtr - 8, 16);
                }

                break;
            }            

            if (listAddr > 0)
            {
                for (int nextIdx = 0; nextIdx < maxListSize; nextIdx++)
                {
                    long nextAddrPtr = testAddr - (nextIdx * 8);
                    long nextEntryAddr = memoryScanner.ReadPointer(nextAddrPtr);

                    if (regInfoEntry.IsInside(nextEntryAddr))
                    {
                        byte[] buffer = memoryScanner.ReadBytes(nextEntryAddr, MemoryLayout.ActorConsts.Size);
                        MemoryLayout.ActorData actorOb = new MemoryLayout.ActorData();
                        actorOb.Set(buffer);

                        if (IsActorValid(actorOb))
                        {
                            actors.Add(actorOb);
                            continue;
                        }
                    }

                    break;
                }

                Logger.WriteLine("Exploring actor list at 0x{0} (game+0x{1}): {2} actors",
                    listAddr.ToString("x"),
                    (listAddr - memoryScanner.GetBaseAddress()).ToString("x"),
                    actors.Count);

                foreach (MemoryLayout.ActorData actorOb in actors)
                {
                    Logger.WriteLine("  name:{0}, type:{1}:{2}, id:{3}, radius:{4}, pos:({5},{6},{7})",
                        actorOb.Name, actorOb.Type, actorOb.SubType, actorOb.NpcId,
                        actorOb.Radius, actorOb.Position.X, actorOb.Position.Y, actorOb.Position.Z);
                }

                string debugStr = "";
                foreach (byte num in dataBeforeHeader)
                {
                    if (debugStr.Length > 0) { debugStr += ", "; }
                    debugStr += num.ToString("x");
                }

                Logger.WriteLine("  before header: {0}, {1} uint64 | {2}, {3}, {4}, {5} int32 => ({6})",
                    BitConverter.ToUInt64(dataBeforeHeader, 0).ToString("x"),
                    BitConverter.ToUInt64(dataBeforeHeader, 8).ToString("x"),
                    BitConverter.ToInt32(dataBeforeHeader, 0),
                    BitConverter.ToInt32(dataBeforeHeader, 4),
                    BitConverter.ToInt32(dataBeforeHeader, 8),
                    BitConverter.ToInt32(dataBeforeHeader, 12),
                    debugStr);
            }

            return listAddr;
        }

        private int FindLEA(long relativeAddr, byte[] buffer)
        {
            for (int idx = 0; idx < buffer.Length - 5; idx++)
            {
                if (buffer[idx] == 0x8d)
                {
                    long currentAddr = idx + 1 + 4; // 1 reg opcode, 4 addr bytes
                    // TODO: validate register?

                    long jumpAddr = BitConverter.ToUInt32(buffer, idx + 2);
                    long testAddr = currentAddr + jumpAddr;
                    if (relativeAddr == testAddr)
                    {
                        return idx + 1;
                    }
                }
            }

            return -1;
        }

        private string GetOpCodeDesc(byte[] buffer, int index, int bytesBefore)
        {
            string desc = "";
            int startPos = Math.Max(0, index - bytesBefore);
            int endPos = index + 4;
            for (int idx = startPos; idx < index; idx++)
            {
                desc += buffer[idx].ToString("x");
            }

            return desc;
        }
    }
#endif
}
