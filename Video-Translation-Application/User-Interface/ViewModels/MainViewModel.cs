using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace User_Interface
{

    public class MainViewModel : ViewModel
    {
        private ICommand _backCommand;
        private ICommand _nextCommand;

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

        private sbyte _pageId = 0;

        public MainViewModel()
        {
            Page SpeechToTextPage = new SpeechToText_Page();
            Page TextToTextPage = new TextToTextTranslation_Page();
            Page TextToSpeechPage = new TextToSpeech_Page();
            Page SpeechToVideoPage = new SpeechToVideo_Page();

            SpeechToTextPage.DataContext = new SpeechToTextViewModel();
            TextToTextPage.DataContext = new TextToTextViewModel();
            TextToSpeechPage.DataContext = new TextToSpeechViewModel();
            SpeechToVideoPage.DataContext = new SpeechToVideoViewModel();

            _pages = new Page[]
            {
                SpeechToTextPage, TextToTextPage, TextToSpeechPage, SpeechToVideoPage,
            };
        }

        private readonly Page[] _pages;

        public sbyte Progress
        {
            get { return _pageId; }
            private set
            {
                _pageId = value;
                OnPropertyChanged(nameof(Progress));
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public Page CurrentPage
        {
            get { return _pages[_pageId]; }
        }
        public void Next()
        {
            Progress += 1;
        }

        public void Back()
        {
            Progress -= 1;
        }

        public bool CanBack()
        {
            if (Progress == 0) return false;
            else return true;
        }

        public bool CanNext()
        {
            if (Progress >= (_pages.Length - 1)) return false;
            else return true;
        }
    }
}
