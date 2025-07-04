using AppIntegradora10A.Helpers;
using Firebase.Auth;
using System;
using System.Threading.Tasks;

namespace AppIntegradora10A.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthHelpers authHelpers;

    public LoginPage()
    {
        InitializeComponent();
        authHelpers = new AuthHelpers();
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, completa todos los campos", "OK");
            return;
        }

        LoginButton.IsEnabled = false;
        LoginButton.Text = "Iniciando sesión...";

        try
        {
            var result = await authHelpers.LoginAsync(EmailEntry.Text, PasswordEntry.Text);

            if (result.IsSuccess)
            {
                await DisplayAlert("Éxito", "¡Bienvenido de vuelta!", "OK");
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                await DisplayAlert("Error", result.ErrorMessage, "OK");
            }
            await Navigation.PushAsync(new MainPage());

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error inesperado: {ex.Message}", "OK");
        }
        finally
        {
            LoginButton.IsEnabled = true;
            LoginButton.Text = "Iniciar Sesión";
        }
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    private async void ForgotPassword_Tapped(object sender, EventArgs e)
    {
        string email = await DisplayPromptAsync("Recuperar contraseña",
            "Ingresa tu correo electrónico:",
            "Enviar",
            "Cancelar",
            "correo@ejemplo.com",
            keyboard: Keyboard.Email);

        if (!string.IsNullOrWhiteSpace(email))
        {
            try
            {
                await authHelpers.ResetPasswordAsync(email);
                await DisplayAlert("Éxito", "Se ha enviado un correo para restablecer tu contraseña", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo enviar el correo: {ex.Message}", "OK");
            }
        }
    }
}