using System;
using System.Windows.Input;
using Progressive.Commons.ViewModels.Commands;
using Progressive.PecaStarter5.Plugin;
using Progressive.PecaStarter5.Models;
using Progressive.Commons.ViewModels;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    class PluginColumnViewModel:ViewModelBase
    {
        private ExternalPlugin plugin;

        public PluginColumnViewModel(ExternalPlugin plugin)
        {
            this.plugin = plugin;
            SettingsCommand = new DelegateCommand(
                () => plugin.Instance.ShowSettingsDialog(),
                () => plugin.Instance.HasSettingsDialog);
            DisablingCommand = new DelegateCommand(
                () => IsEnabled = !IsEnabled);
        }
        public string Name { get { return plugin.Instance.Name; } }
        public Version Version { get { return plugin.Instance.Version; } }
        public bool IsEnabled
        {
            get { return plugin.IsEnabled; }
            set { SetProperty("IsEnabled", plugin.IsEnabled, x => plugin.IsEnabled = x, value); }
        }
        public ICommand SettingsCommand { get; private set; }
        public ICommand DisablingCommand { get; private set; }
    }
}
