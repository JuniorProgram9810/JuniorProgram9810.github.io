using PollAventuras.Views.Support;

namespace PollAventuras;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registrar rutas
        Routing.RegisterRoute(nameof(NewTicketPage), typeof(NewTicketPage));
    }
}