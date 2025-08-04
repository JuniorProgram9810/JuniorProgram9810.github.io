using AppIntegradora10A.Views;

namespace AppIntegradora10A
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar las rutas para navegación
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("AddProductPage", typeof(AddReportePage));
            Routing.RegisterRoute("ListProductPage", typeof(ListReportePage));
            Routing.RegisterRoute("EditProductPage", typeof(EditReportePage));
            
        }
    }
}
