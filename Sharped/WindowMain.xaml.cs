using System;
using System.Windows;
using System.Windows.Input;
using ShControls;

namespace Sharped
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        /// <summary>
        /// Indicates whether to show main menu
        /// </summary>
        public static readonly DependencyProperty IsMainMenuVisibleProperty;

        static WindowMain()
        {
            var metadata = new FrameworkPropertyMetadata();
            metadata.DefaultValue = false;

            IsMainMenuVisibleProperty = DependencyProperty.Register(
                "IsMainMenuVisible", typeof (bool), typeof (WindowMain), metadata);
        }

        public WindowMain()
        {
            InitializeComponent();
        }

        public bool IsMainMenuVisible
        {
            get { return (bool) GetValue(IsMainMenuVisibleProperty); }
            set { SetValue(IsMainMenuVisibleProperty, value); }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                IsMainMenuVisible = !IsMainMenuVisible;
            }
        }


        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();

            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "C# source files (*.cs)|*.cs|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;
            Nullable<bool> result = dlg.ShowDialog(this);
            if (result == true)
            {
                codeBox.LoadFromFile(dlg.FileName);
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            codeBox.Save();
        }

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            codeBox.Clear();
        }

        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();

            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "C# source files (*.cs)|*.cs|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;
            Nullable<bool> result = dlg.ShowDialog(this);
            if (result == true)
            {
                codeBox.SaveToFile(dlg.FileName);
            }
        }

        private void ReplaceItem_Click(object sender, RoutedEventArgs e)
        {
            codeBox.ShowReplacePanel();
        }
    }
}