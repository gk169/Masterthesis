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
            MainWindow window = new()
            {
                //DataContext = new MainViewModel()
            };
            //window.Main.Content = new Start_Page();
            window.Main.Content = new WorkflowFrame();
            window.Show();
        }
    }
}
