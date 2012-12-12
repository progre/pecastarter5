using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Progressive.PecaStarter5.Models.Configurations;

namespace Progressive.PecaStarter5.Models.Plugins
{
    class PluginsModel : IDisposable
    {
        private readonly ConfigurationDao dao;

        public PluginsModel(ConfigurationDao dao, PluginList plugins)
        {
            this.dao = dao;
            Plugins = plugins;
            foreach (var plugin in Plugins)
            {
                plugin.IsEnabledChanged += (s, e) =>
                {
                    var p = (ExternalPlugin)s;
                    if (p.IsEnabled)
                        Initialize(p);
                    else
                        Terminate(p);
                };
            }
            InitializeAll();
        }

        public PluginList Plugins { get; private set; }

        private void InitializeAll()
        {
            Parallel.ForEach(Plugins, plugin =>
            {
                plugin.IsEnabled = Load(dao, plugin).IsEnabled;
            });
        }

        private void Initialize(ExternalPlugin plugin)
        {
            plugin.Instance.Repository = Load(dao, plugin).Repository
                ?? new Dictionary<string, object>();
            plugin.Instance.Initialize();
        }

        private PluginSettings Load(ConfigurationDao dao, ExternalPlugin plugin)
        {
            return dao.GetPluginSettings(
                plugin.AssemblyName,
                plugin.Instance.PluginInfo.Name);
        }

        #region IDisposable メンバー

        public void Dispose()
        {
            Parallel.ForEach(
                Plugins.Where(x => x.IsEnabled),
                plugin => Terminate(plugin));
        }

        #endregion

        private void Terminate(ExternalPlugin plugin)
        {
            plugin.Instance.Terminate();
            Save(dao, plugin);
        }

        public void Save()
        {
            Parallel.ForEach(Plugins.EnabledExternalPlugins, plugin =>
                Save(dao, plugin));
        }

        private void Save(ConfigurationDao dao, ExternalPlugin plugin)
        {
            dao.PutPluginSettings(
                plugin.AssemblyName,
                plugin.Instance.PluginInfo.Name,
                new PluginSettings
                {
                    IsEnabled = plugin.IsEnabled,
                    Repository = plugin.Instance.Repository
                });
        }
    }
}
