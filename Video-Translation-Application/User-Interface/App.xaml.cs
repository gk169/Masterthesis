using System.Windows;

namespace VideoTranslationTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow window = new();
            window.Main.Content = new StartPage();
            window.Show();
        }
    }
}
