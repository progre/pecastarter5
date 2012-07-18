using System;
using System.Text;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;
using Progressive.PecaStarter.View.Control.Behavior;

namespace Progressive.PecaStarter.View.Control
{
    class CountdownButton : Button
    {
        private AskBehavior AskBehavior;
        private string text;
        private ButtonState State;
        private DispatcherTimer Timer;
        private uint Count;

        public string Message { set { AskBehavior.Message = value; } }
        public string Text
        {
            get { return text; }
            set
            {
                if (text == value)
                    return;
                text = value;
                State.SetContent(value);
            }
        }
        public int Delay { get { return ((dynamic)DataContext).Delay; } }

        private string CountdownMessage
        {
            get { return new StringBuilder().Append(Count).Append("...").ToString(); }
        }

        public CountdownButton()
        {
            AskBehavior = new AskBehavior(this);
            State = new NormalState(this);
            Timer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher);
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += (sender, e) =>
            {
                if (State is CountDownState == false)
                {
                    return;
                }
                Count -= 1;
                if (Count > 0)
                {
                    Content = CountdownMessage;
                    return;
                }
                Content = text;
                Timer.Stop();
                base.OnClick();
                State = new NormalState(this);
            };
        }

        protected override void OnClick()
        {
            State = State.OnClick();
        }

        private void BaseOnClick()
        {
            base.OnClick();
        }

        private abstract class ButtonState
        {
            protected CountdownButton Parent;

            public ButtonState(CountdownButton parent)
            {
                Parent = parent;
            }

            public abstract ButtonState OnClick();
            public abstract void SetContent(string content);
        }
        private class NormalState : ButtonState
        {
            public NormalState(CountdownButton parent) : base(parent) { }

            public override ButtonState OnClick()
            {
                if (Parent.Delay == 0)
                {
                    if (!Parent.AskBehavior.OnClick())
                    {
                        return this;
                    }
                    Parent.BaseOnClick();
                    return this;
                }
                return new CountDownState(Parent, (uint)Parent.Delay);
            }

            public override void SetContent(string content)
            {
                Parent.Content = content;
            }
        }
        private class CountDownState : ButtonState
        {
            public CountDownState(CountdownButton parent, uint delay)
                : base(parent)
            {
                Parent.Count = delay;
                Parent.Timer.Start();
                Parent.Content = Parent.CountdownMessage;
            }

            public override ButtonState OnClick()
            {
                Parent.Timer.Stop();
                Parent.Content = Parent.Text;
                return new NormalState(Parent);
            }

            public override void SetContent(string content) { }
        }
    }
}
