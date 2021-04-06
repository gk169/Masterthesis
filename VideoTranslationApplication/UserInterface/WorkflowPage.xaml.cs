using System.Windows.Controls;

namespace VideoTranslationTool
{
    /// <summary>
    /// Interaktionslogik für WorkflowPage.xaml
    /// </summary>
    public partial class WorkflowPage : Page
    {
        public WorkflowPage()
        {
            InitializeComponent();
            DataContext = new WorkflowViewModel();
        }

        private void ReturnButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Start Overview
            MainWindow window = (MainWindow)MainWindow.GetWindow(this);
            window.Main.Content = new OverviewPage();
        }
    }
}
