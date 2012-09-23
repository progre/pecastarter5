using System.ComponentModel.DataAnnotations;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    internal class SettingsViewModel : ViewModelBase
    {
        private Configuration configuration;

        public SettingsViewModel(Configuration configuration)
        {
            this.configuration = configuration;
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
