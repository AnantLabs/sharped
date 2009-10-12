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

namespace Sharped
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        /// <summary>
        /// Indicates whether to show main menu
        /// </summary>
        public static readonly DependencyProperty IsMainMenuVisibleProperty;
        public bool IsMainMenuVisible
        {
            get { return (bool)GetValue(IsMainMenuVisibleProperty); }
            set { SetValue(IsMainMenuVisibleProperty, value); }
        }

        static WindowMain()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
            metadata.DefaultValue = false;

            IsMainMenuVisibleProperty = DependencyProperty.Register(
                "IsMainMenuVisible", typeof(bool), typeof(WindowMain), metadata);
        }

        public WindowMain()
        {
            InitializeComponent();
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO:
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

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                IsMainMenuVisible = !IsMainMenuVisible;
            }
        }
    }
}
