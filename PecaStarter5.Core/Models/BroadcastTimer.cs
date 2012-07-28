using System.Threading;

namespace Progressive.PecaStarter5.Models
{
    class BroadcastTimer
    {
        private Timer m_timer; // スレッドタイマが最も軽量

        public event TimerCallback Ticked;

        public void BeginTimer(string name)
        {
            const int period = 10 * 60 * 1000;
            m_timer = new Timer(Ticked, name, period, period);
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
