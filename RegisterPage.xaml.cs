using AppIntegradora10A.Helpers;
using System;
using System.Threading.Tasks;

namespace AppIntegradora10A.Views;

public partial class RegisterPage : ContentPage
{
    private readonly AuthHelpers authHelpers;

    public RegisterPage()
    {
        InitializeComponent();
        authHelpers = new AuthHelpers();
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
            string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, completa todos los campos", "OK");
            return;
        }

        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
            return;
        }

        if (PasswordEntry.Text.Length < 6)
        {
            await DisplayAlert("Error", "La contraseña debe tener al menos 6 caracteres", "OK");
            return;
        }

        RegisterButton.IsEnabled = false;
        RegisterButton.Text = "Creando cuenta...";

        try
        {
            var result = await authHelpers.RegisterAsync(EmailEntry.Text, PasswordEntry.Text, NameEntry.Text);

            if (result.IsSuccess)
            {
                await DisplayAlert("Éxito", "¡Cuenta creada exitosamente! Bienvenido a GameSupport", "OK");
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                await DisplayAlert("Error", result.ErrorMessage, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error inesperado: {ex.Message}", "OK");
        }
        finally
        {
            RegisterButton.IsEnabled = true;
            RegisterButton.Text = "Crear Cuenta";
        }
    }

    private async void LoginLink_Tapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}