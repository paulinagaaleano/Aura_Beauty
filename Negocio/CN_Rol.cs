using Datos;
using Entidades;
using System.Collections.Generic;

namespace Negocio
{
    /// <summary>
    /// Clase perteneciente a la Capa de Negocio.
    /// Se encarga de gestionar las operaciones relacionadas
    /// con los roles del sistema.
    ///
    /// Actúa como intermediaria entre la Capa de Presentación
    /// y la Capa de Datos.
    /// </summary>
    public class CN_Rol
    {
        // Se crea una instancia de CD_Rol para poder utilizar
        // las operaciones de acceso a datos relacionadas con roles.
        private CD_Rol obj_cd_rol = new CD_Rol();

        /// <summary>
        /// Solicita a la Capa de Datos la lista de roles
        /// registrados en la base de datos.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos Rol.
        /// </returns>
        public List<Rol> Listar()
        {
            // CN_Rol no realiza consultas SQL.
            // Delega esa responsabilidad a CD_Rol.
            return obj_cd_rol.Listar();
        }
    }
}