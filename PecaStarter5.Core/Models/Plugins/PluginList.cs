using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.PecaStarter5.Plugins;

namespace Progressive.PecaStarter5.Models.Plugins
{
    public class PluginList : List<ExternalPlugin>
    {
        public IEnumerable<ExternalPlugin> EnabledExternalPlugins
        {
            get { return this.Where(x => x.IsEnabled); }
        }

        public IEnumerable<IPlugin> EnabledPlugins
        {
            get { return this.Where(x => x.IsEnabled).Select(x => x.Instance); }
        }

        public IEnumerable<IPlugin> GetCurrentTimePlugins(long count)
        {
            return EnabledPlugins
                .Where(x =>
                {
                    var tickInterval = x.PluginConfiguration.TickInterval;
                    return tickInterval > 0 && count % tickInterval == 0;
                });
        }
    }
}
