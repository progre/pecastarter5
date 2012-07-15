using System.Threading.Tasks;
using System.Windows.Input;
using Progressive.PecaStarter5.Models.Services;
using System;

namespace Progressive.PecaStarter.ViewModel.Command
{
    public class BroadcastCommand : ICommand
    {
        public BroadcastCommand()
        {
        }

        #region ICommand メンバー

        public bool CanExecute(object parameter)
        {
            //var param = (BroadcastParameter)parameter;
            //if (string.IsNullOrEmpty(param.StreamUrl)) return false;
            //if (!TryLength(esvm.StreamUrl)) return false;
            //if (string.IsNullOrEmpty(esvm.Name.Value)) return false;
            //if (!TryLength(esvm.Name.Value)) return false;
            //if (!TryLength(esvm.Description.Value)) return false;
            //if (!TryLength(esvm.ContactUrl.Value)) return false;
            //if (!TryLength(esvm.Comment.Value)) return false;
            //var yp = yellowPageses.Single(a => a.Name == ypvm.Name);
            //if (yp is PeercastYellowPages)
            //{
            //}
            //else
            //{
            //    foreach (var param in (yp as WebApiYellowPages).BroadcastParameters)
            //    {
            //        if (WebApiDefine.ExcludeParameters.Contains(param))
            //        {
            //            continue;
            //        }
            //        if (!TryLength(ypvm[param])) return false;
            //    }
            //    return true;
            //}
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            new PeercastService().BroadcastAsync().ContinueWith(t =>
            {
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion
    }
}
