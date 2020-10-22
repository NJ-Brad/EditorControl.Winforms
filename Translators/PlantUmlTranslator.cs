using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translators
{
    public class PlantUmlTranslator
    {
        public string Translate(string input)
        {
            string code = Encoder.EncodeUrl(input);
            string renderUrl = $"http://plantuml.com/plantuml/png/{code}";

            return renderUrl;
        }
    }
}
