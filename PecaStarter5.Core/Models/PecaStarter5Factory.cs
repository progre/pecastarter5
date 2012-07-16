using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Progressive.PecaStarter5.Models.YellowPagesXml;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Daos;

namespace Progressive.PecaStarter5.Models
{
    public static class PecaStarter5Factory
    {
        public static IEnumerable<IYellowPages> YellowPagesList
        {
            get
            {
                var path = ApplicationPath + "yellowpages" + Path.DirectorySeparatorChar;
                var list = new List<IYellowPages>();
                try
                {
                    return Directory.EnumerateFiles(path, "*.xml")
                        .Select(x => YellowPagesParserFactory.GetInstance(x).GetInstance());
                }
                catch (DirectoryNotFoundException)
                {
                    return Enumerable.Empty<IYellowPages>();
                }
            }
        }

        private static string ApplicationPath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar;
            }
        }

        public static Settings Settings
        {
            get { return new SettingsDao() { FilePath = SettingsFilePath }.Get(); }
            set { new SettingsDao() { FilePath = SettingsFilePath }.Put(value); }
        }

        private static string SettingsFilePath
        {
            get { return ApplicationPath + Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location) + ".xml"; }
        }
    }
}
