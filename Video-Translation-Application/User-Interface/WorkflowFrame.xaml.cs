using System.Windows.Controls;

namespace VideoTranslationTool
{
    /// <summary>
    /// Interaktionslogik für WorkflowFrame.xaml
    /// </summary>
    public partial class WorkflowFrame : Page
    {
        public WorkflowFrame()
        {
            InitializeComponent();
            DataContext = new WorkflowViewModel();
        }
    }
}
