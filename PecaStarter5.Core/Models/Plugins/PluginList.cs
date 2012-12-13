using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Progressive.PecaStarter5.Plugins;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.Models.Plugins
{
    public class PluginList : List<ExternalPlugin>
    {
        public event UnhandledExceptionEventHandler AsyncExceptionThrown;

        public IEnumerable<ExternalPlugin> EnabledExternalPlugins
        {
            get { return this.Where(x => x.IsEnabled); }
        }

        public IEnumerable<IPlugin> EnabledPlugins
        {
            get { return this.Where(x => x.IsEnabled).Select(x => x.Instance); }
        }

        public IEnumerable<IPlugin> GetCurrentTimePlugins(long count)
        {
            return EnabledPlugins
                .Where(x =>
                {
                    var tickInterval = x.PluginConfiguration.TickInterval;
                    return tickInterval > 0 && count % tickInterval == 0;
                });
        }

        public void OnBroadcastAsync(BroadcastingParameter parameter)
        {
            SafeForEachEnabledPlugins(p => p.OnBroadcastedAsync(parameter));
        }

        public void OnUpdateAsync(UpdatedParameter parameter)
        {
            SafeForEachEnabledPlugins(p => p.OnUpdatedAsync(parameter));
        }

        public void OnStopAsync(StoppedParameter parameter)
        {
            SafeForEachEnabledPlugins(p => p.OnStopedAsync(parameter));
        }

        public void OnInterruptedAsync(InterruptedParameter parameter)
        {
            SafeForEachEnabledPlugins(p => p.OnInterruptedAsync(parameter));
        }

        public void OnTickedAwait(long count, IChannel channel, Func<IChannel> channelFactory)
        {
            foreach (var plugin in GetCurrentTimePlugins(count))
            {
                if (channel == null)
                    channel = channelFactory();
                plugin.OnTickedAsync(channel).Bind(t => t.Wait());
            }
        }

        private void SafeForEachEnabledPlugins(Func<IPlugin, Task> asyncAction)
        {
            foreach (var plugin in EnabledPlugins)
                try
                {
                    asyncAction(plugin).Bind(t =>
                        t.ContinueWith(t1 => OnAsyncExceptionThrown(t1.Exception),
                            TaskContinuationOptions.OnlyOnFaulted));
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() => OnAsyncExceptionThrown(ex));
                }
        }

        private void OnAsyncExceptionThrown(Exception ex)
        {
            AsyncExceptionThrown(
                this,
                new UnhandledExceptionEventArgs(ex, false));
        }
    }

    static class MaybeExtensions
    {
        public static TResult Bind<TSender, TResult>(
            this TSender obj, Func<TSender, TResult> func)
        {
            return obj == null ? default(TResult) : func(obj);
        }

        public static void Bind<TSender>(
            this TSender obj, Action<TSender> func)
        {
            if (obj == null) return;
            func(obj);
        }
    }
}
