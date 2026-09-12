using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Entidades;

namespace Datos
{
    /// <summary>
    /// Clase perteneciente a la Capa de Datos.
    /// Se encarga de acceder a la tabla Usuario de la base de datos.
    /// 
    /// Esta clase NO contiene lógica de interfaz ni reglas de negocio.
    /// Su responsabilidad es ejecutar consultas SQL y transformar
    /// los resultados obtenidos en objetos de tipo Usuario.
    /// </summary>
    public class CD_Usuario
    {
        /// <summary>
        /// Obtiene todos los usuarios registrados en la base de datos,
        /// incluyendo la información correspondiente a su rol.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos Usuario.
        /// </returns>
        public List<Usuario> Listar()
        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        SELECT 
                            u.id_usuario,
                            u.nombre,
                            u.apellido,
                            u.correo,
                            u.contraseña,
                            u.id_rol,
                            r.descripcion AS RolDescripcion
                        FROM Usuario u
                        INNER JOIN Rol r
                            ON u.id_rol = r.id_rol";

                    SqlCommand cmd = new SqlCommand(query, oconexion);

                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Usuario usuario = new Usuario()
                            {
                                IdUsuario =
                                    Convert.ToInt32(dr["id_usuario"]),

                                Nombre =
                                    dr["nombre"].ToString(),

                                Apellido =
                                    dr["apellido"].ToString(),

                                Correo =
                                    dr["correo"].ToString(),

                                Contraseña =
                                    dr["contraseña"].ToString(),

                                IdRol =
                                    Convert.ToInt32(dr["id_rol"]),

                                oRol = new Rol()
                                {
                                    IdRol =
                                        Convert.ToInt32(dr["id_rol"]),

                                    Descripcion =
                                        dr["RolDescripcion"].ToString()
                                }
                            };

