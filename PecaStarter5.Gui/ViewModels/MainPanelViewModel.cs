using System.Collections.Generic;
using System.Linq;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Services;
using Progressive.PecaStarter5.Models.YellowPagesXml;
using Progressive.PecaStarter5.ViewModels.Controls;
using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.ViewModels
{
    public class MainPanelViewModel : ViewModelBase
    {
        private TaskQueue taskQueue = new TaskQueue();

        public MainPanelViewModel(IEnumerable<string> yellowPagesList, Settings settings)
        {
            var peercast = new Peercast();
            var channelParameter = new ChannelParameter();
            var yellowPages = yellowPagesList.Select(x => YellowPagesParserFactory.GetInstance(x).GetInstance()); ;
            RelayListViewModel = new RelayListViewModel(peercast, yellowPages);
            YellowPagesListViewModel = new YellowPagesListViewModel(yellowPages, settings, taskQueue);
            ExternalSourceViewModel = new ExternalSourceViewModel();
            SettingsViewModel = new SettingsViewModel();
            BroadcastControlViewModel = new BroadcastControlViewModel(this, new PeercastService(peercast));

            ExternalSourceViewModel.Settings = settings;
            SettingsViewModel.Settings = settings;
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
    }
}
