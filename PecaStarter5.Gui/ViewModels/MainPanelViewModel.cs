using System.Collections.Generic;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net;
using Progressive.Commons.ViewModels;
using System.Linq;

namespace Progressive.PecaStarter5.ViewModels
{
    public class MainPanelViewModel : ViewModelBase
    {
        public MainPanelViewModel()
        {
            var peercast = new Peercast();
            IEnumerable<IYellowPages> yellowPages = Enumerable.Empty<IYellowPages>();
            RelayListViewModel = new RelayListViewModel(peercast, yellowPages);
            YellowPagesListViewModel = new YellowPagesListViewModel(yellowPages);
        }

        public RelayListViewModel RelayListViewModel { get; private set; }
        public YellowPagesListViewModel YellowPagesListViewModel { get; private set; }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SetProperty("SelectedIndex", ref selectedIndex, value); }
        }
    }
}
