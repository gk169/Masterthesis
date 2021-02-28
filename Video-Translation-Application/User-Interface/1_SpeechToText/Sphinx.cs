using System.Collections.Generic;

namespace User_Interface
{
    public class Sphinx : SpeechToText
    {
        public Sphinx() : base(name: nameof(Sphinx))
        {
        }

        protected override List<string> LoadSupportedLanguages()
        {
            //TODO load dynamically
            return new List<string>()
            {
                "de", "en",
            };
        }
    }
}
