using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Progressive.PecaStarter5.Models.Broadcasts;
using Progressive.PecaStarter5.ViewModels.Pages;

namespace Progressive.PecaStarter5.ViewModels.Commands
{
    class ReloadCommand : ICommand
    {
        private readonly RelayListViewModel m_parent;
        private readonly PeercastService m_peercastService;

        public ReloadCommand(RelayListViewModel parent, PeercastService peercastService)
        {
            m_parent = parent;
            m_peercastService = peercastService;
        }

        #region ICommand メンバー

        public bool CanExecute(object parameter)
        {
            return !m_parent.IsLoading;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            m_parent.IsLoading = true;
            OnCanExecuteChanged();
            m_peercastService.GetChannelsAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    m_parent.OnExceptionThrown(t.Exception);
                    m_parent.IsLoading = false;
                    OnCanExecuteChanged();
                    return;
                }
                m_parent.Channels = t.Result.ToList();
                m_parent.IsLoading = false;
                OnCanExecuteChanged();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        private void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }
    }
}
