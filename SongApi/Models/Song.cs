namespace MinimalSongApi.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Artist { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int LengthInSeconds { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
