using AppIntegradora10A.Views;

namespace AppIntegradora10A
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnAddProduct_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddReportePage());
        }

        private async void OnListProduct_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListReportePage());
        }

        private async void OnLogout_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("🚪 Cerrar Sesión",
                "¿Estás seguro de que deseas cerrar sesión?\n\nTendrás que volver a iniciar sesión para acceder.",
                "Sí, cerrar sesión", "Cancelar");

            if (confirm)
            {
                // Mostrar mensaje de despedida
                await DisplayAlert("👋 Hasta pronto",
                    "Has cerrado sesión correctamente.\n¡Gracias por usar nuestra app!",
                    "OK");

                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}













