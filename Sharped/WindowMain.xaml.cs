using System;
using System.Windows;
using System.Windows.Input;
using Sharped.Controls;
using Sharped.Controls.Properties;
using ShControls;
using System.Diagnostics;
using System.IO;

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

        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
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

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            codeBox.Save();
        }

        private void SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
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

        private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            codeBox.Clear();
        }

        private void SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = codeBox.Filename != "";
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CompileExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Process compiler = new Process();
            compiler.StartInfo.CreateNoWindow = true;
            compiler.StartInfo.FileName = Settings.Default.CompilerPath;
            compiler.StartInfo.Arguments =
                String.Format(Settings.Default.CompilerOptions,
                Path.GetFileName(codeBox.Filename), codeBox.Filename
            );
            compiler.StartInfo.UseShellExecute = false;
            compiler.StartInfo.RedirectStandardOutput = true;
            compiler.Start();

            tbOutput.Text = string.Format(
                "{0} {1}\r\n",
                compiler.StartInfo.FileName,
                compiler.StartInfo.Arguments
                );
            tbOutput.Text += compiler.StandardOutput.ReadToEnd();

            compiler.WaitForExit();
            tbOutput.ScrollToEnd();
        }

        private void RunExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Process program = new Process();
            program.StartInfo.CreateNoWindow = true;
            program.StartInfo.FileName = String.Format("{0}.exe", Path.GetFileName(codeBox.Filename));
            program.StartInfo.UseShellExecute = false;
            program.StartInfo.RedirectStandardOutput = true;
            program.Start();

            tbOutput.Text += program.StandardOutput.ReadToEnd();

            program.WaitForExit();
        }

        private void SearchExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            codeBox.SwitchSearchPanelVisibility();
        }

        private void RunCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = File.Exists(
                String.Format("{0}.exe", Path.GetFileName(codeBox.Filename))
                );
        }

        private void OptionsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Window options = new WindowOptions();
            options.ShowDialog();
        }
    }

    public static class CompilerCommand
    {
        public static InputGestureCollection compileGestureCollection = new InputGestureCollection()
        {
            new KeyGesture(Key.F7)
        };
        public static InputGestureCollection runGestureCollection = new InputGestureCollection()
        {
            new KeyGesture(Key.F5, ModifierKeys.Control)
        };
        public static InputGestureCollection optionsGestureCollection = new InputGestureCollection()
        {
            new KeyGesture(Key.F7, ModifierKeys.Alt)
        };

        public static readonly RoutedUICommand Compile =
            new RoutedUICommand("Compile", "Compile", typeof(WindowMain), compileGestureCollection);
        public static readonly RoutedUICommand Run =
            new RoutedUICommand("Run compiled application", "Run", typeof(WindowMain), runGestureCollection);
        public static readonly RoutedUICommand Options =
            new RoutedUICommand("Options", "Options", typeof(WindowMain), optionsGestureCollection);
    }
}