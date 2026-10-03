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

                    // Ajustamos las fechas para abarcar desde las 00:00:00 del primer día hasta las 23:59:59 del día final
                    cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                    cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Date.AddDays(1).AddTicks(-1));

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        HashSet<int> idsVentasProcesadas = new HashSet<int>();

                        while (dr.Read())
                        {
                            int idVenta = Convert.ToInt32(dr["Id_ventaCabecera"]);

                            int idVendedorReg = 0;
                            if (dr["id_usuario"] != DBNull.Value)
                            {
                                idVendedorReg = Convert.ToInt32(dr["id_usuario"]);
                            }

                            if (!idUsuario.HasValue || idUsuario.Value == 0 || idUsuario.Value == idVendedorReg)
                            {
                                if (!idsVentasProcesadas.Contains(idVenta))
                                {
                                    ReporteVenta rv = new ReporteVenta
                                    {
                                        IdVenta = idVenta,
                                        FechaVenta = Convert.ToDateTime(dr["Fecha"]),
                                        NroFactura = idVenta.ToString("D8"),
                                        Vendedor = dr["Vendedor"] != DBNull.Value ? dr["Vendedor"].ToString() : "Sin Vendedor",
                                        Cliente = "Consumidor Final",
                                        Total = dr["MontoTotal"] != DBNull.Value ? Convert.ToDecimal(dr["MontoTotal"]) : 0m
                                    };

                                    lista.Add(rv);
                                    idsVentasProcesadas.Add(idVenta);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al consultar ventas en la base de datos: " + ex.Message);
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
