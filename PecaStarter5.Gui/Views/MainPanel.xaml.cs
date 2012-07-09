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
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Progressive.PecaStarter5.Views
{
    /// <summary>
    /// MainPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class MainPanel : UserControl
    {
        public MainPanel()
        {
            InitializeComponent();
        }

        private void OnUpdateHistoryItemClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            //(DataContext as MainWindowViewModel).ExternalSourceViewModel.UpdateHistory();
        }

        private void OnAssemblyPathItemClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName);
        }

        private void OnConfigurationPathItemClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            //Process.Start(new FileInfo((DataContext as MainWindowViewModel).ConfigurationPath).Directory.FullName);
        }
    }
}
