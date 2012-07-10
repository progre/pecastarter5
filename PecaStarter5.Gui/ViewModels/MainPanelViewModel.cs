using System.Collections.Generic;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net;
using Progressive.Commons.ViewModels;

namespace Progressive.PecaStarter5.ViewModels
{
    public class MainPanelViewModel : ViewModelBase
    {
        public MainPanelViewModel()
        {
            var peercast = new Peercast();
            IEnumerable<IYellowPages> yellowPages = null;
            RelayListViewModel = new RelayListViewModel(peercast, yellowPages);
        }

        public RelayListViewModel RelayListViewModel { get; set; }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SetProperty(ref selectedIndex, "SelectedIndex", value); }
        }
    }
}
