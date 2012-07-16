using System.Collections.Generic;
using System.Linq;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class YellowPagesListViewModel : ViewModelBase
    {
        private Settings settings;

        public YellowPagesListViewModel(IEnumerable<IYellowPages> yellowPagesList, Settings settings,
            TaskQueue taskQueue)
        {
            this.YellowPagesViewModels = yellowPagesList.Select(x => new YellowPagesViewModel(x, taskQueue)).ToArray();
            this.settings = settings;

            SelectedYellowPages = YellowPagesViewModels.SingleOrDefault(x => x.Name == settings.SelectedYellowPages);
            foreach (var yp in YellowPagesViewModels)
            {
                yp.PropertyChanged += (sender, e) => OnPropertyChanged("Prefix");

                var ypSetting = settings.YellowPagesList.FirstOrDefault(x => x.Name == yp.Name);
                if (ypSetting == null)
                {
                    ypSetting = new Settings.YellowPages() { Name = yp.Name };
                    settings.YellowPagesList.Add(ypSetting);
                }
                yp.Settings = ypSetting;
            }
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
            set
            {
                SetProperty("SelectedYellowPages", ref selectedYellowPages, value);
                settings.SelectedYellowPages = value.Name;
                OnPropertyChanged("Prefix");
            }
        }

        public string Prefix
        {
            get { return SelectedYellowPages.Prefix; }
        }
    }
}
