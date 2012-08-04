using System;
using System.Threading.Tasks;
using Progressive.Commons.ViewModels;
using Progressive.Commons.ViewModels.Commands;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Channels;
using Progressive.PecaStarter5.ViewModels.Dxos;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    public class BroadcastControlViewModel : ViewModelBase
    {
        private PecaStarterModel m_model;

        public BroadcastControlViewModel(MainPanelViewModel parent, PecaStarterModel model)
        {
            m_model = model;

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
                    parent.SelectedIndex = 2;
                    IsProcessing = false;
                    return;
                }
                var parameter = ViewModelDxo.ToBroadcastParameter(externalSourceViewModel);
                var id = model.Service.BroadcastAsync(
                    yp.Model, yp.AcceptedHash, yp.Parameters.Dictionary, parameter,
                    new Progress<string>(x => parent.Feedback = x)).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        parent.OnException(t.Exception);
                        IsProcessing = false;
                        return;
                    }
                    model.Broadcast(yp.Model, parameter);
                    BroadcastingChannel = new BroadcastingChannel(parameter.Name, t.Result);
                    IsProcessing = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () =>
            {
                if (IsProcessing)
                    return false;
                if (BroadcastingChannel != null)
                    return false;
                if (!string.IsNullOrEmpty(parent.SettingsViewModel.Error))
                    return false;
                if (parent.YellowPagesListViewModel.SelectedYellowPages == null)
                    return false;
                if (!string.IsNullOrEmpty(externalSourceViewModel.Error)
                    || !string.IsNullOrEmpty(externalSourceViewModel.Name.Error)
                    || !string.IsNullOrEmpty(externalSourceViewModel.Genre.Error)
                    || !string.IsNullOrEmpty(externalSourceViewModel.Description.Error)
                    || !string.IsNullOrEmpty(externalSourceViewModel.Comment.Error))
                    return false;
                return true;
            });

            UpdateCommand = new DelegateCommand(() =>
            {
                IsProcessing = true;
                parent.ExternalSourceViewModel.UpdateHistory();
                var yp = parent.YellowPagesListViewModel.SelectedYellowPages;
                model.Service.UpdateAsync(
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
                if (!string.IsNullOrEmpty(parent.SettingsViewModel.Error))
                    return false;
                return true;
            });

            StopCommand = new DelegateCommand(() =>
            {
                IsProcessing = true;
                var yp = parent.YellowPagesListViewModel.SelectedYellowPages;
                model.Service.StopAsync(yp.Model, yp.Parameters.Dictionary,
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
                        model.Stop();
                        IsProcessing = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () =>
            {
                if (IsProcessing)
                    return false;
                if (BroadcastingChannel == null)
                    return false;
                if (!string.IsNullOrEmpty(parent.SettingsViewModel.Error))
                    return false;
                return true;
            });

            externalSourceViewModel.PropertyChanged += OnParameterPropertyChanged;
            externalSourceViewModel.Name.PropertyChanged += OnParameterPropertyChanged;
            externalSourceViewModel.Genre.PropertyChanged += OnParameterPropertyChanged;
            externalSourceViewModel.Description.PropertyChanged += OnParameterPropertyChanged;
            externalSourceViewModel.Comment.PropertyChanged += OnParameterPropertyChanged;
            parent.YellowPagesListViewModel.PropertyChanged += OnParameterPropertyChanged;
            parent.SettingsViewModel.PropertyChanged += OnParameterPropertyChanged;
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
        public Configuration Configuration { get { return m_model.Configuration; } } // カウントダウンボタンが使用

        private void OnParameterPropertyChanged(object sender, EventArgs e)
        {
            BroadcastCommand.OnCanExecuteChanged();
            UpdateCommand.OnCanExecuteChanged();
            StopCommand.OnCanExecuteChanged();
        }
    }
}
