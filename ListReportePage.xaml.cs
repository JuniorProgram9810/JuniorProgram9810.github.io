using AppIntegradora10A.Helpers;
using AppIntegradora10A.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AppIntegradora10A.Views;

public partial class ListReportePage : ContentPage
{
    FirebaseHelpers firebaseHelpers = new FirebaseHelpers();
    private ObservableCollection<Reporte> reportesOriginales = new ObservableCollection<Reporte>();
    private ObservableCollection<Reporte> reportesFiltrados = new ObservableCollection<Reporte>();

    public ListReportePage()
    {
        InitializeComponent();
        ReporteCollectionView.ItemsSource = reportesFiltrados;
        LoadReportes();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadReportes(); // Recargar cuando la página aparece
    }

    private async void LoadReportes()
    {
        await MostrarLoading(true);

        try
        {
            var reportes = await firebaseHelpers.GetAllReportes();
            reportesOriginales.Clear();
            reportesFiltrados.Clear();

            foreach (var reporte in reportes)
            {
                reportesOriginales.Add(reporte);
                reportesFiltrados.Add(reporte);
            }

            ActualizarEstadisticas();
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al cargar reportes: {ex.Message}", "OK");
        }
        finally
        {
            await MostrarLoading(false);
        }
    }

    private void ActualizarEstadisticas()
    {
        var total = reportesFiltrados.Count;
        var abiertos = reportesFiltrados.Count(r => r.Estado == EstadoIncidencia.Abierto);
        var resueltos = reportesFiltrados.Count(r => r.Estado == EstadoIncidencia.Resuelto);

        LblTotalReportes.Text = $"Total: {total}";
        LblReportesAbiertos.Text = $"Abiertos: {abiertos}";
        LblReportesResueltos.Text = $"Resueltos: {resueltos}";
    }

