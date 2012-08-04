using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Controls;

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
            ((dynamic)DataContext).ExternalSourceViewModel.UpdateHistory();
        }

        private void OnAssemblyPathItemClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName);
        }
    }
}
