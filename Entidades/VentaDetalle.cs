using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    /// <summary>
    /// Representa una línea o ítem de una venta.
    ///
    /// Por ejemplo, si una venta contiene:
    ///
    /// 2 Labiales Cherry Red
    /// 1 Base Ruby Rose
    ///
    /// tendremos dos objetos VentaDetalle.
    /// </summary>
    public class VentaDetalle
    {
        /// <summary>
        /// Identificador único del detalle.
        /// Corresponde a Id_detalle en SQL Server.
        /// </summary>
        public int IdDetalle { get; set; }


        /// <summary>
        /// Identificador de la cabecera a la que
        /// pertenece este detalle.
        /// </summary>
        public int IdVentaCabecera { get; set; }


        /// <summary>
        /// Identificador del producto vendido.
        /// </summary>
        public int IdProducto { get; set; }


        /// <summary>
        /// Cantidad de unidades vendidas.
        /// </summary>
        public int Cantidad { get; set; }


        /// <summary>
        /// Precio de una unidad del producto
        /// en el momento de realizar la venta.
        /// </summary>
        public decimal PrecioUnitario { get; set; }


        /// <summary>
        /// Importe correspondiente a esta línea.
        ///
        /// Conceptualmente:
        /// Cantidad × PrecioUnitario = Subtotal.
        /// </summary>
        public decimal Subtotal { get; set; }


        /// <summary>
        /// Objeto Producto asociado al detalle.
        ///
        /// No representa una columna adicional en VentaDetalle.
        /// Facilita el transporte de información entre capas.
        /// </summary>
        public Producto oProducto { get; set; }
    }
}
