using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ShControls
{
    internal struct TokenDefinition
    {
        internal TokenDefinition(string regexp, FontWeight fontWeight, Color foregroundColor)
        {
            Regexp = regexp;
            FontWeight = fontWeight;
            ForegroundColor = foregroundColor;
        }
        public string Regexp;
        public FontWeight FontWeight;
        public Color ForegroundColor;
    }

    class CsharpSyntaxProvider
    {
        internal List<TokenDefinition> Definitions = new List<TokenDefinition>();

        internal CsharpSyntaxProvider()
        {
            
            // quoted string
            Definitions.Add(new TokenDefinition("\".*?\"", FontWeights.Normal, Colors.DarkRed));
            // formal documentation comment
            Definitions.Add(new TokenDefinition("///.*", FontWeights.Normal, Colors.Gray));
            // comment
            Definitions.Add(new TokenDefinition("//.*", FontWeights.Normal, Colors.Green));

            //Regular expression for language-specific syntax
            //such as keywords, operators, namespaces, classes,
            //and functions.
            string[] keywords = { "using", "public", "protected", "private", "string", 
                "void", "namespace", "internal", "struct", "new", "abstract", "in", 
                "foreach", "for", "null", "get", "set", "return", "if", "while", "do"
            };
            foreach (string keyword in keywords)
            {
                Definitions.Add(new TokenDefinition(keyword+"[^:alpha:]", FontWeights.Normal, Colors.Blue) );
            }
        }
    }
}
