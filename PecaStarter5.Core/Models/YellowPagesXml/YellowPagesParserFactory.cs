using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Progressive.PecaStarter5.Models.YellowPagesXml
{
    class YellowPagesParserFactory
    {
        public static YellowPagesParser GetInstance(string path)
        {
            var xml = XDocument.Load(path);
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
