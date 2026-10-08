using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Entidades;

namespace Datos
{
    public class CD_Reporte
    {

        public List<ReporteVenta> ObtenerVentas(
            DateTime fechaInicio,
            DateTime fechaFin,
            int? idUsuario
        )
        {
            List<ReporteVenta> lista = new List<ReporteVenta>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SP_ReporteVentas", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Incluimos todas las ventas del período seleccionado.
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Date.AddDays(1).AddTicks(-1));

                    // Si no se selecciona vendedor, consultamos todos.
                    cmd.Parameters.AddWithValue(
                        "@IdUsuario",
                        idUsuario.HasValue && idUsuario.Value > 0
                            ? (object)idUsuario.Value
                            : DBNull.Value
                    );

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ReporteVenta rv = new ReporteVenta
                            {
                                IdVenta = Convert.ToInt32(dr["IdVenta"]),

                                FechaVenta = Convert.ToDateTime(dr["FechaVenta"]),

                                NroFactura = dr["NroFactura"] != DBNull.Value
                                    ? dr["NroFactura"].ToString()
                                    : "",

                                Vendedor = dr["Vendedor"] != DBNull.Value
                                    ? dr["Vendedor"].ToString()
                                    : "Sin Vendedor",

                                Cliente = dr["Cliente"] != DBNull.Value
                                    ? dr["Cliente"].ToString()
                                    : "Consumidor Final",

                                Total = dr["Total"] != DBNull.Value
                                    ? Convert.ToDecimal(dr["Total"])
                                    : 0m
                            };

                            lista.Add(rv);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        "Error al consultar ventas en la base de datos: "
                        + ex.Message, ex);
                }
            }

            return lista;
        }


        public DataTable ObtenerReporteMovimientosStock(DateTime fechaInicio, DateTime fechaFin, out string mensaje)
        {
            mensaje = string.Empty;
            DataTable tabla = new DataTable();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    // Ajustamos el inicio a las 00:00:00 y el fin a las 23:59:59
                    DateTime inicio = fechaInicio.Date;
                    DateTime fin = fechaFin.Date.AddDays(1).AddSeconds(-1);

                    SqlCommand cmd = new SqlCommand("SP_ReporteMovimientosStock", oconexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FechaInicio", inicio);
                    cmd.Parameters.AddWithValue("@FechaFin", fin);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(tabla);
                }
                catch (Exception ex)
                {
                    mensaje = ex.Message;
                    tabla = new DataTable();
                }
            }

            return tabla;
        }

        
        }
    }
