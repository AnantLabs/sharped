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
        public static TokenDefinition[] Definitions = 
        {
            // quoted string
            new TokenDefinition("\".*?\"", FontWeights.Normal, Colors.DarkRed),
            // comment
            new TokenDefinition("//.*", FontWeights.Normal, Colors.Green)
        };
    }
}
