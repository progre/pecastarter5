using System;
using System.Collections.Generic;
using System.Windows.Input;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter.ViewModel.Command
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
            // YPタブを指定のYPに
            //viewModel.YellowPagesesViewModel.SelectedYellowPages = viewModel.RelayListViewModel.SelectedYellowPages;
            // ソースタブに値を反映
            //var esvm = viewModel.ExternalSourceViewModel;
            //esvm.Name.Value = ch.Name;
            //esvm.Genre.Value = viewModel.YellowPagesesViewModel.SelectedYellowPages.Parse(ch.Genre);
            //esvm.Description.Value = ch.Description;
            //esvm.ContactUrl.Value = ch.ContactUrl;
            //esvm.Comment.Value = ch.Comment;
            //esvm.Name.Value = ch.Name;
            // ソースタブに移動
            //viewModel.SelectedIndex = 3;
            // ログタイマー
            //var ypvm = viewModel.YellowPagesesViewModel.SelectedYellowPages;
            //var yp = yellowPageses.Single(a => a.Name == ypvm.Name);
            //if (yp is WebApiYellowPages || viewModel.SettingsViewModel.Logging)
            //{
            //    viewModel.BeginTimer();
            //}
        }
        #endregion
    }
}
