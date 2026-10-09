using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entidades
{
    // Representa los indicadores de ventas de un producto.
    public class ReporteProducto
    {
        // Identificador del producto en la base de datos.
        public int IdProducto { get; set; }

        // Nombre del producto que se está analizando.
        public string NombreProducto { get; set; }

        // Cantidad total de unidades vendidas del producto.
        public int UnidadesVendidas { get; set; }

        // Importe total facturado por la venta del producto.
        public decimal FacturacionTotal { get; set; }

        // Porcentaje de participación en la facturación del período.
        public decimal PorcentajeFacturacion { get; set; }
    }
}

