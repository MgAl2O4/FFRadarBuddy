using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FFRadarBuddy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MemoryScanner memoryScanner = new MemoryScanner();
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            memoryScanner.OpenProcess("ffxiv_dx11");

            bool bHasMemPaths = true;
            if (memoryScanner.IsValid())
            {
                if (!MemoryLayout.memPathTarget.Resolve(memoryScanner))
                {
                    Logger.WriteLine("Failed to resolve mem path: Target");
                    bHasMemPaths = false;
                }

                if (!MemoryLayout.memPathCamera.Resolve(memoryScanner))
                {
                    Logger.WriteLine("Failed to resolve mem path: Camera");
                    bHasMemPaths = false;
                }

                if (!MemoryLayout.memPathActors.Resolve(memoryScanner))
                {
                    Logger.WriteLine("Failed to resolve mem path: ActorList");
                    bHasMemPaths = false;
                }
            }

            if (bHasMemPaths)
            {
                dispatcherTimer.Interval = TimeSpan.FromSeconds(3);
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                dispatcherTimer.Start();
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (memoryScanner.IsValid())
                {
                    byte[] targetInfoData = memoryScanner.ReadBytes(MemoryLayout.memPathTarget.GetResolvedAddress(), MemoryLayout.TargetConsts.Size);
                    if (targetInfoData != null)
                    {
                        MemoryLayout.TargetData targetData = new MemoryLayout.TargetData(targetInfoData);

                        byte[] targetActorData = memoryScanner.ReadBytes(targetData.CurrentAddr, MemoryLayout.ActorConsts.Size);
                        if (targetActorData != null)
                        {
                            MemoryLayout.ActorData actorData = new MemoryLayout.ActorData(targetActorData);

                            Logger.WriteLine("Target: {0} (type:{1}:{2}) pos[{3},{4},{5}], radius:{6}, actorId:{7} npcId:{8}",
                                actorData.Name, actorData.Type, actorData.SubType,
                                actorData.PosX, actorData.PosY, actorData.PosZ,
                                actorData.Radius,
                                actorData.ActorId, actorData.NpcId);

                        }
                    }

                    byte[] cameraInfoData = memoryScanner.ReadBytes(MemoryLayout.memPathCamera.GetResolvedAddress(), MemoryLayout.CameraConsts.Size);
                    if (cameraInfoData != null)
                    {
                        MemoryLayout.CameraData cameraData = new MemoryLayout.CameraData(cameraInfoData);

                        Logger.WriteLine("Camera: pos[{0},{1},{2}], view[{3},{4},{5}], fov:{6}",
                            cameraData.PosX, cameraData.PosY, cameraData.PosZ,
                            cameraData.FocusX, cameraData.FocusY, cameraData.FocusZ,
                            cameraData.Fov);
                    }

                    long TableReadAddr = MemoryLayout.memPathActors.GetResolvedAddress();
                    long TableSize = memoryScanner.ReadPointer(TableReadAddr);
                    TableReadAddr += 8;
                    for (long ActorIdx = 0; ActorIdx < TableSize; ActorIdx++, TableReadAddr += 8)
                    {
                        long ActorAddr = memoryScanner.ReadPointer(TableReadAddr);
                        if (ActorAddr != 0)
                        {
                            byte[] entryData = memoryScanner.ReadBytes(ActorAddr, MemoryLayout.ActorConsts.Size);
                            if (entryData != null)
                            {
                                MemoryLayout.ActorData entryActor = new MemoryLayout.ActorData(entryData);

                                Logger.WriteLine("[{0}] '{1}' {2}:{3}, actorId:{4}, npcId:{5}",
                                    ActorIdx,
                                    entryActor.Name, entryActor.Type, entryActor.SubType, entryActor.ActorId, entryActor.NpcId);
                            }
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Exception: " + ex);
            }
        }
    }
}
