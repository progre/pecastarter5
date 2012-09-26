using System.Collections.Generic;
using System.Linq;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.YellowPages;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    internal class YellowPagesListViewModel : ViewModelBase
    {
        private Configuration settings;

        public YellowPagesListViewModel(IEnumerable<IYellowPages> yellowPagesList, Configuration settings)
        {
            this.YellowPagesViewModels = yellowPagesList.Select(x => new YellowPagesViewModel(x)).ToArray();
            this.settings = settings;

            var recent = settings.SelectedYellowPages;
            SelectedYellowPages = YellowPagesViewModels.SingleOrDefault(x => x.Name == recent);
            foreach (var yp in YellowPagesViewModels)
            {
                yp.PropertyChanged += (sender, e) => OnPropertyChanged("Prefix");

                var ypSetting = settings.YellowPagesList.FirstOrDefault(x => x.Name == yp.Name);
                if (ypSetting == null)
                {
                    ypSetting = new Configuration.YellowPages() { Name = yp.Name };
                    settings.YellowPagesList.Add(ypSetting);
                }
                yp.Settings = ypSetting;
            }
        }

        public IEnumerable<YellowPagesViewModel> YellowPagesViewModels { get; set; }

        private bool isLocked;
        public bool IsLocked
        {
            get { return isLocked; }
            set { SetProperty("IsLocked", ref isLocked, value); }
        }

        private YellowPagesViewModel selectedYellowPages;
        public YellowPagesViewModel SelectedYellowPages
        {
            get { return selectedYellowPages; }
            set
            {
                SetProperty("SelectedYellowPages", ref selectedYellowPages, value);
                settings.SelectedYellowPages = value != null ? value.Name : null;
                OnPropertyChanged("Prefix");
            }
        }

        public IYellowPages SelectedYellowPagesModel
        {
            get { return selectedYellowPages.Model; }
            set
            {
                SelectedYellowPages = YellowPagesViewModels.First(x => x.Model.Equals(value));
            }
        }

        public string Prefix
        {
            get { return SelectedYellowPages.Prefix; }
        }
    }
}
