using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Entidades;

namespace Datos
{
    /// <summary>
    /// Capa de Datos correspondiente a las solicitudes
    /// de modificación de stock.
    ///
    /// Su responsabilidad es comunicarse con SQL Server.
    /// No contiene controles visuales ni decisiones de interfaz.
    /// </summary>
    public class CD_SolicitudStock
    {
        /// <summary>
        /// Registra una nueva solicitud de modificación de stock.
        ///
        /// IMPORTANTE:
        /// Este método solamente crea la solicitud.
        /// NO modifica el stock del producto.
        /// </summary>
        public bool Registrar(SolicitudStock solicitud)
        {
            using (SqlConnection conexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        INSERT INTO SolicitudStock
                        (
                            id_producto,
                            id_usuario_solicitante,
                            tipo_movimiento,
                            cantidad,
                            motivo
                        )
                        VALUES
                        (
                            @id_producto,
                            @id_usuario_solicitante,
                            @tipo_movimiento,
                            @cantidad,
                            @motivo
                        )";

                    SqlCommand cmd =
                        new SqlCommand(query, conexion);

                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue(
                        "@id_producto",
                        solicitud.IdProducto
                    );

                    cmd.Parameters.AddWithValue(
                        "@id_usuario_solicitante",
                        solicitud.IdUsuarioSolicitante
                    );

                    cmd.Parameters.AddWithValue(
                        "@tipo_movimiento",
                        solicitud.TipoMovimiento
                    );

                    cmd.Parameters.AddWithValue(
                        "@cantidad",
                        solicitud.Cantidad
                    );

                    cmd.Parameters.AddWithValue(
                        "@motivo",
                        solicitud.Motivo
                    );

                    conexion.Open();

                    int filasAfectadas =
                        cmd.ExecuteNonQuery();

                    return filasAfectadas > 0;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        /// <summary>
        /// Obtiene todas las solicitudes que todavía
        /// están pendientes de resolución.
        ///
        /// Incluye información del producto y
        /// del usuario que realizó la solicitud.
        /// </summary>
        public List<SolicitudStock> ListarPendientes()
        {
            List<SolicitudStock> lista =
                new List<SolicitudStock>();

            using (SqlConnection conexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        SELECT
                            ss.id_solicitud,
                            ss.id_producto,
                            p.nombre AS producto,
                            ss.id_usuario_solicitante,
                            u.nombre AS nombre_solicitante,
                            u.apellido AS apellido_solicitante,
                            ss.tipo_movimiento,
                            ss.cantidad,
                            ss.motivo,
                            ss.estado,
                            ss.fecha_solicitud,
                            ss.id_usuario_autorizador,
                            ss.fecha_resolucion
                        FROM SolicitudStock ss
                        INNER JOIN Producto p
                            ON ss.id_producto = p.Id_producto
                        INNER JOIN Usuario u
                            ON ss.id_usuario_solicitante = u.id_usuario
                        WHERE ss.estado = 'PENDIENTE'
                        ORDER BY ss.fecha_solicitud DESC";

                    SqlCommand cmd =
                        new SqlCommand(query, conexion);

                    cmd.CommandType = CommandType.Text;

                    conexion.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SolicitudStock solicitud =
                                new SolicitudStock();

                            solicitud.IdSolicitud =
                                Convert.ToInt32(
                                    dr["id_solicitud"]
                                );

                            solicitud.IdProducto =
                                Convert.ToInt32(
                                    dr["id_producto"]
                                );

                            solicitud.oProducto =
                                new Producto
                                {
                                    IdProducto =
                                        Convert.ToInt32(
                                            dr["id_producto"]
                                        ),

                                    Nombre =
                                        dr["producto"].ToString()
                                };

                            solicitud.IdUsuarioSolicitante =
                                Convert.ToInt32(
                                    dr["id_usuario_solicitante"]
                                );

                            solicitud.oUsuarioSolicitante =
                                new Usuario
                                {
                                    IdUsuario =
                                        Convert.ToInt32(
                                            dr["id_usuario_solicitante"]
                                        ),

                                    Nombre =
                                        dr["nombre_solicitante"]
                                            .ToString(),

                                    Apellido =
                                        dr["apellido_solicitante"]
                                            .ToString()
                                };

                            solicitud.TipoMovimiento =
                                dr["tipo_movimiento"]
                                    .ToString();

                            solicitud.Cantidad =
                                Convert.ToInt32(
                                    dr["cantidad"]
                                );

                            solicitud.Motivo =
                                dr["motivo"].ToString();

                            solicitud.Estado =
                                dr["estado"].ToString();

                            solicitud.FechaSolicitud =
                                Convert.ToDateTime(
                                    dr["fecha_solicitud"]
                                );

                            solicitud.IdUsuarioAutorizador =
                                dr["id_usuario_autorizador"]
                                    == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(
                                    dr["id_usuario_autorizador"]
                                );

                            solicitud.FechaResolucion =
                                dr["fecha_resolucion"]
                                    == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(
                                    dr["fecha_resolucion"]
                                );

                            lista.Add(solicitud);
                        }
                    }

                    return lista;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
