using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIntegradora10A.Models
{
    public class Reporte
    {
        public string Id { get; set; }
        public string TipoIncidencia { get; set; }
        public string Descripcion { get; set; }
        public string CodigoError { get; set; }
        public CategoriaIncidencia Categoria { get; set; }
        public PrioridadIncidencia Prioridad { get; set; }
        public EstadoIncidencia Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string UsuarioReportador { get; set; }

        public Reporte()
        {
            FechaCreacion = DateTime.Now;
            Estado = EstadoIncidencia.Abierto;
            Prioridad = PrioridadIncidencia.Media;
        }

        // Propiedades calculadas para mostrar en la UI
        public string CategoriaTexto => Categoria.ToString();
        public string PrioridadTexto => Prioridad.ToString();
        public string EstadoTexto => Estado.ToString();
        public string FechaTexto => FechaCreacion.ToString("dd/MM/yyyy HH:mm");
        public string EstadoColor => Estado switch
        {
            EstadoIncidencia.Abierto => "#FF6B6B",
            EstadoIncidencia.EnProgreso => "#FFE66D",
            EstadoIncidencia.Resuelto => "#4ECDC4",
            _ => "#95A5A6"
        };
    }

    public enum CategoriaIncidencia
    {
        Bug = 0,
        Crash = 1,
        Performance = 2,
        Gameplay = 3,
        Audio = 4,
        Graficos = 5,
        Conectividad = 6,
        Otro = 7
    }

    public enum PrioridadIncidencia
    {
        Baja = 0,
        Media = 1,
        Alta = 2,
        Critica = 3
    }

    public enum EstadoIncidencia
    {
        Abierto = 0,
        EnProgreso = 1,
        Resuelto = 2,
        Cerrado = 3
    }
}
