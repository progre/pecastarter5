using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Daos;
using Progressive.Peercast4Net.Datas;
using Progressive.Peercast4Net.Utils;

namespace Progressive.Peercast4Net
{
    public class PeerCast : PeerCastBase
    {
        public const string NullId = "00000000000000000000000000000000";

        public PeerCast()
        {
        }

        public override Task<Tuple<string, int>> BroadcastAsync(YellowPages yellowPages, BroadcastParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var dao = new PeercastDao(Address))
                {
                    SetYellowPagesAwait(dao, yellowPages.Url);

                    var status = new XmlStatus(dao.GetViewXmlAsync().Result);
                    if (status.Exists(parameter.Name))
                    {
                        throw new PeerCastException("同名のチャンネルが既にあります。");
                    }

                    dao.FetchAsync(parameter.StreamUrl, parameter.Name, parameter.Genre, parameter.Description,
                        parameter.ContactUrl, parameter.Type).Wait();
                    var tuple = Repeat(() =>
                    {
                        var afterStatus = new XmlStatus(dao.GetViewXmlAsync().Result);
                        return GetChannelInfo(afterStatus, parameter.Name);
                    });
                    if (NullId == tuple.Item1)
                    {
                        dao.StopAsync(tuple.Item1).Wait();
                        throw new PeerCastException("チャンネルの作成に失敗しました。" + Environment.NewLine
                            + "エンコードが開始されているか、またはストリームURLが正しいか確認してください。");
                    }

                    dao.SetMetaAsync(parameter.Name, parameter.Genre, parameter.Description,
                        parameter.ContactUrl, parameter.Comment,
                        parameter.TrackArtist, parameter.TrackTitle, parameter.TrackAlbum,
                        parameter.TrackGenre, parameter.TrackContact).Wait();
                    return tuple;
                }
            });
        }

        public override Task UpdateAsync(UpdateParameter parameter)
        {
            var dao = new PeercastDao(Address);
            return dao.SetMetaAsync(parameter.Name, parameter.Genre, parameter.Description,
                    parameter.ContactUrl, parameter.Comment,
                    parameter.TrackArtist, parameter.TrackTitle, parameter.TrackAlbum,
                    parameter.TrackGenre, parameter.TrackContact)
                .ContinueWith(t => dao.Dispose());
        }

        public override Task StopAsync(string id)
        {
            var dao = new PeercastDao(Address);
            return dao.StopAsync(id).ContinueWith(t => dao.Dispose());
        }

        private void SetYellowPagesAwait(PeercastDao dao, string yellowPagesAddress)
        {
            var nvc = GetSettingsAwait(dao);
            var ypParam = nvc.Single(x => x.Key == "yp");
            if (yellowPagesAddress == ypParam.Value)
                return;

            nvc.Remove(ypParam);
            nvc.Add(new KeyValuePair<string, string>("yp", yellowPagesAddress));
            dao.ApplyAsync(nvc).Wait();
        }

        private IList<KeyValuePair<string, string>> GetSettingsAwait(PeercastDao dao)
        {
            return new SettingsHtmlParser().Parse(dao.GetSettingsHtmlAsync().Result);
        }

        private Tuple<string, int> GetChannelInfo(XmlStatus status, string name)
        {
            return Tuple.Create(status.GetChannelId(name), status.GetBitrate(name));
        }

        private Tuple<string, int> Repeat(Func<Tuple<string, int>> func)
        {
            var result = Tuple.Create(NullId, 0);
            for (int i = 0; i < 5; i++)
            {
                result = func();
                if (!string.IsNullOrEmpty(result.Item1) && result.Item1 != NullId)
                    return result;
                Thread.Sleep(i * 1000);
            }
            return result;
        }
    }
}
