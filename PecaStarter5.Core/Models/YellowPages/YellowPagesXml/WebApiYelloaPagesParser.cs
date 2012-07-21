using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Progressive.PecaStarter5.Models.ExternalYellowPages;
using System.Threading.Tasks;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.Models.YellowPagesXml
{
    public class WebApiYellowPagesParser : YellowPagesParser
    {
        private static string[] DefaultBroadcastParameters
        {
            get
            {
                return new string[]{
                        "name",
                        "bitrate",
                        "tags",
                        "description",
                        "comment",
                        "contact_url",
                        "protocol",
                        "stream_url",
                        "type",
                        "password",
                        "result_format",
                };
            }
        }

        private static string[] DefaultUpdateParameters
        {
            get
            {
                return new string[]{
                        "name",
                        "bitrate",
                        "tags",
                        "description",
                        "comment",
                        "contact_url",
                        "protocol",
                        "stream_url",
                        "type",
                        "password",
                        "result_format",
                };
            }
        }

        private static string[] DefaultStopParameters
        {
            get
            {
                return new string[]{
                        "name",
                        "password",
                        "result_format",
                };
            }
        }

        private static string[] ExcludeParameters
        {
            get
            {
                return new string[]{
                        "name",
                        "bitrate",
                        "tags",
                        "description",
                        "comment",
                        "contact_url",
                        "protocol",
                        "stream_url",
                        "type",
                        "result_format",
                };
            }
        }

        public override IList<string> Components
        {
            get
            {
                var list = base.Components;
                if (list.Count != 0)
                {
                    return list;
                }
                else
                {
                    return DefaultComponents;
                }
            }
        }

        private IList<string> DefaultComponents
        {
            get
            {
                var set = new HashSet<string>();
                foreach (var param in BroadcastParameters) set.Add(param);
                foreach (var param in UpdateParameters) set.Add(param);
                foreach (var param in ExcludeParameters) set.Remove(param);
                set.Add("listeners_invisibility");
                return set.ToList();
            }
        }

        public IList<string> BroadcastParameters
        {
            get
            {
                // 既定値を使用
                // TODO: ファイルから読み込み
                return DefaultBroadcastParameters;
            }
        }

        public IList<string> UpdateParameters
        {
            get
            {
                // 既定値を使用
                // TODO: ファイルから読み込み
                return DefaultUpdateParameters;
            }
        }
        public IList<string> StopParameters
        {
            get
            {
                // 既定値を使用
                // TODO: ファイルから読み込み
                return DefaultStopParameters;
            }
        }

        public string BroadcastUrl
        {
            get
            {
                return Xml.Element("yellowpages").Element("webapi").Element("method").Element("start").Value;
            }
        }

        public string UpdateUrl
        {
            get
            {
                return Xml.Element("yellowpages").Element("webapi").Element("method").Element("update").Value;
            }
        }

        public string StopUrl
        {
            get
            {
                return Xml.Element("yellowpages").Element("webapi").Element("method").Element("stop").Value;
            }
        }

        public WebApiYellowPagesParser(string text) : base(text) { }
        public WebApiYellowPagesParser(XDocument xml) : base(xml) { }

        public override IYellowPages GetInstance()
        {
            return new WebApiYellowPages()
            {
                BroadcastUrl = BroadcastUrl,
                Components = Components,
                Name = Name,
                NoticeUrl = NoticeUrl,
                IsCheckNoticeUrl = IsCheckNoticeUrl,
                StopUrl = StopUrl,
                UpdateUrl = UpdateUrl,
                BroadcastParameters = DefaultBroadcastParameters,
                UpdateParameters = DefaultUpdateParameters,
                StopParameters = DefaultStopParameters,
            };
        }
    }
}
