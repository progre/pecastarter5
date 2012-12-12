using System.Collections.Generic;
using Progressive.PecaStarter5.Models.Configurations;
using Progressive.PecaStarter5.Models.YellowPages;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.ViewModels.Pages
{
    class ChannelViewModel
    {
        public ChannelViewModel(IEnumerable<IYellowPages> yellowPagesList, Configuration configuration)
        {
            YellowPagesListViewModel = new YellowPagesListViewModel(yellowPagesList, configuration);
            ExternalSourceViewModel = new ExternalSourceViewModel(configuration);
        }

        public YellowPagesListViewModel YellowPagesListViewModel { get; private set; }
        public ExternalSourceViewModel ExternalSourceViewModel { get; private set; }

        public void LoadChannel(IYellowPages yellowPages, IChannel channel)
        {
            // YPタブを指定のYPに
            if (yellowPages != null)
                YellowPagesListViewModel.SelectedYellowPagesModel = yellowPages;
            var genre = YellowPagesListViewModel.SelectedYellowPages.Parse(channel.Genre);

            // ソースタブに値を反映
            var esvm = ExternalSourceViewModel;
            esvm.Name.Value = channel.Name;
            esvm.Genre.Value = genre;
            esvm.Description.Value = channel.Description;
            esvm.ContactUrl = channel.ContactUrl;
            esvm.Comment.Value = channel.Comment;
            esvm.Name.Value = channel.Name;
        }

        public void Lock()
        {
            YellowPagesListViewModel.IsLocked = true;
            if (YellowPagesListViewModel.SelectedYellowPages != null)
                YellowPagesListViewModel.SelectedYellowPages.IsLocked = true;
            ExternalSourceViewModel.IsLocked = true;
        }

        public void Unlock()
        {
            YellowPagesListViewModel.IsLocked = false;
            if (YellowPagesListViewModel.SelectedYellowPages != null)
                YellowPagesListViewModel.SelectedYellowPages.IsLocked = false;
            ExternalSourceViewModel.IsLocked = false;
        }

        public void SyncPrefix()
        {
            if (YellowPagesListViewModel.SelectedYellowPages != null)
                ExternalSourceViewModel.Prefix = YellowPagesListViewModel.SelectedYellowPages.Prefix;
        }
    }
}
