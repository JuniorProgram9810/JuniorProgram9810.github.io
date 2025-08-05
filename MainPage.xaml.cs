using AppIntegradora10A.Helpers;
using AppIntegradora10A.Models;
using AppIntegradora10A.Views;

namespace AppIntegradora10A
{
    public partial class MainPage : ContentPage
    {
        FirebaseHelpers firebaseHelpers = new FirebaseHelpers();

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Cargar estadísticas al aparecer la página
            try
            {
                await CargarEstadisticas();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar estadísticas en OnAppearing: {ex.Message}");
            }
        }

        // Métodos que coinciden con los botones en tu XAML
        private async void OnAddProduct_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new AddReportePage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("❌ Error", $"Error al abrir página de agregar reporte: {ex.Message}", "OK");
            }
        }

        private async void OnListProduct_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ListReportePage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("❌ Error", $"Error al abrir lista de reportes: {ex.Message}", "OK");
            }
        }

        private async void BtnActualizar_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MostrarLoading(true);
                await CargarEstadisticas();
                await MostrarLoading(false);
                await DisplayAlert("🔄 Actualizado", "Estadísticas actualizadas correctamente", "OK");
            }
            catch (Exception ex)
            {
                await MostrarLoading(false);
                await DisplayAlert("❌ Error", $"Error al actualizar estadísticas: {ex.Message}", "OK");
            }
        }

        private async void BtnAyuda_Clicked(object sender, EventArgs e)
        {
            try
            {
                await DisplayAlert("❓ Ayuda - GameSupport Hub",
                    "🎮 CÓMO USAR LA APLICACIÓN:\n\n" +
                    "➕ REPORTAR INCIDENCIA:\n" +
                    "• Toca 'Reportar Nueva Incidencia'\n" +
                    "• Llena todos los campos requeridos\n" +
                    "• Describe detalladamente el problema\n\n" +
                    "📋 VER REPORTES:\n" +
                    "• Toca 'Ver Todos los Reportes'\n" +
                    "• Usa filtros para encontrar reportes específicos\n" +
                    "• Toca un reporte para ver detalles completos\n\n" +
                    "🔧 GESTIONAR REPORTES:\n" +
                    "• Editar: Actualiza información del reporte\n" +
                    "• Eliminar: Borra reportes innecesarios\n" +
                    "• Resolver: Marca reportes como solucionados\n\n" +
                    "🎯 FILTROS RÁPIDOS:\n" +
                    "• Usa los botones de filtro para ver categorías específicas\n" +
                    "• Los reportes críticos requieren atención inmediata\n\n" +
                    "💡 ¿NECESITAS MÁS AYUDA?\n" +
                    "Contacta al equipo de desarrollo para soporte adicional.",
                    "Cerrar");
            }
            catch (Exception ex)
            {
                await DisplayAlert("❌ Error", $"Error al mostrar ayuda: {ex.Message}", "OK");
            }
        }

        // Métodos para filtros rápidos
        private async void BtnFiltrarBugs_Clicked(object sender, EventArgs e)
        {
            try
            {
                await DisplayAlert("🐛 Filtrar por Bugs",
                    "Abriendo lista de reportes...\n\nEn la lista, usa el filtro 'Bug' para ver solo reportes de errores de código.",
                    "Continuar");
                await Navigation.PushAsync(new ListReportePage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("❌ Error", $"Error al filtrar bugs: {ex.Message}", "OK");
            }
        }

        private async void BtnFiltrarCrashes_Clicked(object sender, EventArgs e)
        {
            try
            {
                await DisplayAlert("💥 Filtrar por Crashes",
                    "Abriendo lista de reportes...\n\nEn la lista, usa el filtro 'Crash' para ver solo reportes de cierres inesperados.",
                    "Continuar");
                await Navigation.PushAsync(new ListReportePage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("❌ Error", $"Error al filtrar crashes: {ex.Message}", "OK");
            }
        }

        private async void BtnFiltrarPerformance_Clicked(object sender, EventArgs e)
        {
            try
            {
                await DisplayAlert("⚡ Filtrar por Performance",
                    "Abriendo lista de reportes...\n\nEn la lista, usa el filtro 'Performance' para ver solo reportes de rendimiento.",
                    "Continuar");
                await Navigation.PushAsync(new ListReportePage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("❌ Error", $"Error al filtrar performance: {ex.Message}", "OK");
            }
        }

        private async void BtnFiltrarCriticos_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MostrarLoading(true);

                // Buscar reportes críticos
                var todosReportes = await firebaseHelpers.GetAllReportes();
                var reportesCriticos = todosReportes.Where(r => r.Prioridad == PrioridadIncidencia.Critica).ToList();

                await MostrarLoading(false);

                if (reportesCriticos.Any())
                {
                    await DisplayAlert("🎯 Reportes Críticos",
                        $"Se encontraron {reportesCriticos.Count} reporte(s) crítico(s) que requieren atención inmediata.\n\nAbriendo lista de reportes...",
                        "Ver Críticos");
                    await Navigation.PushAsync(new ListReportePage());
                }
                else
                {
                    await DisplayAlert("✅ Excelente",
                        "¡No hay reportes críticos pendientes!\n\nTodos los problemas críticos han sido resueltos.",
                        "Genial");
                }
            }
            catch (Exception ex)
            {
                await MostrarLoading(false);
                await DisplayAlert("❌ Error", $"Error al filtrar críticos: {ex.Message}", "OK");
            }
        }

        private async Task CargarEstadisticas()
        {
            try
            {
                var todosReportes = await firebaseHelpers.GetAllReportes();

                var total = todosReportes.Count;
                var abiertos = todosReportes.Count(r => r.Estado == EstadoIncidencia.Abierto);
                var resueltos = todosReportes.Count(r => r.Estado == EstadoIncidencia.Resuelto);

                // Actualizar labels de estadísticas
                LblTotalReportes.Text = total.ToString();
                LblReportesAbiertos.Text = abiertos.ToString();
                LblReportesResueltos.Text = resueltos.ToString();
            }
            catch (Exception ex)
            {
                // En caso de error, mostrar 0s
                LblTotalReportes.Text = "0";
                LblReportesAbiertos.Text = "0";
                LblReportesResueltos.Text = "0";

                Console.WriteLine($"Error al cargar estadísticas: {ex.Message}");
            }
        }

        private async void OnLogout_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("🚪 Cerrar Sesión",
                "¿Estás seguro de que deseas cerrar sesión?\n\n✓ Se guardarán todos tus reportes\n✓ Podrás volver a acceder cuando quieras\n\n¿Continuar?",
                "Sí, cerrar sesión", "Cancelar");

            if (confirm)
            {
                try
                {
                    // Mostrar mensaje de despedida
                    await DisplayAlert("👋 Hasta pronto",
                        "Has cerrado sesión correctamente.\n\n🎮 ¡Gracias por ayudar a mejorar la experiencia gaming!\n\n¡Te esperamos pronto!",
                        "OK");

                    // Volver al login
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("❌ Error", $"Error al cerrar sesión: {ex.Message}", "OK");
                }
            }
        }

        private async Task MostrarLoading(bool mostrar)
        {
            try
            {
                if (LoadingIndicator != null)
                {
                    LoadingIndicator.IsVisible = mostrar;
                    LoadingIndicator.IsRunning = mostrar;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en MostrarLoading: {ex.Message}");
            }
        }
    }
}