using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VideoTranslationTool.Commands;

namespace VideoTranslationTool.TextToTextModule
{
    /// <summary>
    /// Public class <c>TextToTextViewModel</c> is view model for TextToText view
    /// </summary>
    public class TextToTextViewModel : ViewModel, IModuleViewModel
    {
        #region Members
        private ICommand _exportCommand;
        private TextToText _module;
        private string _sourceLanguage;
        private string _sourceText;
        [ImportMany(typeof(TextToText))] private List<TextToText> _supportedModules;
        private string _targetLanguage;
        private string _targetText;
        private ICommand _translateCommand;
        #endregion Members

        #region Properties
        /// <summary>
        /// Public command <c>ExportCommand</c> calls export method if it is executable
        /// </summary>
        public ICommand ExportCommand
        {
            get
            {
                if (_exportCommand == null) _exportCommand = new RelayCommand(param => this.Export(), param => this.CanExport());
                return _exportCommand;
            }
        }

        /// <summary>
        /// Public property <c>Module</c> to get / set the currently used TextToText module
        /// </summary>
        public TextToText Module
        {
            get { return _module; }
            set
            {
                _module = value;
                OnPropertyChanged(nameof(Module));

                OnPropertyChanged(nameof(SupportedSourceLanguages));
                OnPropertyChanged(nameof(SourceLanguage));

                OnPropertyChanged(nameof(SupportedTargetLanguages));
                OnPropertyChanged(nameof(TargetLanguage));
            }
        }

        /// <summary>
        /// Public property <c>SourceLanguage</c> to get / set the language of the source text as string
        /// </summary>
        public string SourceLanguage
        {
            get { return _sourceLanguage; }
            set
            {
                _sourceLanguage = value;
                OnPropertyChanged(nameof(SourceLanguage));

                OnPropertyChanged(nameof(SupportedTargetLanguages));
                OnPropertyChanged(nameof(TargetLanguage));
            }
        }

        /// <summary>
        /// Public property <c>SourceText</c> to get / set the source text as string
        /// </summary>
        public string SourceText
        {
            get { return _sourceText; }
            set
            {
                _sourceText = value;
                OnPropertyChanged(nameof(SourceText));
            }
        }

        /// <summary>
        /// Public property <c>SupportedModules</c> to get a list of supported TextToText modules
        /// </summary>
        public List<TextToText> SupportedModules => _supportedModules;

        /// <summary>
        /// Public property <c>SupportedSourceLanguages</c> to get a list of supported source languages
        /// </summary>
        public List<string> SupportedSourceLanguages => _module.SupportedTranslations.Keys.ToList();

        /// <summary>
        /// Public property <c>SupportedTargetLanguages</c> to get a list of supported target languages
        /// </summary>
        public List<string> SupportedTargetLanguages => _module.SupportedTranslations[SourceLanguage];
        
        /// <summary>
        /// Public property <c>TargetLanguage</c> to get / set the language source text should be translated to as string
        /// </summary>
        public string TargetLanguage
        {
            get { return _targetLanguage; }
            set
            {
                _targetLanguage = value;
                OnPropertyChanged(nameof(TargetLanguage));
            }
        }
        
        /// <summary>
        /// Public property <c>TargetText</c> to get / set the translated text as string
        /// </summary>
        public string TargetText
        {
            get { return _targetText; }
            set
            {
                _targetText = value;
                OnPropertyChanged(nameof(TargetText));
            }
        }

        /// <summary>
        /// Public command <c>TranslateCommand</c> calls translate method if it is executable
        /// </summary>
        public ICommand TranslateCommand
        {
            get
            {
                if (_translateCommand == null) _translateCommand = new RelayCommand(param => this.Translate(), param => this.CanTranslate());
                return _translateCommand;
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructor of class <c>TextToTextViewModel</c>
        /// </summary>
        public TextToTextViewModel()
        {
            // Load TextToText module plugins
            var catalog = new DirectoryCatalog(".");
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            // Set default module
            Module = _supportedModules[0];

            // Set default languages
            if (SupportedSourceLanguages.Contains("German")) SourceLanguage = "German";
            else if (SupportedSourceLanguages.Contains("English")) SourceLanguage = "English";
            else SourceLanguage = SupportedSourceLanguages[0];

            if (SupportedTargetLanguages.Contains("English")) TargetLanguage = "English";
            else if (SupportedTargetLanguages.Contains("German")) TargetLanguage = "German";
            else TargetLanguage = SupportedTargetLanguages[0];
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Private method <c>CanExport</c> indicates if export method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private bool CanExport() => TargetText is not null and not "";

        /// <summary>
        /// See <see cref="IModuleViewModel.CanNext"/>
        /// </summary>
        /// <returns>
        /// See <see cref="IModuleViewModel.CanNext"/>
        /// </returns>
        public bool CanNext() => TargetText is not null and not "";

        /// <summary>
        /// Private method <c>CanTranslate</c> indicates if translate method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        private bool CanTranslate() => SourceText is not null and not ""
                                       && SourceLanguage is not null and not ""
                                       && TargetLanguage is not null and not "";

        /// <summary>
        /// Private method <c>Export</c> to export the translated text to a txt file
        /// </summary>
        private void Export()
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true) File.WriteAllText(saveFileDialog.FileName, TargetText);
        }

        /// <summary>
        /// Private method <c>Translate</c> to translate the source text and display the result
        /// </summary>
        private void Translate()
        {
            string text = null;

            try { text = _module.Translate(SourceText, SourceLanguage, TargetLanguage); }
            catch (Exception e) { MessageBox.Show(e.ToString()); }

            if (text != null) TargetText = text;
        }
        #endregion Methods
    }
}
