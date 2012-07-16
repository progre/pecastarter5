using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Progressive.PecaStarter5.Models
{
    class YellowPages : IYellowPages
    {
        #region IYellowPages メンバー

        public string Name { get; set; }

        public virtual string Host { get { return "localhost"; } set { } }

        public bool IsCheckNoticeUrl { get; set; }

        public string NoticeUrl { get; set; }

        public IEnumerable<string> Components { get; set; }

        public virtual string GetPrefix(Dictionary<string, string> parameters) { return ""; }

        public virtual Dictionary<string, string> Parse(string value)
        {
            return new Dictionary<string, string>();
        }

        public virtual Task OnBroadcastAsync()
        {
            return Task.Factory.StartNew(() => { });
        }

        public async Task<int> GetNoticeHashAsync()
        {
            return await Task.Factory.StartNew(() =>
            {
                using (var client = new WebClient())
                {
                    return client.DownloadString(NoticeUrl).GetHashCode();
                }
            });
        }

        #endregion
    }
}
