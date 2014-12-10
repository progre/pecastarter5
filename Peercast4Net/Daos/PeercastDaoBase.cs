using System;
using System.Net;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Commons;

namespace Progressive.Peercast4Net.Daos
{
    public class PeercastDaoBase : IDisposable
    {
        protected const string ViewXmlUrl = "/admin?cmd=viewxml";

        protected readonly string address;
        protected readonly WebClient client;

        public PeercastDaoBase(string address)
        {
            client = new WebClient();
            this.address = address;
        }

        #region IDisposable メンバー

        public virtual void Dispose()
        {
            client.Dispose();
        }

        #endregion

        public virtual Task<string> GetViewXmlAsync()
        {
            return DownloadAsync(ViewXmlUrl);
        }

        protected async Task<string> DownloadAsync(string url)
        {
            try
            {
                return await client.DownloadAsync("http://" + address + url);
            }
            catch (WebException)
            {
                throw new PeerCastException("PeerCastへの接続に失敗しました。" + Environment.NewLine
                    + "PeerCastが起動しているか、またはポート番号が正しいか確認してください。");
            }
        }
    }
}
