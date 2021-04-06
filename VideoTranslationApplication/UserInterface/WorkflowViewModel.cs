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
    /// <summary>
    /// Public class <c>WorkflowViewModel</c> is ViewModel of the video translation workflow process
    /// </summary>
    public class WorkflowViewModel : ViewModel
    {
        #region Enums
        private enum PageState
        {
            SpeechToText = 0,
            TextToText = 1,
            TextToSpeech = 2,
            SpeechToVideo = 3,
        }
        #endregion Enums

        #region Members
        private ICommand _backCommand;
        private int _currentPageIndex;
        private ICommand _nextCommand;
        private readonly List<Page> _pages;
        private ICommand _selectSTTCommand;
        private ICommand _selectSTVCommand;
        private ICommand _selectTTSCommand;
        private ICommand _selectTTTCommand;
        private ICommand _skipCommand;
        private int _workflowProgress;
        #endregion Members

        #region Properties
        /// <summary>
        /// Public command <c>BackCommand</c> calls back method if it is executable
        /// </summary>
        public ICommand BackCommand
        {
            get
            {
                if (_backCommand is null) _backCommand = new RelayCommand(param => this.Back(), param => CanBack());
                return _backCommand;
            }
        }

        /// <summary>
        /// Public property <c>CurrentPage</c> to get the current page
        /// </summary>
        public Page CurrentPage => _pages[CurrentPageIndex];

        /// <summary>
        /// Private property <c>CurrentPageIndex</c> to get / set the current page index
        /// </summary>
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

                OnPropertyChanged(nameof(ShowBack));
                OnPropertyChanged(nameof(ShowNext));
                OnPropertyChanged(nameof(ShowSkip));
            }
        }
        
        /// <summary>
        /// Public property <c>IsSTTActive</c> to detect if current page is STT page
        /// </summary>
        public bool IsSTTActive => CurrentPageIndex == (int)PageState.SpeechToText;

        /// <summary>
        /// Public property <c>IsSTVActive</c> to detect if current page is STV page
        /// </summary>
        public bool IsSTVActive => CurrentPageIndex == (int)PageState.SpeechToVideo;

        /// <summary>
        /// Public property <c>IsTTSActive</c> to detect if current page is TTS page
        /// </summary>
        public bool IsTTSActive => CurrentPageIndex == (int)PageState.TextToSpeech;

        /// <summary>
        /// Public property <c>IsTTTActive</c> to detect if current page is TTT page
        /// </summary>
        public bool IsTTTActive => CurrentPageIndex == (int)PageState.TextToText;

        /// <summary>
        /// Public command <c>NextCommand</c> calls next method if it is executable
        /// </summary>
        public ICommand NextCommand
        {
            get
            {
                if (_nextCommand == null) _nextCommand = new RelayCommand(param => this.Next(), param => this.CanNext());
                return _nextCommand;
            }
        }

        /// <summary>
        /// Public command <c>SelectSTTCommand</c> calls SelectSTT method if it is executable
        /// </summary>
        public ICommand SelectSTTCommand
        {
            get
            {
                if (_selectSTTCommand == null) _selectSTTCommand = new RelayCommand(param => SelectSTT(), param => CanSelectSTT());
                return _selectSTTCommand;
            }
        }

        /// <summary>
        /// Public command <c>SelectSTVCommand</c> calls SelectSTV method if it is executable
        /// </summary>
        public ICommand SelectSTVCommand
        {
            get
            {
                if (_selectSTVCommand == null) _selectSTVCommand = new RelayCommand(param => SelectSTV(), param => CanSelectSTV());
                return _selectSTVCommand;
            }
        }

        /// <summary>
        /// Public command <c>SelectTTSCommand</c> calls SelectTTS method if it is executable
        /// </summary>
        public ICommand SelectTTSCommand
        {
            get
            {
                if (_selectTTSCommand == null) _selectTTSCommand = new RelayCommand(param => SelectTTS(), param => CanSelectTTS());
                return _selectTTSCommand;
            }
        }

        /// <summary>
        /// Public command <c>SelectTTTCommand</c> calls SelectTTT method if it is executable
        /// </summary>
        public ICommand SelectTTTCommand
        {
            get
            {
                if (_selectTTTCommand == null) _selectTTTCommand = new RelayCommand(param => SelectTTT(), param => CanSelectTTT());
                return _selectTTTCommand;
            }
        }

        /// <summary>
        /// Public property <c>ShowBack</c> to detect if Back-Button shall be visible
        /// </summary>
        public bool ShowBack => CurrentPageIndex > 0;                   // true if index is higher than first

        /// <summary>
        /// Public property <c>ShowNext</c> to detect if Next-Button shall be visible
        /// </summary>
        public bool ShowNext => CurrentPageIndex < (_pages.Count - 1);  // true if index hasnt reached last element

        /// <summary>
        /// Public property <c>ShowSkip</c> to detect if Skip-Button shall be visible
        /// </summary>
        public bool ShowSkip => CurrentPageIndex < (_pages.Count - 1);  // true if index hasnt reached last element

        /// <summary>
        /// Public command <c>SkipCommand</c> calls Skip method if it is executable
        /// </summary>
        public ICommand SkipCommand
        {
            get
            {
                if (_skipCommand is null) _skipCommand = new RelayCommand(param => this.Skip(), param => CanSkip());
                return _skipCommand;
            }
        }

        /// <summary>
        /// Private property <c>CurrentPageIndex</c> to get / private set the current workflow progress
        /// </summary>
        public int WorkflowProgress
        {
            get { return _workflowProgress; }
            private set
            {
                _workflowProgress = value;
                OnPropertyChanged(nameof(WorkflowProgress));
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructor of <c>WorkflowViewModel</c> class
        /// </summary>
        public WorkflowViewModel()
        {
            // Create pages in correct order
            _pages = new List<Page>()
            {
                new SpeechToTextView(),
                new TextToTextView(),
                new TextToSpeechView(),
                new SpeechToVideoView(),
            };

            // Set start page index
            _currentPageIndex = (int)PageState.SpeechToText;

            // Set start workflow progress value
            _workflowProgress = _currentPageIndex;
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Private method <c>Back</c> to change to previous page in workflow
        /// </summary>
        private void Back() => CurrentPageIndex--;

        /// <summary>
        /// Private method <c>CanBack</c> indicates if Back method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private static bool CanBack() => true;                                              // always possible if visible

        /// <summary>
        /// Private method <c>CanNext</c> indicates if Next method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private bool CanNext()
        {
            // Check if ModuleViewModel is ready to next
            ModuleViewModel viewModel = (ModuleViewModel)CurrentPage.DataContext;
            return viewModel.CanNext();
        }

        /// <summary>
        /// Private method <c>CanSelectSTT</c> indicates if SelectSTT method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private bool CanSelectSTT() => WorkflowProgress >= (int)PageState.SpeechToText;     // true if progress has reached "Started" state

        /// <summary>
        /// Private method <c>CanSelectSTV</c> indicates if SelectSTV method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private bool CanSelectSTV() => WorkflowProgress >= (int)PageState.SpeechToVideo;    // true if progress has reached "Synthesized" state

        /// <summary>
        /// Private method <c>CanSelectTTS</c> indicates if SelectTTS method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private bool CanSelectTTS() => WorkflowProgress >= (int)PageState.TextToSpeech;     // true if progress has reached "Translated" state

        /// <summary>
        /// Private method <c>CanSelectTTT</c> indicates if SelectTTT method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private bool CanSelectTTT() => WorkflowProgress >= (int)PageState.TextToText;       // true if progress has reached "Transcribe" state

        /// <summary>
        /// Private method <c>CanSkip</c> indicates if Skip method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private static bool CanSkip() => true;                                              // always skippable when skip is visible

        /// <summary>
        /// Private method <c>Next</c> to change to next page in workflow
        /// </summary>
        private void Next()
        {
            // Get setting of current step
            ModuleViewModel beforeViewModel = (ModuleViewModel)CurrentPage.DataContext;
            string transfer = beforeViewModel.GetResultforNextStep();

            // Change to next step
            CurrentPageIndex++;
            if (CurrentPageIndex > WorkflowProgress) WorkflowProgress = CurrentPageIndex;

            // set setting for next step
            ModuleViewModel afterViewModel = (ModuleViewModel)CurrentPage.DataContext;
            afterViewModel.SetResultOfPreviousStep(transfer);
        }

        /// <summary>
        /// Private method <c>SelectSTT</c> selects STT page as current page
        /// </summary>
        private void SelectSTT() => CurrentPageIndex = (int)PageState.SpeechToText;

        /// <summary>
        /// Private method <c>SelectSTV</c> selects STV page as current page
        /// </summary>
        private void SelectSTV() => CurrentPageIndex = (int)PageState.SpeechToVideo;

        /// <summary>
        /// Private method <c>SelectTTS</c> selects TTS page as current page
        /// </summary>
        private void SelectTTS() => CurrentPageIndex = (int)PageState.TextToSpeech;

        /// <summary>
        /// Private method <c>SelectTTT</c> selects TTT page as current page
        /// </summary>
        private void SelectTTT() => CurrentPageIndex = (int)PageState.TextToText;

        /// <summary>
        /// Private method <c>Skip</c> to skip to next page in workflow
        /// </summary>
        private void Skip()
        {
            CurrentPageIndex++;
            if (CurrentPageIndex > WorkflowProgress) WorkflowProgress = CurrentPageIndex;
        }
        #endregion Methods
    }
}
