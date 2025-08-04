using AppIntegradora10A.Helpers;
using AppIntegradora10A.Models;
using System;
using System.Threading.Tasks;

namespace AppIntegradora10A.Views;

public partial class EditReportePage : ContentPage
{
    FirebaseHelpers firebaseHelpers = new FirebaseHelpers();
    private Reporte reporte;

    public EditReportePage(Reporte reporte)
    {
        InitializeComponent();
        this.reporte = reporte;
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
        var estadoOriginal = reporte.Estado;
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
            return