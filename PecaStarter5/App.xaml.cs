using System.Windows;
using Progressive.PecaStarter5.ViewModel;
using Progressive.PecaStarter5.Views;
using Progressive.PecaStarter5.Model;

namespace Progressive.PecaStarter5.Gui
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel()
                {
                    // ファイルロードを仕込みたい
                    Settings = new Settings()
                    {
                        Width = 640
                    }
                }
            };
            MainWindow.Show();

            this.DispatcherUnhandledException += (sender, dispatcherUnhandledExceptionEventArgs) =>
            {
                if (MessageBox.Show(
                    "未解決のエラーが発生しました。（" + dispatcherUnhandledExceptionEventArgs.Exception.Message + "）プログラムを終了します。",
                    "PecaStarter", MessageBoxButton.OKCancel, MessageBoxImage.Error)
                    != MessageBoxResult.OK)
                {
                    dispatcherUnhandledExceptionEventArgs.Handled = true;
                }
            };
        }
    }
}
