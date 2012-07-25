using System.Collections.Generic;
using System.Linq;
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
using System;

namespace Progressive.PecaStarter5.ViewModels
{
    public class MainPanelViewModel : ViewModelBase
    {
        public MainPanelViewModel(IEnumerable<string> yellowPagesXmls, Configuration configuration)
        {
            // Models
            var peercast = new Peercast();
            var tuple = GetYellowPagesLists(yellowPagesXmls);
            var yellowPagesList = tuple.Item1;
            var externalYellowPagesList = tuple.Item2;
            var service = new PeercastService(peercast, externalYellowPagesList, Enumerable.Empty<IPlugin>());

            // タブ情報の初期化
            RelayListViewModel = new RelayListViewModel(peercast, yellowPagesList);
            YellowPagesListViewModel = new YellowPagesListViewModel(yellowPagesList, configuration);
            ExternalSourceViewModel = new ExternalSourceViewModel(configuration);
            SettingsViewModel = new SettingsViewModel(configuration, peercast);
            BroadcastControlViewModel = new BroadcastControlViewModel(this, service, configuration);

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
                // ログタイマー
                //var ypvm = viewModel.YellowPagesesViewModel.SelectedYellowPages;
                //var yp = yellowPageses.Single(a => a.Name == ypvm.Name);
                //if (yp is WebApiYellowPages || viewModel.SettingsViewModel.Logging)
                //{
                //    viewModel.BeginTimer();
                //}
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
