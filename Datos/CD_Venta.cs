using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using System.Data;
using System.Data.SqlClient;

namespace Datos
{
    /// <summary>
    /// Clase de Acceso a Datos encargada del registro de ventas.
    ///
    /// Su responsabilidad es comunicarse directamente con
    /// SQL Server para:
    ///
    /// 1. Registrar la cabecera de la venta.
    /// 2. Registrar cada producto en VentaDetalle.
    /// 3. Verificar nuevamente el stock disponible.
    /// 4. Descontar las unidades vendidas del stock.
    ///
    /// Todas estas operaciones se realizan dentro de una
    /// transacción SQL para mantener la consistencia de los datos.
    /// </summary>
    public class CD_Venta
    {
        /// <summary>
        /// Registra una venta completa.
        ///
        /// Devuelve el ID generado para VentaCabecera.
        ///
        /// Si alguna operación falla, se ejecuta Rollback
        /// y ninguna parte de la venta queda registrada.
        /// </summary>
        /// <param name="venta">
        /// Objeto que contiene la cabecera y todos
        /// los detalles de la venta.
        /// </param>
        /// <returns>
        /// ID generado para la nueva venta.
        /// </returns>
        public int Registrar(VentaCabecera venta)
        {
            // Creamos la conexión utilizando la cadena
            // centralizada en Conexion.cs.
            using (SqlConnection conexion =
                new SqlConnection(Conexion.cadena))
            {
                conexion.Open();


                // BeginTransaction inicia una transacción.
                //
                // A partir de este momento las operaciones
                // podrán confirmarse todas juntas mediante
                // Commit o cancelarse mediante Rollback.
                SqlTransaction transaccion =
                    conexion.BeginTransaction();


                try
                {
                    // =================================================
                    // 1. REGISTRAR CABECERA
                    // =================================================

                    string consultaCabecera = @"
                        INSERT INTO VentaCabecera
                        (
                            id_cliente,
                            id_usuario,
                            tipo_factura,
                            nro_factura,
                            total,
                            fecha_venta
                        )
                        VALUES
                        (
                            @idCliente,
                            @idUsuario,
                            @tipoFactura,
                            @nroFactura,
                            @total,
                            GETDATE()
                        );

                        SELECT CAST(SCOPE_IDENTITY() AS INT);";


                    SqlCommand comandoCabecera =
                        new SqlCommand(
                            consultaCabecera,
                            conexion,
                            transaccion
                        );


                    // id_cliente admite NULL.
                    //
                    // Si IdCliente no posee valor,
                    // guardamos DBNull.Value.
                    // Esto permitirá manejar Consumidor Final.
                    comandoCabecera.Parameters.Add(
                        "@idCliente",
                        SqlDbType.Int
                    ).Value =
                        venta.IdCliente.HasValue
                            ? (object)venta.IdCliente.Value
                            : DBNull.Value;


                    comandoCabecera.Parameters.Add(
                        "@idUsuario",
                        SqlDbType.Int
                    ).Value =
                        venta.IdUsuario;


                    comandoCabecera.Parameters.Add(
                        "@tipoFactura",
                        SqlDbType.NVarChar,
                        50
                    ).Value =
                        (object)venta.TipoFactura
                        ?? DBNull.Value;


                    comandoCabecera.Parameters.Add(
                        "@nroFactura",
                        SqlDbType.NVarChar,
                        50
                    ).Value =
                        (object)venta.NroFactura
                        ?? DBNull.Value;


                    // Para importes monetarios utilizamos Decimal.
                    SqlParameter parametroTotal =
                        comandoCabecera.Parameters.Add(
                            "@total",
                            SqlDbType.Decimal
                        );

                    parametroTotal.Precision = 18;
                    parametroTotal.Scale = 2;
                    parametroTotal.Value = venta.Total;


                    // ExecuteScalar devuelve un único valor.
                    // En este caso devuelve el ID generado
                    // por SCOPE_IDENTITY().
                    int idVenta =
                        Convert.ToInt32(
                            comandoCabecera.ExecuteScalar()
                        );


                    // =================================================
                    // 2. REGISTRAR CADA DETALLE
                    // =================================================

                    foreach (
                        VentaDetalle detalle
                        in venta.Detalles
                    )
                    {
                        // ---------------------------------------------
                        // 2A. VERIFICAR STOCK ACTUAL
                        // ---------------------------------------------

                        string consultaStock = @"
                            SELECT stock
                            FROM Producto
                            WHERE Id_producto = @idProducto
                              AND deleted_at IS NULL;";


                        SqlCommand comandoStock =
                            new SqlCommand(
                                consultaStock,
                                conexion,
                                transaccion
                            );


                        comandoStock.Parameters.Add(
                            "@idProducto",
                            SqlDbType.Int
                        ).Value =
                            detalle.IdProducto;


                        object resultadoStock =
                            comandoStock.ExecuteScalar();


                        // Si no encontramos el producto activo,
                        // no podemos continuar con la venta.
                        if (
                            resultadoStock == null
                            ||
                            resultadoStock == DBNull.Value
                        )
                        {
                            throw new Exception(
                                "Uno de los productos de la venta "
                                + "ya no se encuentra disponible."
                            );
                        }


                        int stockActual =
                            Convert.ToInt32(
                                resultadoStock
                            );


                        // Volvemos a verificar el stock en Datos
                        // inmediatamente antes de modificarlo.
                        //
                        // Esto evita registrar una venta cuando
                        // las existencias ya no son suficientes.
                        if (
                            stockActual <
                            detalle.Cantidad
                        )
                        {
                            throw new Exception(
                                "Stock insuficiente para uno "
                                + "de los productos de la venta."
                            );
                        }


                        // ---------------------------------------------
                        // 2B. INSERTAR DETALLE
                        // ---------------------------------------------

                        string consultaDetalle = @"
                            INSERT INTO VentaDetalle
                            (
                                id_ventaCabecera,
                                id_producto,
                                cantidad,
                                precio_unitario,
                                subtotal
                            )
                            VALUES
                            (
                                @idVenta,
                                @idProducto,
                                @cantidad,
                                @precioUnitario,
                                @subtotal
                            );";


                        SqlCommand comandoDetalle =
                            new SqlCommand(
                                consultaDetalle,
                                conexion,
                                transaccion
                            );


                        comandoDetalle.Parameters.Add(
                            "@idVenta",
                            SqlDbType.Int
                        ).Value =
                            idVenta;


                        comandoDetalle.Parameters.Add(
                            "@idProducto",
                            SqlDbType.Int
                        ).Value =
                            detalle.IdProducto;


                        comandoDetalle.Parameters.Add(
                            "@cantidad",
                            SqlDbType.Int
                        ).Value =
                            detalle.Cantidad;


                        SqlParameter parametroPrecio =
                            comandoDetalle.Parameters.Add(
                                "@precioUnitario",
                                SqlDbType.Decimal
                            );

                        parametroPrecio.Precision = 18;
                        parametroPrecio.Scale = 2;
                        parametroPrecio.Value =
                            detalle.PrecioUnitario;


                        SqlParameter parametroSubtotal =
                            comandoDetalle.Parameters.Add(
                                "@subtotal",
                                SqlDbType.Decimal
                            );

                        parametroSubtotal.Precision = 18;
                        parametroSubtotal.Scale = 2;
                        parametroSubtotal.Value =
                            detalle.Subtotal;


                        comandoDetalle.ExecuteNonQuery();


                        // ---------------------------------------------
                        // 2C. DESCONTAR STOCK
                        // ---------------------------------------------

                        string consultaDescontarStock = @"
                            UPDATE Producto
                            SET
                                stock = stock - @cantidad,
                                updated_at = GETDATE()
                            WHERE Id_producto = @idProducto
                              AND deleted_at IS NULL
                              AND stock >= @cantidad;";


                        SqlCommand comandoDescontarStock =
                            new SqlCommand(
                                consultaDescontarStock,
                                conexion,
                                transaccion
                            );


                        comandoDescontarStock.Parameters.Add(
                            "@cantidad",
                            SqlDbType.Int
                        ).Value =
                            detalle.Cantidad;


                        comandoDescontarStock.Parameters.Add(
                            "@idProducto",
                            SqlDbType.Int
                        ).Value =
                            detalle.IdProducto;


                        int filasAfectadas =
                            comandoDescontarStock.ExecuteNonQuery();


                        // Debe modificarse exactamente el producto
                        // correspondiente. Si no ocurrió, abortamos.
                        if (filasAfectadas == 0)
                        {
                            throw new Exception(
                                "No fue posible actualizar el stock "
                                + "de uno de los productos."
                            );
                        }
                    }


                    // =================================================
                    // 3. CONFIRMAR TRANSACCIÓN
                    // =================================================

                    // Si llegamos hasta acá significa que:
                    //
                    // - La cabecera fue insertada.
                    // - Todos los detalles fueron insertados.
                    // - Todos los stocks fueron descontados.
                    //
                    // Commit confirma definitivamente los cambios.
                    transaccion.Commit();


                    return idVenta;
                }
                catch
                {
                    // Si ocurrió cualquier error en los pasos
                    // anteriores, deshacemos TODO.
                    //
                    // De esta manera nunca tendremos, por ejemplo,
                    // una cabecera sin detalles o una venta registrada
                    // sin haber descontado correctamente el stock.
                    try
                    {
                        transaccion.Rollback();
                    }
                    catch
                    {
                        // Si la propia conexión se perdió,
                        // Rollback también podría fallar.
                        //
                        // No reemplazamos acá la excepción original.
                    }


                    // throw vuelve a enviar la excepción original
                    // hacia la capa de Negocio.
                    throw;
                }
            }
        }
    }
}