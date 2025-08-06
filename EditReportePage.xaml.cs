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

        // Verificar permisos antes de cargar
        if (!VerificarPermisos())
        {
            DisplayAlert("🚫 Sin Permisos",
                "No tienes permisos para editar este reporte.", "OK");
            Navigation.PopAsync();
            return;
        }

        CargarDatosReporte();
        ConfigurarInterfazSegunUsuario();
    }

    private bool VerificarPermisos()
    {
        if (UsuarioSesion.UsuarioActual == null)
            return false;

        // El admin puede editar cualquier reporte
        if (UsuarioSesion.UsuarioActual.EsAdmin)
            return true;

        // El usuario común solo puede editar sus propios reportes
        return reporte.IdUsuario == UsuarioSesion.UsuarioActual.LocalId;
    }

    private void ConfigurarInterfazSegunUsuario()
    {
        try
        {
            var usuario = UsuarioSesion.UsuarioActual;
            if (usuario == null) return;

            if (usuario.EsAdmin)
            {
                Title = "✏️ Editar Reporte (Admin)";
                // El admin puede editar todo
                UsuarioEntry.IsReadOnly = false;
                UsuarioEntry.BackgroundColor = Colors.White;

                // Mostrar información adicional para admin
                if (LblInfoAdmin != null)
                {
                    LblInfoAdmin.IsVisible = true;
                    LblInfoAdmin.Text = $"👑 Editando como Admin - Usuario original: {reporte.EmailUsuario}";
                }
            }
            else
            {
                Title = "✏️ Editar Mi Reporte";
                // El usuario común no puede cambiar el usuario reportador
                UsuarioEntry.IsReadOnly = true;
                UsuarioEntry.BackgroundColor = Color.FromArgb("#F5F5F5");

                if (LblInfoAdmin != null)
                {
                    LblInfoAdmin.IsVisible = true;
                    LblInfoAdmin.Text = "🎮 Editando tu propio reporte";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al configurar interfaz: {ex.Message}");
        }
    }

    private void CargarDatosReporte()
    {
        try
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

            // Mostrar información del propietario si es admin
            if (UsuarioSesion.UsuarioActual?.EsAdmin == true && !string.IsNullOrEmpty(reporte.EmailUsuario))
            {
                LblEstadoActual.Text += $"\n👤 Propietario: {reporte.EmailUsuario}";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar datos del reporte: {ex.Message}");
        }
    }

    private async void BtnGuardarCambios_Clicked(object sender, EventArgs e)
    {
        // Verificar sesión
        if (UsuarioSesion.UsuarioActual == null)
        {
            await DisplayAlert("❌ Sesión Expirada",
                "Tu sesión ha expirado. Serás redirigido al login.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        // Verificar permisos nuevamente
        if (!VerificarPermisos())
        {
            await DisplayAlert("🚫 Sin Permisos",
                "No tienes permisos para editar este reporte.\n\n💡 Solo puedes editar tus propios reportes.", "OK");
            return;
        }

        if (!ValidarCampos())
            return;

        await MostrarLoading(true);

        try
        {
            // Crear copia del reporte con los cambios
            var reporteActualizado = new Reporte
            {
                Id = reporte.Id,
                TipoIncidencia = TipoIncidenciaEntry.Text?.Trim(),
                Descripcion = DescripcionEditor.Text?.Trim(),
                CodigoError = CodigoErrorEntry.Text?.Trim() ?? "N/A",
                Categoria = (CategoriaIncidencia)CategoriaPicker.SelectedIndex,
                Prioridad = (PrioridadIncidencia)PrioridadPicker.SelectedIndex,
                Estado = (EstadoIncidencia)EstadoPicker.SelectedIndex,
                UsuarioReportador = UsuarioEntry.Text?.Trim() ?? "Anónimo",
                FechaCreacion = reporte.FechaCreacion,
                FechaActualizacion = DateTime.Now,
                // MANTENER INFORMACIÓN DEL PROPIETARIO ORIGINAL
                EmailUsuario = reporte.EmailUsuario,
                IdUsuario = reporte.IdUsuario
            };

            bool exito = await firebaseHelpers.UpdateReporte(reporte.Id, reporteActualizado);

            if (exito)
            {
                string mensaje = "✅ Reporte actualizado correctamente.\n\n";
                mensaje += $"📝 Tipo: {reporteActualizado.TipoIncidencia}\n";
                mensaje += $"🔄 Estado: {reporteActualizado.EstadoTexto}\n";
                mensaje += $"⚡ Prioridad: {reporteActualizado.PrioridadTexto}\n\n";

                // Mostrar qué cambió
                if (EstadoOriginalCambio())
                {
                    mensaje += $"🔄 Estado cambiado de '{estadoOriginal}' a '{reporteActualizado.EstadoTexto}'\n";
                }

                mensaje += "💾 Los cambios se han guardado exitosamente.";

                await DisplayAlert("🎮 Cambios Guardados", mensaje, "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("❌ Error al Guardar",
                    "Hubo un problema al actualizar el reporte.\n\n💡 Sugerencias:\n• Verifica tu conexión a internet\n• Intenta nuevamente en unos momentos\n• Si el problema persiste, contacta soporte",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("❌ Error Inesperado",
                $"Error al actualizar el reporte:\n\n{ex.Message}\n\n💡 Por favor, intenta nuevamente.",
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
        bool confirmar = await DisplayAlert("⚠️ Confirmar Cancelación",
            "¿Estás seguro de que quieres cancelar?\n\n❌ Se perderán todos los cambios realizados\n✅ El reporte mantendrá su estado original\n\n¿Continuar?",
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
            MostrarError("Por favor, selecciona una categoría.\n\n💡 Esto nos ayuda a clasificar y priorizar el reporte.");
            return false;
        }

        if (PrioridadPicker.SelectedIndex == -1)
        {
            MostrarError("Por favor, selecciona una prioridad.\n\n💡 Indica qué tan crítico es este problema.");
            return false;
        }

        if (EstadoPicker.SelectedIndex == -1)
        {
            MostrarError("Por favor, selecciona un estado.\n\n💡 Indica el estado actual del reporte.");
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

        if (string.IsNullOrWhiteSpace(UsuarioEntry.Text))
        {
            MostrarError("Por favor, ingresa el nombre del usuario reportador.\n\n💡 Este campo identifica quién reportó la incidencia.");
            UsuarioEntry.Focus();
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

        // Verificar sesión al aparecer
        if (UsuarioSesion.UsuarioActual == null)
        {
            DisplayAlert("❌ Sesión Expirada",
                "Tu sesión ha expirado. Serás redirigido al login.", "OK");
            Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        // Refrescar los datos si es necesario
        CargarDatosReporte();
    }