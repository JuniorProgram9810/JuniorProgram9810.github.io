using PollAventuras10A.Services;
using PollAventuras10A.Views;
using Windows.Networking.NetworkOperators;

namespace PollAventuras10A;

public partial class AppShell : Shell
{
    private readonly INavigationService _navigationService;
    private readonly FirebaseService _firebaseService;
    private readonly IDialogService _dialogService;

    public AppShell()
    {
        InitializeComponent();

        // Registrar rutas de navegación
        RegisterRoutes();

        // Obtener servicios del contenedor de dependencias
        _navigationService = Handler?.MauiContext?.Services?.GetService<INavigationService>();
        _firebaseService = Handler?.MauiContext?.Services?.GetService<FirebaseService>();
        _dialogService = Handler?.MauiContext?.Services?.GetService<IDialogService>();

        // Configurar el contexto de datos
        BindingContext = this;
    }

    private void RegisterRoutes()
    {
        // Registrar rutas para navegación
        Routing.RegisterRoute(AppConstants.LoginRoute, typeof(LoginPage));
        Routing.RegisterRoute(AppConstants.RegisterRoute, typeof(RegisterPage));
        Routing.RegisterRoute(AppConstants.MainRoute, typeof(MainPage));
        Routing.RegisterRoute(AppConstants.ArticleDetailRoute, typeof(ArticleDetailPage));
        Routing.RegisterRoute(AppConstants.CategoryRoute, typeof(CategoryPage));
        Routing.RegisterRoute(AppConstants.ProfileRoute, typeof(ProfilePage));
    }

    // Comandos para el menú
    public Command LogoutCommand => new Command(async () => await OnLogoutAsync());
    public Command SettingsCommand => new Command(async () => await OnSettingsAsync());
    public Command AboutCommand => new Command(async () => await OnAboutAsync());

    private async Task OnLogoutAsync()
    {
        try
        {
            var confirm = await _dialogService.ShowConfirmAsync(
                "Cerrar Sesión",
                "¿Estás seguro de que quieres cerrar sesión?",
                "Sí", "No");

            if (confirm)
            {
                await _firebaseService.LogoutAsync();

                // Navegar a la página de login
                Application.Current.MainPage = new LoginPage();
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync("Error",
                $"Error al cerrar sesión: {ex.Message}");
        }
    }

    private async Task OnSettingsAsync()
    {
        await _dialogService.ShowAlertAsync("Configuración",
            "Función de configuración próximamente disponible.");
    }

    private async Task OnAboutAsync()
    {
        await _dialogService.ShowAlertAsync("Acerca de",
            $"{AppConstants.AppName}\nVersión {AppConstants.AppVersion}\n\nUna aplicación informativa desarrollada con .NET MAUI y Firebase.\n\nSoporte: {AppConstants.SupportEmail}");
    }

    // Método para manejar el botón de retroceso en Android
    protected override bool OnBackButtonPressed()
    {
        // Si estamos en la página principal, mostrar confirmación para salir
        if (Shell.Current.CurrentState.Location.ToString().Contains("main"))
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await _dialogService.ShowConfirmAsync(
                    "Salir",
                    "¿Deseas salir de la aplicación?",
                    "Sí", "No");

                if (result)
                {
                    System.Environment.Exit(0);
                }
            });
            return true;
        }

        return base.OnBackButtonPressed();
    }
}