using System.Threading.Tasks;
using System.Windows.Input;
using Progressive.PecaStarter5.Models.Services;
using System;
using Progressive.Peercast4Net;
using Progressive.PecaStarter5.Models;

namespace Progressive.PecaStarter.ViewModel.Command
{
    public class BroadcastCommand : ICommand
    {
        public BroadcastCommand()
        {

            // 実行後にUIを更新する必要がある
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
            var param = (Tuple<IYellowPages, int, BroadcastParameter>)parameter;
            new PeercastService().BroadcastAsync(param.Item1, param.Item2, param.Item3,
                new Progress<string>(x => { }))
                .ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    // ダイアログ通知
                    System.Windows.MessageBox.Show(t.Exception.InnerException.Message, "仮");
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion
    }
}
