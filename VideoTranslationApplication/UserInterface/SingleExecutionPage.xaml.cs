using System.Windows;
using System.Windows.Controls;

namespace VideoTranslationTool
{
    /// <summary>
    /// Interaktionslogik für SingleExecutionPage.xaml
    /// </summary>
    public partial class SingleExecutionPage : Page
    {
        public SingleExecutionPage()
        {
            InitializeComponent();
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            // Start Overview
            MainWindow window = (MainWindow)MainWindow.GetWindow(this);
            window.Main.Content = new OverviewPage();
        }
    }
}
