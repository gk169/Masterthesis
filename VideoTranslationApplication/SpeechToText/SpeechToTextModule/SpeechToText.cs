using System.Collections.Generic;

namespace VideoTranslationTool.SpeechToTextModule
{
    /// <summary>
    /// Public abstract class <c>SpeechToText</c> as parent class for all SpeechToText modules
    /// </summary>
    public abstract class SpeechToText : Module
    {
        #region Properties
        /// <summary>
        /// Public property <c>SupportedAudioLanguages</c> to get a string list of all supported audio languages
        /// </summary>
        public List<string> SupportedAudioLanguages { get; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructor for <c>SpeechToText</c> class
        /// </summary>
        /// <param name="name">
        /// See <see cref="Module.Module(string)"/>
        /// </param>
        protected SpeechToText(string name) : base(name: name) => SupportedAudioLanguages = LoadSupportedAudioLanguages();
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Protected method <c>LoadSupportedAudioLanguages</c> to load supported audio languages specific by module
        /// </summary>
        /// <returns>
        /// Sting list of supported audio languages
        /// </returns>
        protected abstract List<string> LoadSupportedAudioLanguages();

        /// <summary>
        /// Method <c>Transcribe</c> transcribes the audio at given path and returns the text
        /// </summary>
        /// <param name="audioPath">
        /// Loaction of the audio file to be transcribed
        /// </param>
        /// <param name="audioLanguage">
        /// Language of the given audio file
        /// </param>
        /// <returns>
        /// Transcribed text of the audio as string
        /// </returns>
        public abstract string Transcribe(string audioPath, string audioLanguage);
        #endregion Methods
    }
}
