using CommunityToolkit.Mvvm;
using PollAventuras10A.Services;
using PollAventuras10A.ViewModels;
using PollAventuras10A.Views;
using Microsoft.Extensions.Logging;
using Windows.Networking.NetworkOperators;

#if ANDROID
using Plugin.FirebaseAuth;
using Plugin.CloudFirestore;
#endif

namespace PollAventuras10A;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Configuración específica para Android
#if ANDROID
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddAndroid(android => android
                .OnCreate((activity, bundle) => 
                {
                    CrossFirebaseAuth.Current.Initialize(activity, bundle);
                    CrossCloudFirestore.Current.Initialize(activity, bundle);
                }));
        });
#endif

        // Configuración específica para iOS
#if IOS
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddiOS(ios => ios
                .FinishedLaunching((app, options) =>
                {
                    CrossFirebaseAuth.Current.Initialize(app, options);
                    CrossCloudFirestore.Current.Initialize(app, options);
                    return true;
                }));
        });
#endif

        // Registro de servicios
        RegisterServices(builder.Services);

        // Registro de ViewModels
        RegisterViewModels(builder.Services);

        // Registro de Views
        RegisterViews(builder.Services);

        // Configuración de logging
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Servicio principal de Firebase
        services.AddSingleton<FirebaseService>();

        // Servicios adicionales
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IPreferencesService, PreferencesService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        // ViewModels principales
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<ArticleDetailViewModel>();
        services.AddTransient<CategoryViewModel>();
        services.AddTransient<ProfileViewModel>();
    }

    private static void RegisterViews(IServiceCollection services)
    {
        // Páginas principales
        services.AddTransient<LoginPage>();
        services.AddTransient<RegisterPage>();
        services.AddTransient<MainPage>();
        services.AddTransient<ArticleDetailPage>();
        services.AddTransient<CategoryPage>();
        services.AddTransient<ProfilePage>();
    }
}

// Servicio de navegación
public interface INavigationService
{
    Task NavigateToAsync(string route);
    Task NavigateToAsync(string route, IDictionary<string, object> parameters);
    Task GoBackAsync();
    Task NavigateToMainAsync();
    Task NavigateToLoginAsync();
}

public class NavigationService : INavigationService
{
    public async Task NavigateToAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }

    public async Task NavigateToAsync(string route, IDictionary<string, object> parameters)
    {
        await Shell.Current.GoToAsync(route, parameters);
    }

    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public async Task NavigateToMainAsync()
    {
        await Shell.Current.GoToAsync("//main");
    }

    public async Task NavigateToLoginAsync()
    {
        await Shell.Current.GoToAsync("//login");
    }
}

// Servicio de diálogos
public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string accept = "OK");
    Task<bool> ShowConfirmAsync(string title, string message, string accept = "Sí", string cancel = "No");
    Task ShowLoadingAsync(string message = "Cargando...");
    Task HideLoadingAsync();
}

public class DialogService : IDialogService
{
    public async Task ShowAlertAsync(string title, string message, string accept = "OK")
    {
        await Shell.Current.DisplayAlert(title, message, accept);
    }

    public async Task<bool> ShowConfirmAsync(string title, string message, string accept = "Sí", string cancel = "No")
    {
        return await Shell.Current.DisplayAlert(title, message, accept, cancel);
    }

    public async Task ShowLoadingAsync(string message = "Cargando...")
    {
        // Implementar loading dialog personalizado si es necesario
        await Task.CompletedTask;
    }

    public async Task HideLoadingAsync()
    {
        // Ocultar loading dialog
        await Task.CompletedTask;
    }
}

// Servicio de preferencias
public interface IPreferencesService
{
    void Set(string key, string value);
    void Set(string key, bool value);
    void Set(string key, int value);
    string Get(string key, string defaultValue = "");
    bool Get(string key, bool defaultValue = false);
    int Get(string key, int defaultValue = 0);
    void Remove(string key);
    void Clear();
}

public class PreferencesService : IPreferencesService
{
    public void Set(string key, string value)
    {
        Preferences.Set(key, value);
    }

    public void Set(string key, bool value)
    {
        Preferences.Set(key, value);
    }

    public void Set(string key, int value)
    {
        Preferences.Set(key, value);
    }

    public string Get(string key, string defaultValue = "")
    {
        return Preferences.Get(key, defaultValue);
    }

    public bool Get(string key, bool defaultValue = false)
    {
        return Preferences.Get(key, defaultValue);
    }

    public int Get(string key, int defaultValue = 0)
    {
        return Preferences.Get(key, defaultValue);
    }

    public void Remove(string key)
    {
        Preferences.Remove(key);
    }

    public void Clear()
    {
        Preferences.Clear();
    }
}
