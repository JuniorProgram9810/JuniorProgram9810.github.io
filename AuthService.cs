using Firebase.Auth;
using Firebase.Auth.Providers;
using PollAventuras.Models;

namespace PollAventuras.Services
{
    public class AuthService
    {
        private const string FirebaseApiKey = "AIzaSyBIJ7BWRn0Ugr4U1CtZc2YgvHS4MmI-ppY";

        public async Task<User> Login(string email, string password)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

            var user = new User
            {
                Id = auth.User.LocalId,
                Email = auth.User.Email,
                Name = auth.User.DisplayName ?? string.Empty,
                RegistrationDate = DateTime.Now,
                PurchasedGames = new List<string>()
            };

            return user;
        }

        public async Task<bool> Register(string email, string password, string name)
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password, name);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}