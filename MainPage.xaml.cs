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

            // Verificar que el usuario esté logueado
            if (UsuarioSesion.UsuarioActual == null)
            {
                await DisplayAlert("❌ Sesión Expirada",
                    "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            // Configurar interfaz según el tipo de usuario
            ConfigurarInterfazUsuario();

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

        private void ConfigurarInterfazUsuario()
        {
            try
            {
                var usuario = UsuarioSesion.UsuarioActual;
                if (usuario == null) return;

                // Actualizar información del usuario en la interfaz
                if (LblBienvenida != null)
                {
                    string tipoUsuario = usuario.EsAdmin ? "👑 Administrador" : "🎮 Usuario";
                    LblBienvenida.Text = $"¡Hola {tipoUsuario}!";
                }

                if (LblEmailUsuario != null)
                {
                    LblEmailUsuario.Text = usuario.Email;
                }

                // Configurar visibilidad de elementos según el rol
                if (usuario.EsAdmin)
                {
                    // El admin puede ver funciones adicionales
                    if (BtnEstadisticasAdmin != null)
                        BtnEstadisticasAdmin.IsVisible = true;

                    if (LblInfoAdmin != null)
                    {
                        LblInfoAdmin.IsVisible = true;
                        LblInfoAdmin.Text = "🔧 Panel de Administrador - Tienes acceso completo a todos los reportes";
                    }
                }
                else
                {
                    // Usuario común - ocultar funciones de admin
                    if (BtnEstadisticasAdmin != null)
                        BtnEstadisticasAdmin.IsVisible = false;

                    if (LblInfoAdmin != null)
                    {
                        LblInfoAdmin.IsVisible = true;
                        LblInfoAdmin.Text = "🎮 Panel de Usuario - Solo puedes ver y gestionar tus propios reportes";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al configurar interfaz: {ex.Message}");
            }
        }

        // Métodos que coinciden con los botones en tu XAML
        private async void OnAddProduct_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (UsuarioSesion.UsuarioActual == null)
                {
                    await MostrarErrorSesion();
                    return;
                }

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
                if (UsuarioSesion.UsuarioActual == null)
                {
                    await MostrarErrorSesion();
                    return;
                }

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
                if (UsuarioSesion.UsuarioActual == null)
                {
                    await MostrarErrorSesion();
                    return;
                }

                await MostrarLoading(true);
                await CargarEstadisticas();
                await MostrarLoading(false);

                string mensaje = "🔄 Estadísticas actualizadas correctamente";
                if (UsuarioSesion.UsuarioActual.EsAdmin)
                {
                    mensaje += "\n\n📊 Mostrando datos de todos los usuarios";
                }
                else
                {
                    mensaje += "\n\n📊 Mostrando solo tus reportes";
                }

                await DisplayAlert("✅ Actualizado", mensaje, "OK");
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
                string ayudaTexto = "❓ Ayuda - GameSupport Hub\n\n";

                if (UsuarioSesion.UsuarioActual?.EsAdmin == true)
                {
                    ayudaTexto += "👑 PANEL DE ADMINISTRADOR:\n\n";
                    ayudaTexto += "🔧 FUNCIONES DE ADMIN:\n";
                    ayudaTexto += "• Ver TODOS los reportes de usuarios\n";
                    ayudaTexto += "• Editar y eliminar cualquier reporte\n";
                    ayudaTexto += "• Cambiar estados de reportes\n";
                    ayudaTexto += "• Acceder a estadísticas completas\n\n";
                }
                else
                {
                    ayudaTexto += "🎮 PANEL DE USUARIO:\n\n";
                }

                ayudaTexto += "➕ REPORTAR INCIDENCIA:\n";
                ayudaTexto += "• Toca 'Reportar Nueva Incidencia'\n";
                ayudaTexto += "• Llena todos los campos requeridos\n";
                ayudaTexto += "• Describe detalladamente el problema\n\n";

                ayudaTexto += "📋 VER REPORTES:\n";
                ayudaTexto += "• Toca 'Ver Todos los Reportes'\n";

                if (UsuarioSesion.UsuarioActual?.EsAdmin == true)
                {
                    ayudaTexto += "• Como admin, verás reportes de todos los usuarios\n";
                }
                else
                {
                    ayudaTexto += "• Solo verás tus propios reportes\n";
                }

                ayudaTexto += "• Usa filtros para encontrar reportes específicos\n";
                ayudaTexto += "• Toca un reporte para ver detalles completos\n\n";

                ayudaTexto += "🔧 GESTIONAR REPORTES:\n";
                ayudaTexto += "• Editar: Actualiza información del reporte\n";
                ayudaTexto += "• Eliminar: Borra reportes innecesarios\n";
                ayudaTexto += "• Resolver: Marca reportes como solucionados\n\n";

                ayudaTexto += "🎯 FILTROS RÁPIDOS:\n";
                ayudaTexto += "• Usa los botones de filtro para ver categorías específicas\n";
                ayudaTexto += "• Los reportes críticos requieren atención inmediata\n\n";

                ayudaTexto += "💡 ¿NECESITAS MÁS AYUDA?\n";
                ayudaTexto += "Contacta al equipo de desarrollo para soporte adicional.";

                await DisplayAlert("🎮 Ayuda Completa", ayudaTexto, "Cerrar");
            }
            catch (Exception ex)
            {
                await DisplayAlert("❌ Error", $"Error al mostrar ayuda: {ex.Message}", "OK");
            }
        }

        // NUEVO MÉTODO PARA ESTADÍSTICAS DE ADMIN
        private async void BtnEstadisticasAdmin_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (UsuarioSesion.UsuarioActual?.EsAdmin != true)
                {
                    await DisplayAlert("🚫 Sin Permisos", "Solo los administradores pueden ver estas estadísticas.", "OK");
                    return;
                }

                await MostrarLoading(true);

                var estadisticas = await firebaseHelpers.GetEstadisticasGenerales();

                await MostrarLoading(false);

                string mensaje = "📊 ESTADÍSTICAS COMPLETAS DEL SISTEMA\n\n";
                mensaje += $"📋 Total de reportes: {estadisticas.GetValueOrDefault("Total", 0)}\n";
                mensaje += $"🔴 Abiertos: {estadisticas.GetValueOrDefault("Abiertos", 0)}\n";
                mensaje += $"🟡 En progreso: {estadisticas.GetValueOrDefault("EnProgreso", 0)}\n";
                mensaje += $"🟢 Resueltos: {estadisticas.GetValueOrDefault("Resueltos", 0)}\n";
                mensaje += $"⚫ Cerrados: {estadisticas.GetValueOrDefault("Cerrados", 0)}\n\n";
                mensaje += $"🚨 Críticos: {estadisticas.GetValueOrDefault("Criticos", 0)}\n";
                mensaje += $"👥 Usuarios únicos: {estadisticas.GetValueOrDefault("UsuariosUnicos", 0)}\n\n";
                mensaje += "📈 Panel exclusivo para administradores";

                await DisplayAlert("👑 Panel de Admin", mensaje, "Cerrar");
            }
            catch (Exception ex)
            {
                await MostrarLoading(false);
                await DisplayAlert("❌ Error", $"Error al cargar estadísticas de admin: {ex.Message}", "OK");
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
                    string mensaje = $"🎯 Se encontraron {reportesCriticos.Count} reporte(s) crítico(s) que requieren atención inmediata.\n\n";

                    if (UsuarioSesion.UsuarioActual.EsAdmin)
                    {
                        mensaje += "👑 Como admin, verás todos los reportes críticos del sistema.\n\n";
                    }
                    else
                    {
                        mensaje += "🎮 Solo verás tus reportes críticos.\n\n";
                    }

                    mensaje += "Abriendo lista de reportes...";

                    await DisplayAlert("🚨 Reportes Críticos", mensaje, "Ver Críticos");
                    await Navigation.PushAsync(new ListReportePage());
                }
                else
                {
                    string mensaje = "✅ ¡Excelente! No hay reportes críticos pendientes.\n\n";

                    if (UsuarioSesion.UsuarioActual.EsAdmin)
                    {
                        mensaje += "Todos los problemas críticos del sistema han sido resueltos.";
                    }
                    else
                    {
                        mensaje += "No tienes reportes críticos pendientes.";
                    }

                    await DisplayAlert("🎉 Sin Críticos", mensaje, "Genial");
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
                    string tipoUsuario = UsuarioSesion.UsuarioActual?.EsAdmin == true ? "Administrador" : "Usuario";

                    // Cerrar sesión
                    UsuarioSesion.CerrarSesion();

                    // Mostrar mensaje de despedida
                    await DisplayAlert("👋 Hasta pronto",
                        $"Has cerrado sesión correctamente como {tipoUsuario}.\n\n🎮 ¡Gracias por ayudar a mejorar la experiencia gaming!\n\n¡Te esperamos pronto!",
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

        private async Task MostrarErrorSesion()
        {
            await DisplayAlert("❌ Sesión Expirada",
                "Tu sesión ha expirado. Serás redirigido al login.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}