
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
    }

    private void ConfigurarPickers()
    {
        // Establecer valores por defecto
        CategoriaPicker.SelectedIndex = 0; // Bug
        PrioridadPicker.SelectedIndex = 1; // Media
    }

    private async void BtnAgregarReporte_Clicked(object sender, EventArgs e)
    {
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
                UsuarioReportador = UsuarioEntry.Text?.Trim() ?? "Anónimo"
            };

            bool exito = await firebaseHelpers.AddReporte(reporte);

            if (exito)
            {
                await DisplayAlert("✅ Éxito",
                    "Reporte de incidencia agregado correctamente.\n\nNuestro equipo lo revisará pronto.",
                    "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("❌ Error",
                    "Hubo un problema al guardar el reporte.\nPor favor, inténtalo de nuevo.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error",
                $"Error inesperado: {ex.Message}",
                "OK");
        }
        finally
        {
            await MostrarLoading(false);
        }
    }

    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("⚠️ Confirmar",
            "¿Estás seguro de que quieres cancelar?\nSe perderán todos los datos ingresados.",
            "Sí, cancelar", "No, continuar");

        if (confirmar)
        {
            await Navigation.PopAsync();
        }
    }

    private bool ValidarCampos()
    {
        if (string.IsNullOrWhiteSpace(TipoIncidenciaEntry.Text))
        {
            MostrarError("Por favor, describe el tipo de incidencia.");
            TipoIncidenciaEntry.Focus();
            return false;
        }

        if (TipoIncidenciaEntry.Text.Length < 10)
        {
            MostrarError("El tipo de incidencia debe tener al menos 10 caracteres.");
            TipoIncidenciaEntry.Focus();
            return false;
        }

        if (CategoriaPicker.SelectedIndex == -1)
        {
            MostrarError("Por favor, selecciona una categoría.");
            return false;
        }

        if (PrioridadPicker.SelectedIndex == -1)
        {
            MostrarError("Por favor, selecciona una prioridad.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(DescripcionEditor.Text))
        {
            MostrarError("Por favor, proporciona una descripción detallada del problema.");
            DescripcionEditor.Focus();
            return false;
        }

        if (DescripcionEditor.Text.Length < 20)
        {
            MostrarError("La descripción debe tener al menos 20 caracteres para ser útil.");
            DescripcionEditor.Focus();
            return false;
        }

        return true;
    }

    private async void MostrarError(string mensaje)
    {
        await DisplayAlert("⚠️ Datos Incompletos", mensaje, "OK");
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
        // Limpiar campos si es necesario
        LimpiarCampos();
    }

    private void LimpiarCampos()
    {
        TipoIncidenciaEntry.Text = string.Empty;
        DescripcionEditor.Text = string.Empty;
        CodigoErrorEntry.Text = string.Empty;
        UsuarioEntry.Text = string.Empty;
        CategoriaPicker.SelectedIndex = 0;
        PrioridadPicker.SelectedIndex = 1;
    }
}