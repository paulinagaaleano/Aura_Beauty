using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;
using Entidades;


namespace Negocio
{
    /// <summary>
    /// Clase de la capa de Negocio encargada de gestionar
    /// las reglas necesarias para consultar reportes de ventas.
    ///
    /// Su responsabilidad es:
    /// - Validar el rango de fechas.
    /// - Preparar correctamente la fecha "Hasta".
    /// - Enviar los filtros a la capa de Datos.
    ///
    /// No contiene SQL ni controles de Windows Forms.
    /// </summary>
    public class CN_Reporte
    {
        /// <summary>
        /// Objeto de la capa de Datos utilizado
        /// para consultar las ventas registradas.
        /// </summary>
        private readonly CD_Reporte cdReporte =
            new CD_Reporte();


        // =========================================================
        // OBTENER REPORTE DE VENTAS
        // =========================================================

        /// <summary>
        /// Obtiene las ventas comprendidas entre dos fechas.
        ///
        /// Si idUsuario es null, devuelve las ventas
        /// de todos los vendedores.
        ///
        /// Si idUsuario tiene un valor, devuelve únicamente
        /// las ventas realizadas por ese usuario.
        /// </summary>
        public List<ReporteVenta> ObtenerVentas(
            DateTime fechaDesde,
            DateTime fechaHasta,
            int? idUsuario)
        {
            // =====================================================
            // 1. NORMALIZAR FECHA DESDE
            // =====================================================

            // Date elimina la hora y deja:
            //
            // 00:00:00
            //
            // Ejemplo:
            // 14/09/2026 00:00:00
            DateTime desde =
                fechaDesde.Date;


            // =====================================================
            // 2. NORMALIZAR FECHA HASTA
            // =====================================================

            // Para incluir TODO el día seleccionado como "Hasta",
            // no usamos 23:59:59.
            //
            // En cambio tomamos el día siguiente a las 00:00.
            //
            // Ejemplo:
            //
            // Usuario selecciona:
            // Hasta = 14/09/2026
            //
            // Internamente enviamos:
            // 15/09/2026 00:00
            //
            // Y en SQL usamos:
            //
            // fecha_venta < @fechaHasta
            DateTime hastaExclusivo =
                fechaHasta.Date.AddDays(1);


            // =====================================================
            // 3. VALIDAR RANGO
            // =====================================================

            if (
                fechaHasta.Date <
                fechaDesde.Date
            )
            {
                throw new Exception(
                    "La fecha 'Hasta' no puede ser anterior a la fecha 'Desde'."
                );
            }


            // =====================================================
            // 4. VALIDAR VENDEDOR
            // =====================================================

            // null significa "Todos los vendedores".
            //
            // Si viene un ID, debe ser positivo.
            if (
                idUsuario.HasValue
                &&
                idUsuario.Value <= 0
            )
            {
                throw new Exception(
                    "El vendedor seleccionado no es válido."
                );
            }


            // =====================================================
            // 5. CONSULTAR DATOS
            // =====================================================

            return cdReporte.ObtenerVentas(
                desde,
                hastaExclusivo,
                idUsuario
            );
        }
    }
}
