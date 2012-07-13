using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;

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
            var fbd = new Forms.FolderBrowserDialog();
            fbd.Description = "フォルダを指定してください。";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.SelectedPath = @"C:\";
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == Forms.DialogResult.OK)
            {
                ((Button)sender).DataContext = fbd.SelectedPath;
            }
        }
    }
}
