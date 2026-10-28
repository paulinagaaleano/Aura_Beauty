using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Entidades
{
    /// <summary>
    /// Representa una solicitud para restaurar una copia de seguridad.
    /// Permite transportar los datos de la solicitud y su autorización.
    /// No ejecuta directamente la restauración de la base de datos.
    /// </summary>
    public class SolicitudRestauracion
    {
        // Identificador único de la solicitud.
        public int IdSolicitud { get; set; }

        // Identificador de la copia de seguridad que se desea restaurar.
        public int IdBackup { get; set; }

        // Usuario administrador que solicita la restauración.
        public int IdSolicitante { get; set; }

        // Segundo administrador que autoriza la operación.
        // Es nullable porque todavía puede no existir una autorización.
        public int? IdAutorizador { get; set; }

        // Fecha y hora en que se creó la solicitud.
        public DateTime FechaSolicitud { get; set; }

        // Fecha y hora en que se autorizó, rechazó o finalizó el proceso.
        // Es nullable porque una solicitud pendiente todavía no tiene resolución.
        public DateTime? FechaResolucion { get; set; }

        // Estado de la solicitud:
        // Pendiente, Autorizada, Rechazada, Exitosa o Fallida.
        public string Estado { get; set; }

        // Motivo del rechazo, detalles del error u otras observaciones.
        public string Observaciones { get; set; }
    }
}

