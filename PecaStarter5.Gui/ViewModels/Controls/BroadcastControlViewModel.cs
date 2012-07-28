using System;
using System.Threading.Tasks;
using Progressive.Commons.ViewModels;
using Progressive.Commons.ViewModels.Commands;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Channels;
using Progressive.PecaStarter5.Models.Services;
using Progressive.PecaStarter5.ViewModels.Dxos;
using System.Net;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    public class BroadcastControlViewModel : ViewModelBase
    {
        private Models.PecaStarter pecaStarter;

        public BroadcastControlViewModel(MainPanelViewModel parent, Models.PecaStarter pecaStarter, PeercastService service, Configuration configuration)
        {
            this.pecaStarter = pecaStarter;
            Configuration = configuration;

            BroadcastCommand = new DelegateCommand(() =>
            {
                IsProcessing = true;
                parent.ExternalSourceViewModel.UpdateHistory();
                var parameter = ViewModelDxo.ToBroadcastParameter(parent.ExternalSourceViewModel);
                var yp = parent.YellowPagesListViewModel.SelectedYellowPages;
                service.BroadcastAsync(yp.Model, yp.AcceptedHash, yp.Parameters.Dictionary,
                    parameter,
                    new Progress<string>(x => parent.Feedback = x))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            parent.OnException(t.Exception);
                            IsProcessing = false;
                            return;
                        }
                        BroadcastingChannel = new BroadcastingChannel(parameter.Name, t.Result);
                        pecaStarter.Broadcast(parameter);
                        IsProcessing = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () =>
            {
                if (IsProcessing)
                    return false;
                if (BroadcastingChannel != null)
                    return false;
                if (!string.IsNullOrEmpty(parent.ExternalSourceViewModel.Error))
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
                if (BroadcastingChannel == null)
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
                        IsProcessing = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () =>
            {
                if (IsProcessing)
                    return false;
                if (BroadcastingChannel == null)
                    return false;
                return true;
            });

            parent.ExternalSourceViewModel.PropertyChanged += OnParameterPropertyChanged;
            parent.YellowPagesListViewModel.PropertyChanged += OnParameterPropertyChanged;
        }

        private bool isProcessing;
        public bool IsProcessing
        {
            get { return isProcessing; }
            set
            {
                SetProperty("IsProcessing", ref isProcessing, value);
                BroadcastCommand.OnCanExecuteChanged();
                UpdateCommand.OnCanExecuteChanged();
                StopCommand.OnCanExecuteChanged();
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
                BroadcastCommand.OnCanExecuteChanged();
                UpdateCommand.OnCanExecuteChanged();
                StopCommand.OnCanExecuteChanged();
            }
        }

        public DelegateCommand BroadcastCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand StopCommand { get; private set; }
        public Configuration Configuration { get; private set; } // カウントダウンボタンが使用

        private void OnParameterPropertyChanged(object sender, EventArgs e)
        {
            BroadcastCommand.OnCanExecuteChanged();
            UpdateCommand.OnCanExecuteChanged();
        }
    }
}
