using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using VideoTranslationTool.SpeechToTextModule;
using VideoTranslationTool.SpeechToVideoModule;
using VideoTranslationTool.TextToSpeechModule;
using VideoTranslationTool.TextToTextModule;

namespace VideoTranslationTool
{

    public class MainViewModel : ViewModel
    {
        public MainViewModel()
        {
            Page startPage = new Start_Page();
            Page speechToTextView = new SpeechToTextView();
            Page textToTextView = new TextToTextView();
            Page textToSpeechView = new TextToSpeechView
            {
                DataContext = new TextToSpeechViewModel()
            };
            Page speechToVideoView = new SpeechToVideoView
            {
                DataContext = new SpeechToVideoViewModel()
            };

            _pageDictionary = new Dictionary<int, Page>{
                {(int)PagesEnum.Start, startPage},
                {(int)PagesEnum.SpeechToText, speechToTextView},
                {(int)PagesEnum.TextToText, textToTextView},
                {(int)PagesEnum.TextToSpeech, textToSpeechView},
                {(int)PagesEnum.SpeechToVideo, speechToVideoView}
            };
        }

        private readonly Dictionary<int, Page> _pageDictionary;

        private enum StatesEnum
        {
            Started=0,
            Transcribed=1,
            Translated=2,
            Synthesized=3,
            Generated=4
        };

        private enum PagesEnum
        {
            Start=0,
            SpeechToText=1,
            TextToText=2,
            TextToSpeech=3,
            SpeechToVideo=4,
        };

        private StatesEnum _currentState = StatesEnum.Started;
        private PagesEnum _currentPageNumber = PagesEnum.Start;

        public int Progress
        {
            get { return (int)_currentState; }
            set
            {
                _currentState = (StatesEnum)value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        public int CurrentPageNumber
        {
            get { return (int)_currentPageNumber; }
            set
            {
                _currentPageNumber = (PagesEnum)value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public Page CurrentPage
        {
            get { return _pageDictionary[(int)CurrentPageNumber]; }
        }
    }
}
