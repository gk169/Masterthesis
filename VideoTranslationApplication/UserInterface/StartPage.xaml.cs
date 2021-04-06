using System.Windows;
using System.Windows.Controls;

namespace VideoTranslationTool
{
    /// <summary>
    /// Interaktionslogik für StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Start Overview
            MainWindow window = (MainWindow)MainWindow.GetWindow(this);
            window.Main.Content = new OverviewPage();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            // Quit Application
            Application.Current.Shutdown();
        }
    }
}
