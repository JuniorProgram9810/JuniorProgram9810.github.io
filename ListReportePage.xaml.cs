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

        // Verificar sesión
        if (UsuarioSesion.UsuarioActual == null)
        {
            await DisplayAlert("❌ Sesión Expirada",
                "Tu sesión ha expirado. Serás redirigido al login.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        try
        {
            ConfigurarInterfazSegunUsuario();
            await LoadReportes();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en OnAppearing: {ex.Message}");
            await DisplayAlert("❌ Error", "Error al cargar la página", "OK");
        }
    }

    private void ConfigurarInterfazSegunUsuario()
    {
        try
        {
            var usuario = UsuarioSesion.UsuarioActual;
            if (usuario == null) return;

            // Actualizar título según el tipo de usuario
            if (usuario.EsAdmin)
            {
                Title = "📋 Todos los Reportes (Admin)";
                // El admin puede ver información adicional
                if (LblInfoUsuario != null)
                {
                    LblInfoUsuario.IsVisible = true;
                    LblInfoUsuario.Text = "👑 Modo Administrador - Viendo reportes de todos los usuarios";
                }
            }
            else
            {
                Title = "📋 Mis Reportes";
                if (LblInfoUsuario != null)
                {
                    LblInfoUsuario.IsVisible = true;
                    LblInfoUsuario.Text = $"🎮 Tus reportes - {usuario.Email}";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al configurar interfaz: {ex.Message}");
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

            // Mostrar mensaje informativo si no hay reportes
            if (!reportes.Any())
            {
                string mensaje;
                if (UsuarioSesion.UsuarioActual.EsAdmin)
                {
                    mensaje = "📋 No hay reportes registrados en el sistema.\n\n¡Es una buena señal! No hay incidencias pendientes.";
                }
                else
                {
                    mensaje = "📋 No tienes reportes creados aún.\n\n¿Experimentaste algún problema en el juego?\n¡Crea tu primer reporte desde el menú principal!";
                }

                await DisplayAlert("ℹ️ Sin Reportes", mensaje, "OK");
            }
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

            string mensaje = UsuarioSesion.UsuarioActual.EsAdmin
                ? "✅ Lista actualizada - Mostrando reportes de todos los usuarios"
                : "✅ Lista actualizada - Mostrando solo tus reportes";

            await DisplayAlert("🔄 Actualizado", mensaje, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al actualizar: {ex.Message}", "OK");
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
                // Verificar permisos antes de editar
                if (!PuedeEditarReporte(reporte))
                {
                    await DisplayAlert("🚫 Sin Permisos",
                        "No puedes editar este reporte.\n\n📝 Solo puedes editar tus propios reportes.", "OK");
                    return;
                }

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
                // Verificar permisos antes de eliminar
                if (!PuedeEditarReporte(reporte))
                {
                    await DisplayAlert("🚫 Sin Permisos",
                        "No puedes eliminar este reporte.\n\n🗑️ Solo puedes eliminar tus propios reportes.", "OK");
                    return;
                }

                string confirmMessage = $"⚠️ ¿Eliminar este reporte?\n\n";
                confirmMessage += $"📝 Tipo: {reporte.TipoIncidencia}\n";
                confirmMessage += $"👤 Usuario: {reporte.UsuarioReportador}\n";
                confirmMessage += $"📅 Fecha: {reporte.FechaTexto}\n\n";
                confirmMessage += "❌ Esta acción no se puede deshacer.";

                bool confirmar = await DisplayAlert("🗑️ Confirmar Eliminación",
                    confirmMessage, "Sí, eliminar", "Cancelar");

                if (confirmar)
                {
                    await MostrarLoading(true);
                    try
                    {
                        bool exito = await firebaseHelpers.DeleteReporte(reporte.Id);
                        if (exito)
                        {
                            await DisplayAlert("✅ Eliminado",
                                "Reporte eliminado correctamente.\n\n🔄 La lista se actualizará automáticamente.", "OK");
                            await LoadReportes();
                        }
                        else
                        {
                            await DisplayAlert("❌ Error",
                                "No se pudo eliminar el reporte.\n\n💡 Verifica tu conexión e intenta nuevamente.", "OK");
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
                // Verificar permisos
                if (!PuedeEditarReporte(reporte))
                {
                    await DisplayAlert("🚫 Sin Permisos",
                        "No puedes cambiar el estado de este reporte.\n\n✅ Solo puedes gestionar tus propios reportes.", "OK");
                    return;
                }

                if (reporte.Estado == EstadoIncidencia.Resuelto)
                {
                    await DisplayAlert("ℹ️ Ya Resuelto",
                        "Este reporte ya está marcado como resuelto.\n\n✅ ¡Excelente! El problema ya fue solucionado.", "OK");
                    return;
                }

                string confirmMessage = $"✅ ¿Marcar como resuelto?\n\n";
                confirmMessage += $"📝 Tipo: {reporte.TipoIncidencia}\n";
                confirmMessage += $"🔄 Estado actual: {reporte.EstadoTexto}\n\n";
                confirmMessage += "¿Confirmas que este problema ha sido solucionado?";

                bool confirmar = await DisplayAlert("✅ Marcar como Resuelto",
                    confirmMessage, "Sí, resolver", "Cancelar");

                if (confirmar)
                {
                    await MostrarLoading(true);
                    try
                    {
                        bool exito = await firebaseHelpers.UpdateEstadoReporte(reporte.Id, EstadoIncidencia.Resuelto);
                        if (exito)
                        {
                            await DisplayAlert("🎉 ¡Resuelto!",
                                "Reporte marcado como resuelto correctamente.\n\n✅ ¡Gracias por confirmar que el problema fue solucionado!", "OK");
                            await LoadReportes();
                        }
                        else
                        {
                            await DisplayAlert("❌ Error",
                                "No se pudo actualizar el estado del reporte.\n\n💡 Intenta nuevamente en unos momentos.", "OK");
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

    private bool PuedeEditarReporte(Reporte reporte)
    {
        if (UsuarioSesion.UsuarioActual == null)
            return false;

        // El admin puede editar cualquier reporte
        if (UsuarioSesion.UsuarioActual.EsAdmin)
            return true;

        // El usuario común solo puede editar sus propios reportes
        return reporte.IdUsuario == UsuarioSesion.UsuarioActual.LocalId;
    }

    private async void OnReporteTapped(object sender, EventArgs e)
    {
        try
        {
            var grid = sender as Grid;
            var reporte = grid?.BindingContext as Reporte;

            if (reporte != null)
            {
                string detalles = $"🎮 DETALLES DEL REPORTE\n\n";
                detalles += $"📝 Tipo: {reporte.TipoIncidencia}\n";
                detalles += $"📂 Categoría: {reporte.CategoriaTexto}\n";
                detalles += $"⚡ Prioridad: {reporte.PrioridadTexto}\n";
                detalles += $"🔄 Estado: {reporte.EstadoTexto}\n";
                detalles += $"📅 Fecha: {reporte.FechaTexto}\n";
                detalles += $"👤 Usuario: {reporte.UsuarioReportador}\n";

                // Solo mostrar email si es admin
                if (UsuarioSesion.UsuarioActual.EsAdmin && !string.IsNullOrEmpty(reporte.EmailUsuario))
                {
                    detalles += $"📧 Email: {reporte.EmailUsuario}\n";
                }

                detalles += $"🔢 Código Error: {reporte.CodigoError}\n\n";
                detalles += $"📄 Descripción:\n{reporte.Descripcion}";

                if (reporte.FechaActualizacion.HasValue)
                {
                    detalles += $"\n\n🔄 Última actualización: {reporte.FechaActualizacion.Value:dd/MM/yyyy HH:mm}";
                }

                await DisplayAlert("📋 Detalle Completo", detalles, "Cerrar");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error", $"Error al mostrar detalles: {ex.Message}", "OK");
        }
    }

    // Resto de métodos de filtrado... (mantener los existentes)
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

    // Métodos de filtro existentes...
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

    private async Task MostrarErrorSesion()
    {
        await DisplayAlert("❌ Sesión Expirada",
            "Tu sesión ha expirado. Serás redirigido al login.", "OK");
        await Shell.Current.GoToAsync("//LoginPage");
    }
}