using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using Progressive.Commons.ViewModels;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    public class TextBoxWithHistoryViewModel : ViewModelBase
    {
        public TextBoxWithHistoryViewModel(ICommand command)
        {
            Command = command;
        }

        private string value = "";
        public virtual string Value
        {
            get { return value; }
            set { SetProperty("Value", ref this.value, value); }
        }

        private ObservableCollection<string> history = new ObservableCollection<string>();
        public ObservableCollection<string> History
        {
            get { return history; }
            set
            {
                history = value;
                Value = history.Count > 0 ? history[0] : "";
            }
        }

        public ICommand Command { get; private set; }

        public void UpdateHistory()
        {
            string value = Value;
            if (History.Count == 0 && string.IsNullOrEmpty(value))
            {
                return;
            }
            if (History.Contains(value))
            {
                History.Remove(value);
            }
            History.Insert(0, value);
            while (History.Count > 20)
            {
                History.RemoveAt(History.Count - 1);
            }
            Value = History[0];
        }

        public void RemoveText()
        {
            RemoveText(Value);
        }

        public void RemoveText(string value)
        {
            History.Remove(value);
            if (History.Count == 1 && History[0] == "")
            {
                History.RemoveAt(0);
            }
            Value = History.Count > 0 ? History[0] : "";
        }
    }
}
