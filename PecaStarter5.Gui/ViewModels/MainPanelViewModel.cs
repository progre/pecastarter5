using System;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Channels;
using Progressive.PecaStarter5.Models.Plugins;
using Progressive.PecaStarter5.Models.Services;
using Progressive.PecaStarter5.ViewModels.Controls;
using Progressive.PecaStarter5.ViewModels.Pages;

namespace Progressive.PecaStarter5.ViewModels
{
    public class MainPanelViewModel : ViewModelBase
    {
        private PecaStarterModel m_model;

        public MainPanelViewModel(PecaStarterModel model)
        {
            // Models
            m_model = model;
            var tuple = model.GetYellowPagesLists();
            var yellowPagesList = tuple.Item1;
            var externalYellowPagesList = tuple.Item2;
            var service = new PeercastService(m_model.Peercast, externalYellowPagesList, m_model.Plugins);

            // タブ情報の初期化
            RelayListViewModel = new RelayListViewModel(m_model.Peercast, yellowPagesList);
            YellowPagesListViewModel = new YellowPagesListViewModel(yellowPagesList, model.Configuration);
            ExternalSourceViewModel = new ExternalSourceViewModel(model.Configuration);
            SettingsViewModel = new SettingsViewModel(model.Configuration, m_model.Peercast, m_model.LoggerPlugin);
            BroadcastControlViewModel = new BroadcastControlViewModel(this, m_model, service, model.Configuration);

            InitializeEvents();
        }

        public RelayListViewModel RelayListViewModel { get; private set; }
        public YellowPagesListViewModel YellowPagesListViewModel { get; private set; }
        public ExternalSourceViewModel ExternalSourceViewModel { get; private set; }
        public SettingsViewModel SettingsViewModel { get; private set; }
        public BroadcastControlViewModel BroadcastControlViewModel { get; private set; }

        private int m_selectedIndex;
        public int SelectedIndex
        {
            get { return m_selectedIndex; }
            set
            {
                SetProperty("SelectedIndex", ref m_selectedIndex, value);
                if (value == 3 && YellowPagesListViewModel.SelectedYellowPages != null)
                    ExternalSourceViewModel.Prefix = YellowPagesListViewModel.SelectedYellowPages.Prefix;
            }
        }

        public string Alert { get; set; }

        private string m_feedback = "";
        public string Feedback
        {
            get { return m_feedback; }
            set { SetProperty("Feedback", ref m_feedback, value); }
        }

        private void InitializeEvents()
        {
            m_model.AsyncExceptionThrown += (sender, e) =>
            {
                NotifyExceptionAlert((Exception)e.ExceptionObject);
                // TODO: 同期オブジェクトに渡さないとどうなるか実験
                //SynchronizationContext.Current.Post(s => NotifyExceptionAlert((Exception)s), e.ExceptionObject);
            };

            RelayListViewModel.ChannelSelected += (sender, e) =>
            {
                var ch = e.Channel;
                // YPタブを指定のYPに
                YellowPagesListViewModel.SelectedYellowPagesModel = e.YellowPages;

                // ソースタブに値を反映
                var esvm = ExternalSourceViewModel;
                esvm.Name.Value = ch.Name;
                esvm.Genre.Value = YellowPagesListViewModel.SelectedYellowPages.Parse(ch.Genre);
                esvm.Description.Value = ch.Description;
                esvm.ContactUrl = ch.ContactUrl;
                esvm.Comment.Value = ch.Comment;
                esvm.Name.Value = ch.Name;

                // 配信ボタンに反映
                BroadcastControlViewModel.BroadcastingChannel = new BroadcastingChannel(ch.Name, ch.Id);

                // ソースタブに移動
                SelectedIndex = 3;

                // plugin処理
                var parameter = new InterruptedParameter()
                {
                    Name = ch.Name,
                    Genre = ch.Genre,
                    Description = ch.Description,
                    Comment = ch.Comment
                };
                m_model.Interrupt(parameter);
            };

            RelayListViewModel.ExceptionThrown += (sender, e) =>
            {
                Feedback = "中止";
                NotifyExceptionAlert((Exception)e.ExceptionObject);
            };

            BroadcastControlViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "BroadcastingChannel")
                    return;

                if (((BroadcastControlViewModel)sender).BroadcastingChannel == null)
                {
                    YellowPagesListViewModel.IsLocked = false;
                    if (YellowPagesListViewModel.SelectedYellowPages != null)
                        YellowPagesListViewModel.SelectedYellowPages.IsLocked = false;
                    ExternalSourceViewModel.IsLocked = false;
                }
                else
                {
                    YellowPagesListViewModel.IsLocked = true;
                    if (YellowPagesListViewModel.SelectedYellowPages != null)
                        YellowPagesListViewModel.SelectedYellowPages.IsLocked = true;
                    ExternalSourceViewModel.IsLocked = true;
                }
            };
        }

        private void NotifyExceptionAlert(Exception ex)
        {
            var aggregateException = ex as AggregateException;
            if (aggregateException != null && aggregateException.InnerExceptions.Count >= 1)
            {
                foreach (var ex1 in aggregateException.InnerExceptions)
                {
                    NotifyAlert(ex1.Message + Environment.NewLine + ex1.StackTrace);
                }
                return;
            }
            else
            {
                NotifyAlert(ex.Message + ex.StackTrace);
                return;
            }
        }

        private void NotifyAlert(string value)
        {
            Alert = value;
            OnPropertyChanged("Alert");
        }
    }
}
