using AppIntegradora10A.Helpers;
using AppIntegradora10A.Models;
using System.Threading.Tasks;

namespace AppIntegradora10A.Views;

public partial class AddProductPage : ContentPage



{

	FirebaseHelpers firebaseHelpers = new FirebaseHelpers();
    public AddProductPage()
	{
		InitializeComponent();
	}

    private async void BtnOnAddProducto_Clicked(object sender, EventArgs e)
    {

        var producto = new Producto

        {
            Nombre = NombreEntry.Text,
            Descripcion = DescripcionEntry.Text,
            Precio = decimal.Parse(PrecioEntry.Text)


        };



        await firebaseHelpers.AddProducto(producto);
        await DisplayAlert("Exito", "Caso de falla agregado correctamente agregado correctamente", "OK");
        await Navigation.PopAsync();





    }
}