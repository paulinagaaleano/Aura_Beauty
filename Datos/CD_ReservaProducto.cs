
using System;
using System.Data;
using System.Data.SqlClient;
using Entidades;

namespace Datos
{
    /// <summary>
    /// CAPA DATOS - RESERVAS DE PRODUCTOS.
    ///
    /// Ejecuta únicamente procedimientos almacenados:
    ///
    /// SP_ReservaProducto_Reservar
    /// SP_ReservaProducto_Cancelar
    ///
    /// No descuenta stock ni registra ventas.
    /// </summary>
    public class CD_ReservaProducto
    {
        /// <summary>
        /// Crea o actualiza la cantidad TOTAL reservada
        /// de un producto dentro de una operación.
        ///
        /// La verificación de disponibilidad se realiza
        /// en SQL Server, dentro de una transacción.
        /// </summary>
        public ReservaProducto Reservar(
            Guid idOperacion,
            int idProducto,
            int idUsuario,
            int cantidad,
            out int stockFisico,
            out int reservadoPorOtros,
            out int disponibleRestante)
        {
            stockFisico = 0;
            reservadoPorOtros = 0;
            disponibleRestante = 0;

            using (SqlConnection conexion =
                new SqlConnection(Conexion.cadena))
            using (SqlCommand comando =
                new SqlCommand(
                    "dbo.SP_ReservaProducto_Reservar",
                    conexion))
            {
                comando.CommandType =
                    CommandType.StoredProcedure;

                comando.Parameters.Add(
                    "@IdOperacion",
                    SqlDbType.UniqueIdentifier
                ).Value = idOperacion;

                comando.Parameters.Add(
                    "@IdProducto",
                    SqlDbType.Int
                ).Value = idProducto;

                comando.Parameters.Add(
                    "@IdUsuario",
                    SqlDbType.Int
                ).Value = idUsuario;

                comando.Parameters.Add(
                    "@Cantidad",
                    SqlDbType.Int
                ).Value = cantidad;

                try
                {
                    conexion.Open();

                    using (SqlDataReader lector =
                        comando.ExecuteReader())
                    {
                        if (!lector.Read())
                        {
                            throw new InvalidOperationException(
                                "La base de datos no devolvió " +
                                "el resultado de la reserva.");
                        }

                        ReservaProducto reserva =
                            new ReservaProducto
                            {
                                IdReservaProducto =
                                    Convert.ToInt32(
                                        lector["IdReservaProducto"]),

                                IdOperacion =
                                    (Guid)lector["IdOperacion"],

                                IdProducto =
                                    Convert.ToInt32(
                                        lector["IdProducto"]),

                                IdUsuario = idUsuario,

                                Cantidad =
                                    Convert.ToInt32(
                                        lector["CantidadReservada"]),

                                Estado =
                                    Convert.ToString(
                                        lector["Estado"]),

                                IdVentaCabecera = null
                            };

                        DateTime vencimiento =
                            Convert.ToDateTime(
                                lector["FechaVencimientoUtc"]);

                        reserva.FechaVencimientoUtc =
                            DateTime.SpecifyKind(
                                vencimiento,
                                DateTimeKind.Utc);

                        stockFisico =
                            Convert.ToInt32(
                                lector["StockFisico"]);

                        reservadoPorOtros =
                            Convert.ToInt32(
                                lector["ReservadoPorOtros"]);

                        disponibleRestante =
                            Convert.ToInt32(
                                lector["DisponibleRestante"]);

                        return reserva;
                    }
                }
                catch (SqlException ex)
                {
                    throw new InvalidOperationException(
                        "No se pudo reservar el producto: " +
                        ex.Message,
                        ex);
                }
            }
        }

        /// <summary>
        /// Libera una reserva individual o todas las
        /// reservas de un carrito.
        ///
        /// idProducto = null:
        /// cancela todas las reservas activas del carrito.
        ///
        /// idProducto con valor:
        /// cancela solamente ese producto.
        ///
        /// Devuelve la cantidad de unidades vigentes
        /// que quedaron liberadas para otros vendedores.
        ///
        /// No modifica el stock físico.
        /// </summary>
        public int Cancelar(
            Guid idOperacion,
            int idUsuario,
            int? idProducto)
        {
            using (SqlConnection conexion =
                new SqlConnection(Conexion.cadena))
            using (SqlCommand comando =
                new SqlCommand(
                    "dbo.SP_ReservaProducto_Cancelar",
                    conexion))
            {
                comando.CommandType =
                    CommandType.StoredProcedure;

                comando.Parameters.Add(
                    "@IdOperacion",
                    SqlDbType.UniqueIdentifier
                ).Value = idOperacion;

                comando.Parameters.Add(
                    "@IdUsuario",
                    SqlDbType.Int
                ).Value = idUsuario;

                comando.Parameters.Add(
                    "@IdProducto",
                    SqlDbType.Int
                ).Value = idProducto.HasValue
                    ? (object)idProducto.Value
                    : DBNull.Value;

                try
                {
                    conexion.Open();

                    using (SqlDataReader lector =
                        comando.ExecuteReader())
                    {
                        if (!lector.Read())
                        {
                            throw new InvalidOperationException(
                                "La base de datos no devolvió " +
                                "el resultado de la cancelación.");
                        }

                        return Convert.ToInt32(
                            lector["UnidadesLiberadas"]);
                    }
                }
                catch (SqlException ex)
                {
                    throw new InvalidOperationException(
                        "No se pudo cancelar la reserva: " +
                        ex.Message,
                        ex);
                }
            }
        }
    }
}
