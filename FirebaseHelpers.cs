
using AppIntegradora10A.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIntegradora10A.Helpers
{
    public class FirebaseHelpers
    {
        private readonly FirebaseClient firebaseClient;

        public FirebaseHelpers()
        {
            firebaseClient = new FirebaseClient("https://pollaventuras10a-default-rtdb.firebaseio.com/");
        }

        // MÉTODO PARA OBTENER REPORTES SEGÚN EL TIPO DE USUARIO
        public async Task<List<Reporte>> GetAllReportes()
        {
            try
            {
                var reportes = await firebaseClient
                    .Child("Reportes")
                    .OnceAsync<Reporte>();

                var listaReportes = reportes.Select(item => new Reporte
                {
                    Id = item.Key,
                    TipoIncidencia = item.Object.TipoIncidencia,
                    Descripcion = item.Object.Descripcion,
                    CodigoError = item.Object.CodigoError,
                    Categoria = item.Object.Categoria,
                    Prioridad = item.Object.Prioridad,
                    Estado = item.Object.Estado,
                    FechaCreacion = item.Object.FechaCreacion,
                    FechaActualizacion = item.Object.FechaActualizacion,
                    UsuarioReportador = item.Object.UsuarioReportador,
                    EmailUsuario = item.Object.EmailUsuario,
                    IdUsuario = item.Object.IdUsuario
                }).OrderByDescending(r => r.FechaCreacion).ToList();

                // FILTRAR SEGÚN EL TIPO DE USUARIO
                if (UsuarioSesion.UsuarioActual == null)
                    return new List<Reporte>();

                if (UsuarioSesion.UsuarioActual.EsAdmin)
                {
                    // El admin ve todos los reportes
                    return listaReportes;
                }
                else
                {
                    // El usuario común solo ve sus propios reportes
                    return listaReportes.Where(r => r.IdUsuario == UsuarioSesion.UsuarioActual.LocalId).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener reportes: {ex.Message}");
                return new List<Reporte>();
            }
        }

        // MÉTODO ESPECÍFICO PARA ADMIN - VER TODOS LOS REPORTES
        public async Task<List<Reporte>> GetAllReportesAdmin()
        {
            try
            {
                if (UsuarioSesion.UsuarioActual?.EsAdmin != true)
                    return new List<Reporte>();

                var reportes = await firebaseClient
                    .Child("Reportes")
                    .OnceAsync<Reporte>();

                return reportes.Select(item => new Reporte
                {
                    Id = item.Key,
                    TipoIncidencia = item.Object.TipoIncidencia,
                    Descripcion = item.Object.Descripcion,
                    CodigoError = item.Object.CodigoError,
                    Categoria = item.Object.Categoria,
                    Prioridad = item.Object.Prioridad,
                    Estado = item.Object.Estado,
                    FechaCreacion = item.Object.FechaCreacion,
                    FechaActualizacion = item.Object.FechaActualizacion,
                    UsuarioReportador = item.Object.UsuarioReportador,
                    EmailUsuario = item.Object.EmailUsuario,
                    IdUsuario = item.Object.IdUsuario
                }).OrderByDescending(r => r.FechaCreacion).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener todos los reportes (Admin): {ex.Message}");
                return new List<Reporte>();
            }
        }

        // MÉTODO PARA OBTENER REPORTES DE UN USUARIO ESPECÍFICO
        public async Task<List<Reporte>> GetReportesByUsuario(string idUsuario)
        {
            try
            {
                var todosReportes = await GetAllReportesAdmin();
                return todosReportes.Where(r => r.IdUsuario == idUsuario).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al filtrar por usuario: {ex.Message}");
                return new List<Reporte>();
            }
        }

        public async Task<List<Reporte>> GetReportesByCategoria(CategoriaIncidencia categoria)
        {
            try
            {
                var todosReportes = await GetAllReportes();
                return todosReportes.Where(r => r.Categoria == categoria).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al filtrar por categoría: {ex.Message}");
                return new List<Reporte>();
            }
        }

        public async Task<List<Reporte>> GetReportesByEstado(EstadoIncidencia estado)
        {
            try
            {
                var todosReportes = await GetAllReportes();
                return todosReportes.Where(r => r.Estado == estado).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al filtrar por estado: {ex.Message}");
                return new List<Reporte>();
            }
        }

        public async Task<List<Reporte>> SearchReportes(string searchText)
        {
            try
            {
                var todosReportes = await GetAllReportes();
                return todosReportes.Where(r =>
                    r.TipoIncidencia.ToLower().Contains(searchText.ToLower()) ||
                    r.Descripcion.ToLower().Contains(searchText.ToLower()) ||
                    r.CodigoError.ToLower().Contains(searchText.ToLower())
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en búsqueda: {ex.Message}");
                return new List<Reporte>();
            }
        }

        public async Task<bool> AddReporte(Reporte reporte)
        {
            try
            {
                // ASIGNAR INFORMACIÓN DEL USUARIO ACTUAL
                if (UsuarioSesion.UsuarioActual != null)
                {
                    reporte.EmailUsuario = UsuarioSesion.UsuarioActual.Email;
                    reporte.IdUsuario = UsuarioSesion.UsuarioActual.LocalId;

                    // Si no se especificó un usuario reportador, usar el email
                    if (string.IsNullOrWhiteSpace(reporte.UsuarioReportador))
                    {
                        reporte.UsuarioReportador = UsuarioSesion.UsuarioActual.NombreUsuario;
                    }
                }

                await firebaseClient
                    .Child("Reportes")
                    .PostAsync(reporte);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar reporte: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateReporte(string key, Reporte reporte)
        {
            try
            {
                // VERIFICAR PERMISOS DE EDICIÓN
                if (!PuedeEditarReporte(reporte))
                {
                    Console.WriteLine("Usuario no tiene permisos para editar este reporte");
                    return false;
                }

                reporte.FechaActualizacion = DateTime.Now;
                await firebaseClient
                    .Child("Reportes")
                    .Child(key)
                    .PutAsync(reporte);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar reporte: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteReporte(string key)
        {
            try
            {
                // VERIFICAR PERMISOS ANTES DE ELIMINAR
                var reporte = await GetReporteById(key);
                if (reporte != null && !PuedeEditarReporte(reporte))
                {
                    Console.WriteLine("Usuario no tiene permisos para eliminar este reporte");
                    return false;
                }

                await firebaseClient
                    .Child("Reportes")
                    .Child(key)
                    .DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar reporte: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateEstadoReporte(string key, EstadoIncidencia nuevoEstado)
        {
            try
            {
                var reporte = await GetReporteById(key);
                if (reporte != null)
                {
                    // VERIFICAR PERMISOS
                    if (!PuedeEditarReporte(reporte))
                    {
                        Console.WriteLine("Usuario no tiene permisos para cambiar el estado de este reporte");
                        return false;
                    }

                    reporte.Estado = nuevoEstado;
                    return await UpdateReporte(key, reporte);
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar estado: {ex.Message}");
                return false;
            }
        }

        private async Task<Reporte> GetReporteById(string id)
        {
            try
            {
                var reportes = await GetAllReportesAdmin(); // Usar método admin para obtener cualquier reporte
                return reportes.FirstOrDefault(r => r.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener reporte por ID: {ex.Message}");
                return null;
            }
        }

        // MÉTODO PARA VERIFICAR PERMISOS DE EDICIÓN
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

        // MÉTODOS ADICIONALES PARA ESTADÍSTICAS DE ADMIN
        public async Task<Dictionary<string, int>> GetEstadisticasGenerales()
        {
            try
            {
                if (UsuarioSesion.UsuarioActual?.EsAdmin != true)
                    return new Dictionary<string, int>();

                var todosReportes = await GetAllReportesAdmin();

                return new Dictionary<string, int>
                {
                    ["Total"] = todosReportes.Count,
                    ["Abiertos"] = todosReportes.Count(r => r.Estado == EstadoIncidencia.Abierto),
                    ["EnProgreso"] = todosReportes.Count(r => r.Estado == EstadoIncidencia.EnProgreso),
                    ["Resueltos"] = todosReportes.Count(r => r.Estado == EstadoIncidencia.Resuelto),
                    ["Cerrados"] = todosReportes.Count(r => r.Estado == EstadoIncidencia.Cerrado),
                    ["Criticos"] = todosReportes.Count(r => r.Prioridad == PrioridadIncidencia.Critica),
                    ["UsuariosUnicos"] = todosReportes.Select(r => r.IdUsuario).Distinct().Count()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener estadísticas: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }
    }
}