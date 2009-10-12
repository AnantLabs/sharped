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
using Microsoft.Win32;

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

            // clear all properties (we are going to color text ourselves)
            TextRange documentRange = new TextRange(codeBox.Document.ContentStart, codeBox.Document.ContentEnd);
            documentRange.ClearAllProperties();

            // now let’s create navigator to go though the text and hightlight it
            TextPointer navigator = codeBox.Document.ContentStart;
            while (navigator.CompareTo(codeBox.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    //CheckWordsInRun((Run)navigator.Parent);
                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }

            codeBox.TextChanged -= this.codeBox_TextChanged;
            //color text
            codeBox.TextChanged += this.codeBox_TextChanged;
        }
    }
}
