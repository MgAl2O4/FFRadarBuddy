using System;
using System.Collections.Generic;

namespace FFRadarBuddy
{
    public class GameData
    {
        public enum ScannerState
        {
            MissingProcess,
            MissingMemPaths,
            Ready,
        }

        public class ActorItem : MemoryLayout.ActorData
        {
            public string ShowName { get { return Name; } }
            public string ShowType { get { return Type + ":" + SubType; } }
            public string ShowId { get { return "0x" + NpcId.ToString("x"); } }
            public string ShowPos { get { return string.Format("[{0:0.0}, {1:0.0}, {2:0.0}]", Position.X, Position.Y, Position.Z); } }
            public string ShowDistance { get { return Distance.ToString("0.0"); } }
            public int LastScanPass = 0;
            public float Distance = 0.0f;

            public override string ToString()
            {
                return Name + " (" + ShowType + "), id:" + ShowId + ", pos:" + ShowPos;
            }
        };

        public List<ActorItem> listActors = new List<ActorItem>();
        public MemoryLayout.CameraData camera = new MemoryLayout.CameraData();

        public delegate void ActorListChanged(List<ActorItem> addedEntries);
        public event ActorListChanged OnActorListChanged;

        public delegate void ScannerStateChanged(ScannerState newState);
        public event ScannerStateChanged OnScannerStateChanged;

        public MemoryScanner memoryScanner = new MemoryScanner();
        public ScannerState scannerState = ScannerState.MissingProcess;

        private int actorScanPass = 0;

        public void Tick()
        {
            UpdateScanner();
            UpdateActors();
        }

        private void UpdateScanner()
        {
            ScannerState newState = ScannerState.Ready;

            if (!memoryScanner.IsValid())
            {
                memoryScanner.OpenProcess("ffxiv_dx11");

                if (memoryScanner.IsValid())
                {
                    newState = ScannerState.Ready;

                    if (!MemoryLayout.memPathTarget.Resolve(memoryScanner))
                    {
                        Logger.WriteLine("Failed to resolve mem path: Target");
                        newState = ScannerState.MissingMemPaths;
                    }

                    if (!MemoryLayout.memPathCamera.Resolve(memoryScanner))
                    {
                        Logger.WriteLine("Failed to resolve mem path: Camera");
                        newState = ScannerState.MissingMemPaths;
                    }

                    if (!MemoryLayout.memPathActors.Resolve(memoryScanner))
                    {
                        Logger.WriteLine("Failed to resolve mem path: ActorList");
                        newState = ScannerState.MissingMemPaths;
                    }
                }
                else
                {
                    newState = ScannerState.MissingProcess;
                }
            }

            if (newState != scannerState)
            {
                scannerState = newState;
                OnScannerStateChanged?.Invoke(newState);
            }
        }

        private void UpdateActors()
        {
            try
            {
                List<ActorItem> addedActors = new List<ActorItem>();
                actorScanPass++;

                long TableReadAddr = MemoryLayout.memPathActors.GetResolvedAddress();
                if (TableReadAddr != 0)
                {
                    for (long ActorIdx = 0; ActorIdx < 500; ActorIdx++, TableReadAddr += 8)
                    {
                        long ActorAddr = memoryScanner.ReadPointer(TableReadAddr);
                        if (ActorAddr != 0)
                        {
                            byte[] entryData = memoryScanner.ReadBytes(ActorAddr, MemoryLayout.ActorConsts.Size);
                            if (entryData != null)
                            {
                                ActorItem foundItem = listActors.Find(x => (x.MemAddress == ActorAddr));
                                if (foundItem == null)
                                {
                                    ActorItem entryActor = new ActorItem();
                                    entryActor.Set(ActorAddr, entryData);
                                    entryActor.LastScanPass = actorScanPass;

                                    addedActors.Add(entryActor);
                                    listActors.Add(entryActor);
                                }
                                else
                                {
                                    foundItem.Set(ActorAddr, entryData);
                                    foundItem.LastScanPass = actorScanPass;
                                }
                            }
                        }
                    }
                }

                for (int Idx = listActors.Count - 1; Idx >= 0; Idx--)
                {
                    if (listActors[Idx].LastScanPass != actorScanPass)
                    {
                        listActors.RemoveAt(Idx);
                    }
                }

                OnActorListChanged?.Invoke(addedActors);
            }
            catch (Exception) { }
        }

        public bool UpdateCamera()
        {
            try
            {
                if (memoryScanner.IsValid())
                {
                    long cameraAddr = MemoryLayout.memPathCamera.GetResolvedAddress();
                    byte[] cameraInfoData = memoryScanner.ReadBytes(cameraAddr, MemoryLayout.CameraConsts.Size);
                    if (cameraInfoData != null)
                    {
                        camera.Set(cameraAddr, cameraInfoData);
                        return true;
                    }
                }
            }
            catch (Exception) { }

            return false;
        }
    }
}
