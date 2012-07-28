using System;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModels;

namespace Progressive.PecaStarter5.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private bool m_disposed;
        private PecaStarterModel m_model;

        public MainWindowViewModel(PecaStarterModel model)
        {
            m_model = model;
            Configuration.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "HasNotifyIcon")
                    OnPropertyChanged("HasNotifyIcon");
            };

            MainPanelViewModel = new MainPanelViewModel(model);
        }

        ~MainWindowViewModel()
        {
            Dispose(false);
        }

        public string Title { get { return m_model.Title; } }

        public double Left
        {
            get { return Configuration.IsSavePosition ? Configuration.Left : double.NaN; }
            set { Configuration.Left = value; }
        }

        public double Top
        {
            get { return Configuration.IsSavePosition ? Configuration.Top : double.NaN; }
            set { Configuration.Top = value; }
        }

        public double Height
        {
            get { return Configuration.IsSavePosition ? Configuration.Height : 320; }
            set { Configuration.Height = value; }
        }

        public double Width
        {
            get { return Configuration.IsSavePosition ? Configuration.Width : 512; }
            set { Configuration.Width = value; }
        }

        public bool HasNotifyIcon
        {
            get { return Configuration.HasNotifyIcon; }
        }

        public MainPanelViewModel MainPanelViewModel { get; private set; }

        private Configuration Configuration { get { return m_model.Configuration; } }

        #region IDisposable メンバー

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (this.m_disposed)
            {
                return;
            }
            this.m_disposed = true;

            if (disposing)
            {
                // マネージ リソースの解放処理をこの位置に記述します。
                MainPanelViewModel.ExternalSourceViewModel.UpdateHistory();
            }
            // アンマネージ リソースの解放処理をこの位置に記述します。
            OnPropertyChanged("Settings");
        }
    }
}
