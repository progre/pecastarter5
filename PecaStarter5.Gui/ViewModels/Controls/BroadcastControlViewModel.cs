using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    public class BroadcastControlViewModel
    {
        private MainPanelViewModel parent;

        public BroadcastControlViewModel(MainPanelViewModel parent)
        {
            this.parent = parent;
        }

        public ICommand BroadcastCommand { get; private set; }
        public BroadcastParameter BroadcastParameter
        {
            get
            {
                var es = parent.ExternalSourceViewModel;
                return new BroadcastParameter()
                {
                    StreamUrl = es.StreamUrl,
                    Name = es.Name.History[0],
                    Genre = es.Genre.History[0],
                    Description = es.Description.History[0],
                    Type = "WMV",
                    ContactUrl = es.ContactUrl,
                    Comment = es.Comment.History[0],
                    TrackArtist = "",
                    TrackTitle = "",
                    TrackAlbum = "",
                    TrackGenre = "",
                    TrackContact = "",
                };
            }
        }
    }
}
