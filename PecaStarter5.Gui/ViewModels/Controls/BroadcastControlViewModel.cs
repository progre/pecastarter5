using System;
using System.Threading.Tasks;
using Progressive.Commons.ViewModels;
using Progressive.Commons.ViewModels.Commands;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Services;
using Progressive.PecaStarter5.ViewModels.Dxos;
using Progressive.PecaStarter5.Models.Channels;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    public class BroadcastControlViewModel : ViewModelBase
    {
        public BroadcastControlViewModel(MainPanelViewModel parent, PeercastService service, Configuration configuration)
        {
            Configuration = configuration;

            BroadcastCommand = new DelegateCommand(() =>
            {
                var parameter = ViewModelDxo.ToBroadcastParameter(parent.ExternalSourceViewModel);
                var yp = parent.YellowPagesListViewModel.SelectedYellowPages;
                service.BroadcastAsync(yp.Model, yp.AcceptedHash, yp.Parameters.Dictionary,
                    parameter,
                    new Progress<string>(x => parent.Feedback = x))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            OnException(parent, t.Exception);
                            return;
                        }
                        BroadcastingChannel = new BroadcastingChannel(parameter.Name, t.Result);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () => BroadcastingChannel == null);

            UpdateCommand = new DelegateCommand(() =>
            {
                var yp = parent.YellowPagesListViewModel.SelectedYellowPages;
                service.UpdateAsync(
                    yp.Model, yp.Parameters.Dictionary,
                    ViewModelDxo.ToUpdateParameter(BroadcastingChannel.Id, parent.ExternalSourceViewModel),
                    new Progress<string>(x => parent.Feedback = x))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            OnException(parent, t.Exception);
                            return;
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () => BroadcastingChannel != null);

            StopCommand = new DelegateCommand(() =>
            {
                var yp = parent.YellowPagesListViewModel.SelectedYellowPages;
                service.StopAsync(yp.Model, yp.Parameters.Dictionary,
                    BroadcastingChannel.Name, BroadcastingChannel.Id,
                    new Progress<string>(x => parent.Feedback = x))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            OnException(parent, t.Exception);
                            return;
                        }
                        BroadcastingChannel = null;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () => BroadcastingChannel != null);
        }

        private BroadcastingChannel broadcastingChannel;
        public BroadcastingChannel BroadcastingChannel
        {
            get { return broadcastingChannel; }
            set // Selectorから入るので外から設定可
            {
                if (!SetProperty("BroadcastingChannel", ref broadcastingChannel, value))
                    return;
                BroadcastCommand.OnCanExecuteChanged();
                UpdateCommand.OnCanExecuteChanged();
                StopCommand.OnCanExecuteChanged();
            }
        }

        public DelegateCommand BroadcastCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand StopCommand { get; private set; }
        public Configuration Configuration { get; private set; } // カウントダウンボタンが使用

        private void OnException(MainPanelViewModel parent, Exception ex)
        {
            // ダイアログ通知
            parent.Feedback = "中止";
            parent.Alert = ex.InnerException.Message + ex.StackTrace;
        }
    }
}
