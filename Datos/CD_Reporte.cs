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
    /// Clase de Acceso a Datos encargada de obtener
    /// información para los reportes de ventas.
    ///
    /// No crea ni modifica ventas.
    /// Su función es consultar información ya registrada
    /// en la base de datos.
    /// </summary>
    public class CD_Reporte
    {
        /// <summary>
        /// Obtiene las ventas comprendidas entre dos fechas.
        ///
        /// Si idUsuario es null, se muestran las ventas
        /// de todos los vendedores.
        ///
        /// Si idUsuario tiene un valor, se filtran únicamente
        /// las ventas realizadas por ese usuario.
        /// </summary>
        public List<ReporteVenta> ObtenerVentas(
            DateTime fechaDesde,
            DateTime fechaHasta,
            int? idUsuario)
        {
            List<ReporteVenta> lista =
                new List<ReporteVenta>();


            using (SqlConnection conexion =
                new SqlConnection(Conexion.cadena))
            {
                try
                {
                    conexion.Open();


                    // =================================================
                    // CONSULTA
                    // =================================================

                    string consulta = @"
                        SELECT
                            vc.Id_ventaCabecera,
                            vc.fecha_venta,
                            vc.nro_factura,

                            u.nombre + ' ' + u.apellido
                                AS Vendedor,

                            CASE
                                WHEN c.Id_cliente IS NULL
                                    THEN 'Consumidor Final'
                                ELSE
                                    c.apellido + ', ' + c.nombre
                            END
                                AS Cliente,

                            vc.total

                        FROM VentaCabecera vc

                        INNER JOIN Usuario u
                            ON u.id_usuario = vc.id_usuario

                        LEFT JOIN Cliente c
                            ON c.Id_cliente = vc.id_cliente

                        WHERE
                            vc.fecha_venta >= @fechaDesde

                            AND vc.fecha_venta < @fechaHasta

                            AND
                            (
                                @idUsuario IS NULL
                                OR vc.id_usuario = @idUsuario
                            )

                        ORDER BY
                            vc.fecha_venta DESC;";


                    SqlCommand comando =
                        new SqlCommand(
                            consulta,
                            conexion
                        );


                    // =================================================
                    // PARÁMETROS DE FECHA
                    // =================================================

                    comando.Parameters.Add(
                        "@fechaDesde",
                        SqlDbType.DateTime
                    ).Value =
                        fechaDesde;


                    comando.Parameters.Add(
                        "@fechaHasta",
                        SqlDbType.DateTime
                    ).Value =
                        fechaHasta;


                    // =================================================
                    // PARÁMETRO VENDEDOR
                    // =================================================

                    comando.Parameters.Add(
                        "@idUsuario",
                        SqlDbType.Int
                    ).Value =
                        idUsuario.HasValue
                            ? (object)idUsuario.Value
                            : DBNull.Value;


                    // =================================================
                    // EJECUTAR CONSULTA
                    // =================================================

                    using (SqlDataReader lector =
                        comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            ReporteVenta reporte =
                                new ReporteVenta
                                {
                                    IdVenta =
                                        Convert.ToInt32(
                                            lector[
                                                "Id_ventaCabecera"
                                            ]
                                        ),

                                    FechaVenta =
                                        Convert.ToDateTime(
                                            lector[
                                                "fecha_venta"
                                            ]
                                        ),

                                    NroFactura =
                                        lector[
                                            "nro_factura"
                                        ] != DBNull.Value
                                            ? lector[
                                                "nro_factura"
                                              ].ToString()
                                            : "",

                                    Vendedor =
                                        lector[
                                            "Vendedor"
                                        ] != DBNull.Value
                                            ? lector[
                                                "Vendedor"
                                              ].ToString()
                                            : "",

                                    Cliente =
                                        lector[
                                            "Cliente"
                                        ] != DBNull.Value
                                            ? lector[
                                                "Cliente"
                                              ].ToString()
                                            : "Consumidor Final",

                                    Total =
                                        lector[
                                            "total"
                                        ] != DBNull.Value
                                            ? Convert.ToDecimal(
                                                lector["total"]
                                              )
                                            : 0
                                };


                            lista.Add(
                                reporte
                            );
                        }
                    }
                }
                catch
                {
                    // No transformamos la excepción acá.
                    // La enviamos hacia Negocio para que
                    // pueda ser tratada por la aplicación.
                    throw;
                }
            }


            return lista;
        }
    }
}
