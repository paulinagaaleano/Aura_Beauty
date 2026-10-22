
using System;

namespace Entidades
{
    /// <summary>
    /// Representa una reserva temporal de productos
    /// mientras un vendedor prepara una venta.
    ///
    /// Pertenece exclusivamente a la capa Entidades.
    /// No realiza consultas SQL ni descuenta stock.
    /// </summary>
    public class ReservaProducto
    {
        // Estados admitidos por la tabla SQL.
        public const string EstadoActiva = "ACTIVA";
        public const string EstadoConfirmada = "CONFIRMADA";
        public const string EstadoCancelada = "CANCELADA";
        public const string EstadoVencida = "VENCIDA";

        /// <summary>
        /// Identificador único de la reserva.
        /// </summary>
        public int IdReservaProducto { get; set; }

        /// <summary>
        /// Identificador del carrito de venta.
        /// </summary>
        public Guid IdOperacion { get; set; }

        /// <summary>
        /// Producto que se está reservando.
        /// </summary>
        public int IdProducto { get; set; }

        /// <summary>
        /// Usuario vendedor responsable.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Cantidad de unidades reservadas.
        /// </summary>
        public int Cantidad { get; set; }

        /// <summary>
        /// Estado actual de la reserva.
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Fecha de creación en horario UTC.
        /// </summary>
        public DateTime FechaReservaUtc { get; set; }

        /// <summary>
        /// Fecha de vencimiento en horario UTC.
        /// </summary>
        public DateTime FechaVencimientoUtc { get; set; }

        /// <summary>
        /// Fecha de la última actualización.
        /// </summary>
        public DateTime FechaActualizacionUtc { get; set; }

        /// <summary>
        /// Identificador de la venta definitiva.
        /// Permanece null hasta su confirmación.
        /// </summary>
        public int? IdVentaCabecera { get; set; }

        /// <summary>
        /// Inicializa una reserva nueva en memoria.
        /// </summary>
        public ReservaProducto()
        {
            IdOperacion = Guid.Empty;
            Estado = EstadoActiva;
            Cantidad = 1;
            IdVentaCabecera = null;
        }
    }
}
