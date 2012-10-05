using System;
using System.IO;
using System.Xml.Linq;

namespace Progressive.PecaStarter5.Models.YellowPages.YellowPagesXml
{
    internal class YellowPagesParserFactory
    {
        public static YellowPagesParser GetInstance(Stream stream)
        {
            var xml = XDocument.Load(stream);
            var list = xml.Elements();
            if (xml.Element("yellowpages").Element("peercast") != null)
            {
                return new PeercastYellowPagesParser(xml);
            }
            if (xml.Element("yellowpages").Element("webapi") != null)
            {
                return new WebApiYellowPagesParser(xml);
            }
            throw new NotSupportedException();
        }
    }
}
