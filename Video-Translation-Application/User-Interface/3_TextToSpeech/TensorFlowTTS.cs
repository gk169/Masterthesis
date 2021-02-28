using System.Collections.Generic;

namespace User_Interface
{
    public class TensorFlowTTS : TextToSpeech
    {
        public TensorFlowTTS() : base(name: nameof(TensorFlowTTS))
        {
        }

        protected override Dictionary<string, List<string>> LoadSupportedVoices()
        {
            // TODO load dynamically
            return new Dictionary<string, List<string>>()
            {
                {"de", new List<string>(){"Hans", "Lisa"}},
                {"en", new List<string>(){"John", "Megan"}},
            };
        }
    }
}
