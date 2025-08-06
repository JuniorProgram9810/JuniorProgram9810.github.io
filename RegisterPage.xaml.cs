using AppIntegradora10A.Services;
using System.Text.RegularExpressions;

namespace AppIntegradora10A.Views;

public partial class RegisterPage : ContentPage
{
    private readonly FirebaseAuthService authService;

    public RegisterPage()
    {
        InitializeComponent();
        authService = new FirebaseAuthService();
    }

    private async void OnRegister_Clicked(object sender, EventArgs e)
    {
        if (!ValidarCampos())
            return;

        await MostrarLoading(true);

        try
        {
            string email = emailEntry.Text.Trim();
            string password = passwordEntry.Text;

            var response = await authService.RegisterWithEmalAndPasswordAsync(email, password);

            if (response != null)
            {
                // Registro exitoso
                string mensaje = "🎉 ¡Cuenta creada exitosamente!\n\n";
                mensaje += $"Email: {response.Email}\n";

                if (!string.IsNullOrWhiteSpace(gamerNameEntry.Text))
                {
                    mensaje += $"Nombre Gamer: {gamerNameEntry.Text.Trim()}\n";
                }

                mensaje += "\nYa puedes iniciar sesión con tus credenciales.";

                await DisplayAlert("✅ Registro Exitoso", mensaje, "Ir al Login");

                // Volver al login
                await Navigation.PopAsync();
            }
            else
            {
                // Error en registro
                await DisplayAlert("❌ Error de Registro",
                    "No se pudo crear la cuenta.\n\nPosibles causas:\n• El email ya está registrado\n• Email inválido\n• Contraseña muy débil\n\nIntenta con otros datos.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error",
                $"Ocurrió un error inesperado:\n{ex.Message}",
                "OK");
        }
        finally
        {
            await MostrarLoading(false);
        }
    }

    private async void OnBackToLogin_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnTermsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("📋 Términos y Condiciones",
            "TÉRMINOS Y CONDICIONES DE USO\n\n" +
            "1. Esta aplicación es para reportar incidencias de videojuegos\n" +
            "2. Proporciona información veraz y detallada\n" +
            "3. No uses lenguaje ofensivo en los reportes\n" +
            "4. Respeta a otros usuarios y desarrolladores\n" +
            "5. Tu información personal está protegida\n" +
            "6. Puedes eliminar tu cuenta en cualquier momento\n\n" +
            "Al usar esta app, aceptas estos términos.",
            "Entendido");
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
            MostrarError("Por favor, ingresa un email válido.");
            emailEntry.Focus();
            return false;
        }

        // Validar contraseña
        if (string.IsNullOrWhiteSpace(passwordEntry.Text))
        {
            MostrarError("Por favor, crea una contraseña.");
            passwordEntry.Focus();
            return false;
        }

        if (passwordEntry.Text.Length < 6)
        {
            MostrarError("La contraseña debe tener al menos 6 caracteres.");
            passwordEntry.Focus();
            return false;
        }

        if (!IsValidPassword(passwordEntry.Text))
        {
            MostrarError("La contraseña debe contener al menos:\n• Una letra\n• Un número");
            passwordEntry.Focus();
            return false;
        }

        // Validar confirmación de contraseña
        if (string.IsNullOrWhiteSpace(confirmPasswordEntry.Text))
        {
            MostrarError("Por favor, confirma tu contraseña.");
            confirmPasswordEntry.Focus();
            return false;
        }

        if (passwordEntry.Text != confirmPasswordEntry.Text)
        {
            MostrarError("Las contraseñas no coinciden.\nVerifica que ambas sean iguales.");
            confirmPasswordEntry.Focus();
            return false;
        }

        // Validar términos y condiciones
        if (!termsCheckBox.IsChecked)
        {
            MostrarError("Debes aceptar los términos y condiciones para continuar.");
            return false;
        }

        // Validar nombre gamer (si se proporciona)
        if (!string.IsNullOrWhiteSpace(gamerNameEntry.Text))
        {
            if (gamerNameEntry.Text.Trim().Length < 3)
            {
                MostrarError("El nombre de gamer debe tener al menos 3 caracteres.");
                gamerNameEntry.Focus();
                return false;
            }

            if (!IsValidGamerName(gamerNameEntry.Text.Trim()))
            {
                MostrarError("El nombre de gamer solo puede contener letras, números y guiones bajos.");
                gamerNameEntry.Focus();
                return false;
            }
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

    private bool IsValidPassword(string password)
    {
        // Debe contener al menos una letra y un número
        return Regex.IsMatch(password, @"^(?=.*[A-Za-z])(?=.*\d).+$");
    }

    private bool IsValidGamerName(string gamerName)
    {
        // Solo letras, números y guiones bajos
        return Regex.IsMatch(gamerName, @"^[A-Za-z0-9_]+$");
    }

    private async void MostrarError(string mensaje)
    {
        await DisplayAlert("⚠️ Error de Validación", mensaje, "OK");
    }

    private async Task MostrarLoading(bool mostrar)
    {
        LoadingIndicator.IsVisible = mostrar;
        LoadingIndicator.IsRunning = mostrar;
        OnRegister.IsEnabled = !mostrar;
        OnBackToLogin.IsEnabled = !mostrar;

        if (mostrar)
        {
            OnRegister.Text = "⏳ Creando Cuenta...";
        }
        else
        {
            OnRegister.Text = "✅ Crear Cuenta";
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Limpiar campos al aparecer
        LimpiarCampos();
    }

    private void LimpiarCampos()
    {
        emailEntry.Text = string.Empty;
        passwordEntry.Text = string.Empty;
        confirmPasswordEntry.Text = string.Empty;
        gamerNameEntry.Text = string.Empty;
        termsCheckBox.IsChecked = false;
    }
}