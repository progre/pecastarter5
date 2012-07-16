using System.Collections.Generic;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Commons.Models;
using Progressive.PecaStarter5.Models;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class YellowPagesViewModel : ViewModelBase
    {
        public IYellowPages Model { get; private set; }

        public YellowPagesViewModel(IYellowPages model)
        {
            this.Parameters = new DynamicStringDictionary();
            this.Model = model;
            foreach (var component in model.Components)
            {
                switch (component)
                {
                    case "listeners_invisibility":
                    case "listeners_visibility":
                    case "no_log":
                    case "time_invisibility":
                        Parameters.Dictionary[component] = "False";
                        continue;
                    default:
                        Parameters.Dictionary[component] = "";
                        continue;
                }
            }

            Parameters.PropertyChanged += (sender, e) =>
            {
                settings.Prefix = Prefix;
            };
        }

        private Settings.YellowPages settings;
        public Settings.YellowPages Settings
        {
            set
            {
                settings = value;
                var dic = parameters.Dictionary;
                foreach (var kv in Model.Parse(value.Prefix))
                {
                    dic[kv.Key] = kv.Value;
                }
            }
        }

        public int AcceptedHash
        {
            get { return settings.AcceptedHash; }
            set { settings.AcceptedHash = value; }
        }

        public string Name { get { return Model.Name; } }

        public string NoticeUrl { get { return Model.NoticeUrl; } }

        private bool isAccepted;
        public bool IsAccepted
        {
            get { return isAccepted; }
            set { SetProperty("IsAccepted", ref isAccepted, value); }
        }

        private DynamicStringDictionary parameters;
        public DynamicStringDictionary Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public string Prefix
        {
            get { return Model.GetPrefix(Parameters.Dictionary); }
        }
    }
}
