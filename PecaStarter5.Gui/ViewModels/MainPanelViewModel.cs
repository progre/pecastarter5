using System;
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
        private PecaStarterModel m_model;

        public MainPanelViewModel(PecaStarterModel model)
        {
            // Models
            m_model = model;

            // タブ情報の初期化
            RelayListViewModel = new RelayListViewModel(model.Service, model.YellowPagesList);
            YellowPagesListViewModel = new YellowPagesListViewModel(model.YellowPagesList, model.Configuration);
            ExternalSourceViewModel = new ExternalSourceViewModel(model.Configuration);
            SettingsViewModel = new SettingsViewModel(model.Configuration, model.LoggerPlugin);
            BroadcastControlViewModel = new BroadcastControlViewModel(this, model);

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

        public void OnException(Exception ex)
        {
            Feedback = "中止";
            NotifyExceptionAlert(ex);
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
                    Contact = ch.ContactUrl,
                    Comment = ch.Comment
                };
                m_model.Interrupt(e.YellowPages, parameter);
            };

            RelayListViewModel.ExceptionThrown += (sender, e) => OnException((Exception)e.ExceptionObject);

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
            if (ex is PeercastException)
            {
                NotifyAlert(ex.Message);
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
        }
    }
}
