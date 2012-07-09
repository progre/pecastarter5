using System.Windows.Controls;

namespace Progressive.PecaStarter.View.Control.Behavior
{
    abstract class BehaviorBase
    {
        protected Button Called { get; private set; }

        public BehaviorBase(Button called)
        {
            Called = called;
        }
    }
}
