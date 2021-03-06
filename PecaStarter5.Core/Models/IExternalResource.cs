﻿using System.Collections.Generic;
using System.IO;
using Progressive.PecaStarter5.Plugins;
using Progressive.PecaStarter5.Models.Plugins;

namespace Progressive.PecaStarter5.Models
{
    public interface IExternalResource
    {
        Stream GetConfigurationInputStream();
        Stream GetConfigurationOutputStream();
        Stream GetPluginSettingsInputStream(string pluginName);
        Stream GetPluginSettingsOutputStream(string pluginName);
        IEnumerable<Stream> GetYellowPagesDefineInputStream();
        PluginList GetPlugins();
        string DefaultLogPath { get; }
    }
}
