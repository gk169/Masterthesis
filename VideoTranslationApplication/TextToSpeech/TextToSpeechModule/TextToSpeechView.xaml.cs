using System.Windows.Controls;

namespace VideoTranslationTool.TextToSpeechModule
{
    /// <summary>
    /// Interaktionslogik für TextToSpeechView.xaml
    /// </summary>
    public partial class TextToSpeechView : Page
    {
        public TextToSpeechView()
        {
            InitializeComponent();
            DataContext = new TextToSpeechViewModel();
        }
    }
}
