using System;
using System.Windows.Input;
using Progressive.Commons.ViewModels.Commands;
using Progressive.PecaStarter5.Plugin;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    class PluginColumnViewModel
    {
        private IPlugin plugin;

        public PluginColumnViewModel(IPlugin plugin)
        {
            this.plugin = plugin;
            SettingsCommand = new DelegateCommand(
                () => { plugin.ShowSettingsDialog(); },
                () => plugin.HasSettingsDialog);
        }
        public string Name { get { return plugin.Name; } }
        public Version Version { get { return plugin.Version; } }
        public ICommand SettingsCommand { get; private set; }
    }
}
