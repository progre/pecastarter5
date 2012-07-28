using Progressive.PecaStarter.View.Control.Behavior;

namespace Progressive.PecaStarter.View.Control
{
    class AskButton : ButtonBase
    {
        public string Message { set { AskBehavior.Message = value; } }
        private AskBehavior AskBehavior { get; set; }

        public AskButton()
        {
            AskBehavior = new AskBehavior(this);
            AddBehavior(AskBehavior.OnClick);
        }
    }
}
