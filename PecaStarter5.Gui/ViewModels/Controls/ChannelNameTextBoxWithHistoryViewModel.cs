using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    public class ChannelNameTextBoxWithHistoryViewModel : TextBoxWithHistoryViewModel
    {
        public ChannelNameTextBoxWithHistoryViewModel(ICommand command)
            : base(command)
        {
        }

        private string value = "";
        [Required(ErrorMessage = "チャンネル名は必須です")]
        [CustomValidation(typeof(ParameterValidator), "ValidateChannelName", ErrorMessage = "文字数の制限を超えています")]
        public override string Value
        {
            get { return value; }
            set
            {
                SetProperty("Value", ref this.value, value);
            }
        }
    }
}
