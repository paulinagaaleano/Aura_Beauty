using System;
using System.Collections.Generic;
using System.Data;
using Datos;
using Entidades;

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

        public DataTable ObtenerReporteMovimientosStock(DateTime fechaInicio, DateTime fechaFin, out string mensaje)
        {
            if (fechaInicio.Date > fechaFin.Date)
            {
                mensaje = "La fecha de inicio no puede ser posterior a la fecha de fin.";
                return new DataTable();
            }

            return objcd_reporte.ObtenerReporteMovimientosStock(fechaInicio, fechaFin, out mensaje);
        }
    }
}