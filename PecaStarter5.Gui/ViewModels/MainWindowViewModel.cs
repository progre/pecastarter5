using System;
using System.Collections.Generic;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModels;

namespace Progressive.PecaStarter5.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private bool disposed;

        public MainWindowViewModel(IEnumerable<string> yellowPagesList, Settings settings)
        {
            Settings = settings;
            settings.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "HasNotifyIcon")
                    OnPropertyChanged("HasNotifyIcon");
            };

            MainPanelViewModel = new MainPanelViewModel(yellowPagesList, settings);
        }

        ~MainWindowViewModel()
        {
            Dispose(false);
        }

        public Settings Settings { get; private set; }

        public double Left
        {
            get { return Settings.IsSavePosition ? Settings.Left : double.NaN; }
            set { Settings.Left = value; }
        }

        public double Top
        {
            get { return Settings.IsSavePosition ? Settings.Top : double.NaN; }
            set { Settings.Top = value; }
        }

        public double Height
        {
            get { return Settings.IsSavePosition ? Settings.Height : 320; }
            set { Settings.Height = value; }
        }

        public double Width
        {
            get { return Settings.IsSavePosition ? Settings.Width : 512; }
            set { Settings.Width = value; }
        }

        public bool HasNotifyIcon
        {
            get { return Settings.HasNotifyIcon; }
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
