using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Progressive.PecaStarter5.Daos;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.YellowPagesXml;

namespace Progressive.PecaStarter5
{
    static class ExternalResource
    {
        public static Configuration Configuration
        {
            get { return new ConfigurationDao() { FilePath = ConfigurationFilePath }.Get(); }
            set { new ConfigurationDao() { FilePath = ConfigurationFilePath }.Put(value); }
        }

        private static string ConfigurationFilePath
        {
            get { return ApplicationPath + Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".xml"; }
        }

        public static IEnumerable<string> YellowPagesList
        {
            get
            {
                try
                {
                    return Directory.EnumerateFiles(YellowPagesDirectoryPath, "*.xml");
                }
                catch (DirectoryNotFoundException)
                {
                    return Enumerable.Empty<string>();
                }
            }
        }

        private static string YellowPagesDirectoryPath
        {
            get { return ApplicationPath + "yellowpages" + Path.DirectorySeparatorChar; }
        }

        private static string ApplicationPath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar;
            }
        }
    }
}