                            lista.Add(usuario);
                        }
                    }
                }
                catch (Exception)
                {
                    // Se relanza la excepción para que pueda ser manejada
                    // en una capa superior.
                    throw;
                }
            }

            return lista;
        }

        /// <summary>
        /// Registra un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="usuario">
        /// Objeto Usuario que contiene los datos a guardar.
        /// </param>
        /// <returns>
        /// Devuelve el id_usuario generado automáticamente
        /// por SQL Server.
        /// </returns>
        public int Registrar(Usuario usuario)
        {
            /*
             * Como id_usuario es IDENTITY,
             * NO lo incluimos dentro del INSERT.
             *
             * SQL Server genera ese número automáticamente.
             */

            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        INSERT INTO Usuario
                        (
                            nombre,
                            apellido,
                            correo,
                            contraseña,
                            id_rol
                        )
                        VALUES
                        (
                            @nombre,
                            @apellido,
                            @correo,
                            @contraseña,
                            @id_rol
                        );

                        SELECT CAST(SCOPE_IDENTITY() AS INT);";


                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType = CommandType.Text;


                    /*
                     * Los parámetros permiten enviar valores
                     * a SQL sin concatenarlos directamente
                     * dentro de la consulta.
                     */
                    cmd.Parameters.AddWithValue(
                        "@nombre",
                        usuario.Nombre
                    );

                    cmd.Parameters.AddWithValue(
                        "@apellido",
                        usuario.Apellido
                    );

                    cmd.Parameters.AddWithValue(
                        "@correo",
                        usuario.Correo
                    );

                    cmd.Parameters.AddWithValue(
                        "@contraseña",
                        usuario.Contraseña
                    );

                    cmd.Parameters.AddWithValue(
                        "@id_rol",
                        usuario.IdRol
                    );


                    oconexion.Open();


                    /*
                     * ExecuteScalar()
                     *
                     * Ejecuta una consulta y devuelve
                     * solamente el primer valor
                     * de la primera fila obtenida.
                     *
                     * Aquí devuelve el nuevo id_usuario.
                     *
                     * SCOPE_IDENTITY()
                     * devuelve el último valor IDENTITY
                     * generado por nuestro INSERT.
                     */
                    int idUsuarioGenerado =
                        Convert.ToInt32(
                            cmd.ExecuteScalar()
                        );


                    return idUsuarioGenerado;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }



        /// <summary>
        /// Modifica los datos de un usuario existente.
        /// </summary>
        /// <param name="usuario">
        /// Usuario con los datos actualizados.
        /// </param>
        /// <returns>
        /// Devuelve true si se modificó al menos una fila.
        /// </returns>
        public bool Editar(Usuario usuario)
        {
            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        UPDATE Usuario
                        SET
                            nombre = @nombre,
                            apellido = @apellido,
                            correo = @correo,
                            contraseña = @contraseña,
                            id_rol = @id_rol
                        WHERE id_usuario = @id_usuario";


                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType = CommandType.Text;


                    cmd.Parameters.AddWithValue(
                        "@nombre",
                        usuario.Nombre
                    );

                    cmd.Parameters.AddWithValue(
                        "@apellido",
                        usuario.Apellido
                    );

                    cmd.Parameters.AddWithValue(
                        "@correo",
                        usuario.Correo
                    );

                    cmd.Parameters.AddWithValue(
                        "@contraseña",
                        usuario.Contraseña
                    );

                    cmd.Parameters.AddWithValue(
                        "@id_rol",
                        usuario.IdRol
                    );

                    cmd.Parameters.AddWithValue(
                        "@id_usuario",
                        usuario.IdUsuario
                    );


                    oconexion.Open();


                    /*
                     * ExecuteNonQuery()
                     *
                     * Se utiliza principalmente para:
                     *
                     * INSERT
                     * UPDATE
                     * DELETE
                     *
                     * Devuelve la cantidad de filas afectadas.
                     */
                    int filasAfectadas =
                        cmd.ExecuteNonQuery();


                    /*
                     * filasAfectadas > 0
                     *
                     * Si al menos una fila fue modificada,
                     * la operación fue exitosa.
                     */
                    return filasAfectadas > 0;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }



        /// <summary>
        /// Elimina un usuario de la base de datos.
        /// </summary>
        /// <param name="idUsuario">
        /// Identificador del usuario a eliminar.
        /// </param>
        /// <returns>
        /// Devuelve true si se eliminó una fila.
        /// </returns>
        public bool Eliminar(int idUsuario)
        {
            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        DELETE FROM Usuario
                        WHERE id_usuario = @id_usuario";


                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType = CommandType.Text;


                    cmd.Parameters.AddWithValue(
                        "@id_usuario",
                        idUsuario
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
        /// Comprueba si un correo electrónico ya pertenece
        /// a otro usuario.
        /// </summary>
        /// <param name="correo">
        /// Correo que se desea verificar.
        /// </param>
        /// <param name="idUsuarioExcluir">
        /// Id de un usuario que debe excluirse de la búsqueda.
        ///
        /// Se utiliza especialmente al editar.
        ///
        /// Si vale 0, no se excluye ningún usuario.
        /// </param>
        /// <returns>
        /// true si el correo ya existe.
        /// false si está disponible.
        /// </returns>
        public bool ExisteCorreo(
            string correo,
            int idUsuarioExcluir = 0)
        {
            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        SELECT COUNT(*)
                        FROM Usuario
                        WHERE correo = @correo
                          AND id_usuario <> @id_usuario_excluir";


                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType = CommandType.Text;


                    cmd.Parameters.AddWithValue(
                        "@correo",
                        correo
                    );

                    cmd.Parameters.AddWithValue(
                        "@id_usuario_excluir",
                        idUsuarioExcluir
                    );


                    oconexion.Open();


                    int cantidad =
                        Convert.ToInt32(
                            cmd.ExecuteScalar()
                        );


                    /*
                     * Si cantidad es mayor que cero,
                     * significa que existe al menos
                     * un usuario con ese correo.
                     */
                    return cantidad > 0;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        /// <summary>
        /// Busca en la base de datos un usuario cuyo correo y contraseña
        /// coincidan con los datos ingresados.
        /// </summary>
        /// <param name="correo">
        /// Correo electrónico ingresado por el usuario.
        /// </param>
        /// <param name="contraseña">
        /// Contraseña ingresada por el usuario.
        /// </param>
        /// <returns>
        /// Devuelve un objeto Usuario si las credenciales son correctas.
        /// Si no existe coincidencia, devuelve null.
        /// </returns>
        public Usuario ValidarLogin(string correo, string contraseña)
        {
            Usuario usuarioEncontrado = null;

            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    string query = @"
                        SELECT
                            u.id_usuario,
                            u.nombre,
                            u.apellido,
                            u.correo,
                            u.contraseña,
                            u.id_rol,
                            r.descripcion AS RolDescripcion
                        FROM Usuario u
                        INNER JOIN Rol r
                            ON u.id_rol = r.id_rol
                        WHERE u.correo = @correo
                          AND u.contraseña = @contraseña";

                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType = CommandType.Text;

                    // Se utilizan parámetros para evitar concatenar
                    // directamente los valores dentro de la consulta SQL.
                    cmd.Parameters.AddWithValue(
                        "@correo",
                        correo
                    );

                    cmd.Parameters.AddWithValue(
                        "@contraseña",
                        contraseña
                    );

                    oconexion.Open();

                    using (SqlDataReader dr =
                           cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            usuarioEncontrado =
                                new Usuario()
                                {
                                    IdUsuario =
                                        Convert.ToInt32(
                                            dr["id_usuario"]
                                        ),

                                    Nombre =
                                        dr["nombre"].ToString(),

                                    Apellido =
                                        dr["apellido"].ToString(),

                                    Correo =
                                        dr["correo"].ToString(),

                                    Contraseña =
                                        dr["contraseña"].ToString(),

                                    IdRol =
                                        Convert.ToInt32(
                                            dr["id_rol"]
                                        ),

                                    oRol =
                                        new Rol()
                                        {
                                            IdRol =
                                                Convert.ToInt32(
                                                    dr["id_rol"]
                                                ),

                                            Descripcion =
                                                dr[
                                                    "RolDescripcion"
                                                ].ToString()
                                        }
                                };
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return usuarioEncontrado;
        }
    }
}