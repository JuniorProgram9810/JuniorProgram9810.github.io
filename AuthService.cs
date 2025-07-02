// Services/AuthService.cs
using Firebase.Auth;
using Firebase.Auth.Providers;

public class AuthService
{
    private const string FirebaseApiKey = "AIzaSyD2fUenY8Bm5ZXgFfecDEz5n9WTii1VGtA";

    public async Task<User> Login(string email, string password)
    {
        var authProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey));
        var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

        var user = new User
        {
            Id = auth.User.LocalId,
            Email = auth.User.Email,
            Name = auth.User.DisplayName
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