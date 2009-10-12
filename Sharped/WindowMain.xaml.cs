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

namespace Sharped
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        public WindowMain()
        {
            InitializeComponent();
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextInput.Document == null)
                return;

            // clear all properties (we are going to color text ourselves)
            TextRange documentRange = new TextRange(TextInput.Document.ContentStart, TextInput.Document.ContentEnd);
            documentRange.ClearAllProperties();

            // now let’s create navigator to go though the text and hightlight it
            TextPointer navigator = TextInput.Document.ContentStart;
            while (navigator.CompareTo(TextInput.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    //CheckWordsInRun((Run)navigator.Parent);
                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }

            //color text
            TextInput.TextChanged -= this.RichTextBox_TextChanged;
            TextInput.TextChanged += this.RichTextBox_TextChanged;

        }


        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "C# source files (*.cs)|*.cs|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                LoadTextFile(TextInput, dlg.FileName);
            }
        }

        private void LoadTextFile(RichTextBox richTextBox, string filename)
        {
            richTextBox.Document.Blocks.Clear(); 
            using (StreamReader streamReader = File.OpenText(filename))
            {
                Paragraph paragraph = new Paragraph(new Run(streamReader.ReadToEnd()));
                richTextBox.Document.Blocks.Add(paragraph);
            }
        }
    }
}
