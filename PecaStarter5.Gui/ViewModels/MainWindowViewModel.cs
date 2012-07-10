using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Model;
using System;
using Progressive.PecaStarter5.ViewModels;

namespace Progressive.PecaStarter5.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private Settings settings = new Settings();
        public Settings Settings
        {
            set
            {
                if (settings == null)
                    throw new ArgumentNullException();
                if (settings == value)
                    return;
                settings = value;
                settings.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "HasTaskTrayIcon")
                        OnPropertyChanged("HasTaskTrayIcon");
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
            set { settings.HasNotifyIcon = value; }
        }

        public MainPanelViewModel MainPanelViewModel { get; private set; }

        public MainWindowViewModel()
        {
            Height = 240;
            Width = 640;
            MainPanelViewModel = new MainPanelViewModel();
        }
    }
}
