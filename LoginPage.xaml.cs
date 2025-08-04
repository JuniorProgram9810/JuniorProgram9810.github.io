
using AppIntegradora10A;
using AppIntegradora10A.Services;
using System.Threading.Tasks;

namespace AppIntegradora10A.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLogin_Clicked(object sender, EventArgs e)
    {
        string email = emailEntry.Text;
        string password = passwordEntry.Text;

        if (string.IsNullOrEmpty(email))
        {
            await DisplayAlert("AVISO", "Debe escribir un correo electronico", "OK");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Aviso", "Debe escribir la contraseña", "OK");
            return;
        }

        //Validar la longitud de la contraseña
        if (password.Length < 6)
        {
            await DisplayAlert("ERROR", "La contraseña debe contener mínimo seis caracteres", "OK");
            return;
        }

        var authService = new FirebaseAuthService();
        var user = await authService.SignInWithEmailAndPasswordAsync(email, password);

        if (user != null)
        {
            await DisplayAlert("Exito", $"Bienvenido, {user.Email}", "OK");

            // Navegar al menú principal del CRUD
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            await DisplayAlert("Error", "Credenciales incorrectas", "OK");
        }
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        string email = emailEntry.Text;
        string password = passwordEntry.Text;

        if (string.IsNullOrEmpty(email))
        {
            await DisplayAlert("AVISO", "Debe escribir un correo electronico", "OK");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Aviso", "Debe escribir la contraseña", "OK");
            return;
        }

        if (password.Length < 6)
        {
            await DisplayAlert("ERROR", "La contraseña debe contener mínimo seis caracteres", "OK");
            return;
        }

        var authService = new FirebaseAuthService();
        var user = await authService.RegisterWithEmalAndPasswordAsync(email, password);

        if (user != null)
        {
            await DisplayAlert("Registro exitoso", $"Cuenta creada para: {user.Email}", "OK");

            // Navegar al menú principal del CRUD después del registro
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            await DisplayAlert("Error", "No se pudo registrar. Verifica los datos.", "OK");
        }
    }
}