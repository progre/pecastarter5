using System;
using System.Windows.Input;
using Progressive.PecaStarter5.ViewModels.Controls;

namespace Progressive.Commons.ViewModels.Commands
{
    class RemoveItemCommand : ICommand
    {
        public RemoveItemCommand() { }

        #region ICommand メンバー
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            var vm = parameter as TextBoxWithHistoryViewModel;
            if (vm == null)
            {
                return false;
            }
            if (vm.History.Count == 1 && vm.History[0] == "")
            {
                return false;
            }
            if (vm.History.Count <= 0)
            {
                return false;
            }
            return true;
        }

        public void Execute(object parameter)
        {
            var vm = parameter as TextBoxWithHistoryViewModel;
            if (vm != null)
            {
                vm.RemoveText();
                return;
            }
            var tpl = parameter as Tuple<object, object>;
            if (tpl != null)
            {
               (tpl.Item1 as TextBoxWithHistoryViewModel).RemoveText(tpl.Item2 as string);
                return;
            }
        }
        #endregion
    }
}
