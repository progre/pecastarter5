using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace Progressive.PecaStarter5.Models.YellowPagesXml
{
    public class YellowPagesParserFactory
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
