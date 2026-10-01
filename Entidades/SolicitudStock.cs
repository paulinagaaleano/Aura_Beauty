using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    /// <summary>
    /// Representa una solicitud de modificación del stock.
    ///
    /// Una solicitud es generada por un usuario Repositor
    /// cuando necesita justificar un ingreso o egreso
    /// manual de mercadería.
    ///
    /// La solicitud no modifica por sí misma el stock.
    /// Primero debe ser aprobada por un Administrador.
    /// </summary>
    public class SolicitudStock
    {
        /// <summary>
        /// Identificador único de la solicitud.
        /// Corresponde a id_solicitud en la base de datos.
        /// </summary>
        public int IdSolicitud { get; set; }


        /// <summary>
        /// Identificador del producto cuyo stock
        /// se solicita modificar.
        /// </summary>
        public int IdProducto { get; set; }


        /// <summary>
        /// Producto relacionado con la solicitud.
        ///
        /// Nos permite transportar también los datos
        /// completos del producto entre las capas.
        /// </summary>
        public Producto oProducto { get; set; }


        /// <summary>
        /// Identificador del usuario que realizó
        /// la solicitud.
        /// </summary>
        public int IdUsuarioSolicitante { get; set; }


        /// <summary>
        /// Usuario que generó la solicitud.
        /// </summary>
        public Usuario oUsuarioSolicitante { get; set; }


        /// <summary>
        /// Tipo de movimiento solicitado.
        ///
        /// Valores permitidos:
        /// INGRESO
        /// EGRESO
        /// </summary>
        public string TipoMovimiento { get; set; }


        /// <summary>
        /// Cantidad de unidades que se solicita
        /// ingresar o retirar.
        /// </summary>
        public int Cantidad { get; set; }


        /// <summary>
        /// Justificación de la solicitud.
        /// Es obligatoria.
        /// </summary>
        public string Motivo { get; set; }


        /// <summary>
        /// Estado actual de la solicitud.
        ///
        /// PENDIENTE
        /// APROBADA
        /// RECHAZADA
        /// </summary>
        public string Estado { get; set; }


        /// <summary>
        /// Fecha y hora en la que se realizó
        /// la solicitud.
        /// </summary>
        public DateTime FechaSolicitud { get; set; }


        /// <summary>
        /// Identificador del Administrador que
        /// aprobó o rechazó la solicitud.
        ///
        /// Es nullable porque mientras la solicitud
        /// esté pendiente todavía no existe autorizador.
        /// </summary>
        public int? IdUsuarioAutorizador { get; set; }


        /// <summary>
        /// Administrador que resolvió la solicitud.
        /// Mientras esté pendiente puede ser null.
        /// </summary>
        public Usuario oUsuarioAutorizador { get; set; }


        /// <summary>
        /// Fecha en la que el Administrador aprobó
        /// o rechazó la solicitud.
        ///
        /// Es nullable porque una solicitud pendiente
        /// todavía no tiene fecha de resolución.
        /// </summary>
        public DateTime? FechaResolucion { get; set; }
    }
}
