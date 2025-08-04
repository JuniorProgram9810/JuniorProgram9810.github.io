
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
            firebaseClient = new FirebaseClient("https://integradira10a-default-rtdb.firebaseio.com/");
        }

        public async Task<List<Reporte>> GetAllReportes()
        {
            try
            {
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
                    UsuarioReportador = item.Object.UsuarioReportador
                }).OrderByDescending(r => r.FechaCreacion).ToList();
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error al obtener reportes: {ex.Message}");
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
                var reportes = await GetAllReportes();
                return reportes.FirstOrDefault(r => r.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener reporte por ID: {ex.Message}");
                return null;
            }
        }
    }
}