using System.Collections.Generic;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.ViewModels
{
    public class MainPanelViewModel
    {
        public RelayListViewModel RelayListViewModel { get; set; }

        public MainPanelViewModel()
        {
            var peercast = new Peercast();
            IEnumerable<IYellowPages> yellowPages = null;
            RelayListViewModel = new RelayListViewModel(peercast, yellowPages);
        }
    }
}
