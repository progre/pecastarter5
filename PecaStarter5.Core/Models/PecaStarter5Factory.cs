using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Progressive.PecaStarter5.Models.YellowPagesXml;

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
    }
}
