using System.Linq;
using System.Collections.Generic;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net;
using Progressive.PecaStarter5.Models;
using System.Windows.Input;
using Progressive.PecaStarter5.ViewModels.Controls;

namespace Progressive.PecaStarter5.ViewModels
{
    public class MainPanelViewModel : ViewModelBase
    {
        private TaskQueue taskQueue = new TaskQueue();

        public MainPanelViewModel()
        {
            var peercast = new Peercast();
            var yellowPages = PecaStarter5Factory.YellowPagesList;
            BroadcastControlViewModel = new BroadcastControlViewModel(this);
            RelayListViewModel = new RelayListViewModel(peercast, yellowPages);
            YellowPagesListViewModel = new YellowPagesListViewModel(yellowPages, taskQueue);
            ExternalSourceViewModel = new ExternalSourceViewModel();
            SettingsViewModel = new SettingsViewModel();
        }

        public Settings Settings
        {
            set
            {
                YellowPagesListViewModel.Settings = value;
                ExternalSourceViewModel.Settings = value;
                SettingsViewModel.Settings = value;
            }
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
