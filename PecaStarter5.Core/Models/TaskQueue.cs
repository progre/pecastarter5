using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Progressive.PecaStarter5.Models
{
    public class TaskQueue
    {
        private object lockObj = new object();
        private Queue<Action> actions = new Queue<Action>();
        private bool executing;

        public void Enqueue(Action action)
        {
            lock (lockObj)
            {
                actions.Enqueue(action);
                if (!executing)
                {
                    Task.Factory.StartNew(Execute);
                }
            }
        }

        private void Execute()
        {
            for (; ; )
            {
                Action action;
                lock (lockObj)
                {
                    if (!actions.Any())
                    {
                        executing = false;
                        return;
                    }
                    action = actions.Dequeue();
                }
                action();
            }
        }
    }
}
