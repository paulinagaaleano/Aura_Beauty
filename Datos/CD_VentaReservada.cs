
using System;
using System.Data;
using System.Data.SqlClient;
using Entidades;

namespace Datos
{
    /// <summary>
    /// CAPA DATOS - CONFIRMACIÓN DE VENTAS RESERVADAS.
    ///
    /// Esta clase permite registrar una venta definitiva
    /// utilizando las reservas previamente realizadas.
    ///
    /// Ejecuta solamente el procedimiento almacenado:
    ///
    /// dbo.SP_Venta_ConfirmarReservas
    ///
    /// El procedimiento se encarga de:
    /// 1. Validar las reservas del carrito.
    /// 2. Verificar las cantidades y los precios.
    /// 3. Registrar VentaCabecera.
    /// 4. Registrar VentaDetalle.
    /// 5. Descontar definitivamente el stock.
    /// 6. Marcar las reservas como confirmadas.
    ///
    /// Todas esas operaciones ocurren dentro de
    /// una única transacción SQL Server.
    ///
    /// Esta clase NO descuenta stock por separado.
    /// </summary>
    public class CD_VentaReservada
    {
        /// <summary>
        /// Confirma una venta que tiene productos reservados.
        ///
        /// El identificador de operación debe ser el mismo
        /// utilizado al agregar los productos al carrito.
        ///
        /// El procedimiento devuelve el identificador
        /// de la venta y un indicador de duplicación.
        /// </summary>
        /// <param name="idOperacion">
        /// Identificador único del carrito.
        /// </param>
        /// <param name="venta">
        /// Cabecera y detalles de la venta.
        /// </param>
        /// <param name="yaRegistrada">
        /// Indica si esta operación ya había sido confirmada.
        /// </param>
        /// <returns>
        /// Identificador definitivo de VentaCabecera.
        /// </returns>
        public int Confirmar(
            Guid idOperacion,
            VentaCabecera venta,
            out bool yaRegistrada)
        {
            yaRegistrada = false;

            // ============================================
            // 1. CONSTRUIR LOS DETALLES PARA SQL SERVER
            // ============================================

            /*
             * Nuestro procedimiento recibe un parámetro
             * de tipo tabla:
             *
             * dbo.TVP_DetalleVentaReserva
             *
             * Ese tipo fue creado en el paso anterior.
             *
             * Los nombres y tipos de las columnas deben
             * coincidir con el tipo definido en SQL Server.
             */

            DataTable tablaDetalles = new DataTable();

            tablaDetalles.Columns.Add(
                "IdProducto",
                typeof(int));

            tablaDetalles.Columns.Add(
                "Cantidad",
                typeof(int));

            tablaDetalles.Columns.Add(
                "PrecioUnitario",
                typeof(decimal));

            foreach (VentaDetalle detalle in venta.Detalles)
            {
                tablaDetalles.Rows.Add(
                    detalle.IdProducto,
                    detalle.Cantidad,
                    detalle.PrecioUnitario);
            }

            // ============================================
            // 2. ABRIR CONEXIÓN
            // ============================================

            using (SqlConnection conexion =
                new SqlConnection(Conexion.cadena))
            using (SqlCommand comando =
                new SqlCommand(
                    "dbo.SP_Venta_ConfirmarReservas",
                    conexion))
            {
                comando.CommandType =
                    CommandType.StoredProcedure;

                // Se establece un tiempo máximo razonable
                // para obtener los bloqueos transaccionales.
                comando.CommandTimeout = 60;

                // ========================================
                // 3. IDENTIFICADOR DE OPERACIÓN
                // ========================================

                comando.Parameters.Add(
                    "@IdOperacion",
                    SqlDbType.UniqueIdentifier
                ).Value = idOperacion;

                // ========================================
                // 4. VENDEDOR
                // ========================================

                comando.Parameters.Add(
                    "@IdUsuario",
                    SqlDbType.Int
                ).Value = venta.IdUsuario;

                // ========================================
                // 5. CLIENTE
                // ========================================

                /*
                 * El cliente es opcional.
                 *
                 * Cuando se trata de Consumidor Final,
                 * enviamos NULL a SQL Server.
                 */

                comando.Parameters.Add(
                    "@IdCliente",
                    SqlDbType.Int
                ).Value =
                    venta.IdCliente.HasValue
                        ? (object)venta.IdCliente.Value
                        : DBNull.Value;

                // ========================================
                // 6. TIPO DE COMPROBANTE
                // ========================================

                comando.Parameters.Add(
                    "@TipoFactura",
                    SqlDbType.NVarChar,
                    50
                ).Value = venta.TipoFactura;

                // ========================================
                // 7. NÚMERO DE COMPROBANTE
                // ========================================

                comando.Parameters.Add(
                    "@NroFactura",
                    SqlDbType.NVarChar,
                    50
                ).Value = venta.NroFactura;

                // ========================================
                // 8. DETALLES DE VENTA
                // ========================================

                /*
                 * SqlDbType.Structured permite enviar
                 * varias filas como un solo parámetro.
                 *
                 * De esta manera evitamos registrar
                 * manualmente cada detalle desde C#.
                 *
                 * SQL Server procesa toda la venta
                 * dentro de su propia transacción.
                 */

                SqlParameter parametroDetalles =
                    comando.Parameters.Add(
                        "@Detalles",
                        SqlDbType.Structured);

                parametroDetalles.TypeName =
                    "dbo.TVP_DetalleVentaReserva";

                parametroDetalles.Value =
                    tablaDetalles;

                // ========================================
                // 9. EJECUTAR CONFIRMACIÓN
                // ========================================

                try
                {
                    conexion.Open();

                    using (SqlDataReader lector =
                        comando.ExecuteReader())
                    {
                        if (!lector.Read())
                        {
                            throw new InvalidOperationException(
                                "SQL Server no devolvió " +
                                "el resultado de la venta.");
                        }

                        /*
                         * El procedimiento devuelve:
                         *
                         * IdVentaCabecera
                         * YaRegistrada
                         *
                         * YaRegistrada = true significa
                         * que ese carrito ya había sido
                         * confirmado anteriormente.
                         */

                        int idVenta =
                            Convert.ToInt32(
                                lector["IdVentaCabecera"]);

                        yaRegistrada =
                            Convert.ToBoolean(
                                lector["YaRegistrada"]);

                        if (idVenta <= 0)
                        {
                            throw new InvalidOperationException(
                                "La base de datos devolvió " +
                                "un identificador de venta inválido.");
                        }

                        return idVenta;
                    }
                }
                catch (SqlException ex)
                {
                    /*
                     * No convertimos un error SQL
                     * en una venta exitosa.
                     *
                     * Conservamos el mensaje original:
                     * reserva vencida, precio cambiado,
                     * stock insuficiente, etc.
                     */

                    throw new InvalidOperationException(
                        "No se pudo confirmar la venta. " +
                        ex.Message,
                        ex);
                }
            }
        }
    }
}
