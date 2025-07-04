using AppIntegradora10A.Helpers;
using System;
using System.Threading.Tasks;

namespace AppIntegradora10A.Views;

public partial class GameMainPage : ContentPage
{
    private readonly AuthHelpers authHelpers;

    public GameMainPage()
    {
        InitializeComponent();
        authHelpers = new AuthHelpers();
        LoadUserInfo();
    }

    private async void LoadUserInfo()
    {
        try
        {
            // Aquí puedes cargar información del usuario si la tienes guardada
            // Por ejemplo, desde SecureStorage
            var userName = await SecureStorage.GetAsync("user_name");
            if (!string.IsNullOrEmpty(userName))
            {
                WelcomeLabel.Text = $"¡Bienvenido, {userName}!";
            }

            // Verificar si el usuario es administrador
            var isAdmin = await IsUserAdminAsync();
            AdminFrame.IsVisible = isAdmin;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar información: {ex.Message}", "OK");
        }
    }

    private async Task<bool> IsUserAdminAsync()
    {
        // Aquí puedes implementar la lógica para verificar si el usuario es administrador
        // Por ejemplo, verificando en tu base de datos Firebase
        // Por ahora, retornamos false por defecto
        return false;
    }

    private async void GuidesButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GuidesPage());
    }

    private async void TipsButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TipsPage());
    }

    private async void SupportButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SupportPage());
    }

    private async void FaqButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FaqPage());
    }

    private async void AddProductButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddProductPage());
    }

    private async void ListProductButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ListProductPage());
    }

    private async void ProfileButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage());
    }

    private async void LogoutButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Confirmar", "¿Estás seguro de que quieres cerrar sesión?", "Sí", "No");

        if (answer)
        {
            // Navegar de vuelta a la página de login
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}