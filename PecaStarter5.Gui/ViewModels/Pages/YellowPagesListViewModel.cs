﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class YellowPagesListViewModel : ViewModelBase
    {
        public YellowPagesListViewModel(IEnumerable<IYellowPages> yellowPagesList)
        {
            YellowPagesViewModels = yellowPagesList.Select(x => new YellowPagesViewModel(x)).ToArray();
        }

        private Settings settings;
        public Settings Settings
        {
            set
            {
                settings = value;
                var selectedYellowPages = value.SelectedYellowPages;
                if (!string.IsNullOrEmpty(selectedYellowPages))
                    SelectedYellowPages = YellowPagesViewModels.SingleOrDefault(x => x.Name == selectedYellowPages);
                foreach (var yp in YellowPagesViewModels)
                {
                    var ypSetting = value.YellowPagesList.FirstOrDefault(x => x.Name == yp.Name);
                    if (ypSetting == null)
                    {
                        ypSetting = new Settings.YellowPages() { Name = yp.Name };
                        value.YellowPagesList.Add(ypSetting);
                    }
                    yp.Settings = ypSetting;
                }
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
            }
        }
    }
}
