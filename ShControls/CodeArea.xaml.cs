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
            using (StreamReader streamReader = File.OpenText(filename))
            {
                Text = streamReader.ReadToEnd();
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

            using (StreamWriter streamWriter = new StreamWriter(_filename))
            {
                streamWriter.Write(Text);
            }
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
            throw new NotImplementedException();
        }
    }
}
