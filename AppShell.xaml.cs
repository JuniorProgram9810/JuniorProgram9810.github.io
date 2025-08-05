using AppIntegradora10A.Views;

namespace AppIntegradora10A
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar las rutas para navegación con nombres coherentes
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("AddReportePage", typeof(AddReportePage));
            Routing.RegisterRoute("ListReportePage", typeof(ListReportePage));
            Routing.RegisterRoute("EditReportePage", typeof(EditReportePage));
        }
    }
}
