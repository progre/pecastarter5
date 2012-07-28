using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Channels;
using Progressive.PecaStarter5.Models.ExternalYellowPages;
using Progressive.PecaStarter5.Models.Plugins;
using Progressive.PecaStarter5.Models.Services;
using Progressive.PecaStarter5.Models.YellowPagesXml;
using Progressive.PecaStarter5.ViewModels.Controls;
using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.ViewModels
{
    public class MainPanelViewModel : ViewModelBase
    {
        private Models.PecaStarter pecaStarter;

        public MainPanelViewModel(IEnumerable<string> yellowPagesXmls, Configuration configuration)
        {
            // Models
            this.pecaStarter = new Models.PecaStarter();
            pecaStarter.ExceptionThrown += (sender, e) => OnException((Exception)e.ExceptionObject);
            var tuple = GetYellowPagesLists(yellowPagesXmls);
            var yellowPagesList = tuple.Item1;
            var externalYellowPagesList = tuple.Item2;
            var service = new PeercastService(pecaStarter.Peercast, externalYellowPagesList, pecaStarter.Plugins);

            // タブ情報の初期化
            RelayListViewModel = new RelayListViewModel(pecaStarter.Peercast, yellowPagesList);
            YellowPagesListViewModel = new YellowPagesListViewModel(yellowPagesList, configuration);
            ExternalSourceViewModel = new ExternalSourceViewModel(configuration);
            SettingsViewModel = new SettingsViewModel(configuration, pecaStarter.Peercast, pecaStarter.LoggerPlugin);
            BroadcastControlViewModel = new BroadcastControlViewModel(this, pecaStarter, service, configuration);

            InitializeEvents();
        }

        public BroadcastControlViewModel BroadcastControlViewModel { get; private set; }
        public RelayListViewModel RelayListViewModel { get; private set; }
        public YellowPagesListViewModel YellowPagesListViewModel { get; private set; }
        public ExternalSourceViewModel ExternalSourceViewModel { get; private set; }
        public SettingsViewModel SettingsViewModel { get; private set; }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                SetProperty("SelectedIndex", ref selectedIndex, value);
                if (value == 3 && YellowPagesListViewModel.SelectedYellowPages != null)
                    ExternalSourceViewModel.Prefix = YellowPagesListViewModel.SelectedYellowPages.Prefix;
            }
        }

        public string Alert { get; set; }

        private string feedback = "";
        public string Feedback
        {
            get { return feedback; }
            set { SetProperty("Feedback", ref feedback, value); }
        }

        private void InitializeEvents()
        {
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
                pecaStarter.Interrupt(parameter);
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

        public void OnException(Exception ex)
        {
            // ダイアログ通知
            Feedback = "中止";

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
            }
        }

        private Tuple<List<IYellowPages>, List<IExternalYellowPages>> GetYellowPagesLists(
            IEnumerable<string> yellowPagesXmls)
        {
            var yellowPagesList = new List<IYellowPages>();
            var externalYellowPagesList = new List<IExternalYellowPages>();
            foreach (var xml in yellowPagesXmls)
            {
                var yp = YellowPagesParserFactory.GetInstance(xml).GetInstance();
                yellowPagesList.Add(yp);
                if (yp.IsExternal)
                {
                    externalYellowPagesList.Add((IExternalYellowPages)yp);
                }
            }
            return Tuple.Create(yellowPagesList, externalYellowPagesList);
        }

        public void NotifyAlert(string value)
        {
            Alert = value;
            OnPropertyChanged("Alert");
        }
    }
}
