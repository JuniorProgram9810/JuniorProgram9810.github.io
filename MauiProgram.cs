// MauiProgram.cs
using Microsoft.Extensions.Logging;
using PollAventuras.Services;
using PollAventuras.ViewModels.Support;
using PollAventuras.Views.Support;

namespace PollAventuras;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Registrar servicios
        builder.Services.AddSingleton<IFirebaseService, FirebaseService>();

        // Registrar ViewModels
        builder.Services.AddTransient<NewTicketViewModel>();

        // Registrar páginas
        builder.Services.AddTransient<NewTicketPage>();

        return builder.Build();
    }
}