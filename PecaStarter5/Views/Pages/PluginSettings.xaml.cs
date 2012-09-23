using System.ComponentModel;
using System.Windows;
namespace Progressive.PecaStarter5.Views.Pages
{
    /// <summary>
    /// Settings.xaml の相互作用ロジック
    /// </summary>
    public partial class PluginSettings
    {
        public PluginSettings()
        {
            //if (DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = new Hoge2{ Col = new[] { "", "", "", "", "", },
                Hoge = "hg"};
            }

            InitializeComponent();
        }

        public class Hoge2
        {
            public object Col { get; set; }
            public object Hoge { get; set; }
        }
    }
}