    private async void BtnRefresh_Clicked(object sender, EventArgs e)
    {
        await LoadReportes();
        SearchEntry.Text = string.Empty;
        await DisplayAlert("✅ Actualizado", "Lista de reportes actualizada correctamente", "OK");
    }

    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        FiltrarReportes(e.NewTextValue);
    }

    private async void FiltrarReportes(string searchText)
    {
        reportesFiltrados.Clear();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            foreach (var reporte in reportesOriginales)
            {
                reportesFiltrados.Add(reporte);
            }
        }
        else
        {
            var reportesFiltradosTemp = await firebaseHelpers.SearchReportes(searchText);
            foreach (var reporte in reportesFiltradosTemp)
            {
                reportesFiltrados.Add(reporte);
            }
        }

        ActualizarEstadisticas();
    }

    // Filtros por categoría
    private async void BtnTodos_Clicked(object sender, EventArgs e)
    {
        await AplicarFiltroTodos();
    }

    private async void BtnBug_Clicked(object sender, EventArgs e)
    {
        await AplicarFiltroPorCategoria(CategoriaIncidencia.Bug);
    }

    private async void BtnCrash_Clicked(object sender, EventArgs e)
    {
        await AplicarFiltroPorCategoria(CategoriaIncidencia.Crash);
    }

    private async void BtnAbiertos_Clicked(object sender, EventArgs e)
    {
        await AplicarFiltroPorEstado(EstadoIncidencia.Abierto);
    }

    private async void BtnEnProgreso_Clicked(object sender, EventArgs e)
    {
        await AplicarFiltroPorEstado(EstadoIncidencia.EnProgreso);
    }

    private async void BtnResueltos_Clicked(object sender, EventArgs e)
    {
        await AplicarFiltroPorEstado(EstadoIncidencia.Resuelto);
    }

    private async Task AplicarFiltroTodos()
    {
        await MostrarLoading(true);
        try
        {
            var todosReportes = await firebaseHelpers.GetAllReportes();
            reportesFiltrados.Clear();
            foreach (var reporte in todosReportes)
            {
                reportesFiltrados.Add(reporte);
            }
            ActualizarEstadisticas();
        }
        finally
        {
            await MostrarLoading(false);
        }
    }

    private async Task AplicarFiltroPorCategoria(CategoriaIncidencia categoria)
    {
        await MostrarLoading(true);
        try
        {
            var reportesFiltradosTemp = await firebaseHelpers.GetReportesByCategoria(categoria);
            reportesFiltrados.Clear();
            foreach (var reporte in reportesFiltradosTemp)
            {
                reportesFiltrados.Add(reporte);
            }
            ActualizarEstadisticas();
        }
        finally
        {
            await MostrarLoading(false);
        }
    }

    private async Task AplicarFiltroPorEstado(EstadoIncidencia estado)
    {
        await MostrarLoading(true);
        try
        {
            var reportesFiltradosTemp = await firebaseHelpers.GetReportesByEstado(estado);
            reportesFiltrados.Clear();
            foreach (var reporte in reportesFiltradosTemp)
            {
                reportesFiltrados.Add(reporte);
            }
            ActualizarEstadisticas();
        }
        finally
        {
            await MostrarLoading(false);
        }
    }

    private async void BtnEditReporte_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var reporte = button?.BindingContext as Reporte;

        if (reporte != null)
        {
            await Navigation.PushAsync(new EditReportePage(reporte));
        }
    }

    private async void BtnDeleteReporte_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var reporte = button?.BindingContext as Reporte;

        if (reporte != null && !string.IsNullOrEmpty(reporte.Id))
        {
            bool confirmar = await DisplayAlert("⚠️ Confirmar Eliminación",
                $"¿Estás seguro de que quieres eliminar este reporte?\n\nTipo: {reporte.TipoIncidencia}\nUsuario: {reporte.UsuarioReportador}",
                "Sí, eliminar", "Cancelar");

            if (confirmar)
            {
                await MostrarLoading(true);
                try
                {
                    bool exito = await firebaseHelpers.DeleteReporte(reporte.Id);
                    if (exito)
                    {
                        await DisplayAlert("✅ Eliminado", "Reporte eliminado correctamente", "OK");
                        await LoadReportes();
                    }
                    else
                    {
                        await DisplayAlert("❌ Error", "No se pudo eliminar el reporte", "OK");
                    }
                }
                finally
                {
                    await MostrarLoading(false);
                }
            }
        }
        else
        {
            await DisplayAlert("❌ Error", "No se pudo encontrar el reporte para eliminar", "OK");
        }
    }

    private async void BtnResolverReporte_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var reporte = button?.BindingContext as Reporte;

        if (reporte != null && !string.IsNullOrEmpty(reporte.Id))
        {
            if (reporte.Estado == EstadoIncidencia.Resuelto)
            {
                await DisplayAlert("ℹ️ Información", "Este reporte ya está marcado como resuelto", "OK");
                return;
            }

            bool confirmar = await DisplayAlert("✅ Marcar como Resuelto",
                $"¿Marcar este reporte como resuelto?\n\nTipo: {reporte.TipoIncidencia}",
                "Sí, resolver", "Cancelar");

            if (confirmar)
            {
                await MostrarLoading(true);
                try
                {
                    bool exito = await firebaseHelpers.UpdateEstadoReporte(reporte.Id, EstadoIncidencia.Resuelto);
                    if (exito)
                    {
                        await DisplayAlert("✅ Resuelto", "Reporte marcado como resuelto correctamente", "OK");
                        await LoadReportes();
                    }
                    else
                    {
                        await DisplayAlert("❌ Error", "No se pudo actualizar el estado del reporte", "OK");
                    }
                }
                finally
                {
                    await MostrarLoading(false);
                }
            }
        }
    }

    private async void OnReporteTapped(object sender, EventArgs e)
    {
        var grid = sender as Grid;
        var reporte = grid?.BindingContext as Reporte;

        if (reporte != null)
        {
            string detalles = $"🎮 DETALLES DEL REPORTE\n\n" +
                            $"📝 Tipo: {reporte.TipoIncidencia}\n" +
                            $"📂 Categoría: {reporte.CategoriaTexto}\n" +
                            $"⚡ Prioridad: {reporte.PrioridadTexto}\n" +
                            $"🔄 Estado: {reporte.EstadoTexto}\n" +
                            $"📅 Fecha: {reporte.FechaTexto}\n" +
                            $"👤 Usuario: {reporte.UsuarioReportador}\n" +
                            $"🔢 Código Error: {reporte.CodigoError}\n\n" +
                            $"📄 Descripción:\n{reporte.Descripcion}";

            await DisplayAlert("📋 Detalle Completo", detalles, "Cerrar");
        }
    }

    private async Task MostrarLoading(bool mostrar)
    {
        LoadingIndicator.IsVisible = mostrar;
        LoadingIndicator.IsRunning = mostrar;
    }
}