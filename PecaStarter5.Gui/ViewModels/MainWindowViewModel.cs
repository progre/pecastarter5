using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Model;
using System;
using Progressive.PecaStarter5.ViewModels;

namespace Progressive.PecaStarter5.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            MainPanelViewModel = new MainPanelViewModel();
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
            get { return settings.Left; }
            set { settings.Left = value; }
        }
        public double Top
        {
            get { return settings.Top; }
            set { settings.Top = value; }
        }
        public double Height
        {
            get { return settings.Height; }
            set { settings.Height = value; }
        }
        public double Width
        {
            get { return settings.Width; }
            set { settings.Width = value; }
        }
        public bool HasNotifyIcon
        {
            get { return settings.HasNotifyIcon; }
        }

        public MainPanelViewModel MainPanelViewModel { get; private set; }
    }
}
