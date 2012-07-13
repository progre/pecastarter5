using System.Collections.Generic;
using System.Windows;
using System;
using System.Windows.Input;
using Progressive.Commons.ViewModels;
using Progressive.Commons.ViewModels.Controls;
using Progressive.Commons.ViewModels.Commands;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class ExternalSourceViewModel : ViewModelBase
    {
        private readonly ICommand removeItemCommand;

        private string streamUrl = "http://";
        public string StreamUrl
        {
            get { return streamUrl; }
            set { SetProperty("StreamUrl", ref streamUrl, value); }
        }

        private string prefix;
        public string Prefix
        {
            get { return prefix; }
            set { SetProperty("Prefix", ref prefix, value); }
        }

        public TextBoxWithHistoryViewModel Name { get; private set; }
        public TextBoxWithHistoryViewModel Genre { get; private set; }
        public TextBoxWithHistoryViewModel Description { get; private set; }
        public TextBoxWithHistoryViewModel ContactUrl { get; private set; }
        public TextBoxWithHistoryViewModel Comment { get; private set; }

        public ExternalSourceViewModel()
        {
            removeItemCommand = new RemoveItemCommand();
            Name = new TextBoxWithHistoryViewModel(removeItemCommand);
            Genre = new TextBoxWithHistoryViewModel(removeItemCommand);
            Description = new TextBoxWithHistoryViewModel(removeItemCommand);
            ContactUrl = new TextBoxWithHistoryViewModel(removeItemCommand);
            Comment = new TextBoxWithHistoryViewModel(removeItemCommand);
        }

        public void UpdateHistory()
        {
            Name.UpdateHistory();
            Genre.UpdateHistory();
            Description.UpdateHistory();
            ContactUrl.UpdateHistory();
            Comment.UpdateHistory();
        }

        private string UpdateHistory(IList<string> history, string value)
        {
            if (history.Contains(value))
            {
                history.Remove(value);
            }
            history.Insert(0, value);
            while (history.Count > 20)
            {
                history.RemoveAt(history.Count - 1);
            }
            return history[0];
        }
    }
}
