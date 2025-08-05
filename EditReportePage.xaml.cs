using AppIntegradora10A.Helpers;
using AppIntegradora10A.Models;
using System;
using System.Threading.Tasks;

namespace AppIntegradora10A.Views;

public partial class EditReportePage : ContentPage
{
    FirebaseHelpers firebaseHelpers = new FirebaseHelpers();
    private Reporte reporte;
    private EstadoIncidencia estadoOriginal;

    public EditReportePage(Reporte reporte)
    {
        InitializeComponent();
        this.reporte = reporte;
        this.estadoOriginal = reporte.Estado;
        CargarDatosReporte();
    }

    private void CargarDatosReporte()
    {
        // Cargar datos existentes
        TipoIncidenciaEntry.Text = reporte.TipoIncidencia;
        DescripcionEditor.Text = reporte.Descripcion;
        CodigoErrorEntry.Text = reporte.CodigoError;
        UsuarioEntry.Text = reporte.UsuarioReportador;

        // Establecer selecciones en pickers
        CategoriaPicker.SelectedIndex = (int)reporte.Categoria;
        PrioridadPicker.SelectedIndex = (int)reporte.Prioridad;
        EstadoPicker.SelectedIndex = (int)reporte.Estado;

        // Mostrar información del reporte
        LblFechaCreacion.Text = $"Creado: {reporte.FechaTexto}";
        LblEstadoActual.Text = $"Estado actual: {reporte.EstadoTexto}";

        if (reporte.FechaActualizacion.HasValue)
        {
            LblEstadoActual.Text += $" (Actualizado: {reporte.FechaActualizacion.Value:dd/MM/yyyy HH:mm})";
        }
    }

    private async void BtnGuardarCambios_Clicked(object sender, EventArgs e)
    {
        if (!ValidarCampos())
            return;

        await MostrarLoading(true);

        try
        {
            // Actualizar los datos del reporte
            reporte.TipoIncidencia = TipoIncidenciaEntry.Text?.Trim();
            reporte.Descripcion = DescripcionEditor.Text?.Trim();
            reporte.CodigoError = CodigoErrorEntry.Text?.Trim() ?? "N/A";
            reporte.Categoria = (CategoriaIncidencia)CategoriaPicker.SelectedIndex;
            reporte.Prioridad = (PrioridadIncidencia)PrioridadPicker.SelectedIndex;
            reporte.Estado = (EstadoIncidencia)EstadoPicker.SelectedIndex;
            reporte.UsuarioReportador = UsuarioEntry.Text?.Trim() ?? "Anónimo";
            reporte.FechaActualizacion = DateTime.Now;

            bool exito = await firebaseHelpers.UpdateReporte(reporte.Id, reporte);

            if (exito)
            {
                string mensaje = "✅ Reporte actualizado correctamente.\n\n";

                // Mostrar qué cambió
                if (EstadoOriginalCambio())
                {
                    mensaje += $"🔄 Estado cambiado a: {reporte.EstadoTexto}\n";
                }

                mensaje += "Los cambios se han guardado exitosamente.";

                await DisplayAlert("✅ Éxito", mensaje, "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("❌ Error",
                    "Hubo un problema al actualizar el reporte.\nPor favor, inténtalo de nuevo.",
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

    private bool EstadoOriginalCambio()
    {
        var estadoNuevo = (EstadoIncidencia)EstadoPicker.SelectedIndex;
        return estadoOriginal != estadoNuevo;
    }

    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("⚠️ Confirmar",
            "¿Estás seguro de que quieres cancelar?\nSe perderán todos los cambios realizados.",
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

        if (EstadoPicker.SelectedIndex == -1)
        {
            MostrarError("Por favor, selecciona un estado.");
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

        if (string.IsNullOrWhiteSpace(UsuarioEntry.Text))
        {
            MostrarError("Por favor, ingresa el nombre del usuario reportador.");
            UsuarioEntry.Focus();
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
        BtnGuardarCambios.IsEnabled = !mostrar;
        BtnCancelar.IsEnabled = !mostrar;

        if (mostrar)
        {
            BtnGuardarCambios.Text = "⏳ Guardando...";
        }
        else
        {
            BtnGuardarCambios.Text = "💾 Guardar Cambios";
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Refrescar los datos si es necesario
        CargarDatosReporte();
    }
}