using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class YellowPagesListViewModel : ViewModelBase
    {
        public YellowPagesListViewModel(IEnumerable<IYellowPages> yellowPagesList)
        {
            YellowPagesViewModels = yellowPagesList.Select(x => new YellowPagesViewModel(x));
        }

        public IEnumerable<YellowPagesViewModel> YellowPagesViewModels { get; set; }

        private bool isSelectorEnabled;
        public bool IsSelectorEnabled
        {
            get { return isSelectorEnabled; }
            set { SetProperty("IsSelectorEnabled", ref isSelectorEnabled, value); }
        }

        private YellowPagesViewModel selectedYellowPages;
        public YellowPagesViewModel SelectedYellowPages
        {
            get { return selectedYellowPages; }
            set { SetProperty("SelectedYellowPages", ref selectedYellowPages, value); }
        }
    }
}
