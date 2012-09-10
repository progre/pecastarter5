using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    internal class ParameterTextBoxWithHistoryViewModel : TextBoxWithHistoryViewModel
    {
        public ParameterTextBoxWithHistoryViewModel(ICommand command)
            : base(command)
        {
        }

        [CustomValidation(typeof(ParameterValidator), "ValidateParameter")]
        public override string Value
        {
            get { return base.Value; }
            set { base.Value = value; }
        }
    }
}
