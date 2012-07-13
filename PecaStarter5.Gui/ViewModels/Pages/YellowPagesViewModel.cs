using System;
using System.Collections.Generic;
using System.Text;
using Progressive.PecaStarter5.Models;
using Progressive.Commons.ViewModels;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class YellowPagesViewModel : ViewModelBase
    {
        private IYellowPages model;

        public YellowPagesViewModel(IYellowPages model)
        {
            this.Parameters = new Dictionary<string, string>();
            this.model = model;
            foreach (var component in model.Components)
            {
                switch (component)
                {
                    case "listeners_invisibility":
                    case "listeners_visibility":
                    case "no_log":
                    case "time_invisibility":
                        Parameters[component] = "False";
                        continue;
                    default:
                        Parameters[component] = "";
                        continue;
                }
            }
        }

        public int AcceptedHash { get; set; }

        public string Name { get { return model.Name; } }

        public string NoticeUrl { get { return model.NoticeUrl; } }

        private bool isAccepted;
        public bool IsAccepted
        {
            get { return isAccepted; }
            set { SetProperty("IsAccepted", ref isAccepted, value); }
        }

        public IEnumerable<string> Keys { get { return Parameters.Keys; } }
        public Dictionary<string, string> Parameters { get; set; }

        public string Prefix
        {
            get
            {
                return model.GetPrefix(Parameters);
            }
        }
    }
}
