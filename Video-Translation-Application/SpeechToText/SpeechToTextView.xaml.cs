using System.Windows.Controls;

namespace VideoTranslationTool.SpeechToTextModule
{
    /// <summary>
    /// Interaktionslogik für SpeechToTextView.xaml
    /// </summary>
    public partial class SpeechToTextView : Page
    {
        public SpeechToTextView()
        {
            InitializeComponent();
            DataContext = new SpeechToTextViewModel();
        }
    }
}
