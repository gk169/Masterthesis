using System.Windows.Controls;

namespace VideoTranslationTool.TextToTextModule
{
    /// <summary>
    /// Interaktionslogik für TextToTextView.xaml
    /// </summary>
    public partial class TextToTextView : Page
    {
        public TextToTextView()
        {
            InitializeComponent();
            DataContext = new TextToTextViewModel();
        }
    }
}
