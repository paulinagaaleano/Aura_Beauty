using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entidades
    {
        /// <summary>
        /// Representa una copia de seguridad de la base de datos AuraBeautyDB.
        /// Esta clase transporta información entre las capas del sistema.
        /// No ejecuta el backup ni accede directamente a SQL Server.
        /// </summary>
        public class Backup
        {
            // Identificador único del registro de backup en la base de datos.
            public int IdBackup { get; set; }

            // Nombre del archivo físico de copia de seguridad (.bak).
            public string NombreArchivo { get; set; }

            // Ruta donde SQL Server guarda o encuentra el archivo de backup.
            public string RutaArchivo { get; set; }

            // Fecha y hora en que se registró la creación de la copia.
            public DateTime FechaCreacion { get; set; }

            // Identificador del usuario que realizó el backup.
            public int IdUsuario { get; set; }

            // Estado de la operación: "Exitoso" o "Fallido".
            public string Estado { get; set; }

            // Información adicional sobre el resultado de la operación.
            public string Observaciones { get; set; }
        }
    
}

