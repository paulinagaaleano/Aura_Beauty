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