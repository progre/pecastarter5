using System;
using System.Threading.Tasks;
using Progressive.Commons.ViewModels;
using Progressive.Commons.ViewModels.Commands;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Broadcasts;
using Progressive.PecaStarter5.Models.Channels;
using Progressive.PecaStarter5.Models.Services;
using Progressive.PecaStarter5.ViewModels.Dxos;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    internal class BroadcastControlViewModel : ViewModelBase
    {
        private Configuration configuration;

        public BroadcastControlViewModel(MainPanelViewModel parent,
            BroadcastModel broadcastModel, Configuration configuration, PeercastService service)
        {
            this.configuration = configuration;

            var externalSourceViewModel = parent.ExternalSourceViewModel;
            BroadcastCommand = new DelegateCommand(() =>
            {
                // 画面ロック
                IsProcessing = true;
                // ヒストリ更新
                externalSourceViewModel.UpdateHistory();
                // YP規約チェック
                var yp = parent.YellowPagesListViewModel.SelectedYellowPages;
                if (!yp.IsAccepted)
                {
                    parent.OnException(new ApplicationException("YPの規約に同意していません。" + Environment.NewLine
                        + "配信を開始するにはYPの規約を確認し、同意する必要があります。"));
                    parent.SelectedIndex = MainPanelViewModel.YellowPagesTabIndex;
                    IsProcessing = false;
                    return;
                }
                var parameter = ViewModelDxo.ToBroadcastParameter(externalSourceViewModel);
                var id = service.BroadcastAsync(
                    yp.Model, yp.AcceptedHash, yp.Parameters.Dictionary, parameter,
                    new Progress<string>(x => parent.Feedback = x)).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        parent.OnException(t.Exception);
                        IsProcessing = false;
                        return;
                    }
                    broadcastModel.Broadcast(yp.Model, parameter);
                    BroadcastingChannel = new BroadcastingChannel(parameter.Name, t.Result);
                    IsProcessing = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () =>
            {
                if (IsProcessing)
                    return false;
                if (IsBroadcasting)
                    return false;
                if (parent.SettingsViewModel.HasError)
                    return false;
                if (parent.YellowPagesListViewModel.SelectedYellowPages == null)
                    return false;
                if (externalSourceViewModel.HasError)
                    return false;
                return true;
            });

            UpdateCommand = new DelegateCommand(() =>
            {
                IsProcessing = true;
                parent.ExternalSourceViewModel.UpdateHistory();
                var yp = parent.YellowPagesListViewModel.SelectedYellowPages;
                service.UpdateAsync(
                    yp.Model, yp.Parameters.Dictionary,
                    ViewModelDxo.ToUpdateParameter(BroadcastingChannel.Id, parent.ExternalSourceViewModel),
                    new Progress<string>(x => parent.Feedback = x))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            parent.OnException(t.Exception);
                        }
                        IsProcessing = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () =>
            {
                if (IsProcessing)
                    return false;
                if (!IsBroadcasting)
                    return false;
                if (parent.SettingsViewModel.HasError)
                    return false;
                if (externalSourceViewModel.HasError)
                    return false;
                return true;
            });

            StopCommand = new DelegateCommand(() =>
            {
                IsProcessing = true;
                var yp = parent.YellowPagesListViewModel.SelectedYellowPages;
                service.StopAsync(yp.Model, yp.Parameters.Dictionary,
                    BroadcastingChannel.Name, BroadcastingChannel.Id,
                    new Progress<string>(x => parent.Feedback = x))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            parent.OnException(t.Exception);
                            IsProcessing = false;
                            return;
                        }
                        BroadcastingChannel = null;
                        broadcastModel.Stop();
                        IsProcessing = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () =>
            {
                if (IsProcessing)
                    return false;
                if (!IsBroadcasting)
                    return false;
                if (parent.SettingsViewModel.HasError)
                    return false;
                return true;
            });
        }

        private bool isProcessing;
        public bool IsProcessing
        {
            get { return isProcessing; }
            private set
            {
                SetProperty("IsProcessing", ref isProcessing, value);
                RaiseCanExecuteChanged();
            }
        }

        private BroadcastingChannel broadcastingChannel;
        public BroadcastingChannel BroadcastingChannel
        {
            get { return broadcastingChannel; }
            set // Selectorから入るので外から設定可
            {
                if (!SetProperty("BroadcastingChannel", ref broadcastingChannel, value))
                    return;
                RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand BroadcastCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand StopCommand { get; private set; }
        public Configuration Configuration { get { return configuration; } } // カウントダウンボタンが使用

        public bool IsBroadcasting
        {
            get { return BroadcastingChannel != null; }
        }

        public void RaiseCanExecuteChanged()
        {
            BroadcastCommand.OnCanExecuteChanged();
            UpdateCommand.OnCanExecuteChanged();
            StopCommand.OnCanExecuteChanged();
        }
    }
}
