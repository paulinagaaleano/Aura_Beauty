using Datos;
using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Negocio
{
    public class CN_Reporte
    {
        private readonly CD_Reporte objcd_reporte = new CD_Reporte();

        public List<ReporteVenta> ObtenerVentas(
            DateTime fechaInicio,
            DateTime fechaFin,
            int? idUsuario
        )
        {
            if (fechaInicio.Date > fechaFin.Date)
            {
                throw new Exception("La fecha de inicio no puede ser posterior a la fecha de fin.");
            }

            return objcd_reporte.ObtenerVentas(
                fechaInicio,
                fechaFin,
                idUsuario
            );
        }


        public List<ReporteVenta> ObtenerVentasAnalisis(
    DateTime fechaInicio,
    DateTime fechaFin,
    int? idUsuario)
        {
            if (fechaInicio.Date > fechaFin.Date)
            {
                throw new Exception(
                    "La fecha de inicio no puede ser posterior a la fecha de fin.");
            }

            return objcd_reporte.ObtenerVentasAnalisis(
                fechaInicio,
                fechaFin,
                idUsuario);
        }

        public DataTable ObtenerReporteMovimientosStock(DateTime fechaInicio, DateTime fechaFin, out string mensaje)
        {
            if (fechaInicio.Date > fechaFin.Date)
            {
                mensaje = "La fecha de inicio no puede ser posterior a la fecha de fin.";
                return new DataTable();
            }

            return objcd_reporte.ObtenerReporteMovimientosStock(fechaInicio, fechaFin, out mensaje);
        }


        public ResumenVentas ObtenerResumenVentas(
        DateTime fechaInicio,
        DateTime fechaFin,
        int? idUsuario)
        {
            if (fechaInicio.Date > fechaFin.Date)
            {
                throw new Exception(
                    "La fecha de inicio no puede ser posterior a la fecha de fin.");
            }

            // Obtener ventas
            List<ReporteVenta> ventas =
                objcd_reporte.ObtenerVentasAnalisis(
                    fechaInicio,
                    fechaFin,
                    idUsuario);

            // Obtener detalles
            List<DetalleVentaAnalisis> detalles =
                objcd_reporte.ObtenerDetallesVentasAnalisis(
                    fechaInicio,
                    fechaFin,
                    idUsuario);

            ResumenVentas resumen = new ResumenVentas();

            // Si no hay ventas, devolvemos los indicadores en cero
            if (ventas.Count == 0)
            {
                return resumen;
            }

            decimal facturacionTotal = 0;
            int cantidadVentas = 0;
            int unidadesVendidas = 0;

            decimal ventaMaxima = ventas[0].Total;
            decimal ventaMinima = ventas[0].Total;

            // Procesamos las ventas
            foreach (ReporteVenta venta in ventas)
            {
                facturacionTotal += venta.Total;
                cantidadVentas++;

                if (venta.Total > ventaMaxima)
                {
                    ventaMaxima = venta.Total;
                }

                if (venta.Total < ventaMinima)
                {
                    ventaMinima = venta.Total;
                }
            }

            // Procesamos los detalles
            foreach (DetalleVentaAnalisis detalle in detalles)
            {
                unidadesVendidas += detalle.Cantidad;
            }

            // Calculamos el ticket promedio
            decimal ticketPromedio =
                facturacionTotal / cantidadVentas;

            // Guardamos los resultados
            resumen.FacturacionTotal = facturacionTotal;
            resumen.CantidadVentas = cantidadVentas;
            resumen.TicketPromedio = ticketPromedio;
            resumen.UnidadesVendidas = unidadesVendidas;
            resumen.VentaMaxima = ventaMaxima;
            resumen.VentaMinima = ventaMinima;

            return resumen;
        }


        public List<VentaPorDia> ObtenerVentasPorDia(
            DateTime fechaInicio,
            DateTime fechaFin,
            int? idUsuario)
        {
            if (fechaInicio.Date > fechaFin.Date)
            {
                throw new Exception(
                    "La fecha de inicio no puede ser posterior a la fecha de fin.");
            }

            List<ReporteVenta> ventas =
                objcd_reporte.ObtenerVentasAnalisis(
                    fechaInicio,
                    fechaFin,
                    idUsuario);

            List<VentaPorDia> resultado =
                new List<VentaPorDia>();

            var grupos = ventas
                .GroupBy(v => v.FechaVenta.Date)
                .OrderBy(g => g.Key);

            foreach (var grupo in grupos)
            {
                VentaPorDia ventaDia = new VentaPorDia
                {
                    Fecha = grupo.Key,
                    Total = grupo.Sum(v => v.Total),
                    CantidadVentas = grupo.Count()
                };

                resultado.Add(ventaDia);
            }

            return resultado;
        }

        
            // Compara la facturación y la cantidad de ventas de dos períodos.
            public ComparacionVentas CompararVentas(
                DateTime fechaInicioActual,
                DateTime fechaFinActual,
                DateTime fechaInicioAnterior,
                DateTime fechaFinAnterior,
                int? idUsuario)
        {
            // Valida las fechas del período actual.
            if (fechaInicioActual.Date > fechaFinActual.Date)
            {
                throw new Exception(
                    "La fecha inicial del período actual no puede ser posterior a la fecha final.");
            }

            // Valida las fechas del período anterior.
            if (fechaInicioAnterior.Date > fechaFinAnterior.Date)
            {
                throw new Exception(
                    "La fecha inicial del período anterior no puede ser posterior a la fecha final.");
            }

            // Obtiene las ventas del período actual.
            List<ReporteVenta> ventasActuales = ObtenerVentasAnalisis(
                fechaInicioActual,
                fechaFinActual,
                idUsuario);

            // Obtiene las ventas del período anterior.
            List<ReporteVenta> ventasAnteriores = ObtenerVentasAnalisis(
                fechaInicioAnterior,
                fechaFinAnterior,
                idUsuario);

            // Suma los importes de todas las ventas actuales.
            decimal facturacionActual = ventasActuales.Sum(v => v.Total);

            // Suma los importes de todas las ventas anteriores.
            decimal facturacionAnterior = ventasAnteriores.Sum(v => v.Total);

            // Calcula cuánto aumentó o disminuyó la facturación.
            decimal diferencia = facturacionActual - facturacionAnterior;

            // El porcentaje queda sin calcular inicialmente.
            decimal? variacion = null;

            // Evita dividir por cero si el período anterior no facturó.
            if (facturacionAnterior > 0)
            {
                variacion = (diferencia / facturacionAnterior) * 100;
            }

            // Guarda los resultados en la entidad de comparación.
            ComparacionVentas resultado = new ComparacionVentas
            {
                FacturacionActual = facturacionActual,
                FacturacionAnterior = facturacionAnterior,
                DiferenciaFacturacion = diferencia,
                VariacionPorcentual = variacion,
                CantidadVentasActual = ventasActuales.Count,
                CantidadVentasAnterior = ventasAnteriores.Count
            };

            // Devuelve los resultados calculados.
            return resultado;
        }


    


    }
}
