using System;
using System.Windows.Input;
using Progressive.PecaStarter5.ViewModels.Pages;

namespace Progressive.PecaStarter5.ViewModels.Commands
{
    public class OpenCommand : ICommand
    {
        private readonly RelayListViewModel m_parent;

        public OpenCommand(RelayListViewModel parent)
        {
            m_parent = parent;
        }

        #region ICommand メンバー

        public bool CanExecute(object parameter)
        {
            if (m_parent.SelectedYellowPages == null)
            {
                return false;
            }
            if (m_parent.SelectedChannel == null
                || (m_parent.SelectedChannel.Status != "BROADCAST"
                && m_parent.SelectedChannel.Status != "Receiving"))
            {
                return false;
            }
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            m_parent.OnChannelSelected();
        }
        #endregion

        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }
    }
}
