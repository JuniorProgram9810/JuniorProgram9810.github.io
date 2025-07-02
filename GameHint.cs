namespace PollAventuras.Models;
public class GameHint
{
    public string Id { get; set; }
    public string GameId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Level { get; set; }
    public DateTime DatePublished { get; set; }
}