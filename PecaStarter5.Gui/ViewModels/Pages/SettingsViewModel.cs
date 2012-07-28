using System.ComponentModel.DataAnnotations;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Plugins;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class SettingsViewModel : ViewModelBase
    {
        private Configuration configuration;
        private Peercast peercast;
        private LoggerPlugin loggerPlugin;

        public SettingsViewModel(Configuration configuration, Peercast peercast, LoggerPlugin loggerPlugin)
        {
            this.configuration = configuration;
            this.peercast = peercast;
            this.loggerPlugin = loggerPlugin;

            // configurationと連動していない項目
            peercast.Address = "localhost:" + configuration.Port;
            loggerPlugin.IsEnabled = Logging;
            loggerPlugin.BasePath = LogPath;
        }

        [Required(ErrorMessage = "1～65535の値を入力してください")]
        [Range(1, 65535, ErrorMessage = "1～65535の値を入力してください")]
        public int Port
        {
            get { return configuration.Port; }
            set
            {
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
