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
using System.Windows.Shapes;
using Sharped.Controls.Properties;

namespace Sharped.Controls
{
    /// <summary>
    /// Interaction logic for WindowOptions.xaml
    /// </summary>
    public partial class WindowOptions : Window
    {
        public WindowOptions()
        {
            InitializeComponent();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
        }

        private bool ApplyButtonEnabled()
        {
            return false;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Apply_Click(sender, e);
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Settings_changed(object sender, TextChangedEventArgs e)
        {
            ApplyButton.IsEnabled = true;
        }
    }
}
