using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    internal class ChannelNameTextBoxWithHistoryViewModel : TextBoxWithHistoryViewModel
    {
        public ChannelNameTextBoxWithHistoryViewModel(ICommand command)
            : base(command)
        {
        }

        [Required(ErrorMessage = "チャンネル名は必須です")]
        [CustomValidation(typeof(ParameterValidator), "ValidateChannelName")]
        public override string Value
        {
            get { return base.Value; }
            set { base.Value = value; }
        }
    }
}
