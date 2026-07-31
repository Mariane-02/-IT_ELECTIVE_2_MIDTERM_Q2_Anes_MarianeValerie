namespace PlaylistApp.Models.Entities
{
    public class Playlist
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public List<VideoItem> Videos { get; set; } = new();
    }

    public class VideoItem
    {
        public string YouTubeUrl { get; set; } = string.Empty;
        public string TrackNote { get; set; } = string.Empty;
    }
}
