﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Daos;
using Progressive.Peercast4Net.Utils;
using Progressive.Peercast4Net.Datas;

namespace Progressive.Peercast4Net
{
    public class Peercast : PeercastBase
    {
        public const string NullId = "00000000000000000000000000000000";

        public Peercast()
        {
        }

        public override async Task<IEnumerable<IChannel>> GetChannelsAsync()
        {
            using (var dao = new PeercastDao(Address))
            {
                var xml = await dao.GetViewXmlAsync();
                return new XmlStatus(xml).Channels;
            }
        }

        public override async Task<Tuple<int, int>> GetListenersAsync(string name)
        {
            using (var dao = new PeercastDao(Address))
            {
                return (await GetXmlStatusAsync(dao)).GetHits(name);
            }
        }

        public override Task<Tuple<string, int>> BroadcastAsync(YellowPages yellowPages, BroadcastParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var dao = new PeercastDao(Address))
                {
                    SetYellowPagesAwait(dao, yellowPages.Url);

                    if (ExistsAsync(dao, parameter.Name).Result)
                    {
                        throw new PeercastException("同名のチャンネルが既にあります。");
                    }
                    dao.FetchAsync(parameter.StreamUrl, parameter.Name, parameter.Genre, parameter.Description,
                        parameter.ContactUrl, parameter.Type).Wait();
                    var tuple = Repeat(() => GetChannelInfoAsync(dao, parameter.Name)).Result;
                    if (NullId == tuple.Item1)
                    {
                        dao.StopAsync(tuple.Item1).Wait();
                        throw new PeercastException("チャンネルの作成に失敗しました。" + Environment.NewLine
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

        public override async Task UpdateAsync(UpdateParameter parameter)
        {
            using (var dao = new PeercastDao(Address))
            {
                await dao.SetMetaAsync(parameter.Name, parameter.Genre, parameter.Description,
                    parameter.ContactUrl, parameter.Comment,
                    parameter.TrackArtist, parameter.TrackTitle, parameter.TrackAlbum,
                    parameter.TrackGenre, parameter.TrackContact);
            }
        }

        public override Task StopAsync(string id)
        {
            using (var dao = new PeercastDao(Address))
            {
                return dao.StopAsync(id);
            }
        }

        private void SetYellowPagesAwait(PeercastDao dao, string yellowPagesAddress)
        {
            var nvc = GetSettingsAsync(dao).Result;
            var ypParam = nvc.Single(x => x.Key == "yp");
            if (yellowPagesAddress == ypParam.Value)
                return;

            nvc.Remove(ypParam);
            nvc.Add(new KeyValuePair<string, string>("yp", yellowPagesAddress));
            dao.ApplyAsync(nvc).Wait();
        }

        private async Task<IList<KeyValuePair<string, string>>> GetSettingsAsync(PeercastDao dao)
        {
            var html = await dao.GetSettingsHtmlAsync();
            return new SettingsHtmlParser().Parse(html);
        }

        private async Task<bool> ExistsAsync(PeercastDao dao, string name)
        {
            return (await GetXmlStatusAsync(dao)).Exists(name);
        }

        private async Task<Tuple<string, int>> GetChannelInfoAsync(PeercastDao dao, string name)
        {
            var status = await GetXmlStatusAsync(dao);
            return Tuple.Create(status.GetChannelId(name), status.GetBitrate(name));
        }

        private async Task<XmlStatus> GetXmlStatusAsync(PeercastDao dao)
        {
            var xml = await dao.GetViewXmlAsync();
            return new XmlStatus(xml);
        }

        private async Task<Tuple<string, int>> Repeat(Func<Task<Tuple<string, int>>> func)
        {
            var result = Tuple.Create(NullId, 0);
            for (int i = 0; i < 5; i++)
            {
                result = await func();
                if (!string.IsNullOrEmpty(result.Item1) && result.Item1 != NullId)
                    return result;
                Thread.Sleep(i * 1000);
            }
            return result;
        }
    }
}
