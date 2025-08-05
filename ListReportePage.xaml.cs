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
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await LoadReportes(); // Recargar cuando la página aparece
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en OnAppearing: {ex.Message}");
            await DisplayAlert("❌ Error", "Error al cargar la página", "OK");
        }
    }

    private async Task LoadReportes()
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
        try
        {
            await LoadReportes();
            SearchEntry.Text = string.Empty;
            await DisplayAlert("✅ Actualizado", "Lista de reportes actualizada correctamente", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al actualizar: {ex.Message}", "OK");
        }
    }

    private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            FiltrarReportes(e.NewTextValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en búsqueda: {ex.Message}");
        }
    }

    private async void FiltrarReportes(string searchText)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error al filtrar reportes: {ex.Message}");
        }
    }

    // Filtros por categoría
    private async void BtnTodos_Clicked(object sender, EventArgs e)
    {
        try
        {
            await AplicarFiltroTodos();
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al aplicar filtro: {ex.Message}", "OK");
        }
    }

    private async void BtnBug_Clicked(object sender, EventArgs e)
    {
        try
        {
            await AplicarFiltroPorCategoria(CategoriaIncidencia.Bug);
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al aplicar filtro: {ex.Message}", "OK");
        }
    }

    private async void BtnCrash_Clicked(object sender, EventArgs e)
    {
        try
        {
            await AplicarFiltroPorCategoria(CategoriaIncidencia.Crash);
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al aplicar filtro: {ex.Message}", "OK");
        }
    }

    private async void BtnAbiertos_Clicked(object sender, EventArgs e)
    {
        try
        {
            await AplicarFiltroPorEstado(EstadoIncidencia.Abierto);
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al aplicar filtro: {ex.Message}", "OK");
        }
    }

    private async void BtnEnProgreso_Clicked(object sender, EventArgs e)
    {
        try
        {
            await AplicarFiltroPorEstado(EstadoIncidencia.EnProgreso);
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al aplicar filtro: {ex.Message}", "OK");
        }
    }

    private async void BtnResueltos_Clicked(object sender, EventArgs e)
    {
        try
        {
            await AplicarFiltroPorEstado(EstadoIncidencia.Resuelto);
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al aplicar filtro: {ex.Message}", "OK");
        }
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
        catch (Exception ex)
        {
            throw new Exception($"Error al cargar todos los reportes: {ex.Message}");
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
        catch (Exception ex)
        {
            throw new Exception($"Error al filtrar por categoría: {ex.Message}");
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
        catch (Exception ex)
        {
            throw new Exception($"Error al filtrar por estado: {ex.Message}");
        }
        finally
        {
            await MostrarLoading(false);
        }
    }

    private async void BtnEditReporte_Clicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var reporte = button?.BindingContext as Reporte;

            if (reporte != null)
            {
                await Navigation.PushAsync(new EditReportePage(reporte));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al abrir editor: {ex.Message}", "OK");
        }
    }

    private async void BtnDeleteReporte_Clicked(object sender, EventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al eliminar reporte: {ex.Message}", "OK");
        }
    }

    private async void BtnResolverReporte_Clicked(object sender, EventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al resolver reporte: {ex.Message}", "OK");
        }
    }

    private async void OnReporteTapped(object sender, EventArgs e)
    {
        try
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
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al mostrar detalles: {ex.Message}", "OK");
        }
    }

    private async Task MostrarLoading(bool mostrar)
    {
        try
        {
            LoadingIndicator.IsVisible = mostrar;
            LoadingIndicator.IsRunning = mostrar;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al mostrar loading: {ex.Message}");
        }
    }
}