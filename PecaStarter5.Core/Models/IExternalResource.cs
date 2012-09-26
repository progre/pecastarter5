using System.Collections.Generic;
using System.IO;
using Progressive.PecaStarter5.Plugin;

namespace Progressive.PecaStarter5.Models
{
    public interface IExternalResource
    {
        Stream GetConfigurationInputStream();
        Stream GetConfigurationOutputStream();
        IEnumerable<Stream> GetYellowPagesDefineInputStream();
        IEnumerable<IPlugin> GetPlugins();
        string DefaultLogPath { get; }
    }
}
