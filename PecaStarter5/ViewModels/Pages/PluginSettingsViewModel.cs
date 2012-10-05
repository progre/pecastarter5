using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.Commons.ViewModels;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Plugins;
using System.Windows.Input;
using Progressive.Commons.ViewModels.Commands;
using System.Windows;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    class PluginSettingsViewModel : ViewModelBase
    {
        public PluginSettingsViewModel(IEnumerable<ExternalPlugin> plugins)
        {
            Plugins = plugins.Select(x => new PluginColumnViewModel(x)).ToList();
        }

        public IList<PluginColumnViewModel> Plugins { get; private set; }
    }
}
