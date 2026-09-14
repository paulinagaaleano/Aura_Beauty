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
    /// Se encarga de acceder a la tabla Categoria
    /// de la base de datos.
    ///
    /// Realiza las operaciones de:
    /// - Listar categorías activas.
    /// - Registrar categorías.
    /// - Editar categorías.
    /// - Dar de baja categorías de forma lógica.
    ///
    /// Esta clase NO contiene lógica visual
    /// ni reglas de negocio.
    /// </summary>
    public class CD_Categoria
    {
        /// <summary>
        /// Obtiene todas las categorías activas.
        ///
        /// Una categoría se considera activa
        /// cuando deleted_at es NULL.
        /// </summary>
        public List<Categoria> Listar()
        {
            List<Categoria> lista =
                new List<Categoria>();

            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        SELECT
                            Id_categoria,
                            nombre,
                            descripcion,
                            imagen,
                            created_at,
                            updated_at,
                            deleted_at
                        FROM Categoria
                        WHERE deleted_at IS NULL
                        ORDER BY nombre";

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
                            Categoria categoria =
                                new Categoria()
                                {
                                    IdCategoria =
                                        Convert.ToInt32(
                                            dr["Id_categoria"]
                                        ),

                                    Nombre =
                                        dr["nombre"] == DBNull.Value
                                            ? ""
                                            : dr["nombre"].ToString(),

                                    Descripcion =
                                        dr["descripcion"] == DBNull.Value
                                            ? ""
                                            : dr["descripcion"].ToString(),

                                    Imagen =
                                        dr["imagen"] == DBNull.Value
                                            ? ""
                                            : dr["imagen"].ToString(),

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

                            lista.Add(categoria);
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
        /// Registra una nueva categoría.
        ///
        /// Id_categoria no se incluye porque
        /// SQL Server lo genera automáticamente.
        /// </summary>
        public int Registrar(
            Categoria categoria)
        {
            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        INSERT INTO Categoria
                        (
                            nombre,
                            descripcion,
                            imagen,
                            created_at
                        )
                        VALUES
                        (
                            @nombre,
                            @descripcion,
                            @imagen,
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
                        categoria.Nombre
                    );

                    cmd.Parameters.AddWithValue(
                        "@descripcion",
                        string.IsNullOrWhiteSpace(
                            categoria.Descripcion
                        )
                            ? (object)DBNull.Value
                            : categoria.Descripcion
                    );

                    cmd.Parameters.AddWithValue(
                        "@imagen",
                        string.IsNullOrWhiteSpace(
                            categoria.Imagen
                        )
                            ? (object)DBNull.Value
                            : categoria.Imagen
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
        /// Modifica una categoría existente.
        ///
        /// updated_at registra automáticamente
        /// la fecha de modificación.
        /// </summary>
        public bool Editar(
            Categoria categoria)
        {
            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        UPDATE Categoria
                        SET
                            nombre = @nombre,
                            descripcion = @descripcion,
                            imagen = @imagen,
                            updated_at = GETDATE()
                        WHERE Id_categoria = @id_categoria
                          AND deleted_at IS NULL";

                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType =
                        CommandType.Text;

                    cmd.Parameters.AddWithValue(
                        "@nombre",
                        categoria.Nombre
                    );

                    cmd.Parameters.AddWithValue(
                        "@descripcion",
                        string.IsNullOrWhiteSpace(
                            categoria.Descripcion
                        )
                            ? (object)DBNull.Value
                            : categoria.Descripcion
                    );

                    cmd.Parameters.AddWithValue(
                        "@imagen",
                        string.IsNullOrWhiteSpace(
                            categoria.Imagen
                        )
                            ? (object)DBNull.Value
                            : categoria.Imagen
                    );

                    cmd.Parameters.AddWithValue(
                        "@id_categoria",
                        categoria.IdCategoria
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
        /// Realiza una baja lógica.
        ///
        /// NO elimina físicamente la categoría.
        /// Solamente guarda la fecha de baja
        /// en deleted_at.
        /// </summary>
        public bool Eliminar(
            int idCategoria)
        {
            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        UPDATE Categoria
                        SET
                            deleted_at = GETDATE(),
                            updated_at = GETDATE()
                        WHERE Id_categoria = @id_categoria
                          AND deleted_at IS NULL";

                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType =
                        CommandType.Text;

                    cmd.Parameters.AddWithValue(
                        "@id_categoria",
                        idCategoria
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
    }
}
