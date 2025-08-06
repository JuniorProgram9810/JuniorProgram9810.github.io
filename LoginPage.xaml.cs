using AppIntegradora10A.Models;
using AppIntegradora10A.Services;

namespace AppIntegradora10A.Views;

public partial class LoginPage : ContentPage
{
    private readonly FirebaseAuthService authService;

    public LoginPage()
    {
        InitializeComponent();
        authService = new FirebaseAuthService();
    }

    private async void OnLogin_Clicked(object sender, EventArgs e)
    {
        if (!ValidarCampos())
            return;

        // Mostrar indicador de carga
        OnLogin.Text = "⏳ Iniciando Sesión...";
        OnLogin.IsEnabled = false;

        try
        {
            string email = emailEntry.Text.Trim();
            string password = passwordEntry.Text;

            var response = await authService.SignInWithEmailAndPasswordAsync(email, password);

            if (response != null)
            {
                // INICIALIZAR SESIÓN DE USUARIO
                UsuarioSesion.IniciarSesion(
                    email: response.Email,
                    idToken: response.IdToken,
                    localId: response.LocalId,
                    nombreUsuario: response.Email
                );

                // Determinar tipo de usuario y mostrar mensaje personalizado
                string tipoUsuario = UsuarioSesion.UsuarioActual.EsAdmin ? "Administrador" : "Usuario";
                string mensaje = $"✅ ¡Bienvenido {tipoUsuario}!\n\n";
                mensaje += $"Email: {response.Email}\n";
                mensaje += $"Tipo de cuenta: {tipoUsuario}\n\n";

                if (UsuarioSesion.UsuarioActual.EsAdmin)
                {
                    mensaje += "🔧 Como administrador puedes:\n";
                    mensaje += "• Ver todos los reportes de usuarios\n";
                    mensaje += "• Editar y eliminar cualquier reporte\n";
                    mensaje += "• Acceder a estadísticas completas";
                }
                else
                {
                    mensaje += "🎮 Como usuario puedes:\n";
                    mensaje += "• Crear nuevos reportes de incidencias\n";
                    mensaje += "• Ver y editar solo tus reportes\n";
                    mensaje += "• Seguir el estado de tus incidencias";
                }

                await DisplayAlert("🎮 GameSupport Hub", mensaje, "Continuar");

                // Navegar al menú principal
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                // Error en login
                await DisplayAlert("❌ Error de Autenticación",
                    "Email o contraseña incorrectos.\n\n💡 Sugerencias:\n• Verifica que el email esté bien escrito\n• Revisa las mayúsculas y minúsculas\n• ¿Olvidaste tu contraseña?\n\nIntenta de nuevo.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error de Conexión",
                $"No se pudo conectar con el servidor:\n\n{ex.Message}\n\n💡 Verifica tu conexión a internet e intenta nuevamente.",
                "OK");
        }
        finally
        {
            // Restaurar botón
            OnLogin.Text = "Log in";
            OnLogin.IsEnabled = true;
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    private bool ValidarCampos()
    {
        // Validar email
        if (string.IsNullOrWhiteSpace(emailEntry.Text))
        {
            MostrarError("Por favor, ingresa tu email.");
            emailEntry.Focus();
            return false;
        }

        if (!IsValidEmail(emailEntry.Text.Trim()))
        {
            MostrarError("Por favor, ingresa un email válido.\n\nEjemplo: usuario@gmail.com");
            emailEntry.Focus();
            return false;
        }

        // Validar contraseña
        if (string.IsNullOrWhiteSpace(passwordEntry.Text))
        {
            MostrarError("Por favor, ingresa tu contraseña.");
            passwordEntry.Focus();
            return false;
        }

        if (passwordEntry.Text.Length < 6)
        {
            MostrarError("La contraseña debe tener al menos 6 caracteres.");
            passwordEntry.Focus();
            return false;
        }

        return true;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private async void MostrarError(string mensaje)
    {
        await DisplayAlert("⚠️ Campo Requerido", mensaje, "OK");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Limpiar campos al aparecer
        emailEntry.Text = string.Empty;
        passwordEntry.Text = string.Empty;

        // Cerrar sesión anterior si existe
        UsuarioSesion.CerrarSesion();
    }
}