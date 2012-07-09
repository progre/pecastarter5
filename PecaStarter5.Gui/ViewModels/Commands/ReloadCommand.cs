using System;
using System.Windows.Input;
using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter.ViewModel.Command
{
    class ReloadCommand : ICommand
    {
        private readonly RelayListViewModel parent;
        private readonly Peercast peercast;

        public ReloadCommand(RelayListViewModel parent, Peercast peercast)
        {
            this.parent = parent;
            this.peercast = peercast;
        }

        #region ICommand メンバー

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            peercast.GetChannelsAsync().ContinueWith((x) =>
            {
                parent.Channels = x.Result;
            });
        }

        #endregion
    }
}
