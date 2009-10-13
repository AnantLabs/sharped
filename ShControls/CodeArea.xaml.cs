using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;

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
        string _filename = "";

        public CodeArea()
        {
            InitializeComponent();
        }

        public void LoadFromFile(string filename)
        {
            TextRange documentTextRange = new TextRange( codeBox.Document.ContentStart, 
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

        public string Text
        {
            get
            {
                // use a TextRange to fish out the Text from the Document
                TextRange textRange = new TextRange(codeBox.Document.ContentStart,
                    codeBox.Document.ContentEnd);
                return textRange.Text;
            }
            set
            {
                codeBox.Document.Blocks.Clear();
                Paragraph paragraph = new Paragraph(new Run(value));
                codeBox.Document.Blocks.Add(paragraph);
            }
        }


        private void codeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (codeBox.Document == null)
                return;

            // Paragraph.Tag flags in the document to indicates whether a paragraph needs re-highlighting
            // NULL (the default) indicates it DOES need highlighting
            foreach (TextChange C in e.Changes)
            {
                TextPointer P = codeBox.Document.ContentStart.GetPositionAtOffset(C.Offset);
                if (P.Paragraph != null) 
                    P.Paragraph.Tag = true; //It needs reformatting
            }

            HighlightAsNeeded();
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
                    Paragraph P = B as Paragraph;
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

            Run run = (Run)P.Inlines.FirstInline;
            (new TextRange(run.ElementStart, run.ElementEnd)).ClearAllProperties();

            TextRange rangeToHighlight = new TextRange(run.ContentStart, run.ContentEnd);
            string quotedStr = "\".*?\"";
            Regex reQuotedString = new Regex(quotedStr, RegexOptions.Multiline);
            MatchCollection quotedStrings = reQuotedString.Matches(run.Text);
            foreach (Match m in quotedStrings)
            {
                TextPointer start = run.ElementStart.GetPositionAtOffset(m.Index + 1);
                TextPointer end = run.ElementStart.GetPositionAtOffset(m.Index + m.Length + 1);
                TextRange token = new TextRange(start, end);
                token.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.DarkRed));
            }
        }

        public void Clear()
        {
            codeBox.Document.Blocks.Clear();
        }

        public void SaveToFile(string filename)
        {
            TextRange documentTextRange = new TextRange(codeBox.Document.ContentStart,
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
                // Show dialog and rename word.Text documetn wide
                //word.Text;
            }
        }
    }
}
