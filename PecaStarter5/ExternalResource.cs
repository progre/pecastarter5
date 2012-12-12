using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Plugins;
using System.Text.RegularExpressions;
using Progressive.PecaStarter5.Models.Plugins;

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

        public Stream GetPluginSettingsInputStream(string pluginName)
        {
            return new FileStream(PluginDirectoryPath + pluginName, FileMode.Open);
        }

        public Stream GetPluginSettingsOutputStream(string pluginName)
        {
            return new FileStream(PluginDirectoryPath + pluginName, FileMode.Create);
        }

        public IEnumerable<Stream> GetYellowPagesDefineInputStream()
        {
            foreach (var path in Directory.GetFiles(YellowPagesDirectoryPath, "*.xml"))
            {
                yield return new FileStream(path, FileMode.Open);
            }
        }

        public PluginList GetPlugins()
        {
            var iPluginName = typeof(IPlugin).FullName;

            var list = new PluginList();
            string[] files;
            try
            {
                files = Directory.GetFiles(PluginDirectoryPath, "*.dll")
                    .Concat(Directory.GetDirectories(PluginDirectoryPath)
                        .SelectMany(x => Directory.GetFiles(x, "*.dll")))
                    .ToArray();
            }
            catch
            {
                return new PluginList();
            }
            foreach (var path in files)
            {
                try
                {
                    var assembly = Assembly.LoadFile(path);
                    list.AddRange(assembly.GetTypes()
                        .Where(x => x.IsClass && !x.IsAbstract && !x.IsNotPublic
                            && x.GetInterface(iPluginName) != null)
                        .Select(x => new ExternalPlugin(
                            GetRelativeFileNameWithoutExtension(path),
                            (IPlugin)Activator.CreateInstance(x))));
                }
                catch
                {
                    continue;
                }
            }
            return list;
        }

        private string GetRelativeFileNameWithoutExtension(string path)
        {
            return Regex.Replace(path.Replace(PluginDirectoryPath, ""),
                @"\.DLL", "", RegexOptions.IgnoreCase);
        }
    }
}
