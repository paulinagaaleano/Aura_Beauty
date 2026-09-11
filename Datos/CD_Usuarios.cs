using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Entidades;


namespace Datos
{
    public class CD_Usuario
    {
        public List<Usuario> Listar()
        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    // Consulta con los nombres de columnas en minúscula exactos
                    string query = "select u.id_usuario, u.nombre, u.apellido, u.correo, u.contraseña, u.id_rol, r.descripcion as RolDescripcion from USUARIO u join ROL r on u.id_rol = r.id_rol";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Usuario()
                            {
                                IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                Nombre = dr["nombre"].ToString(),
                                Apellido = dr["apellido"].ToString(),
                                Correo = dr["correo"].ToString(),
                                Contraseña = dr["contraseña"].ToString(),
                                IdRol = Convert.ToInt32(dr["id_rol"]),
                                oRol = new Rol() { IdRol = Convert.ToInt32(dr["id_rol"]), Descripcion = dr["RolDescripcion"].ToString() }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Si prefieres ver el error por consola mientras pruebas, puedes usar: 
                    // Console.WriteLine(ex.Message);
                    lista = new List<Usuario>();
                }
            }

            return lista;
        }
    }
}