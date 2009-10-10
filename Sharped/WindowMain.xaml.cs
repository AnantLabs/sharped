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
using Microsoft.Win32;
using System.IO;

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
            doHello();
        }

        #region textbox
        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (codeTextBox.Document == null)
                return;

            TextRange documentRange = new TextRange(codeTextBox.Document.ContentStart, codeTextBox.Document.ContentEnd);
            documentRange.ClearAllProperties();

            TextPointer navigator = codeTextBox.Document.ContentStart;
            while (navigator.CompareTo(codeTextBox.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    CheckWordsInRun((Run)navigator.Parent);

                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }

            Format();
        }
        new struct Tag
        {
            public TextPointer StartPosition;
            public TextPointer EndPosition;
            public string Word;

        }
        List<Tag> m_tags = new List<Tag>();
        void Format()
        {
            codeTextBox.TextChanged -= this.RichTextBox_TextChanged;

            for (int i = 0; i < m_tags.Count; i++)
            {
                TextRange range = new TextRange(m_tags[i].StartPosition, m_tags[i].EndPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            m_tags.Clear();

            codeTextBox.TextChanged += this.RichTextBox_TextChanged;
        }

        void CheckWordsInRun(Run run)
        {
            string text = run.Text;

            int sIndex = 0;
            int eIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]) | JSSyntaxProvider.GetSpecials.Contains(text[i]))
                {
                    if (i > 0 && !(Char.IsWhiteSpace(text[i - 1]) | JSSyntaxProvider.GetSpecials.Contains(text[i - 1])))
                    {
                        eIndex = i - 1;
                        string word = text.Substring(sIndex, eIndex - sIndex + 1);

                        if (JSSyntaxProvider.IsKnownTag(word))
                        {
                            Tag t = new Tag();
                            t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                            t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                            t.Word = word;
                            m_tags.Add(t);
                        }
                    }
                    sIndex = i + 1;
                }
            }

            string lastWord = text.Substring(sIndex, text.Length - sIndex);
            if (JSSyntaxProvider.IsKnownTag(lastWord))
            {
                Tag t = new Tag();
                t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                t.Word = lastWord;
                m_tags.Add(t);
            }
        }
        #endregion

        #region menu
        bool isSaved = true;
        string currentFileName = string.Empty;
        protected void NewCommandExecutedHandler(Object sender, ExecutedRoutedEventArgs e)
        {
            if (!isSaved)
            {
                MessageBoxResult res = MessageBox.Show("Do you want to save changes?", "Unsaved content", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        SaveCommandExecutedHandler(this, null);
                        break;
                    case MessageBoxResult.Cancel:
                        e.Handled = true;
                        return;
                        break;
                }
            }
            currentFileName = string.Empty;
            codeTextBox.Document.Blocks.Clear();
        }
        protected void OpenCommandExecutedHandler(Object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "JavaScript|*.js|All files|*.*";
            o.Multiselect = false;
            if (o.ShowDialog(this) == true)
            {
                currentFileName = o.FileName;
                isSaved = false;
                LoadJS();
            }
        }
        protected void CanExecuteSaveHandler(Object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isSaved;

        }
        protected void SaveCommandExecutedHandler(Object sender, ExecutedRoutedEventArgs e)
        {
            if (currentFileName == string.Empty)
            {
                SaveFileDialog s = new SaveFileDialog();
                s.Filter = "JavaScript|*.js";
                s.RestoreDirectory = true;
                if (s.ShowDialog() == true)
                {
                    SaveJS();
                }
            }
            else { SaveJS(); }
        }
        protected void CloseCommandExecutedHandler(Object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        void SaveJS()
        {
            using (FileStream stream = File.OpenWrite(currentFileName))
            {
                TextRange range = new TextRange(codeTextBox.Document.ContentStart, codeTextBox.Document.ContentEnd);
                range.Save(stream, DataFormats.Text);
            }
            isSaved = true;
        }
        void LoadJS()
        {
            using (FileStream stream = File.OpenRead(currentFileName))
            {
                TextRange range = new TextRange(codeTextBox.Document.ContentStart, codeTextBox.Document.ContentEnd);
                range.Load(stream, DataFormats.Text);
                stream.Close();
            }
        }
        protected void doHello()
        {
            TextRange range = new TextRange(codeTextBox.Document.ContentStart, codeTextBox.Document.ContentEnd);
            range.Text = "package Hello.World\n{\n\tclass HelloWorld\n\t{\n\t\tpublic static function GetHelloWorld():String \n\t\t{\n\t\t\treturn \"Hello World From JScript\";\n\t\t}\n\t}\n}";
        }

        #endregion
    }
}
