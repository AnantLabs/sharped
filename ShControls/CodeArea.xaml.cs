using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;

namespace ShControls
{
    /// <summary>
    /// Interaction logic for CodeArea.xaml
    /// </summary>
    public partial class CodeArea : UserControl
    {
        /// <summary>
        /// file loaded in editor
        /// </summary>
        private string _filename = "";

        private CsharpSyntaxProvider _syntaxProvider = new CsharpSyntaxProvider();

        public CodeArea()
        {
            InitializeComponent();
        }

        public string Text
        {
            get
            {
                // use a TextRange to fish out the Text from the Document
                var textRange = new TextRange(codeBox.Document.ContentStart,
                                              codeBox.Document.ContentEnd);
                return textRange.Text;
            }
            set
            {
                codeBox.Document.Blocks.Clear();
                var paragraph = new Paragraph(new Run(value));
                codeBox.Document.Blocks.Add(paragraph);
            }
        }

        public void LoadFromFile(string filename)
        {
            var documentTextRange = new TextRange(codeBox.Document.ContentStart,
                                                  codeBox.Document.ContentEnd);
            using (FileStream fs = File.Open(filename, FileMode.Open))
            {
                documentTextRange.Load(fs, DataFormats.Text);
            }
            HighlightAsNeeded();
        }

        public void Save()
        {
            if (_filename == "")
            {
                // open saveas dialog
                return;
            }
            SaveToFile(_filename);
        }


        private void codeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (codeBox.Document == null)
                return;

            // Paragraph.Tag flags in the document to indicates whether a paragraph needs re-highlighting
            // NULL (the default) indicates it DOES need highlighting
            foreach (TextChange c in e.Changes)
            {
                TextPointer p = codeBox.Document.ContentStart.GetPositionAtOffset(c.Offset);
                if (p.Paragraph != null)
                    p.Paragraph.Tag = null; //It needs reformatting
            }

            codeBox.TextChanged -= codeBox_TextChanged;
            HighlightAsNeeded();
            codeBox.TextChanged += codeBox_TextChanged;
        }

        private void HighlightAsNeeded()
        {
            if (codeBox.Document.Blocks.Count == 0)
            {
                return;
            }

            Block B = codeBox.Document.Blocks.FirstBlock;
            while (B != null)
            {
                if (B is Paragraph)
                {
                    var P = B as Paragraph;
                    if (P.Tag == null)
                    {
                        Highlight(P);
                        P.Tag = true; //It has been highlighted
                    }
                }
                else
                    throw new Exception("Unknown block type " + B.ToString());
                B = B.NextBlock;
            }
        }

        private void Highlight(Paragraph P)
        {
            if (P.Inlines.FirstInline == null)
                return;
            Inline inline = P.Inlines.FirstInline;
            var run = (Run) inline;

            var rangeToHighlight = new TextRange(P.Inlines.FirstInline.ElementStart,
                                                 P.Inlines.LastInline.ElementEnd);
            rangeToHighlight.ClearAllProperties();

            foreach (TokenDefinition comment in _syntaxProvider.Comments)
            {
                var reToken = new Regex(comment.Regexp, RegexOptions.Multiline);
                MatchCollection matches = reToken.Matches(rangeToHighlight.Text);
                foreach (Match m in matches)
                {
                    int startIndex = m.Groups[1].Index; // highlight first groupn in ()
                    int endIndex = m.Groups[1].Index + m.Groups[1].Length;
                    TextPointer start = rangeToHighlight.Start.GetPositionAtOffset(startIndex);
                    TextPointer end = rangeToHighlight.Start.GetPositionAtOffset(endIndex);
                    var tokenRange = new TextRange(start, end);
                    tokenRange.ApplyPropertyValue(TextElement.ForegroundProperty,
                                         new SolidColorBrush(comment.ForegroundColor));
                    tokenRange.ApplyPropertyValue(TextElement.FontWeightProperty,
                                                 comment.FontWeight);
                }
                if (matches.Count > 0)
                    return;
            }

            // since highlighting brokes offset pointing, store ranges
            List<Token> tokens = new List<Token>();
            foreach (TokenDefinition definition in _syntaxProvider.Definitions)
            {
                var reToken = new Regex(definition.Regexp, RegexOptions.Multiline);
                MatchCollection matches = reToken.Matches(rangeToHighlight.Text);
                foreach (Match m in matches)
                {
                    int startIndex = m.Groups[1].Index; // highlight first groupn in ()
                    int endIndex = m.Groups[1].Index + m.Groups[1].Length;
                    TextPointer start = rangeToHighlight.Start.GetPositionAtOffset(startIndex);
                    TextPointer end = rangeToHighlight.Start.GetPositionAtOffset(endIndex);
                    var tokenRange = new TextRange(start, end);
                    tokens.Add(new Token(definition, tokenRange));
                }
            }

            // do actual highlighting work
            foreach (var token in tokens)
            {
                token.Highlight();
            }
        }

        public void Clear()
        {
            codeBox.Document.Blocks.Clear();
        }

        public void SaveToFile(string filename)
        {
            var documentTextRange = new TextRange(codeBox.Document.ContentStart,
                                                  codeBox.Document.ContentEnd);
            using (FileStream fs = File.Create(filename))
            {
                documentTextRange.Save(fs, DataFormats.Text);
            }
        }

        private void codeBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                // Get the nearest TextPointer to the mouse position.
                TextPointer location = codeBox.GetPositionFromPoint(Mouse.GetPosition(codeBox), true);
                // Get the nearest word using this TextPointer.
                TextRange word = WordBreaker.GetWordRange(location);
                // Show dialog and rename word.Text document wide
            }
        }

        public void ShowReplacePanel()
        {
            throw new NotImplementedException();
        }
    }
}