using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Progressive.Peercast4Net;
using Progressive.PecaStarter.ViewModel.Command;
using Progressive.PecaStarter5.Models;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    public class BroadcastControlViewModel
    {
        private MainPanelViewModel parent;

        public BroadcastControlViewModel(MainPanelViewModel parent)
        {
            this.parent = parent;
            BroadcastCommand = new BroadcastCommand();
        }

        public ICommand BroadcastCommand { get; private set; }
        public object BroadcastParameter
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
