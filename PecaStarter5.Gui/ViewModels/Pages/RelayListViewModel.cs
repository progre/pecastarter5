using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter.ViewModel.Command;
using Progressive.PecaStarter5.Models;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class RelayListViewModel : ViewModelBase
    {
        private bool isListEnabled;
        public bool IsListEnabled
        {
            get { return isListEnabled; }
            set
            {
                isListEnabled = value;
                NotifyPropertyChanged("IsListEnabled");
            }
        }

        private IEnumerable<IChannel> channels;
        public IEnumerable<IChannel> Channels
        {
            get { return channels; }
            set
            {
                channels = value;
                NotifyPropertyChanged("Channels");
            }
        }

        private IChannel selectedChannel;
        public IChannel SelectedChannel
        {
            get { return selectedChannel; }
            set
            {
                selectedChannel = value;
                NotifyPropertyChanged("SelectedChannel");
            }
        }

        public Visibility LoadingVisibility { get { return IsListEnabled ? Visibility.Collapsed : Visibility.Visible; } }

        public IEnumerable<IYellowPages> YellowPages { get; set; }

        private IYellowPages selectedYellowPages;
        public IYellowPages SelectedYellowPages
        {
            get { return selectedYellowPages; }
            set
            {
                selectedYellowPages = value;
                NotifyPropertyChanged("SelectedYellowPages");
            }
        }

        public ICommand ReloadCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }

        public event EventHandler<SelectedEventArgs> ChannelSelected;

        public RelayListViewModel(Peercast peercast, IEnumerable<IYellowPages> yellowPageses)
        {
            selectedYellowPages = null;
            isListEnabled = true;
            Channels = Enumerable.Empty<IChannel>();
            ReloadCommand = new ReloadCommand(this, peercast);
            OpenCommand = new OpenCommand(this, peercast, yellowPageses);
        }

        internal void OnChannelSelected()
        {
            if (ChannelSelected != null)
                ChannelSelected(this, new SelectedEventArgs());
        }
    }

    public class SelectedEventArgs : EventArgs
    {
    }
}
