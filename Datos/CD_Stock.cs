using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using System.Data.SqlClient;

namespace Datos
{
    /// <summary>
    /// Capa de Acceso a Datos correspondiente a la gestión de stock.
    ///
    /// Su responsabilidad es comunicarse directamente con SQL Server
    /// para consultar las existencias de los productos.
    ///
    /// Esta clase NO contiene reglas de negocio ni elementos visuales.
    /// </summary>
    public class CD_Stock
    {
        /// <summary>
        /// Obtiene todos los productos activos junto con su categoría
        /// y cantidad disponible en stock.
        /// </summary>
        /// <returns>
        /// Lista de productos activos.
        /// </returns>
        public List<Producto> Listar()
        {
            List<Producto> lista = new List<Producto>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                string consulta = @"
                    SELECT
                        p.Id_producto,
                        p.nombre,
                        p.descripcion,
                        p.precio,
                        p.stock,
                        p.id_categoria,
                        c.nombre AS CategoriaNombre
                    FROM Producto p
                    INNER JOIN Categoria c
                        ON p.id_categoria = c.Id_categoria
                    WHERE p.deleted_at IS NULL
                      AND c.deleted_at IS NULL
                    ORDER BY p.nombre;";

                SqlCommand comando =
                    new SqlCommand(consulta, conexion);

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Producto producto = new Producto
                        {
                            IdProducto =
                                Convert.ToInt32(
                                    lector["Id_producto"]
                                ),

                            Nombre =
                                lector["nombre"] == DBNull.Value
                                    ? ""
                                    : lector["nombre"].ToString(),

                            Descripcion =
                                lector["descripcion"] == DBNull.Value
                                    ? ""
                                    : lector["descripcion"].ToString(),

                            Precio =
                                lector["precio"] == DBNull.Value
                                    ? 0
                                    : Convert.ToDecimal(
                                        lector["precio"]
                                    ),

                            Stock =
                                lector["stock"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(
                                        lector["stock"]
                                    ),

                            IdCategoria =
                                lector["id_categoria"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(
                                        lector["id_categoria"]
                                    ),

                            oCategoria = new Categoria
                            {
                                IdCategoria =
                                    lector["id_categoria"] == DBNull.Value
                                        ? 0
                                        : Convert.ToInt32(
                                            lector["id_categoria"]
                                        ),

                                Nombre =
                                    lector["CategoriaNombre"] == DBNull.Value
                                        ? ""
                                        : lector["CategoriaNombre"].ToString()
                            }
                        };

                        lista.Add(producto);
                    }
                }
            }

            return lista;
        }
    }
}