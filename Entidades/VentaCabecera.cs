using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entidades
{
    /// <summary>
    /// Representa la cabecera o información general de una venta.
    ///
    /// Una venta posee una única cabecera y puede contener
    /// varios detalles, uno por cada producto vendido.
    ///
    /// Esta clase pertenece a la capa de Entidades.
    /// No contiene SQL, controles visuales ni reglas de negocio.
    /// </summary>
    public class VentaCabecera
    {
        /// <summary>
        /// Identificador único de la venta.
        /// Corresponde a Id_ventaCabecera en SQL Server.
        /// </summary>
        public int IdVentaCabecera { get; set; }


        /// <summary>
        /// Identificador del cliente.
        ///
        /// Es int? porque la columna id_cliente admite NULL.
        /// Cuando sea null, podremos interpretar la operación
        /// como una venta a Consumidor Final.
        /// </summary>
        public int? IdCliente { get; set; }


        /// <summary>
        /// Identificador del usuario que realizó la venta.
        /// Corresponde al vendedor actualmente logueado.
        /// </summary>
        public int IdUsuario { get; set; }


        /// <summary>
        /// Tipo de comprobante o factura.
        /// </summary>
        public string TipoFactura { get; set; }


        /// <summary>
        /// Número identificatorio de la factura.
        /// </summary>
        public string NroFactura { get; set; }


        /// <summary>
        /// Importe total de la venta.
        /// </summary>
        public decimal Total { get; set; }


        /// <summary>
        /// Fecha y hora en que se realizó la venta.
        /// </summary>
        public DateTime? FechaVenta { get; set; }


        /// <summary>
        /// Cliente relacionado con la venta.
        ///
        /// Esta propiedad permite transportar información
        /// completa del cliente cuando sea necesario.
        /// No representa una columna adicional de SQL Server.
        /// </summary>
        public Cliente oCliente { get; set; }


        /// <summary>
        /// Usuario o vendedor responsable de la operación.
        ///
        /// Tampoco representa una columna adicional:
        /// sirve para transportar información entre capas.
        /// </summary>
        public Usuario oUsuario { get; set; }


        /// <summary>
        /// Productos que forman parte de esta venta.
        ///
        /// Una cabecera puede tener muchos detalles.
        /// </summary>
        public List<VentaDetalle> Detalles { get; set; }
            = new List<VentaDetalle>();
    }
}
