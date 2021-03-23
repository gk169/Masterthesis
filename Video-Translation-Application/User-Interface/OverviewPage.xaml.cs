using System.Windows;
using System.Windows.Controls;
using VideoTranslationTool.SpeechToTextModule;
using VideoTranslationTool.SpeechToVideoModule;
using VideoTranslationTool.TextToSpeechModule;
using VideoTranslationTool.TextToTextModule;

namespace VideoTranslationTool
{
    /// <summary>
    /// Interaktionslogik für OverviewPage.xaml
    /// </summary>
    public partial class OverviewPage : Page
    {
        public OverviewPage()
        {
            InitializeComponent();
        }

        private void WorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            // Start workflow
            MainWindow window = (MainWindow)MainWindow.GetWindow(this);
            window.Main.Content = new WorkflowPage();
        }

        private void SpeechToTextButton_Click(object sender, RoutedEventArgs e)
        {
            // Start SpeechToText
            MainWindow window = (MainWindow)MainWindow.GetWindow(this);
            SingleExecutionPage singleExecutionPage = new();
            singleExecutionPage.MainFrame.Content = new SpeechToTextView();
            window.Main.Content = singleExecutionPage;
        }

        private void TextToTextButton_Click(object sender, RoutedEventArgs e)
        {
            // Start TextToText
            MainWindow window = (MainWindow)MainWindow.GetWindow(this);
            SingleExecutionPage singleExecutionPage = new();
            singleExecutionPage.MainFrame.Content = new TextToTextView();
            window.Main.Content = singleExecutionPage;
        }

        private void TextToSpeechButton_Click(object sender, RoutedEventArgs e)
        {
            // Start TextToSpeech
            MainWindow window = (MainWindow)MainWindow.GetWindow(this);
            SingleExecutionPage singleExecutionPage = new();
            singleExecutionPage.MainFrame.Content = new TextToSpeechView();
            window.Main.Content = singleExecutionPage;
        }

        private void SpeechToVideoButton_Click(object sender, RoutedEventArgs e)
        {
            // Start SpeechToVideo
            MainWindow window = (MainWindow)MainWindow.GetWindow(this);
            SingleExecutionPage singleExecutionPage = new();
            singleExecutionPage.MainFrame.Content = new SpeechToVideoView();
            window.Main.Content = singleExecutionPage;
        }
    }
}
