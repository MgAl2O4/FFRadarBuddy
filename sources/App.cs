using System;

namespace FFRadarBuddy
{
    public class App : System.Windows.Application
    {
        [STAThread]
        public static void Main(string[] Args)
        {
            Logger.Initialize(Args);

            App app = new App
            {
                StartupUri = new System.Uri("MainWindow.xaml", System.UriKind.Relative)
            };
            app.Run();
        }
    }
}
