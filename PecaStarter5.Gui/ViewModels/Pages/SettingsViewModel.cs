using System;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class SettingsViewModel : ViewModelBase
    {
        private Configuration configuration;
        private Peercast peercast;

        public SettingsViewModel(Configuration configuration, Peercast peercast)
        {
            this.configuration = configuration;
            this.peercast = peercast;
            peercast.Address = "localhost:" + configuration.Port;
        }

        public int Port
        {
            get { return configuration.Port; }
            set
            {
                if (value < 1 || 65535 < value)
                    throw new ArgumentException("1～65535の間で指定してください");
                if (configuration.Port == value)
                    return;
                configuration.Port = value;
                peercast.Address = "localhost:" + value;
                OnPropertyChanged("Port");
            }
        }

        public bool HasNotifyIcon
        {
            get { return configuration.HasNotifyIcon; }
            set
            {
                if (configuration.HasNotifyIcon == value)
                    return;
                configuration.HasNotifyIcon = value;
                OnPropertyChanged("HasNotifyIcon");
            }
        }

        public bool IsSavePosition
        {
            get { return configuration.IsSavePosition; }
            set
            {
                if (configuration.IsSavePosition == value)
                    return;
                configuration.IsSavePosition = value;
                OnPropertyChanged("IsSavePosition");
            }
        }

        public bool Logging
        {
            get { return configuration.Logging; }
            set
            {
                if (configuration.Logging == value)
                    return;
                configuration.Logging = value;
                OnPropertyChanged("Logging");
            }
        }

        public string LogPath
        {
            get { return configuration.LogPath; }
            set
            {
                if (configuration.LogPath == value)
                    return;
                configuration.LogPath = value;
                OnPropertyChanged("LogPath");
            }
        }

        public int Delay
        {
            get { return configuration.Delay; }
            set
            {
                if (value < 0)
                    throw new ArgumentException();
                configuration.Delay = value;
                OnPropertyChanged("Delay");
            }
        }
    }
}
