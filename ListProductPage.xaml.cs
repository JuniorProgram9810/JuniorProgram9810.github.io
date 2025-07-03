using AppIntegradora10A.Helpers;
using AppIntegradora10A.Models;
using System.Threading.Tasks;

namespace AppIntegradora10A.Views;

public partial class ListProductPage : ContentPage
{
    FirebaseHelpers firebaseHelpers=new FirebaseHelpers();
    

	public ListProductPage()
	{
		InitializeComponent();
        LoadProduct();

        
	}

    private async void LoadProduct()
    {
        var productos = await firebaseHelpers.GetAllproducts();
        ProductListView.ItemsSource = productos;

    }


    private async void BtnEditProduct_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var producto = button?.BindingContext as Producto;

        if (producto != null)
        {
            await Navigation.PushAsync(new EditProductPage(producto));
        }
    }



    private async void BtnDeleteProduct_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var producto = button?.BindingContext as Producto;

        if (producto != null && !string.IsNullOrEmpty(producto.Id))
        {
            await firebaseHelpers.DeleteProducto(producto.Id);
            await DisplayAlert("AVISO", "Producto Eliminado", "Ok");
            LoadProduct();
        }
        else
        {
            await DisplayAlert("Error", "No se ha podido encontrar el producto para eliminar", "Ok");
        }
    }

}