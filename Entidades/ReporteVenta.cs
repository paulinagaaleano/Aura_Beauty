using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entidades
{
    /// <summary>
    /// Representa una fila del reporte de ventas.
    ///
    /// Esta clase NO corresponde a una tabla propia de la base
    /// de datos. Su función es transportar información obtenida
    /// a partir de varias tablas:
    ///
    /// - VentaCabecera
    /// - Usuario
    /// - Cliente
    ///
    /// Se utiliza para mostrar de manera sencilla los resultados
    /// en la pantalla de Reportes.
    /// </summary>
    public class ReporteVenta
    {
        /// <summary>
        /// Identificador de la venta.
        /// </summary>
        public int IdVenta { get; set; }


        /// <summary>
        /// Fecha y hora en que se registró la venta.
        /// </summary>
        public DateTime FechaVenta { get; set; }


        /// <summary>
        /// Número interno del comprobante.
        /// </summary>
        public string NroFactura { get; set; }


        /// <summary>
        /// Nombre completo del usuario que realizó la venta.
        /// </summary>
        public string Vendedor { get; set; }


        /// <summary>
        /// Nombre del cliente.
        ///
        /// Cuando id_cliente es NULL, se mostrará
        /// "Consumidor Final".
        /// </summary>
        public string Cliente { get; set; }


        /// <summary>
        /// Importe total de la venta.
        /// </summary>
        public decimal Total { get; set; }
    }
}