using System;
using System.Linq;
using System.Collections.Generic;
using Progressive.PecaStarter5.Models.Broadcasts;
using Progressive.PecaStarter5.Models.Configurations;
using Progressive.PecaStarter5.Models.YellowPages;
using Progressive.PecaStarter5.Models.YellowPages.YellowPagesXml;
using Progressive.PecaStarter5.Plugin;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.Models
{
    // TODO: VMにあるロジック・エンティティを可能な限りここに移動
    public class PecaStarterModel
    {
        private readonly Peercast m_peercast;
        private readonly PeercastStation m_peercastStation;
        private readonly IExternalResource m_externalResource;
        private readonly List<IExternalYellowPages> m_externalYellowPagesList;

        public PecaStarterModel(string title, IExternalResource externalResource)
        {
            Title = title;
            m_externalResource = externalResource;
            m_peercast = new Peercast();
            m_peercastStation = new PeercastStation();
            Plugins = externalResource.GetPlugins().Select(x => new ExternalPlugin(x));
            var tuple = GetYellowPagesLists();
            m_externalYellowPagesList = tuple.Item2;
            YellowPagesList = tuple.Item1;

            Configuration = new ConfigurationDao(externalResource).Get();
            Configuration.DefaultLogPath = externalResource.DefaultLogPath;
            BroadcastModel = new BroadcastModel(Configuration, m_externalYellowPagesList, Plugins);
        }

        public BroadcastModel BroadcastModel { get; private set; }
        public string Title { get; private set; }
        public Configuration Configuration { get; private set; }
        public IEnumerable<ExternalPlugin> Plugins { get; private set; }
        public List<IYellowPages> YellowPagesList { get; private set; }

        public void Save()
        {
            new ConfigurationDao(m_externalResource).Put(Configuration);
        }

        private Tuple<List<IYellowPages>, List<IExternalYellowPages>> GetYellowPagesLists()
        {
            var yellowPagesList = new List<IYellowPages>();
            var externalYellowPagesList = new List<IExternalYellowPages>();
            foreach (var xml in m_externalResource.GetYellowPagesDefineInputStream())
            {
                var yp = YellowPagesParserFactory.GetInstance(xml).GetInstance();
                yellowPagesList.Add(yp);
                if (yp.IsExternal)
                {
                    externalYellowPagesList.Add((IExternalYellowPages)yp);
                }
            }
            return Tuple.Create(yellowPagesList, externalYellowPagesList);
        }
    }
}
