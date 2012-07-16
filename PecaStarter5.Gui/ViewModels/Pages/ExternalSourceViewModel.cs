using System.Collections.Generic;
using System.Windows;
using System;
using System.Linq;
using System.Windows.Input;
using Progressive.Commons.ViewModels;
using Progressive.Commons.ViewModels.Controls;
using Progressive.Commons.ViewModels.Commands;
using Progressive.PecaStarter5.Models;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class ExternalSourceViewModel : ViewModelBase
    {
        private readonly ICommand removeItemCommand;

        public ExternalSourceViewModel()
        {
            removeItemCommand = new RemoveItemCommand();
            Name = new TextBoxWithHistoryViewModel(removeItemCommand);
            Genre = new TextBoxWithHistoryViewModel(removeItemCommand);
            Description = new TextBoxWithHistoryViewModel(removeItemCommand);
            Comment = new TextBoxWithHistoryViewModel(removeItemCommand);
        }

        private Settings settings;
        public Settings Settings
        {
            set
            {
                settings = value;
                StreamUrl = settings.StreamUrl;
                Name.History = new ObservableCollection<string>(settings.NameHistory);
                settings.NameHistory = Name.History;
                Genre.History = new ObservableCollection<string>(settings.GenreHistory);
                settings.GenreHistory = Genre.History;
                Description.History = new ObservableCollection<string>(settings.DescriptionHistory);
                settings.DescriptionHistory = Description.History;
                Comment.History = new ObservableCollection<string>(settings.CommentHistory);
                settings.CommentHistory = Comment.History;
            }
        }

        public string StreamUrl
        {
            get { return settings.StreamUrl; }
            set
            {
                if (settings.StreamUrl == value)
                    return;
                settings.StreamUrl = value;
                OnPropertyChanged("StreamUrl");
            }
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

        public string ContactUrl
        {
            get { return settings.ContactUrl; }
            set
            {
                if (settings.ContactUrl == value)
                    return;
                settings.ContactUrl = value;
                OnPropertyChanged("ContactUrl");
            }
        }

        public TextBoxWithHistoryViewModel Comment { get; private set; }

        public void UpdateHistory()
        {
            Name.UpdateHistory();
            Genre.UpdateHistory();
            Description.UpdateHistory();
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
