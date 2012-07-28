using System.IO;
using System.Reflection;
using System.Windows;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModel;
using Progressive.PecaStarter5.Views;

namespace Progressive.PecaStarter5
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private string Title
        {
            get { return "Peca Starter " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " DP"; }
        }

        private string ApplicationName
        {
            get { return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location); }
        }

        private string ApplicationPath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var model = new PecaStarterModel(Title, new ExternalResource(ApplicationName, ApplicationPath));
            var viewModel = new MainWindowViewModel(model);
            MainWindow = new MainWindow() { DataContext = viewModel };
            MainWindow.Deactivated += (sender, e1) => model.Save();
            MainWindow.Show();

            DispatcherUnhandledException += (sender, dispatcherUnhandledExceptionEventArgs) =>
            {
                model.Save();
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
