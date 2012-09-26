﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Progressive.PecaStarter5.Models.ExternalYellowPages;
using Progressive.PecaStarter5.Plugin;
using Progressive.Peercast4Net;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.Models.Broadcasts
{
    internal class PeercastService
    {
        public Task<BroadcastingParameter> BroadcastAsync(
            IPeercast peercast, IEnumerable<IExternalYellowPages> externalYellowPagesList,
            IYellowPages yellowPages, int? acceptedHash,
            Dictionary<string, string> yellowPagesParameter,
            BroadcastParameter parameter, IProgress<string> progress)
        {
            return Task.Factory.StartNew(() =>
            {
                // YPの更新確認
                if (yellowPages.IsCheckNoticeUrl && acceptedHash.HasValue)
                {
                    progress.Report("規約の更新を確認中...");
                    if (IsUpdatedYellowPagesAsync(yellowPages, acceptedHash.Value).Result)
                    {
                        throw new YellowPagesException("イエローページの規約が更新されています。規約を再確認してください。");
                    }
                }

                // 開始
                progress.Report("チャンネルを作成中...");
                var param = (BroadcastParameter)parameter.Clone();
                param.Genre = yellowPages.GetPrefix(yellowPagesParameter) + param.Genre;
                var tuple = peercast.BroadcastAsync(
                    new Peercast4Net.Datas.YellowPages() { Name = yellowPages.Name, Url = yellowPages.Host },
                    param).Result;

                var broadcastedParameter = new BroadcastingParameter
                {
                    Bitrate = tuple.Item2,
                    Id = tuple.Item1,
                    YellowPagesParameters = yellowPagesParameter,
                    BroadcastParameter = parameter
                };

                // 外部YPに通知
                if (yellowPages.IsExternal)
                {
                    Exception exception = null;
                    try
                    {
                        Find(externalYellowPagesList, yellowPages.Name)
                            .OnBroadcastedAsync(broadcastedParameter).Wait();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        // catch内でawait出来ないので外出し
                    }
                    if (exception != null)
                    {
                        peercast.StopAsync(tuple.Item1).Wait();
                        throw exception;
                    }
                }

                progress.Report("チャンネルを作成しました");
                return broadcastedParameter;
            });
        }

        public Task<UpdatedParameter> UpdateAsync(
            IPeercast peercast, IEnumerable<IExternalYellowPages> externalYellowPagesList,
            IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            UpdateParameter parameter, IProgress<string> progress)
        {
            return Task.Factory.StartNew(() =>
            {
                // 更新
                progress.Report("通信中...");
                var param = (UpdateParameter)parameter.Clone();
                param.Genre = yellowPages.GetPrefix(yellowPagesParameter) + param.Genre;
                peercast.UpdateAsync(param).Wait();

                var updatedParameter = new UpdatedParameter
                {
                    YellowPagesParameters = yellowPagesParameter,
                    UpdateParameter = parameter
                };

                // 外部YPに通知
                if (yellowPages.IsExternal)
                {
                    Find(externalYellowPagesList, yellowPages.Name).OnUpdatedAsync(updatedParameter).Wait();
                }

                progress.Report("チャンネル情報を更新しました");
                return updatedParameter;
            });
        }

        public Task<StoppedParameter> StopAsync(
            IPeercast peercast, IEnumerable<IExternalYellowPages> externalYellowPagesList,
            IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            string name, string id, IProgress<string> progress)
        {
            return Task.Factory.StartNew(() =>
            {
                // 停止
                progress.Report("通信中...");
                peercast.StopAsync(id).Wait();

                var stoppedParameter = new StoppedParameter
                {
                    Name = name,
                    Id = id,
                    YellowPagesParameters = yellowPagesParameter
                };

                // 外部YPに通知
                if (yellowPages.IsExternal)
                {
                    Find(externalYellowPagesList, yellowPages.Name).OnStopedAsync(stoppedParameter).Wait();
                }

                progress.Report("チャンネルを切断しました");
                return stoppedParameter;
            });
        }

        public Task<IEnumerable<IChannel>> GetChannelsAsync(IPeercast peercast)
        {
            return peercast.GetChannelsAsync();
        }

        public Task<Tuple<int, int>> OnTickedAsync(IPeercast peercast, IYellowPages yellowPages, string name)
        {
            return Task.Factory.StartNew(() =>
            {
                var tuple = peercast.GetListenersAsync(name).Result;

                // 外部YPに通知
                if (yellowPages.IsExternal)
                {
                    ((IExternalYellowPages)yellowPages).OnTickedAsync(name, tuple.Item1, tuple.Item2).Wait();
                }

                return tuple;
            });
        }

        private async Task<bool> IsUpdatedYellowPagesAsync(IYellowPages yellowPages, int acceptedHash)
        {
            if (acceptedHash != await yellowPages.GetNoticeHashAsync())
            {
                return true;
            }
            return false;
        }

        private IExternalYellowPages Find(IEnumerable<IExternalYellowPages> list, string name)
        {
            return list.Single(x => x.Name == name);
        }
    }
}
