using System;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
        }

        private Settings settings;
        public Settings Settings
        {
            set { settings = value; }
        }

        public int Port
        {
            get { return settings.Port; }
            set
            {
                if (value < 1 || 65535 < value)
                    throw new ArgumentException("1～65535の間で指定してください");
                if (settings.Port == value)
                    return;
                settings.Port = value;
                OnPropertyChanged("Port");
            }
        }

        public bool HasNotifyIcon
        {
            get { return settings.HasNotifyIcon; }
            set
            {
                if (settings.HasNotifyIcon == value)
                    return;
                settings.HasNotifyIcon = value;
                OnPropertyChanged("HasNotifyIcon");
            }
        }

        public bool IsSavePosition
        {
            get { return settings.IsSavePosition; }
            set
            {
                if (settings.IsSavePosition == value)
                    return;
                settings.IsSavePosition = value;
                OnPropertyChanged("IsSavePosition");
            }
        }

        public bool Logging
        {
            get { return settings.Logging; }
            set
            {
                if (settings.Logging == value)
                    return;
                settings.Logging = value;
                OnPropertyChanged("Logging");
            }
        }

        public string LogPath
        {
            get { return settings.LogPath; }
            set
            {
                if (settings.LogPath == value)
                    return;
                settings.LogPath = value;
                OnPropertyChanged("LogPath");
            }
        }

        public int Delay
        {
            get { return settings.Delay; }
            set
            {
                if (value < 0)
                    throw new ArgumentException();
                settings.Delay = value;
                OnPropertyChanged("Delay");
            }
        }
    }
}
