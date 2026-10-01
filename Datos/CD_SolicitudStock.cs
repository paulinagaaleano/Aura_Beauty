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
    /// Esta clase se comunica con SQL Server mediante
    /// procedimientos almacenados.
    ///
    /// La lógica SQL se encuentra en la base de datos
    /// y esta clase se encarga de enviar parámetros,
    /// ejecutar los procedimientos y transformar
    /// los resultados en objetos de Entidades.
    /// </summary>
    public class CD_SolicitudStock
    {
        /// <summary>
        /// Registra una nueva solicitud de modificación
        /// de stock utilizando el procedimiento almacenado
        /// SP_SolicitudStock_Registrar.
        ///
        /// Registrar una solicitud NO modifica
        /// el stock del producto.
        /// </summary>
        public bool Registrar(SolicitudStock solicitud)
        {
            using (SqlConnection conexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    /*
                     * En lugar de escribir un INSERT acá,
                     * indicamos el nombre del procedimiento
                     * almacenado que queremos ejecutar.
                     */
                    SqlCommand cmd =
                        new SqlCommand(
                            "SP_SolicitudStock_Registrar",
                            conexion
                        );


                    /*
                     * StoredProcedure indica que el texto
                     * anterior NO es una consulta SQL,
                     * sino el nombre de un procedimiento
                     * almacenado de SQL Server.
                     */
                    cmd.CommandType =
                        CommandType.StoredProcedure;


                    /*
                     * Enviamos los valores que necesita
                     * el procedimiento almacenado.
                     */
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


                    /*
                     * Abrimos la conexión con SQL Server.
                     */
                    conexion.Open();


                    /*
                      * ExecuteScalar ejecuta el procedimiento
                      * y recupera el primer valor devuelto.
                      *
                      * Nuestro procedimiento devuelve:
                      * SELECT CAST(1 AS INT) AS Resultado
                      *
                      * Por lo tanto, si recibimos 1 significa
                      * que la solicitud fue registrada.
                      */
                    object resultado =
                        cmd.ExecuteScalar();

                    return Convert.ToInt32(resultado) == 1;
                }
                catch (Exception)
                {
                    /*
                     * throw vuelve a lanzar la excepción
                     * para que pueda ser tratada por
                     * las capas superiores.
                     */
                    throw;
                }
            }
        }


        /// <summary>
        /// Obtiene las solicitudes que se encuentran
        /// en estado PENDIENTE.
        ///
        /// Utiliza el procedimiento almacenado
        /// SP_SolicitudStock_ListarPendientes.
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
                    /*
                     * Indicamos el procedimiento almacenado
                     * que devolverá las solicitudes.
                     */
                    SqlCommand cmd =
                        new SqlCommand(
                            "SP_SolicitudStock_ListarPendientes",
                            conexion
                        );


                    cmd.CommandType =
                        CommandType.StoredProcedure;


                    conexion.Open();


                    /*
                     * ExecuteReader se utiliza porque
                     * esperamos recibir varias filas
                     * desde SQL Server.
                     */
                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        /*
                         * Read avanza fila por fila
                         * sobre el resultado.
                         */
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


                            /*
                             * Creamos también el objeto
                             * Producto relacionado.
                             */
                            solicitud.oProducto =
                                new Producto
                                {
                                    IdProducto =
                                        Convert.ToInt32(
                                            dr["id_producto"]
                                        ),

                                    Nombre =
                                        dr["producto"]
                                            .ToString()
                                };


                            solicitud.IdUsuarioSolicitante =
                                Convert.ToInt32(
                                    dr["id_usuario_solicitante"]
                                );


                            /*
                             * Construimos el usuario
                             * que realizó la solicitud.
                             */
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
                                dr["motivo"]
                                    .ToString();


                            solicitud.Estado =
                                dr["estado"]
                                    .ToString();


                            solicitud.FechaSolicitud =
                                Convert.ToDateTime(
                                    dr["fecha_solicitud"]
                                );


                            /*
                             * El autorizador puede ser NULL
                             * mientras la solicitud continúe
                             * pendiente.
                             */
                            solicitud.IdUsuarioAutorizador =
                                dr["id_usuario_autorizador"]
                                    == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(
                                    dr["id_usuario_autorizador"]
                                );


                            /*
                             * La fecha de resolución también
                             * es NULL mientras nadie haya
                             * aprobado o rechazado.
                             */
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
