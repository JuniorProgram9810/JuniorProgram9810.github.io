using AppIntegradora10A.Helpers;
using AppIntegradora10A.Models;
using System;
using System.Threading.Tasks;

namespace AppIntegradora10A.Views;

public partial class AddReportePage : ContentPage
{
    FirebaseHelpers firebaseHelpers = new FirebaseHelpers();

    public AddReportePage()
    {
        InitializeComponent();
        ConfigurarPickers();
        ConfigurarUsuario();
    }

    private void ConfigurarPickers()
    {
        // Establecer valores por defecto
        CategoriaPicker.SelectedIndex = 0; // Bug
        PrioridadPicker.SelectedIndex = 1; // Media
    }

    private void ConfigurarUsuario()
    {
        try
        {
            // Pre-llenar el campo de usuario con la información del usuario logueado
            if (UsuarioSesion.UsuarioActual != null)
            {
                UsuarioEntry.Text = UsuarioSesion.UsuarioActual.NombreUsuario;
                // Hacer el campo de solo lectura para que no se pueda cambiar
                UsuarioEntry.IsReadOnly = true;
                UsuarioEntry.BackgroundColor = Color.FromArgb("#F5F5F5");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al configurar usuario: {ex.Message}");
        }
    }

    private async void BtnAgregarReporte_Clicked(object sender, EventArgs e)
    {
        // Verificar que el usuario esté logueado
        if (UsuarioSesion.UsuarioActual == null)
        {
            await DisplayAlert("❌ Error de Sesión",
                "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        if (!ValidarCampos())
            return;

        await MostrarLoading(true);

        try
        {
            var reporte = new Reporte
            {
                TipoIncidencia = TipoIncidenciaEntry.Text?.Trim(),
                Descripcion = DescripcionEditor.Text?.Trim(),
                CodigoError = CodigoErrorEntry.Text?.Trim() ?? "N/A",
                Categoria = (CategoriaIncidencia)CategoriaPicker.SelectedIndex,
                Prioridad = (PrioridadIncidencia)PrioridadPicker.SelectedIndex,
                Estado = EstadoIncidencia.Abierto,
                FechaCreacion = DateTime.Now,
                UsuarioReportador = UsuarioEntry.Text?.Trim() ?? "Anónimo",
                // ASIGNAR INFORMACIÓN DEL USUARIO ACTUAL
                EmailUsuario = UsuarioSesion.UsuarioActual.Email,
                IdUsuario = UsuarioSesion.UsuarioActual.LocalId
            };

            bool exito = await firebaseHelpers.AddReporte(reporte);

            if (exito)
            {
                string mensaje = "✅ Reporte de incidencia creado correctamente.\n\n";
                mensaje += $"📝 Tipo: {reporte.TipoIncidencia}\n";
                mensaje += $"📂 Categoría: {reporte.CategoriaTexto}\n";
                mensaje += $"⚡ Prioridad: {reporte.PrioridadTexto}\n";
                mensaje += $"👤 Reportado por: {reporte.UsuarioReportador}\n\n";
                mensaje += "🔄 Tu reporte ha sido registrado y nuestro equipo lo revisará pronto.\n";
                mensaje += "📋 Puedes consultar el estado en 'Ver Reportes'.";

                await DisplayAlert("🎮 Reporte Creado", mensaje, "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("❌ Error",
                    "Hubo un problema al guardar el reporte.\n\n💡 Sugerencias:\n• Verifica tu conexión a internet\n• Intenta nuevamente en unos momentos\n• Si el problema persiste, contacta soporte",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error Inesperado",
                $"Error al crear el reporte:\n\n{ex.Message}\n\n💡 Por favor, intenta nuevamente.",
                "OK");
        }
        finally
        {
            await MostrarLoading(false);
        }
    }

    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("⚠️ Confirmar Cancelación",
            "¿Estás seguro de que quieres cancelar?\n\n❌ Se perderán todos los datos ingresados\n✅ No se creará el reporte\n\n¿Continuar?",
            "Sí, cancelar", "No, continuar editando");

        if (confirmar)
        {
            await Navigation.PopAsync();
        }
    }

    private bool ValidarCampos()
    {
        if (string.IsNullOrWhiteSpace(TipoIncidenciaEntry.Text))
        {
            MostrarError("Por favor, describe el tipo de incidencia.\n\n💡 Ejemplo: 'Error al cargar partida guardada'");
            TipoIncidenciaEntry.Focus();
            return false;
        }

        if (TipoIncidenciaEntry.Text.Length < 10)
        {
            MostrarError("El tipo de incidencia debe tener al menos 10 caracteres.\n\n💡 Sé más específico para ayudarnos a entender mejor el problema.");
            TipoIncidenciaEntry.Focus();
            return false;
        }

        if (CategoriaPicker.SelectedIndex == -1)
        {
            MostrarError("Por favor, selecciona una categoría.\n\n💡 Esto nos ayuda a clasificar y priorizar tu reporte.");
            return false;
        }

        if (PrioridadPicker.SelectedIndex == -1)
        {
            MostrarError("Por favor, selecciona una prioridad.\n\n💡 Indica qué tan crítico es este problema para tu experiencia de juego.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(DescripcionEditor.Text))
        {
            MostrarError("Por favor, proporciona una descripción detallada del problema.\n\n💡 Incluye pasos para reproducir el error si es posible.");
            DescripcionEditor.Focus();
            return false;
        }

        if (DescripcionEditor.Text.Length < 20)
        {
            MostrarError("La descripción debe tener al menos 20 caracteres para ser útil.\n\n💡 Más detalles = mejor soporte técnico.");
            DescripcionEditor.Focus();
            return false;
        }

        return true;
    }

    private async void MostrarError(string mensaje)
    {
        await DisplayAlert("⚠️ Información Requerida", mensaje, "OK");
    }

    private async Task MostrarLoading(bool mostrar)
    {
        LoadingIndicator.IsVisible = mostrar;
        LoadingIndicator.IsRunning = mostrar;
        BtnAgregarReporte.IsEnabled = !mostrar;
        BtnCancelar.IsEnabled = !mostrar;

        if (mostrar)
        {
            BtnAgregarReporte.Text = "⏳ Guardando...";
        }
        else
        {
            BtnAgregarReporte.Text = "✅ Agregar Reporte";
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Verificar sesión al aparecer
        if (UsuarioSesion.UsuarioActual == null)
        {
            DisplayAlert("❌ Sesión Expirada",
                "Tu sesión ha expirado. Serás redirigido al login.", "OK");
            Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        // Limpiar campos si es necesario
        LimpiarCampos();
        ConfigurarUsuario(); // Reconfigurar usuario por si acaso
    }

    private void LimpiarCampos()
    {
        TipoIncidenciaEntry.Text = string.Empty;
        DescripcionEditor.Text = string.Empty;
        CodigoErrorEntry.Text = string.Empty;
        // No limpiar UsuarioEntry porque se auto-completa
        CategoriaPicker.SelectedIndex = 0;
        PrioridadPicker.SelectedIndex = 1;
    }
}