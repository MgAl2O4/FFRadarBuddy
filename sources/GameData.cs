using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

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

        public class OverlaySettings
        {
            public enum DisplayMode
            {
                Never,
                WhenLookingAt,
                WhenClose,
                WhenCloseAndLookingAt,
                Always,
            }

            public bool IsHighlighted = false;
            public DisplayMode Mode = DisplayMode.WhenLookingAt;
            public Pen DrawPen = Pens.Black;
            public string Description;

            public void SetDefault(string Name)
            {
                Mode = DisplayMode.WhenClose;
                DrawPen = Pens.Gray;
                Description = Name;
            }

            public override string ToString()
            {
                return Description + ", mode:" + Mode + (IsHighlighted ? ", Highlighted!" : "");
            }
        }

        public class ActorItem : MemoryLayout.ActorData
        {
            public string ShowName { get { return Name; } }
            public string ShowType { get { return Type.ToString(); } }
            public string ShowId { get { return "0x" + NpcId.ToString("x"); } }
            public string ShowActorId { get { return string.Format("{0:x}:{1:x}", ActorIdA, ActorIdB); } }
            public string ShowPos { get { return string.Format("[{0:0.0}, {1:0.0}, {2:0.0}]", Position.X, Position.Y, Position.Z); } }
            public string ShowDistance { get { return Distance.ToString("0.0"); } }
            public int LastScanPass = 0;
            public float Distance = 0.0f;
            public OverlaySettings OverlaySettings = new OverlaySettings();

            public override string ToString()
            {
                return Name + " (" + ShowType + "), id:" + ShowId + ", pos:" + ShowPos;
            }
        };

        public List<ActorItem> listActors = new List<ActorItem>();
        public MemoryLayout.CameraData camera = new MemoryLayout.CameraData();

        public delegate void ActorListChanged();
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
            if (scannerState != ScannerState.Ready)
            {
                return;
            }

            try
            {
                actorScanPass++;
                bool hasChanges = false;

                long TableReadAddr = MemoryLayout.memPathActors.GetResolvedAddress();
                if (TableReadAddr != 0)
                {
                    List<long> KnownAddr = new List<long>();
                    for (long ActorIdx = 0; ActorIdx < 500; ActorIdx++, TableReadAddr += 8)
                    {
                        long ActorAddr = memoryScanner.ReadPointer(TableReadAddr);
                        if (ActorAddr != 0 && !KnownAddr.Contains(ActorAddr))
                        {
                            byte[] entryData = memoryScanner.ReadBytes(ActorAddr, MemoryLayout.ActorConsts.Size);
                            if (entryData != null)
                            {
                                KnownAddr.Add(ActorAddr);

                                ActorItem entryActor = new ActorItem();
                                entryActor.SetIdOnly(entryData);

                                bool bFound = false;
                                for (int ExistingIdx = 0; ExistingIdx < listActors.Count; ExistingIdx++)
                                {
                                    if (listActors[ExistingIdx].UniqueId == entryActor.UniqueId)
                                    {
                                        entryActor = listActors[ExistingIdx];
                                        bFound = true;
                                    }
                                }

                                if (!bFound)
                                {
                                    listActors.Add(entryActor);
                                    hasChanges = true;
                                }

                                entryActor.SetDataOnly(entryData);
                                entryActor.LastScanPass = actorScanPass;
                                entryActor.Distance = Vector3.Distance(entryActor.Position, camera.Position);
                            }
                        }
                    }
                }

                for (int Idx = listActors.Count - 1; Idx >= 0; Idx--)
                {
                    if (listActors[Idx].LastScanPass != actorScanPass)
                    {
                        listActors.RemoveAt(Idx);
                        hasChanges = true;
                    }
                }

                if (hasChanges)
                {
                    OnActorListChanged?.Invoke();
                }
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
                        camera.Set(cameraInfoData);
                        return true;
                    }
                }
            }
            catch (Exception) { }

            return false;
        }
    }
}
