﻿using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Progressive.Commons.ViewModels;
using Progressive.Commons.ViewModels.Commands;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.ViewModels.Controls;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    public class ExternalSourceViewModel : ViewModelBase
    {
        private readonly Configuration configuration;

        public ExternalSourceViewModel(Configuration configuration)
        {
            var removeItemCommand = new RemoveItemCommand();
            Name = new ChannelNameTextBoxWithHistoryViewModel(removeItemCommand);
            Genre = new ParameterTextBoxWithHistoryViewModel(removeItemCommand);
            Description = new ParameterTextBoxWithHistoryViewModel(removeItemCommand);
            Comment = new ParameterTextBoxWithHistoryViewModel(removeItemCommand);

            this.configuration = configuration;
            StreamUrl = configuration.StreamUrl;
            Name.History = new ObservableCollection<string>(configuration.NameHistory);
            configuration.NameHistory = Name.History;
            Genre.History = new ObservableCollection<string>(configuration.GenreHistory);
            configuration.GenreHistory = Genre.History;
            Description.History = new ObservableCollection<string>(configuration.DescriptionHistory);
            configuration.DescriptionHistory = Description.History;
            Comment.History = new ObservableCollection<string>(configuration.CommentHistory);
            configuration.CommentHistory = Comment.History;
        }

        private bool isLocked;
        public bool IsLocked
        {
            get { return isLocked; }
            set { SetProperty("IsLocked", ref isLocked, value); }
        }

        [Required(ErrorMessage = "ストリームURLは必須です")]
        [RegularExpression("^(http|mms)://.+$", ErrorMessage = "http://で始めてください")]
        [CustomValidation(typeof(ParameterValidator), "ValidateParameter", ErrorMessage = "文字数の制限を超えています")]
        public string StreamUrl
        {
            get { return configuration.StreamUrl; }
            set
            {
                if (configuration.StreamUrl == value)
                    return;
                configuration.StreamUrl = value;
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

        [CustomValidation(typeof(ParameterValidator), "ValidateParameter", ErrorMessage = "文字数の制限を超えています")]
        public string ContactUrl
        {
            get { return configuration.ContactUrl; }
            set
            {
                if (configuration.ContactUrl == value)
                    return;
                configuration.ContactUrl = value;
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
    }
}
