using Firebase.Database;
using Firebase.Database.Query;
using PollAventuras.Models;
using System.Threading.Tasks;

namespace PollAventuras.Helpers

{
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
            ticket.Id = Guid.NewGuid().ToString();
            ticket.CreatedDate = DateTime.Now;
            ticket.Status = "Open";

            await _firebase
                .Child("supportTickets")
                .PostAsync(ticket);
        }

        public async Task<User> GetUserAsync(string userId)
        {
            var user = await _firebase
                .Child("users")
                .Child(userId)
                .OnceSingleAsync<User>();
            return user;
        }

        public async Task<List<GameHint>> GetHintsForGame(string gameId)
        {
            var hints = await _firebase
                .Child("gameHints")
                .Child(gameId)
                .OnceAsync<GameHint>();

            return hints.Select(h => new GameHint
            {
                Id = h.Key,
                GameId = h.Object.GameId,
                Title = h.Object.Title,
                Description = h.Object.Description,
                Level = h.Object.Level,
                DatePublished = h.Object.DatePublished
            }).ToList();
        }

        public async Task<List<SupportTicket>> GetUserTickets(string userId)
        {
            var tickets = await _firebase
                .Child("supportTickets")
                .OrderBy("UserId")
                .EqualTo(userId)
                .OnceAsync<SupportTicket>();

            return tickets.Select(t => new SupportTicket
            {
                Id = t.Key,
                UserId = t.Object.UserId,
                Title = t.Object.Title,
                Description = t.Object.Description,
                Status = t.Object.Status,
                CreatedDate = t.Object.CreatedDate,
                ResolvedDate = t.Object.ResolvedDate
            }).ToList();
        }
    }

    public interface IFirebaseService
    {
        Task AddSupportTicket(SupportTicket ticket);
        Task<User> GetUserAsync(string userId);
        Task<List<GameHint>> GetHintsForGame(string gameId);
        Task<List<SupportTicket>> GetUserTickets(string userId);
    }
}