using System.Windows.Forms;

namespace FFRadarBuddy
{
    public class ListViewDoubleBuffered : ListView
    {
        public ListViewDoubleBuffered()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            const int WM_ERASEBKGND = 0x14;
            if (m.Msg != WM_ERASEBKGND)
            {
                base.OnNotifyMessage(m);
            }
        }
    }
}
