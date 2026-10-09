using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entidades
{
    // Representa los resultados de comparar las ventas de dos períodos.
    public class ComparacionVentas
    {
        // Facturación acumulada del período actual.
        public decimal FacturacionActual { get; set; }

        // Facturación acumulada del período anterior.
        public decimal FacturacionAnterior { get; set; }

        // Diferencia monetaria entre ambos períodos.
        public decimal DiferenciaFacturacion { get; set; }

        // Puede ser null cuando no existe una facturación anterior
        // que permita calcular el porcentaje de variación.
        public decimal? VariacionPorcentual { get; set; }

        // Cantidad de ventas del período actual.
        public int CantidadVentasActual { get; set; }

        // Cantidad de ventas del período anterior.
        public int CantidadVentasAnterior { get; set; }
    }
}

