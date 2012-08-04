using System.IO;
using System.Reflection;
using System.Windows;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModel;

namespace Progressive.PecaStarter5
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private PecaStarterModel _model;
        private MainWindowViewModel _viewModel;

        public App()
        {
            _model = new PecaStarterModel(Title, new ExternalResource(ApplicationName, ApplicationPath));
            _viewModel = new MainWindowViewModel(_model);
            Resources.Add("DataContext", _viewModel);

            DispatcherUnhandledException += (sender, dispatcherUnhandledExceptionEventArgs) =>
            {
                Save(_viewModel, _model);
                if (MessageBox.Show(
                    "未解決のエラーが発生しました。（" + dispatcherUnhandledExceptionEventArgs.Exception.Message + "）プログラムを終了します。",
                    "PecaStarter", MessageBoxButton.OKCancel, MessageBoxImage.Error)
                    != MessageBoxResult.OK)
                {
                    dispatcherUnhandledExceptionEventArgs.Handled = true;
                }
            };
        }

        private string Title
        {
            get { return "Peca Starter " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " Beta"; }
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

        protected override void OnDeactivated(System.EventArgs e)
        {
            Save(_viewModel, _model);
            base.OnDeactivated(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Save(_viewModel, _model);
            base.OnExit(e);
        }

        private void Save(MainWindowViewModel viewModel, PecaStarterModel model)
        {
            viewModel.UpdateModel();
            model.Save();
        }
    }
}
