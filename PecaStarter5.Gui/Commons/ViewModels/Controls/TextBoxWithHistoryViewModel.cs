using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Progressive.Commons.ViewModels.Controls
{
    public class TextBoxWithHistoryViewModel : ViewModelBase
    {
        private string value;

        public string Value
        {
            get { return value; }
            set { SetProperty("Value", ref this.value, value); }
        }

        public ObservableCollection<string> History { get; set; }

        public ICommand Command { get; private set; }

        public TextBoxWithHistoryViewModel(ICommand command)
        {
            value = "";
            History = new ObservableCollection<string>();
            Command = command;
        }

        public void UpdateHistory()
        {
            string value = this.value;
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
