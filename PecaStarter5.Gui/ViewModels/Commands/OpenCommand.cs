using System;
using System.Collections.Generic;
using System.Windows.Input;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.ViewModels.Commands
{
    public class OpenCommand : ICommand
    {
        private readonly RelayListViewModel parent;
        private readonly Peercast peercast;
        private readonly IEnumerable<IYellowPages> yellowPageses;

        public OpenCommand(RelayListViewModel parent, Peercast peercast, IEnumerable<IYellowPages> yellowPageses)
        {
            this.parent = parent;
            this.peercast = peercast;
            this.yellowPageses = yellowPageses;
        }

        #region ICommand メンバー

        public bool CanExecute(object parameter)
        {
            if (parent.SelectedYellowPages == null)
            {
                return false;
            }
            if (parent.SelectedChannel == null || parent.SelectedChannel.Status != "BROADCAST")
            {
                return false;
            }
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            parent.OnChannelSelected();
        }
        #endregion

        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }
    }
}
