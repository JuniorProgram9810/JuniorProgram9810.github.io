
using PollAventuras10A.Services;
using PollAventuras10A.Views;

namespace PollAventuras10A;

public partial class App : Application
{
    private readonly FirebaseService _firebaseService;

    public App(FirebaseService firebaseService)
    {
        InitializeComponent();
        _firebaseService = firebaseService;

        // Verificar si el usuario está autenticado
        if (_firebaseService.IsUserAuthenticated())
        {
            MainPage = new AppShell();
        }
        else
        {
            MainPage = new LoginPage();
        }
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        // Configurar el tamaño de la ventana (útil para escritorio)
        if (window != null)
        {
            window.Title = "Mi App Informativa";

#if WINDOWS
            window.Width = 400;
            window.Height = 800;
            window.X = 100;
            window.Y = 100;
#endif
        }

        return window;
    }
}

// Clase estática para constantes de la aplicación
public static class AppConstants
{
    // Rutas de navegación
    public const string LoginRoute = "login";
    public const string RegisterRoute = "register";
    public const string MainRoute = "main";
    public const string ArticleDetailRoute = "articledetail";
    public const string CategoryRoute = "category";
    public const string ProfileRoute = "profile";

    // Claves de preferencias
    public const string IsFirstLaunchKey = "IsFirstLaunch";
    public const string UserEmailKey = "UserEmail";
    public const string UserNameKey = "UserName";
    public const string ThemeKey = "Theme";
    public const string NotificationsEnabledKey = "NotificationsEnabled";

    // Configuración de Firebase
    public const string FirebaseProjectId = "tu-proyecto-firebase";
    public const string FirebaseApiKey = "tu-api-key";
    public const string FirebaseAuthDomain = "tu-proyecto-firebase.firebaseapp.com";

    // Configuración de la app
    public const string AppName = "Mi App Informativa";
    public const string AppVersion = "1.0.0";
    public const string SupportEmail = "soporte@miapp.com";
}