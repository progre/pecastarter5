using System.Collections.Generic;
using System.IO;
using Progressive.PecaStarter5.Models;

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

        private string ConfigurationFilePath
        {
            get { return m_path + m_name + ".xml"; }
        }

        private string YellowPagesDirectoryPath
        {
            get { return m_path + "yellowpages" + Path.DirectorySeparatorChar; }
        }
    }
}
