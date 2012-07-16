using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Progressive.Peercast4Net;
using Progressive.PecaStarter.ViewModel.Command;
using Progressive.PecaStarter5.Models;
using Progressive.PecaStarter5.Models.Services;
using System.ComponentModel;
using Progressive.Commons.ViewModels;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    public class BroadcastControlViewModel : ViewModelBase
    {
        private MainPanelViewModel parent;

        public BroadcastControlViewModel(MainPanelViewModel parent, PeercastService service)
        {
            this.parent = parent;
            BroadcastCommand = new BroadcastCommand(service);
            parent.YellowPagesListViewModel.PropertyChanged += (sender, e) =>
                OnPropertyChanged("BroadcastParameter");
            foreach (var vm in parent.YellowPagesListViewModel.YellowPagesViewModels)
            {
                vm.PropertyChanged += (sender, e) =>
                    OnPropertyChanged("BroadcastParameter");
            }
        }

        public ICommand BroadcastCommand { get; private set; }
        public Tuple<IYellowPages, int?, BroadcastParameter> BroadcastParameter
        {
            get
            {
                var yp = parent.YellowPagesListViewModel.SelectedYellowPages;
                var es = parent.ExternalSourceViewModel;
                return Tuple.Create(
                    yp.Model,
                    yp.AcceptedHash,
                    new BroadcastParameter()
                    {
                        StreamUrl = es.StreamUrl,
                        Name = es.Name.Value,
                        Genre = es.Genre.Value,
                        Description = es.Description.Value,
                        Type = "WMV",
                        ContactUrl = es.ContactUrl,
                        Comment = es.Comment.Value,
                        TrackArtist = "",
                        TrackTitle = "",
                        TrackAlbum = "",
                        TrackGenre = "",
                        TrackContact = "",
                    });
            }
        }
    }
}
