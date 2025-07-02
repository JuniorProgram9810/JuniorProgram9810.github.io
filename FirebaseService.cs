using Firebase.Database;
using Firebase.Database.Query;
using PollAventuras.Models;
using System.Threading.Tasks;

namespace PollAventuras.Services;

public class FirebaseService : IFirebaseService
{
    private readonly FirebaseClient _firebase;
    private const string FirebaseUrl = "https://pollaventuras-default-rtdb.firebaseio.com/";

    public FirebaseService()
    {
        _firebase = new FirebaseClient(FirebaseUrl);
    }

    public async Task AddSupportTicket(SupportTicket ticket)
    {
        await _firebase
            .Child("supportTickets")
            .PostAsync(ticket);
    }
}

public interface IFirebaseService
{
    Task AddSupportTicket(SupportTicket ticket);
}