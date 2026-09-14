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
    /// Clase perteneciente a la Capa de Datos.
    ///
    /// Se encarga del acceso a la tabla Producto.
    ///
    /// Permite:
    /// - Listar productos activos.
    /// - Buscar productos.
    /// - Registrar.
    /// - Editar.
    /// - Realizar una baja lógica.
    ///
    /// También relaciona Producto con Categoria.
    /// </summary>
    public class CD_Producto
    {
        /// <summary>
        /// Obtiene todos los productos activos
        /// junto con su categoría.
        /// </summary>
        public List<Producto> Listar()
        {
            List<Producto> lista =
                new List<Producto>();

            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        SELECT
                            p.Id_producto,
                            p.nombre,
                            p.descripcion,
                            p.precio,
                            p.stock,
                            p.id_categoria,
                            p.created_at,
                            p.updated_at,
                            p.deleted_at,
                            c.nombre AS CategoriaNombre
                        FROM Producto p
                        INNER JOIN Categoria c
                            ON p.id_categoria = c.Id_categoria
                        WHERE p.deleted_at IS NULL
                          AND c.deleted_at IS NULL
                        ORDER BY p.nombre";

                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType =
                        CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Producto producto =
                                MapearProducto(dr);

                            lista.Add(producto);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return lista;
        }


        /// <summary>
        /// Busca productos activos por nombre.
        /// </summary>
        public List<Producto> Buscar(
            string texto)
        {
            List<Producto> lista =
                new List<Producto>();

            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        SELECT
                            p.Id_producto,
                            p.nombre,
                            p.descripcion,
                            p.precio,
                            p.stock,
                            p.id_categoria,
                            p.created_at,
                            p.updated_at,
                            p.deleted_at,
                            c.nombre AS CategoriaNombre
                        FROM Producto p
                        INNER JOIN Categoria c
                            ON p.id_categoria = c.Id_categoria
                        WHERE p.deleted_at IS NULL
                          AND c.deleted_at IS NULL
                          AND p.nombre LIKE @texto
                        ORDER BY p.nombre";

                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType =
                        CommandType.Text;

                    cmd.Parameters.AddWithValue(
                        "@texto",
                        "%" + texto + "%"
                    );

                    oconexion.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Producto producto =
                                MapearProducto(dr);

                            lista.Add(producto);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return lista;
        }


        /// <summary>
        /// Registra un producto nuevo.
        ///
        /// Id_producto es IDENTITY,
        /// por lo tanto SQL Server lo genera.
        /// </summary>
        public int Registrar(
            Producto producto)
        {
            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        INSERT INTO Producto
                        (
                            nombre,
                            descripcion,
                            precio,
                            stock,
                            id_categoria,
                            created_at
                        )
                        VALUES
                        (
                            @nombre,
                            @descripcion,
                            @precio,
                            @stock,
                            @id_categoria,
                            GETDATE()
                        );

                        SELECT CAST(
                            SCOPE_IDENTITY() AS INT
                        );";

                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType =
                        CommandType.Text;

                    cmd.Parameters.AddWithValue(
                        "@nombre",
                        producto.Nombre
                    );

                    cmd.Parameters.AddWithValue(
                        "@descripcion",
                        string.IsNullOrWhiteSpace(
                            producto.Descripcion
                        )
                            ? (object)DBNull.Value
                            : producto.Descripcion
                    );

                    cmd.Parameters.AddWithValue(
                        "@precio",
                        producto.Precio
                    );

                    cmd.Parameters.AddWithValue(
                        "@stock",
                        producto.Stock
                    );

                    cmd.Parameters.AddWithValue(
                        "@id_categoria",
                        producto.IdCategoria
                    );

                    oconexion.Open();

                    int idGenerado =
                        Convert.ToInt32(
                            cmd.ExecuteScalar()
                        );

                    return idGenerado;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        /// <summary>
        /// Modifica un producto existente.
        /// </summary>
        public bool Editar(
            Producto producto)
        {
            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        UPDATE Producto
                        SET
                            nombre = @nombre,
                            descripcion = @descripcion,
                            precio = @precio,
                            stock = @stock,
                            id_categoria = @id_categoria,
                            updated_at = GETDATE()
                        WHERE Id_producto = @id_producto
                          AND deleted_at IS NULL";

                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType =
                        CommandType.Text;

                    cmd.Parameters.AddWithValue(
                        "@nombre",
                        producto.Nombre
                    );

                    cmd.Parameters.AddWithValue(
                        "@descripcion",
                        string.IsNullOrWhiteSpace(
                            producto.Descripcion
                        )
                            ? (object)DBNull.Value
                            : producto.Descripcion
                    );

                    cmd.Parameters.AddWithValue(
                        "@precio",
                        producto.Precio
                    );

                    cmd.Parameters.AddWithValue(
                        "@stock",
                        producto.Stock
                    );

                    cmd.Parameters.AddWithValue(
                        "@id_categoria",
                        producto.IdCategoria
                    );

                    cmd.Parameters.AddWithValue(
                        "@id_producto",
                        producto.IdProducto
                    );

                    oconexion.Open();

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
        /// Realiza la baja lógica de un producto.
        ///
        /// El registro permanece en SQL Server,
        /// pero deleted_at recibe la fecha actual.
        /// </summary>
        public bool Eliminar(
            int idProducto)
        {
            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        UPDATE Producto
                        SET
                            deleted_at = GETDATE(),
                            updated_at = GETDATE()
                        WHERE Id_producto = @id_producto
                          AND deleted_at IS NULL";

                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType =
                        CommandType.Text;

                    cmd.Parameters.AddWithValue(
                        "@id_producto",
                        idProducto
                    );

                    oconexion.Open();

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
        /// Convierte una fila obtenida desde SQL Server
        /// en un objeto Producto.
        ///
        /// Este método evita repetir el mismo código
        /// tanto en Listar() como en Buscar().
        /// </summary>
        private Producto MapearProducto(
            SqlDataReader dr)
        {
            Producto producto =
                new Producto()
                {
                    IdProducto =
                        Convert.ToInt32(
                            dr["Id_producto"]
                        ),

                    Nombre =
                        dr["nombre"] == DBNull.Value
                            ? ""
                            : dr["nombre"].ToString(),

                    Descripcion =
                        dr["descripcion"] == DBNull.Value
                            ? ""
                            : dr["descripcion"].ToString(),

                    Precio =
                        dr["precio"] == DBNull.Value
                            ? 0
                            : Convert.ToDecimal(
                                dr["precio"]
                            ),

                    Stock =
                        dr["stock"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(
                                dr["stock"]
                            ),

                    IdCategoria =
                        dr["id_categoria"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(
                                dr["id_categoria"]
                            ),

                    oCategoria =
                        new Categoria()
                        {
                            IdCategoria =
                                dr["id_categoria"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(
                                        dr["id_categoria"]
                                    ),

                            Nombre =
                                dr["CategoriaNombre"] == DBNull.Value
                                    ? ""
                                    : dr["CategoriaNombre"].ToString()
                        },

                    CreatedAt =
                        dr["created_at"] == DBNull.Value
                            ? (DateTime?)null
                            : Convert.ToDateTime(
                                dr["created_at"]
                            ),

                    UpdatedAt =
                        dr["updated_at"] == DBNull.Value
                            ? (DateTime?)null
                            : Convert.ToDateTime(
                                dr["updated_at"]
                            ),

                    DeletedAt =
                        dr["deleted_at"] == DBNull.Value
                            ? (DateTime?)null
                            : Convert.ToDateTime(
                                dr["deleted_at"]
                            )
                };

            return producto;
        }
    }
}