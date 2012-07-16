using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using System;
using Progressive.PecaStarter5.ViewModels;
using Progressive.PecaStarter5.Models;

namespace Progressive.PecaStarter5.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        public MainWindowViewModel()
        {
            MainPanelViewModel = new MainPanelViewModel();
            Settings = PecaStarter5Factory.Settings;
        }

        ~MainWindowViewModel()
        {
            Dispose();
        }

        private Settings settings;
        public Settings Settings
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (settings == value)
                    return;
                settings = value;
                MainPanelViewModel.Settings = settings;
                settings.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "HasNotifyIcon")
                        OnPropertyChanged("HasNotifyIcon");
                };
            }
        }

        public double Left
        {
            get { return settings.IsSavePosition ? settings.Left : double.NaN; }
            set { settings.Left = value; }
        }

        public double Top
        {
            get { return settings.IsSavePosition ? settings.Top : double.NaN; }
            set { settings.Top = value; }
        }

        public double Height
        {
            get { return settings.IsSavePosition ? settings.Height : 320; }
            set { settings.Height = value; }
        }

        public double Width
        {
            get { return settings.IsSavePosition ? settings.Width : 512; }
            set { settings.Width = value; }
        }

        public bool HasNotifyIcon
        {
            get { return settings.HasNotifyIcon; }
        }

        public MainPanelViewModel MainPanelViewModel { get; private set; }

        #region IDisposable メンバー

        public void Dispose()
        {
            PecaStarter5Factory.Settings = settings;

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
