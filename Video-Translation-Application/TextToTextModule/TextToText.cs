using System.Collections.Generic;

namespace VideoTranslationTool.TextToTextModule
{
    /// <summary>
    /// Public abstract class <c>TextToText</c> as parent class for all TextToText modules
    /// </summary>
    public abstract class TextToText : Module
    {
        #region Properties
        /// <summary>
        /// Public property <c>SupportedTranslations</c> to get a dictionary of language - possible translation languages
        /// </summary>
        public Dictionary<string, List<string>> SupportedTranslations { get; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructor of class <c>TextToText</c>
        /// </summary>
        /// <param name="name">
        /// See <see cref="Module.Module(string)"/>
        /// </param>
        protected TextToText(string name) : base(name: name) => SupportedTranslations = LoadSupportedTranslations();
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Protected method <c>LoadSupportedTranslations</c> to load module specific supported translation dictionary
        /// </summary>
        /// <returns>
        /// Dictionary: source language - List of target languages
        /// </returns>
        protected abstract Dictionary<string, List<string>> LoadSupportedTranslations();

        /// <summary>
        /// Public method <c>Translate</c> to translate text to one language to another
        /// </summary>
        /// <param name="text">
        /// Text to be translated as string
        /// </param>
        /// <param name="sourceLanguage">
        /// Language of the text as string
        /// </param>
        /// <param name="targetLanguage">
        /// Language the text should be translated to as string
        /// </param>
        /// <returns></returns>
        public abstract string Translate(string text, string sourceLanguage, string targetLanguage);
        #endregion Methods
    }
}
