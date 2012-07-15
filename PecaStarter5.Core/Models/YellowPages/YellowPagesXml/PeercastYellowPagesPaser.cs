using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Progressive.PecaStarter5.Models.YellowPagesXml
{
    public class PeercastYellowPagesParser : YellowPagesParser
    {
        public override IList<string> Components
        {
            get
            {
                var list = base.Components;
                if (list.Count != 0)
                {
                    return list;
                }
                return Parameters;
            }
        }
        public IList<string> Parameters
        {
            get
            {
                var list = new List<string>();
                foreach (var element in Xml.Element("yellowpages").Element("peercast").Element("prefix").Elements())
                {
                    list.Add(element.Name.LocalName);
                }
                return list;
            }
        }
        public string Host { get { return Xml.Element("yellowpages").Element("peercast").Element("host").Value; } }
        public string Header { get { return Xml.Element("yellowpages").Element("peercast").Element("prefix").Attribute("header").Value; } }

        public PeercastYellowPagesParser(string text) : base(text) { }
        public PeercastYellowPagesParser(XDocument xml) : base(xml) { }

        public override IYellowPages GetInstance()
        {
            return new PeercastYellowPages()
            {
                Components = Components,
                Header = Header,
                Name = Name,
                NoticeUrl = NoticeUrl,
                IsCheckNoticeUrl = IsCheckNoticeUrl
            };
        }
    }
}
