using System;
using System.Threading;
using Progressive.PecaStarter5.Models.YellowPages;

namespace Progressive.PecaStarter5.Models.Broadcasts
{
    class BroadcastTimer
    {
        private Timer timer; // スレッドタイマが最も軽量
        private long count;

        public event Action<long, IYellowPages, string> Ticked;

        public void BeginTimer(IYellowPages yellowPages, string id)
        {
            const int Period = 1 * 60 * 1000;
            this.count = 0;
            this.timer = new Timer(Callback, Tuple.Create(yellowPages, id), Period, Period);
        }

        private void Callback(object obj)
        {
            count++;
            var tuple = (Tuple<IYellowPages, string>)obj;
            Ticked(count, tuple.Item1, tuple.Item2);
        }

        public void EndTimer()
        {
            if (timer == null)
                return;
            timer.Dispose();
            timer = null;
        }
    }
}
