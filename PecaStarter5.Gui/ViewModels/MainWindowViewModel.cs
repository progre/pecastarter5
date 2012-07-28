using System;
using System.Collections.Generic;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModels;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private bool disposed;

        public MainWindowViewModel(PecaStarterModel model)
        {
            Title = model.Title;
            Configuration = model.Configuration;
            configuration.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "HasNotifyIcon")
                    OnPropertyChanged("HasNotifyIcon");
            };

            MainPanelViewModel = new MainPanelViewModel(model, configuration);
        }

        ~MainWindowViewModel()
        {
            Dispose(false);
        }

        public string Title { get; set; }

        private Configuration configuration;
        public Configuration Configuration
        {
            get { return configuration; }
            private set { configuration = value; }
        }

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

        #region IDisposable メンバー

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }
            this.disposed = true;

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
