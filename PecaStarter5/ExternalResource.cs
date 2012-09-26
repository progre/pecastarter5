using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Plugin;

namespace Progressive.PecaStarter5
{
    class ExternalResource : IExternalResource
    {
        private readonly string m_name;
        private readonly string m_path;

        public ExternalResource(string name, string path)
        {
            m_name = name;
            m_path = path;
        }

        public string DefaultLogPath
        {
            get { return m_path + "log"; }
        }

        private string ConfigurationFilePath
        {
            get { return m_path + m_name + ".xml"; }
        }

        private string YellowPagesDirectoryPath
        {
            get { return m_path + "yellowpages" + Path.DirectorySeparatorChar; }
        }

        private string PluginDirectoryPath
        {
            get { return m_path + "plugins" + Path.DirectorySeparatorChar; }
        }

        public Stream GetConfigurationInputStream()
        {
            return new FileStream(ConfigurationFilePath, FileMode.Open);
        }

        public Stream GetConfigurationOutputStream()
        {
            return new FileStream(ConfigurationFilePath, FileMode.Create);
        }

        public IEnumerable<Stream> GetYellowPagesDefineInputStream()
        {
            foreach (var path in Directory.GetFiles(YellowPagesDirectoryPath, "*.xml"))
            {
                yield return new FileStream(path, FileMode.Open);
            }
        }

        public IEnumerable<IPlugin> GetPlugins()
        {
            var iPluginName = typeof(IPlugin).FullName;

            return Directory.GetFiles(PluginDirectoryPath, "*.dll")
                .Select(x => LoadAssemblyFile(x))
                .Where(x => x != null)
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && !x.IsAbstract && !x.IsNotPublic
                    && x.GetInterface(iPluginName) != null)
                .Select(x => (IPlugin)Activator.CreateInstance(x));
        }

        private Assembly LoadAssemblyFile(string path)
        {
            try
            {
                return Assembly.LoadFile(path);
            }
            catch
            {
                return null;
            }
        }
    }
}
