using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;

namespace ShControls
{
    internal struct TokenDefinition
    {
        public FontWeight FontWeight;
        public Color ForegroundColor;
        public string Regexp;

        internal TokenDefinition(string regexp, FontWeight fontWeight, Color foregroundColor)
        {
            Regexp = regexp;
            FontWeight = fontWeight;
            ForegroundColor = foregroundColor;
        }
    }

    internal struct Token
    {
        private TokenDefinition _definition;
        private TextRange _tokenRange;
        public Token(TokenDefinition d, TextRange range)
        {
            _definition = d;
            _tokenRange = range;
        }
        public void Highlight()
        {
            _tokenRange.ApplyPropertyValue(TextElement.ForegroundProperty,
                                         new SolidColorBrush(_definition.ForegroundColor));
            _tokenRange.ApplyPropertyValue(TextElement.FontWeightProperty,
                                         _definition.FontWeight);
        }
    }

    internal class CsharpSyntaxProvider
    {
        internal List<TokenDefinition> Definitions = new List<TokenDefinition>();
        internal List<TokenDefinition> Comments = new List<TokenDefinition>();

        internal CsharpSyntaxProvider()
        {
            // quoted string
            Comments.Add(new TokenDefinition("(\".*?\")", FontWeights.Normal, Colors.DarkRed));
            // formal documentation comment
            Comments.Add(new TokenDefinition("(///.*)", FontWeights.Normal, Colors.Gray));
            // comment
            Comments.Add(new TokenDefinition("(//.*)", FontWeights.Normal, Colors.Green));

            //Regular expression for language-specific syntax
            //such as keywords, operators, namespaces, classes,
            //and functions.
            string[] keywords = {
                                    "using", "public", "protected", "private", "string",
                                    "void", "namespace", "internal", "struct", "new", "abstract", "in",
                                    "foreach", "for", "null", "get", "set", "return", "if", "while", "do",
                                    "class", "var", "static", "readonly", "true", "false", "partial",
                                    "typeof", "int", "bool", "byte", "value", "object"
                                };
            foreach (string keyword in keywords)
            {
                Definitions.Add(
                    new TokenDefinition(
                        string.Format("(^|[^A-Za-z])({0})([^A-Za-z]|$)", keyword), 
                        FontWeights.Normal, Colors.Blue)
                        );
            }
        }

        public int HIGHLIGHT_GROUP_INDEX = 2;
    }
}