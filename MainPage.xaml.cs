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

    }

}
