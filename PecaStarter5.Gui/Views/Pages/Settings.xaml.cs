using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;
using Xceed.Wpf.Toolkit;

namespace Progressive.PecaStarter5.Views.Pages
{
    /// <summary>
    /// Settings.xaml の相互作用ロジック
    /// </summary>
    public partial class Settings
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var button = (Button)sender;
            var fbd = new Forms.FolderBrowserDialog();
            fbd.Description = "フォルダを指定してください。";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.SelectedPath = button.DataContext as string;
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == Forms.DialogResult.OK)
            {
                button.DataContext = fbd.SelectedPath;
                button.GetBindingExpression(Button.DataContextProperty).UpdateSource();
            }
        }

        private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
                ((IntegerUpDown)sender).Value = (int?)e.OldValue;
        }
    }
}
