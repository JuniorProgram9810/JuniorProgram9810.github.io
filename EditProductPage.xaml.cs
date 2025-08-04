using AppIntegradora10A.Helpers;
using AppIntegradora10A.Models;
using System.Threading.Tasks;

namespace AppIntegradora10A.Views;

public partial class EditProductPage : ContentPage
{
	FirebaseHelpers firebaseHelpers = new FirebaseHelpers();
	private Producto producto;


    public EditProductPage(Producto producto)
	{
		InitializeComponent();
		this.producto = producto;

		NombreEntry.Text = producto.Nombre;	
		DescripcionEntry.Text = producto.Descripcion;	
		PrecioEntry.Text = producto.Precio.ToString();


    }


	private async void BtnOnEditProducto_Clicked(object sender, EventArgs e)
	{
		producto.Nombre = NombreEntry.Text;
		producto.Descripcion = DescripcionEntry.Text;
		producto.Precio = decimal.Parse(PrecioEntry.Text);



		await firebaseHelpers.UpdateProducto(producto.Id, producto);
		await DisplayAlert("AVISO", "Producto actualizacion de reporte", "OK");
		await Navigation.PopAsync();


    }
}