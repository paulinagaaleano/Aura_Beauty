using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Datos
{
    /// <summary>
    /// Clase perteneciente a la Capa de Datos.
    /// Se encarga de realizar las operaciones relacionadas
    /// con los roles almacenados en la base de datos.
    /// </summary>
    public class CD_Rol
    {
        /// <summary>
        /// Obtiene todos los roles registrados en la tabla Rol.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos Rol.
        /// Cada objeto contiene el identificador y la descripción del rol.
        /// </returns>
        public List<Rol> Listar()
        {
            // Se crea una lista vacía donde se almacenarán
            // los roles recuperados desde SQL Server.
            List<Rol> lista = new List<Rol>();

            // Se crea la conexión utilizando la cadena definida
            // en la clase Conexion.
            // "using" garantiza que la conexión se cierre y libere
            // correctamente al finalizar la operación.
            using (SqlConnection oconexion =
                   new SqlConnection(Conexion.cadena))
            {
                try
                {
                    // Consulta SQL para obtener todos los roles.
                    // Los nombres deben coincidir exactamente
                    // con las columnas existentes en dbo.Rol.
                    string query =
                        "SELECT id_rol, descripcion FROM Rol";

                    // Se prepara el comando SQL que se ejecutará
                    // utilizando la conexión creada anteriormente.
                    SqlCommand cmd =
                        new SqlCommand(query, oconexion);

                    cmd.CommandType = CommandType.Text;

                    // Se abre la conexión con SQL Server.
                    oconexion.Open();

                    // ExecuteReader se utiliza porque SELECT puede
                    // devolver uno o varios registros.
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // Se recorren uno por uno los registros obtenidos.
                        while (dr.Read())
                        {
                            // Cada fila obtenida desde SQL Server
                            // se transforma en un objeto Rol.
                            Rol rol = new Rol()
                            {
                                IdRol =
                                    Convert.ToInt32(dr["id_rol"]),

                                Descripcion =
                                    dr["descripcion"].ToString()
                            };

                            // Se agrega el objeto a la lista.
                            lista.Add(rol);
                        }
                    }
                }
                catch (Exception)
                {
                    // Si ocurre un error de conexión o de SQL,
                    // se vuelve a lanzar la excepción para que pueda
                    // ser tratada por una capa superior.
                    throw;
                }
            }

            // Se devuelve la lista completa de roles.
            return lista;
        }
    }
}