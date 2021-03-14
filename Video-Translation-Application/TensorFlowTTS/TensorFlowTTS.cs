using System.Collections.Generic;

namespace VideoTranslationTool.TextToSpeechModule
{
    public class TensorFlowTTS : TextToSpeech
    {
        public TensorFlowTTS() : base(name: nameof(TensorFlowTTS)) { }

        public override string Synthesize(string text, string language, string voice)
        {
            throw new System.NotImplementedException();
        }

        protected override Dictionary<string, List<string>> LoadSupportedVoices()
        {
            throw new System.NotImplementedException();
        }
    }
}
