using System.Collections.Generic;
using System.Linq;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Services;
using Progressive.PecaStarter5.Models.YellowPagesXml;
using Progressive.PecaStarter5.ViewModels.Controls;
using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net;
using Progressive.PecaStarter5.ViewModels.Dxos;

namespace Progressive.PecaStarter5.ViewModels
{
    public class MainPanelViewModel : ViewModelBase
    {
        public MainPanelViewModel(IEnumerable<string> yellowPagesList, Configuration configuration)
        {
            var peercast = new Peercast();
            var service = new PeercastService(peercast);
            var yellowPagesModels = new List<IYellowPages>();
            foreach (var xml in yellowPagesList)
            {
                yellowPagesModels.Add(YellowPagesParserFactory.GetInstance(xml).GetInstance());
            }
            RelayListViewModel = new RelayListViewModel(peercast, yellowPagesModels);
            YellowPagesListViewModel = new YellowPagesListViewModel(yellowPagesModels, configuration);
            ExternalSourceViewModel = new ExternalSourceViewModel() { Configuration = configuration };
            SettingsViewModel = new SettingsViewModel(configuration, peercast);
            BroadcastControlViewModel = new BroadcastControlViewModel(this, service, configuration);

            RelayListViewModel.ChannelSelected += (sender, e) =>
            {
                // YPタブを指定のYPに
                YellowPagesListViewModel.SelectedYellowPagesModel = RelayListViewModel.SelectedYellowPages;
                var ch = e.SelectedChannel;
                // ソースタブに値を反映
                var esvm = ExternalSourceViewModel;
                esvm.Name.Value = ch.Name;
                esvm.Genre.Value = YellowPagesListViewModel.SelectedYellowPages.Parse(ch.Genre);
                esvm.Description.Value = ch.Description;
                esvm.ContactUrl = ch.ContactUrl;
                esvm.Comment.Value = ch.Comment;
                esvm.Name.Value = ch.Name;
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

        private string alert;
        public string Alert
        {
            get { return alert; }
            set { SetProperty("Alert", ref alert, value); }
        }

        public BroadcastParameter BroadcastParameter
        {
            get { return ViewModelDxo.ToBroadcastParameter(this); }
        }

        public UpdateParameter UpdateParameter
        {
            get { return ViewModelDxo.ToUpdateParameter(this); }
        }
    }
}
