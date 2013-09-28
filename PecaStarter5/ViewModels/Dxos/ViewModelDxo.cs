using Progressive.PecaStarter5.ViewModels.Pages;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.ViewModels.Dxos
{
    static class ViewModelDxo
    {
        public static BroadcastParameter ToBroadcastParameter(ExternalSourceViewModel viewModel)
        {
            return new BroadcastParameter()
            {
                StreamUrl = viewModel.StreamUrl,
                Name = viewModel.Name.Value,
                Genre = viewModel.Genre.Value,
                Description = viewModel.Description.Value,
                Type = viewModel.Type,
                ContactUrl = viewModel.ContactUrl,
                Comment = viewModel.Comment.Value,
                TrackArtist = "",
                TrackTitle = "",
                TrackAlbum = "",
                TrackGenre = "",
                TrackContact = "",
            };
        }

        public static UpdateParameter ToUpdateParameter(string id, ExternalSourceViewModel viewModel)
        {
            return new UpdateParameter()
            {
                Id = id,
                Name = viewModel.Name.Value,
                Genre = viewModel.Genre.Value,
                Description = viewModel.Description.Value,
                ContactUrl = viewModel.ContactUrl,
                Comment = viewModel.Comment.Value,
                TrackArtist = "",
                TrackTitle = "",
                TrackAlbum = "",
                TrackGenre = "",
                TrackContact = "",
            };
        }
    }
}
