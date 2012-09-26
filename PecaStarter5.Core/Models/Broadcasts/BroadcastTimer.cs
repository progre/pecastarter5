using System;
using System.Threading;

namespace Progressive.PecaStarter5.Models.Broadcasts
{
    class BroadcastTimer
    {
        private Timer m_timer; // スレッドタイマが最も軽量

        public event TimerCallback Ticked;

        public void BeginTimer(IYellowPages yellowPages, string name)
        {
            const int period = 10 * 60 * 1000;
            m_timer = new Timer(Ticked, Tuple.Create(yellowPages, name), period, period);
        }

        public void EndTimer()
        {
            if (m_timer == null)
                return;
            m_timer.Dispose();
            m_timer = null;
        }
    }
}
