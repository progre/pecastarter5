using System.Windows;
using System.Windows.Controls;
using Progressive.Commons.Views.Utils;

namespace Progressive.PecaStarter.View.Control.Behavior
{
    class AskBehavior : BehaviorBase
    {
        public string Message { get; set; }

        public AskBehavior(Button called) : base(called) { }

        public bool OnClick()
        {
            if (Xceed.Wpf.Toolkit.MessageBox.Show(WpfUtils.GetRoot(Called), Message, "確認",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)
                != MessageBoxResult.Yes)
            {
                return false;
            }
            return true;
        }
    }
}
