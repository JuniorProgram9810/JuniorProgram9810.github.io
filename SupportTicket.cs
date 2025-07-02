namespace PollAventuras.Models;
public class SupportTicket
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; } // "Open", "In Progress", "Closed"
    public DateTime CreatedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
}