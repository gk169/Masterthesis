using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using VideoTranslationTool.Commands;
using VideoTranslationTool.SpeechToTextModule;
using VideoTranslationTool.SpeechToVideoModule;
using VideoTranslationTool.TextToSpeechModule;
using VideoTranslationTool.TextToTextModule;

namespace VideoTranslationTool
{
    public class WorkflowViewModel : ViewModel
    {
        private enum ProgressState
        {
            Started = 0,
            Transcribed = 1,
            Translated = 2,
            Synthesized = 3,
            Generated = 4,
        };
        private enum PageState
        {
            SpeechToText = 0,
            TextToText = 1,
            TextToSpeech = 2,
            SpeechToVideo = 3,
        }
        
        private ICommand _backCommand;
        private ICommand _nextCommand;
        private ICommand _skipCommand;
        private ICommand _selectSTTCommand;
        private ICommand _selectTTTCommand;
        private ICommand _selectTTSCommand;
        private ICommand _selectSTVCommand;

        private readonly List<Page> _pages;
        private int _currentPageIndex;
        private ProgressState _currentProgressState;

        public ICommand NextCommand
        {
            get
            {
                if (_nextCommand == null) _nextCommand = new RelayCommand(param => this.Next(), param => this.CanNext());
                return _nextCommand;
            }
        }
        public ICommand BackCommand
        {
            get
            {
                if (_backCommand == null) _backCommand = new RelayCommand(param => this.Back(), param => this.CanBack());
                return _backCommand;
            }
        }
        public ICommand SkipCommand
        {
            get
            {
                if (_skipCommand == null) _skipCommand = new RelayCommand(param => this.Skip(), param => this.CanSkip());
                return _skipCommand;
            }
        }
        public ICommand SelectSTTCommand
        {
            get
            {
                if (_selectSTTCommand == null) _selectSTTCommand = new RelayCommand(param => SelectSTT(), param => CanSelectSTT());
                return _selectSTTCommand;
            }
        }
        public ICommand SelectTTTCommand
        {
            get
            {
                if (_selectTTTCommand == null) _selectTTTCommand = new RelayCommand(param => SelectTTT(), param => CanSelectTTT());
                return _selectTTTCommand;
            }
        }
        public ICommand SelectTTSCommand
        {
            get
            {
                if (_selectTTSCommand == null) _selectTTSCommand = new RelayCommand(param => SelectTTS(), param => CanSelectTTS());
                return _selectTTSCommand;
            }
        }
        public ICommand SelectSTVCommand
        {
            get
            {
                if (_selectSTVCommand == null) _selectSTVCommand = new RelayCommand(param => SelectSTV(), param => CanSelectSTV());
                return _selectSTVCommand;
            }
        }

        private void Next() => CurrentPageIndex++;
        private void Back() => CurrentPageIndex--;
        private void Skip()
        {
            CurrentPageIndex++;
        }

        private void SelectSTT() => CurrentPageIndex = (int)PageState.SpeechToText;
        private void SelectTTT() => CurrentPageIndex = (int)PageState.TextToText;
        private void SelectTTS() => CurrentPageIndex = (int)PageState.TextToSpeech;
        private void SelectSTV() => CurrentPageIndex = (int)PageState.SpeechToVideo;

        private bool CanBack() => CurrentPageIndex > 0;                                     // true is index isnot first element
        private bool CanNext() => CurrentPageIndex < (_pages.Count - 1);                    // true if index hasnt reached last element
        //                          && Module.CanNext????;
        private bool CanSkip() => CurrentPageIndex < (_pages.Count - 1);                    // true if index hasnt reached last element

        private static bool CanSelectSTT() => true;                                         // always true => STT is first element and therfore is always selectable
        private bool CanSelectTTT() => CurrentProgress >= (int)ProgressState.Transcribed;   // true if progress has reached "Transcribe" state
        private bool CanSelectTTS() => CurrentProgress >= (int)ProgressState.Translated;    // true if progress has reached "Translated" state
        private bool CanSelectSTV() => CurrentProgress >= (int)ProgressState.Synthesized;   // true if progress has reached "Synthesized" state

        public WorkflowViewModel()
        {
            _pages = new List<Page>()
            {
                new SpeechToTextView(),
                new TextToTextView(),
                new TextToSpeechView(),
                new SpeechToVideoView(),
            };
            _currentPageIndex = 0;
            _currentProgressState = ProgressState.Started;
        }

        public Page CurrentPage => _pages[CurrentPageIndex];

        private int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                _currentPageIndex = value;

                OnPropertyChanged(nameof(CurrentPage));

                OnPropertyChanged(nameof(IsSTTActive));
                OnPropertyChanged(nameof(IsTTTActive));
                OnPropertyChanged(nameof(IsTTSActive));
                OnPropertyChanged(nameof(IsSTVActive));
            }
        }

        public bool IsSTTActive => CurrentPageIndex == (int)PageState.SpeechToText;
        public bool IsTTTActive => CurrentPageIndex == (int)PageState.TextToText;
        public bool IsTTSActive => CurrentPageIndex == (int)PageState.TextToSpeech;
        public bool IsSTVActive => CurrentPageIndex == (int)PageState.SpeechToVideo;

        public int CurrentProgress
        {
            get { return (int)_currentProgressState; }
            private set
            {
                _currentProgressState = (ProgressState)value;
                OnPropertyChanged(nameof(CurrentProgress));
            }
        }
    }
}
