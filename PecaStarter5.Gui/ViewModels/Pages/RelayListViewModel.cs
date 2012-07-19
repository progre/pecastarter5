using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.ViewModels.Commands;
using Progressive.PecaStarter5.Models;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class RelayListViewModel : ViewModelBase
    {
        public RelayListViewModel(Peercast peercast, IEnumerable<IYellowPages> yellowPagesList)
        {
            YellowPagesList = yellowPagesList;
            isListEnabled = true;
            Channels = new List<IChannel>();
            ReloadCommand = new ReloadCommand(this, peercast);
            OpenCommand = new OpenCommand(this, peercast, yellowPagesList);
        }

        private bool isListEnabled;
        public bool IsListEnabled
        {
            get { return isListEnabled; }
            set
            {
                isListEnabled = value;
                OnPropertyChanged("IsListEnabled");
            }
        }

        private IList<IChannel> channels;
        public IList<IChannel> Channels
        {
            get { return channels; }
            set
            {
                channels = value;
                OnPropertyChanged("Channels");
            }
        }

        private IChannel selectedChannel;
        public IChannel SelectedChannel
        {
            get { return selectedChannel; }
            set
            {
                if (!SetProperty("SelectedChannel", ref selectedChannel, value))
                    return;
                OpenCommand.OnCanExecuteChanged();
            }
        }

        public Visibility LoadingVisibility { get { return IsListEnabled ? Visibility.Collapsed : Visibility.Visible; } }

        public IEnumerable<IYellowPages> YellowPagesList { get; private set; }

        private IYellowPages selectedYellowPages;
        public IYellowPages SelectedYellowPages
        {
            get { return selectedYellowPages; }
            set
            {
                if (!SetProperty("SelectedYellowPages", ref selectedYellowPages, value))
                    return;
                OpenCommand.OnCanExecuteChanged();
            }
        }

        public ICommand ReloadCommand { get; private set; }
        public OpenCommand OpenCommand { get; private set; }

        public event EventHandler<SelectedEventArgs> ChannelSelected;

        internal void OnChannelSelected()
        {
            if (ChannelSelected != null)
                ChannelSelected(this, new SelectedEventArgs(SelectedChannel));
        }
    }

    public class SelectedEventArgs : EventArgs
    {
        public IChannel SelectedChannel { get; private set; }

        public SelectedEventArgs(IChannel selectedChannel)
        {
            SelectedChannel = selectedChannel;
        }
    }
}
