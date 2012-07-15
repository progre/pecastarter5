using System.Linq;
using System.Collections.Generic;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net;
using Progressive.PecaStarter5.Model;
using System.Windows.Input;
using Progressive.PecaStarter5.ViewModels.Controls;

namespace Progressive.PecaStarter5.ViewModels
{
    public class MainPanelViewModel : ViewModelBase
    {
        public MainPanelViewModel()
        {
            var peercast = new Peercast();
            var yellowPages = PecaStarter5Factory.YellowPagesList;
            BroadcastControlViewModel = new BroadcastControlViewModel(this);
            RelayListViewModel = new RelayListViewModel(peercast, yellowPages);
            YellowPagesListViewModel = new YellowPagesListViewModel(yellowPages);
            ExternalSourceViewModel = new ExternalSourceViewModel();
            SettingsViewModel = new SettingsViewModel();
        }

        public Settings Settings
        {
            set
            {
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
    }
}
