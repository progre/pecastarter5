using System;
using System.Linq;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Daos;
using Progressive.Peercast4Net.Datas;

namespace Progressive.Peercast4Net
{
    public class PeerCastStation : PeerCastBase
    {
        #region IPeercast メンバー

        public override Task<Tuple<string, int>> BroadcastAsync(YellowPages yellowPages, BroadcastParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var dao = new PeercastStationDao(Address))
                {
                    var ypId = GetOrCreateYellowPagesIdAwait(dao, yellowPages);

                    var id = dao.BroadcastChannelAsync(ypId, parameter.StreamUrl,
                        parameter.StreamUrl.Contains("rtmp://") ? "RTMP Source" : "http",
                        parameter.Type == "WMV" ? "ASF(WMV or WMA)"
                        : parameter.Type == "FLV" ? "Flash Video (FLV)"
                        : "",
                        parameter.Name, parameter.ContactUrl, -1, "",
                        parameter.Genre, parameter.Description, parameter.Comment,
                        parameter.TrackTitle, parameter.TrackArtist, parameter.TrackGenre,
                        parameter.TrackAlbum, parameter.TrackContact).Result;
                    return Tuple.Create(id, -1);
                }
            });
        }

        public override async Task StopAsync(string id)
        {
            using (var dao = new PeercastStationDao(Address))
            {
                await dao.StopChannelAsync(id);
            }
        }

        public override async Task UpdateAsync(UpdateParameter parameter)
        {
            using (var dao = new PeercastStationDao(Address))
            {
                await dao.SetChannelInfoAsync(parameter.Id,
                    parameter.Name, parameter.ContactUrl, null, null, null,
                    parameter.Genre, parameter.Description, parameter.Comment,
                    parameter.TrackTitle, parameter.TrackArtist, parameter.TrackGenre,
                    parameter.TrackAlbum, parameter.TrackContact);
            }
        }

        #endregion

        private int GetOrCreateYellowPagesIdAwait(PeercastStationDao dao, YellowPages yellowPages)
        {
            var ypName = yellowPages.Name;
            var ypUrl = "pcp://" + yellowPages.Url + "/";
            var ypInfo = dao.GetYellowPagesAsync().Result.FirstOrDefault(x => x.Item2 == ypName);
            if (ypInfo == null)
            {
                return dao.AddYellowPageAsync("pcp", yellowPages.Name, ypUrl).Result;
            }
            if (ypInfo.Item3 != ypUrl)
            {
                dao.RemoveYellowPageAsync(ypInfo.Item1).Wait();
                return dao.AddYellowPageAsync("pcp", yellowPages.Name, ypUrl).Result;
            }
            return ypInfo.Item1;
        }
    }
}
