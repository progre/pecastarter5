using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Internal.Codeplex.Data;
using Progressive.Peercast4Net.Utils;
using System.Text;
using Progressive.Peercast4Net.Datas;

namespace Progressive.Peercast4Net.Daos
{
    class PeercastStationDao : PeercastDaoBase
    {
        private const string ApiUrl = "/api/1";
        public PeercastStationDao(string address)
            : base(address)
        {
            client.Encoding = Encoding.UTF8;
        }

        public async Task<int> AddYellowPageAsync(string protocol, string name, string uri)
        {
            var json = DynamicJson.Parse(await UploadAsync(ApiUrl, GetJsonRpc("addYellowPage",
                DynamicJson.Serialize(new
                {
                    protocol = protocol,
                    name = name,
                    uri = uri
                }))));
            return (int)json.result.yellowPageId;
        }

        public async Task RemoveYellowPageAsync(int yellowPageId)
        {
            var json = DynamicJson.Parse(await UploadAsync(ApiUrl, GetJsonRpc("removeYellowPage",
                DynamicJson.Serialize(new
                {
                    yellowPageId = yellowPageId,
                }))));
            return;
        }

        public async Task<List<Tuple<int, string, string>>> GetYellowPagesAsync()
        {
            var json = DynamicJson.Parse(await UploadAsync(ApiUrl, GetJsonRpc("getYellowPages")));
            var list = new List<Tuple<int, string, string>>();
            foreach (dynamic yp in (object[])json.result)
            {
                list.Add(Tuple.Create((int)yp.yellowPageId, (string)yp.name, (string)yp.uri));
            }
            return list;
        }

        public async Task<string> BroadcastChannelAsync(int yellowPageId, string sourceUri, string sourceStream, string contentReader,
            string name, string url, int bitrate, string mimeType, string genre, string desc, string comment,
            string trackName, string trackCreator, string trackGenre, string trackAlbum, string trackUrl)
        {
            var jsonObject = DynamicJson.Serialize(new
            {
                yellowPageId = yellowPageId,
                sourceUri = sourceUri,
                sourceStream = sourceStream,
                contentReader = contentReader,
                info = new
                {
                    name = name,
                    url = url,
                    bitrate = bitrate,
                    mimeType = mimeType,
                    genre = genre,
                    desc = desc,
                    comment = comment,
                },
                track = new
                {
                    name = trackName,
                    creator = trackCreator,
                    genre = trackGenre,
                    album = trackAlbum,
                    url = trackUrl
                }
            });
            var json = DynamicJson.Parse(await UploadAsync(ApiUrl, GetJsonRpc("broadcastChannel", jsonObject)));
            if (!json.IsDefined("result"))
            {
                var status = new XmlStatus(GetViewXmlAsync().Result);
                var id = status.GetChannelId(name);
                if (id == PeerCast.NullId)
                    throw new PeerCastException("PeerCastがエラーを返しました:" + Environment.NewLine + json.error.message);
                return id;
            }
            return json.result;
        }

        public async Task SetChannelInfoAsync(string channelId,
            string name, string url, int? bitrate, string contentType, string mimeType,
            string genre, string desc, string comment,
            string trackName, string trackCreator, string trackGenre, string trackAlbum, string trackUrl)
        {
            var jsonObject = DynamicJson.Serialize(new
            {
                channelId = channelId,
                info = new
                {
                    name = name,
                    url = url,
                    bitrate = bitrate,
                    contentType = contentType,
                    mimeType = mimeType,
                    genre = genre,
                    desc = desc,
                    comment = comment,
                },
                track = new
                {
                    name = trackName,
                    creator = trackCreator,
                    genre = trackGenre,
                    album = trackAlbum,
                    url = trackUrl
                }
            });
            var json = DynamicJson.Parse(await UploadAsync(ApiUrl, GetJsonRpc("setChannelInfo", jsonObject)));
            if (json.IsDefined("error"))
            {
                throw new PeerCastException("PeerCastがエラーを返しました:" + Environment.NewLine + json.error.message);
            }
        }

        public async Task StopChannelAsync(string channelId)
        {
            var jsonObject = DynamicJson.Serialize(new { channelId = channelId });
            var json = DynamicJson.Parse(await UploadAsync(ApiUrl, GetJsonRpc("stopChannel", jsonObject)));
            if (json.IsDefined("error"))
            {
                throw new PeerCastException("PeerCastがエラーを返しました:" + Environment.NewLine + json.error.message);
            }
        }

        private async Task<string> UploadAsync(string url, string data)
        {
            try
            {
                return await client.UploadStringTaskAsync("http://" + address + url, data);
            }
            catch (WebException)
            {
                throw new PeerCastException("PeerCastへの接続に失敗しました。" + Environment.NewLine
                    + "PeerCastが起動しているか、またはポート番号が正しいか確認してください。");
            }
        }

        private string GetJsonRpc(string method)
        {
            return "{\"jsonrpc\": \"2.0\", \"id\": 0, \"method\": \"" + method + "\"}";
        }

        private string GetJsonRpc(string method, string parameters)
        {
            return "{\"jsonrpc\": \"2.0\", \"id\": 0, \"method\": \"" + method + "\", \"params\": " + parameters + "}";
        }
    }
}
