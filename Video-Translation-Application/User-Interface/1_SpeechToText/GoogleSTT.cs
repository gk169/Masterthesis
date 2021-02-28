using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User_Interface
{
    public class GoogleSTT : SpeechToText
    {
        public GoogleSTT() : base(name: nameof(GoogleSTT))
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
