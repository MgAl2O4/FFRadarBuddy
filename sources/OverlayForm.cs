using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Numerics;
using System.Drawing;
using System.Drawing.Imaging;

namespace FFRadarBuddy
{
    public partial class OverlayForm : Form
    {
        public GameData gameData = null;

        private Matrix4x4 viewTM = Matrix4x4.Identity;
        private Matrix4x4 projectionTM = Matrix4x4.Identity;
        private Matrix4x4 gameToScreenTM = Matrix4x4.Identity;
        private Vector3 cameraUp = new Vector3(0, 1, 0);
        private const float cameraNearPlane = 0.1f;
        private const float cameraFarPlane = 150.0f;
        private float lastFov = 0;
        private float aspectRatio = 0;

        public OverlayForm()
        {
            InitializeComponent();
        }

        public void SetScanActive(bool bActive)
        {
            timerCameraScan.Enabled = bActive;

            if (bActive)
            {
                Tick();
            }
        }

        public void Tick()
        {
            UpdateOverlayPosition();
        }

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private void OverlayForm_Load(object sender, EventArgs e)
        {
            const int WS_EX_TRANSPARENT = 0x00000020;
            const int GWL_EXSTYLE = (-20);

            IntPtr hWindow = this.Handle;
            int extendedStyle = GetWindowLong(hWindow, GWL_EXSTYLE);
            SetWindowLong(hWindow, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

        private void UpdateOverlayPosition()
        {
            Logger.WriteLine("Camera fov:{0}, pos:[{1:0.00},{2:0.00},{3:0.00}], target:[{4:0.00},{5:0.00},{6:0.00}]",
               gameData.camera.Fov,
               gameData.camera.Position.X, gameData.camera.Position.Y, gameData.camera.Position.Z,
               gameData.camera.Target.X, gameData.camera.Target.Y, gameData.camera.Target.Z);

            Process gameProcess = (gameData != null && gameData.memoryScanner != null) ? gameData.memoryScanner.GetProcess() : null;
            if (gameProcess != null && gameProcess.MainWindowHandle != IntPtr.Zero)
            {
                POINT testPt = new POINT() { X = 0, Y = 0 };
                ClientToScreen(gameProcess.MainWindowHandle, ref testPt);
                GetClientRect(gameProcess.MainWindowHandle, out RECT gameClientRect);

                Left = testPt.X;
                Top = testPt.Y;
                Width = gameClientRect.Right - gameClientRect.Left;
                Height = gameClientRect.Bottom - gameClientRect.Top;
                aspectRatio = (float)Width / (float)Height;

                projectionTM = (gameData.camera.Fov > 0) ? Matrix4x4.CreatePerspectiveFieldOfView(gameData.camera.Fov, aspectRatio, cameraNearPlane, cameraFarPlane) : Matrix4x4.Identity;
            }
        }

        private void timerCameraScan_Tick(object sender, EventArgs e)
        {
            if (gameData.UpdateCamera())
            {
                if (gameData.camera.Fov != lastFov)
                {
                    lastFov = gameData.camera.Fov;
                    projectionTM = (lastFov > 0) ? Matrix4x4.CreatePerspectiveFieldOfView(lastFov, aspectRatio, cameraNearPlane, cameraFarPlane) : Matrix4x4.Identity;
                }

                viewTM = Matrix4x4.CreateLookAt(gameData.camera.Position, gameData.camera.Target, cameraUp);
                gameToScreenTM = viewTM * projectionTM;
            }

            Invalidate();
        }

        private void OverlayForm_Paint(object sender, PaintEventArgs e)
        {
            float canvasHalfX = (float)Width / 2;
            float canvasHalfY = (float)Height / 2;
            float markerHalfSize = 3;

            foreach (GameData.ActorItem actor in gameData.listActors)
            {
                Vector3 projectedPt = Vector3.Transform(actor.Position, gameToScreenTM);
                if (projectedPt.Z > 0)
                {
                    Vector2 canvasPt = new Vector2(canvasHalfX + (projectedPt.X / projectedPt.Z * canvasHalfX), canvasHalfY - (projectedPt.Y / projectedPt.Z * canvasHalfY));

                    e.Graphics.DrawRectangle(Pens.Red, canvasPt.X - markerHalfSize, canvasPt.Y - markerHalfSize, markerHalfSize * 2, markerHalfSize * 2);
                }
            }
        }
    }
}
