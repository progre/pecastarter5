using System.Collections.Generic;
using System.IO;
using Progressive.PecaStarter5.Plugins;

namespace Progressive.PecaStarter5.Models
{
    public interface IExternalResource
    {
        Stream GetConfigurationInputStream();
        Stream GetConfigurationOutputStream();
        Stream GetPluginSettingsInputStream(string pluginName);
        Stream GetPluginSettingsOutputStream(string pluginName);
        IEnumerable<Stream> GetYellowPagesDefineInputStream();
        IList<ExternalPlugin> GetPlugins();
        string DefaultLogPath { get; }
    }
}
