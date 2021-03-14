namespace VideoTranslationTool.SpeechToVideoModule
{
    /// <summary>
    /// Public abstract class <c>SpeechToVideo</c> as parent class for all SpeechToVideo modules
    /// </summary>
    public abstract class SpeechToVideo : Module
    {
        #region Constructors
        /// <summary>
        /// Constructor for <c>SpeechToVideo</c> class
        /// </summary>
        /// <param name="name">
        /// See <see cref="Module.Module(string)"/>
        /// </param>
        protected SpeechToVideo(string name) : base(name: name) { }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Public method <c>Generate</c> to generate a video with given input video and audio
        /// </summary>
        /// <param name="audioPath">
        /// Audio file path as string of a speeking person
        /// </param>
        /// <param name="videoPath">
        /// Video file path as string of a single person
        /// </param>
        /// <returns>
        /// Video file path of the generated video
        /// </returns>
        public abstract string Generate(string audioPath, string videoPath);
        #endregion Methods
    }
}
