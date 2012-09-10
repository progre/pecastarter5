using System.ComponentModel.DataAnnotations;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Plugins;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    internal class SettingsViewModel : ViewModelBase
    {
        private Configuration configuration;
        private LoggerPlugin loggerPlugin;

        public SettingsViewModel(Configuration configuration, LoggerPlugin loggerPlugin)
        {
            this.configuration = configuration;
            this.loggerPlugin = loggerPlugin;

            // configurationと連動していない項目
            loggerPlugin.IsEnabled = Logging;
            loggerPlugin.BasePath = LogPath;
        }

        public PeercastType PeercastType
        {
            get { return configuration.PeercastType; }
            set
            {
                if (configuration.PeercastType == value)
                    return;
                configuration.PeercastType = value;
                OnPropertyChanged("PeercastType");
            }
        }

        public int Port
        {
            get { return configuration.Port; }
            set
            {
                if (configuration.Port == value)
                    return;
                configuration.Port = value;
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
                loggerPlugin.IsEnabled = value;
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
                loggerPlugin.BasePath = configuration.LogPath;
                OnPropertyChanged("LogPath");
            }
        }

        [Range(0, int.MaxValue, ErrorMessage = "0以上の値を入力してください")]
        public int Delay
        {
            get { return configuration.Delay; }
            set
            {
                configuration.Delay = value;
                OnPropertyChanged("Delay");
            }
        }
    }
}
