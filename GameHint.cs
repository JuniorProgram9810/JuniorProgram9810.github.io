namespace PollAventuras.Models
{
    public class GameHint
    {
        public string Id { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public DateTime DatePublished { get; set; } = DateTime.Now;
    }
}