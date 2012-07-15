using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Net;

namespace Progressive.PecaStarter5.Models.YellowPagesXml
{
    public abstract class YellowPagesParser
    {
        protected XDocument Xml { get; private set; }

        public string Name
        {
            get { return Xml.Element("yellowpages").Element("name").Value; }
        }
        public string NoticeUrl
        {
            get { return Xml.Element("yellowpages").Element("notice").Value; }
        }
        public bool IsCheckNoticeUrl
        {
            get
            {
                var check = Xml.Element("yellowpages").Element("notice").Attribute("check");
                return check == null || check.Value == "true";
            }
        }
        public virtual IList<string> Components
        {
            get
            {
                if (Xml.Element("yellowpages").Element("components") == null)
                {
                    return new List<string>();
                }
                throw new NotImplementedException();
            }
        }

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
            throw new ArgumentException();
        }

        public YellowPagesParser(string text)
        {
            Xml = XDocument.Parse(text);
        }
        public YellowPagesParser(XDocument xml)
        {
            Xml = xml;
        }

        public abstract IYellowPages GetInstance();

        public int GetNoticeHash()
        {
            return new WebClient().DownloadString(NoticeUrl).GetHashCode();
        }

        public async Task<int> GetNoticeHashAsync()
        {
            return (await new WebClient().DownloadStringTaskAsync(NoticeUrl)).GetHashCode();
        }
    }
}
