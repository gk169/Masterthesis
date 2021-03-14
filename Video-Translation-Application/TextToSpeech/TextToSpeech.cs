using System.Collections.Generic;

namespace VideoTranslationTool.TextToSpeechModule
{
    /// <summary>
    /// Public abstract class <c>TextToSpeech</c> as parent for all TextToSpeech modules
    /// </summary>
    public abstract class TextToSpeech : Module
    {
        #region Properties
        /// <summary>
        /// Public property <c>SupportedVoices</c> to get a dictionary of language - List of supported voices
        /// </summary>
        public Dictionary<string, List<string>> SupportedVoices { get; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructor of class <c>TextToSpeech</c>
        /// </summary>
        /// <param name="name">
        /// See <see cref="Module.Module(string)"/>
        /// </param>
        protected TextToSpeech(string name) : base(name: name) => SupportedVoices = LoadSupportedVoices();
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Protected method <c>LoadSupportedVoices</c> to load the module specific supported voices
        /// </summary>
        /// <returns>
        /// Dictionary: language - list of supported voices
        /// </returns>
        protected abstract Dictionary<string, List<string>> LoadSupportedVoices();

        /// <summary>
        /// Public method <c>Synthesize</c> to synthesize a text with selected voice
        /// </summary>
        /// <param name="text">
        /// Text to be synthesized as string
        /// </param>
        /// <param name="language">
        /// Language of the synthesized audio as string
        /// </param>
        /// <param name="voice">
        /// Voice to be used for synthesization as string
        /// </param>
        /// <returns>
        /// Path of the synthesized audio file as string
        /// </returns>
        public abstract string Synthesize(string text, string language, string voice);
        #endregion Methods
    }
}
