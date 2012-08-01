using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Progressive.Peercast4Net.Daos
{
    class PeercastStationDao : PeercastDaoBase
    {
        public PeercastStationDao(string address)
            : base(address)
        {
        }

        public Task<string> AddYellowPageAsync(string protocol, string name, string uri)
        {
            return UploadAsync("/api/1",
                "{\"jsonrpc\": \"2.0\", \"id\": 0, \"method\": \"addYellowPage\", \"params\": "
                + "{\"protocol\": \"" + protocol + "\", \"name\": \"" + name + "\", "
                + "\"uri\": \"" + protocol + "://" + uri + "\"}" + "}");
        }

        private async Task<string> UploadAsync(string url, string data)
        {
            try
            {
                return await client.UploadStringTaskAsync("http://" + address + url, data);
            }
            catch (WebException)
            {
                throw new PeercastException("Peercastへの接続に失敗しました。" + Environment.NewLine
                    + "Peercastが起動しているか、またはポート番号が正しいか確認してください。");
            }
        }
    }
}
