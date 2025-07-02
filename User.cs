namespace PollAventuras.Models;
public class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime RegistrationDate { get; set; }
    public List<string> PurchasedGames { get; set; }
}