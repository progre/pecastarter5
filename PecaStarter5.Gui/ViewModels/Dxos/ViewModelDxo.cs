using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.ViewModels.Dxos
{
    static class ViewModelDxo
    {
        public static BroadcastParameter ToBroadcastParameter(MainPanelViewModel viewModel)
        {
            var es = viewModel.ExternalSourceViewModel;
            return new BroadcastParameter()
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
            };
        }

        public static UpdateParameter ToUpdateParameter(MainPanelViewModel viewModel)
        {
            var es = viewModel.ExternalSourceViewModel;
            return new UpdateParameter()
            {
                Id = "",
                Name = es.Name.Value,
                Genre = es.Genre.Value,
                Description = es.Description.Value,
                ContactUrl = es.ContactUrl,
                Comment = es.Comment.Value,
                TrackArtist = "",
                TrackTitle = "",
                TrackAlbum = "",
                TrackGenre = "",
                TrackContact = "",
            };
        }
    }
}
