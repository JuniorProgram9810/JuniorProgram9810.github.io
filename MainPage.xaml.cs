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
            await Navigation.PushAsync(new AddProductPage());
        }

        private async void OnListProduct_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListProductPage());
        }

        private async void OnLogout_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Cerrar Sesión", "¿Está seguro que desea cerrar sesión?", "Sí", "No");
            if (confirm)
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}

















