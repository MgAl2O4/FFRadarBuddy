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

        private Bitmap overlayImage = null;
        private Font labelFont = null;
        private Font labelFontLarge = null;
        private Brush labelBackgroundBrush = new SolidBrush(Color.FromArgb(64, Color.Black));
        private Brush labelForegroundBrush = new SolidBrush(Color.FromArgb(192, Color.White));
        private float highlightAnimAlpha = 0;

        private float maxProjectedDistFromCenterSq = 0.25f;
        private float maxDistanceFromCamera = 100.0f;

        public OverlayForm()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            PlayerSettings settings = PlayerSettings.Get();
            maxProjectedDistFromCenterSq = settings.MaxDistanceFromCenter * settings.MaxDistanceFromCenter;
            maxDistanceFromCamera = settings.MaxDistanceFromCamera;

            labelFont = new Font(FontFamily.GenericSansSerif, settings.FontSize);
            labelFontLarge = new Font(FontFamily.GenericSansSerif, settings.FontSize + 2.0f, FontStyle.Bold);
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

        private void UpdateOverlayPosition()
        {
            Process gameProcess = (gameData != null && gameData.memoryScanner != null) ? gameData.memoryScanner.GetProcess() : null;
            if (gameProcess != null && gameProcess.MainWindowHandle != IntPtr.Zero)
            {
                POINT testPt = new POINT() { X = 0, Y = 0 };
                ClientToScreen(gameProcess.MainWindowHandle, ref testPt);
                GetClientRect(gameProcess.MainWindowHandle, out RECT gameClientRect);

                Left = testPt.X;
                Top = testPt.Y;
                int NewWidth = gameClientRect.Right - gameClientRect.Left;
                int NewHeight = gameClientRect.Bottom - gameClientRect.Top;
                if (NewWidth != Width || NewHeight != Height)
                {
                    Width = NewWidth;
                    Height = NewHeight;
                    aspectRatio = (float)Width / (float)Height;
                    projectionTM = (gameData.camera.Fov > 0) ? Matrix4x4.CreatePerspectiveFieldOfView(gameData.camera.Fov, aspectRatio, cameraNearPlane, cameraFarPlane) : Matrix4x4.Identity;
                    overlayImage = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
                }
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

            highlightAnimAlpha = (highlightAnimAlpha >= 1.0f) ? 0.0f : (highlightAnimAlpha + 0.05f);
            UpdateOverlayBitmap();
        }

        private bool CanShowActor(GameData.ActorItem actor, Vector3 projectedPt)
        {
            bool canShow = false;
            if (projectedPt.Z > 0)
            {
                if (actor.OverlaySettings.Mode == GameData.OverlaySettings.DisplayMode.Always || actor.OverlaySettings.IsHighlighted)
                {
                    canShow = true;
                }
                else if (actor.OverlaySettings.Mode == GameData.OverlaySettings.DisplayMode.WhenClose)
                {
                    canShow = (actor.Distance < maxDistanceFromCamera);
                }
                else if (actor.OverlaySettings.Mode == GameData.OverlaySettings.DisplayMode.WhenLookingAt || actor.OverlaySettings.Mode == GameData.OverlaySettings.DisplayMode.WhenCloseAndLookingAt)
                {
                    float distFromCenterSq = ((projectedPt.X / projectedPt.Z) * (projectedPt.X / projectedPt.Z)) + ((projectedPt.Y / projectedPt.Z) * (projectedPt.Y / projectedPt.Z));
                    if (distFromCenterSq < maxProjectedDistFromCenterSq)
                    {
                        if (actor.OverlaySettings.Mode == GameData.OverlaySettings.DisplayMode.WhenLookingAt)
                        {
                            canShow = true;
                        }
                        else
                        {
                            canShow = (actor.Distance < maxDistanceFromCamera);
                        }
                    }
                }

                if (actor.IsHidden && !actor.OverlaySettings.IsHighlighted)
                {
                    canShow = false;
                }
            }
            else
            {
                if (actor.OverlaySettings.IsHighlighted)
                {
                    canShow = true;
                    projectedPt.Z = -projectedPt.Z;
                }
            }

            return canShow;
        }

        private void DrawActorLabelSimple(GameData.ActorItem actor, Vector2 canvasPt, Graphics graphics, float markerRadius, Font useFont)
        {
            SizeF drawTextSize = graphics.MeasureString(actor.OverlaySettings.Description, useFont);
            float textPosX = canvasPt.X - (drawTextSize.Width * 0.5f);
            float textPosY = canvasPt.Y + markerRadius + 2;

            graphics.FillRectangle(labelBackgroundBrush, textPosX - 2, textPosY, drawTextSize.Width + 4, drawTextSize.Height);
            graphics.DrawString(actor.OverlaySettings.Description, useFont, labelForegroundBrush, textPosX, textPosY);
        }

        private void DrawActorLabelFancy(GameData.ActorItem actor, Vector2 canvasPt, Graphics graphics, float markerRadius, Font useFont)
        {
            const float markerOffset = 5;

            SizeF drawTextSize = graphics.MeasureString(actor.OverlaySettings.Description, useFont);
            float textPosX = canvasPt.X + (markerOffset * 2);
            float anchorX0 = canvasPt.X + markerRadius - 1;
            float anchorX1 = textPosX - 2;
            float anchorX2 = textPosX + drawTextSize.Width + 2;

            if (anchorX2 > Width)
            {
                textPosX = canvasPt.X - drawTextSize.Width - (markerOffset * 2);
                anchorX2 = textPosX - 2;
                anchorX1 = textPosX + drawTextSize.Width + 2;
                anchorX0 = canvasPt.X - markerRadius + 1;
            }

            float textPosY = canvasPt.Y - markerOffset - drawTextSize.Height;
            float anchorY = textPosY + drawTextSize.Height;
            float anchorY0 = canvasPt.Y - markerRadius + 1;

            if (textPosY < 0)
            {
                textPosY = canvasPt.Y + markerOffset;
                anchorY = textPosY;
                anchorY0 = canvasPt.Y + markerRadius - 1;
            }

            graphics.FillRectangle(labelBackgroundBrush, textPosX - 2, textPosY, drawTextSize.Width + 4, drawTextSize.Height);
            graphics.DrawString(actor.OverlaySettings.Description, useFont, labelForegroundBrush, textPosX, textPosY);
            graphics.DrawLine(actor.OverlaySettings.DrawPen, anchorX0, anchorY0, anchorX1, anchorY);
            graphics.DrawLine(actor.OverlaySettings.DrawPen, anchorX1, anchorY, anchorX2, anchorY);
        }

        private void DrawOverlay(Graphics graphics)
        {
            float canvasHalfX = (float)Width / 2;
            float canvasHalfY = (float)Height / 2;
            const float markerRadius = 3;

            foreach (GameData.ActorItem actor in gameData.listActors)
            {
                Vector3 projectedPt = Vector3.Transform(actor.Position, gameToScreenTM);
                bool bCanShow = CanShowActor(actor, projectedPt);
                if (bCanShow)
                {
                    Vector2 canvasPt = new Vector2(canvasHalfX + (projectedPt.X / projectedPt.Z * canvasHalfX), canvasHalfY - (projectedPt.Y / projectedPt.Z * canvasHalfY));
                    canvasPt.X = Math.Min(Width, Math.Max(0, canvasPt.X));
                    canvasPt.Y = Math.Min(Height, Math.Max(0, canvasPt.Y));

                    graphics.DrawEllipse(actor.OverlaySettings.DrawPen, canvasPt.X - markerRadius, canvasPt.Y - markerRadius, markerRadius * 2, markerRadius * 2);
                    if (actor.OverlaySettings.IsHighlighted)
                    {
                        float highlightRadius = markerRadius + (highlightAnimAlpha * 50.0f);
                        using (Pen highlightPen = new Pen(Color.FromArgb((int)(255 * (1.0f - highlightAnimAlpha)), actor.OverlaySettings.DrawPen.Color)))
                        {
                            graphics.DrawEllipse(highlightPen, canvasPt.X - highlightRadius, canvasPt.Y - highlightRadius, highlightRadius * 2, highlightRadius * 2);
                        }

                        DrawActorLabelFancy(actor, canvasPt, graphics, markerRadius, labelFontLarge);
                    }
                    else
                    {
                        DrawActorLabelSimple(actor, canvasPt, graphics, markerRadius, labelFont);
                    }
                }
            }
        }

        private void UpdateOverlayBitmap()
        {
            if (overlayImage == null)
            {
                return;
            }

            using (Graphics graphics = Graphics.FromImage(overlayImage))
            {
                graphics.Clear(Color.Transparent);
                DrawOverlay(graphics);
            }

            IntPtr screenDC = GetDC(IntPtr.Zero);
            IntPtr overlayDC = CreateCompatibleDC(screenDC);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr hBitmapPrev = IntPtr.Zero;

            try
            {
                hBitmap = overlayImage.GetHbitmap(Color.FromArgb(0));
                hBitmapPrev = SelectObject(overlayDC, hBitmap);

                SIZE windowSize = new SIZE() { Width = overlayImage.Width, Heigth = overlayImage.Height };
                POINT windowsPos = new POINT() { X = Left, Y = Top };
                POINT overlayOrigin = new POINT() { X = 0, Y = 0 };

                const int AC_SRC_OVER = 0x0;
                const int AC_SRC_ALPHA = 0x1;
                BLENDFUNCTION blendFunc = new BLENDFUNCTION() { BlendOp = AC_SRC_OVER, BlendFlags = 0, SourceConstantAlpha = 255, AlphaFormat = AC_SRC_ALPHA };

                const int ULW_ALPHA = 0x2;
                UpdateLayeredWindow(Handle, screenDC, ref windowsPos, ref windowSize, overlayDC, ref overlayOrigin, 0, ref blendFunc, ULW_ALPHA);
            }
            finally
            {
                ReleaseDC(IntPtr.Zero, screenDC);
                if (hBitmap != IntPtr.Zero)
                {
                    SelectObject(overlayDC, hBitmapPrev);
                    DeleteObject(hBitmap);
                }
                DeleteDC(overlayDC);
            }
        }

        #region Native Stuff

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

        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
        	public int Width;
            public int Heigth;
     	}

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool UpdateLayeredWindow(IntPtr hWnd, IntPtr hdcDest, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_LAYERED = 0x00080000;
                const int WS_EX_TRANSPARENT = 0x00000020;
                const int WS_EX_TOOLWINDOW = 0x00000080;

                CreateParams windowParams = base.CreateParams;
                windowParams.ExStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW;
                return windowParams;
            }
        }

        #endregion
    }
}
