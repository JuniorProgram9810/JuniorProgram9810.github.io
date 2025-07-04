using Firebase.Auth;
using System;
using System.Threading.Tasks;

namespace AppIntegradora10A.Helpers
{
    public class AuthHelpers
    {
        private readonly FirebaseAuthProvider authProvider;
        private const string FirebaseApiKey = "TU_API_KEY_DE_FIREBASE"; // Reemplaza con tu API key

        public AuthHelpers()
        {
            authProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey));
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

                // Guardar el token del usuario
                await SaveUserTokenAsync(auth.FirebaseToken);

                return new AuthResult { IsSuccess = true, User = auth.User };
            }
            catch (FirebaseAuthException ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = GetFriendlyErrorMessage(ex.Reason)
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error de conexión: {ex.Message}"
                };
            }
        }

        public async Task<AuthResult> RegisterAsync(string email, string password, string displayName)
        {
            try
            {
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password, displayName);

                // Guardar el token del usuario
                await SaveUserTokenAsync(auth.FirebaseToken);

                return new AuthResult { IsSuccess = true, User = auth.User };
            }
            catch (FirebaseAuthException ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = GetFriendlyErrorMessage(ex.Reason)
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error de conexión: {ex.Message}"
                };
            }
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            try
            {
                await authProvider.SendPasswordResetEmailAsync(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> IsUserLoggedInAsync()
        {
            try
            {
                var token = await GetUserTokenAsync();
                return !string.IsNullOrEmpty(token);
            }
            catch
            {
                return false;
            }
        }

        public async Task SignOutAsync()
        {
            await ClearUserTokenAsync();
        }

        private async Task SaveUserTokenAsync(string token)
        {
            await SecureStorage.SetAsync("firebase_token", token);
        }

        private async Task<string> GetUserTokenAsync()
        {
            return await SecureStorage.GetAsync("firebase_token");
        }

        private async Task ClearUserTokenAsync()
        {
            SecureStorage.Remove("firebase_token");
        }

        private string GetFriendlyErrorMessage(AuthErrorReason reason)
        {
            return reason switch
            {
                AuthErrorReason.InvalidEmailAddress => "El correo electrónico no es válido",
                AuthErrorReason.WrongPassword => "Contraseña incorrecta",
                AuthErrorReason.UserNotFound => "No existe una cuenta con este correo electrónico",
                AuthErrorReason.UserDisabled => "Esta cuenta ha sido deshabilitada",
                AuthErrorReason.EmailAlreadyInUse => "Ya existe una cuenta con este correo electrónico",
                AuthErrorReason.WeakPassword => "La contraseña es muy débil",
                AuthErrorReason.NetworkRequestFailed => "Error de conexión a internet",
                _ => "Error desconocido. Intenta nuevamente"
            };
        }
    }

    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public User User { get; set; }
    }
}