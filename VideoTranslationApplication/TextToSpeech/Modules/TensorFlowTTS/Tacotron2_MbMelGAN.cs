using System.ComponentModel.Composition;

namespace VideoTranslationTool.TextToSpeechModule
{
    [Export(typeof(TextToSpeech))]
    /// <summary>
    /// Class <c>Tacotron2_MbMelGAN</c> for TensorFlowTTS Tacotron2 + Multiband MelGAN implementation
    /// </summary>
    public class Tacotron2_MbMelGAN : TensorFlowTTS
    {
        #region Properties
        /// <summary>
        /// See <see cref="TensorFlowTTS.SynthesizerName"/>
        /// </summary>
        protected override string SynthesizerName => "Tacotron2";

        /// <summary>
        /// See <see cref="TensorFlowTTS.VocoderName"/>
        /// </summary>
        protected override string VocoderName => "Multiband MelGAN";
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructor of <c>Tacotron2_MbMelGAN</c> class
        /// </summary>
        public Tacotron2_MbMelGAN() : base(name: nameof(Tacotron2_MbMelGAN)) { }
        #endregion Constructors
    }
}
