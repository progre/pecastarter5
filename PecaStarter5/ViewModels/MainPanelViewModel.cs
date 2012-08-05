using System;
using System.Threading;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Channels;
using Progressive.PecaStarter5.Models.Plugins;
using Progressive.PecaStarter5.ViewModels.Controls;
using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.ViewModels
{
    public class MainPanelViewModel : ViewModelBase
    {
        public const int YellowPagesTabIndex = 2;
        public const int ExternalSourceTabIndex = 3;

        private readonly PecaStarterModel m_model;
        private readonly ChannelViewModel m_channelViewModel;

        public MainPanelViewModel(PecaStarterModel model)
        {
            // Models
            m_model = model;

            // タブ情報の初期化
            RelayListViewModel = new RelayListViewModel(model.Service, model.YellowPagesList);
            m_channelViewModel = new ChannelViewModel(model.YellowPagesList, model.Configuration);
            SettingsViewModel = new SettingsViewModel(model.Configuration, model.LoggerPlugin);
            BroadcastControlViewModel = new BroadcastControlViewModel(this, model);

            InitializeEvents();
        }

        public string Alert { get; set; }

        private string m_feedback = "";
        public string Feedback
        {
            get { return m_feedback; }
            set { SetProperty("Feedback", ref m_feedback, value); }
        }

        public BroadcastControlViewModel BroadcastControlViewModel { get; private set; }

        private int m_selectedIndex = YellowPagesTabIndex;
        public int SelectedIndex
        {
            get { return m_selectedIndex; }
            set
            {
                SetProperty("SelectedIndex", ref m_selectedIndex, value);
                if (value == 3)
                    m_channelViewModel.SyncPrefix();
            }
        }

        public RelayListViewModel RelayListViewModel { get; private set; }
        public YellowPagesListViewModel YellowPagesListViewModel
        {
            get { return m_channelViewModel.YellowPagesListViewModel; }
        }
        public ExternalSourceViewModel ExternalSourceViewModel
        {
            get { return m_channelViewModel.ExternalSourceViewModel; }
        }
        public SettingsViewModel SettingsViewModel { get; private set; }

        private void InitializeEvents()
        {
            var syncContext = SynchronizationContext.Current;
            m_model.AsyncExceptionThrown += (sender, e) =>
                syncContext.Post(s => NotifyExceptionAlert((Exception)s), e.ExceptionObject);

            BroadcastControlViewModel.PropertyChanged += (sender, e) =>
            {
                var propertyName = e.PropertyName;
                if (propertyName != "IsProcessing" && propertyName != "BroadcastingChannel")
                    return;

                var vm = (BroadcastControlViewModel)sender;
                if (vm.IsProcessing || vm.BroadcastingChannel != null)
                    m_channelViewModel.Lock();
                else
                    m_channelViewModel.Unlock();
            };

            RelayListViewModel.ChannelSelected += (sender, e) =>
            {
                var ch = e.Channel;
                // 配信ボタンに反映
                BroadcastControlViewModel.BroadcastingChannel = new BroadcastingChannel(ch.Name, ch.Id);
                // YPタブとソースタブに反映
                m_channelViewModel.LoadChannel(e.YellowPages, ch);
                // ソースタブに移動
                SelectedIndex = ExternalSourceTabIndex;
                // plugin処理
                m_model.Interrupt(e.YellowPages, new InterruptedParameter(ch));
            };
            RelayListViewModel.ExceptionThrown += (sender, e) => OnException((Exception)e.ExceptionObject);
        }

        public void OnException(Exception ex)
        {
            Feedback = "中止";
            NotifyExceptionAlert(ex);
        }

        private void NotifyExceptionAlert(Exception ex)
        {
            if (ex is PeercastException)
            {
                NotifyAlert(ex.Message);
                return;
            }
            if (ex is YellowPagesException)
            {
                NotifyAlert(ex.Message);
                SelectedIndex = YellowPagesTabIndex;
                YellowPagesListViewModel.SelectedYellowPages.IsAccepted = false;
                return;
            }
            var aggregateException = ex as AggregateException;
            if (aggregateException != null && aggregateException.InnerExceptions.Count >= 1)
            {
                foreach (var ex1 in aggregateException.InnerExceptions)
                {
                    NotifyExceptionAlert(ex1);
                }
                return;
            }
            NotifyAlert(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace);
        }

        private void NotifyAlert(string value)
        {
            Alert = value;
            OnPropertyChanged("Alert");
            Alert = "";
            OnPropertyChanged("Alert");
        }
    }
}
