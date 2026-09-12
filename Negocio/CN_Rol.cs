using System.Collections.Generic;
using Datos;
using Entidades;

namespace Negocio
{
    /// <summary>
    /// Clase perteneciente a la Capa de Negocio.
    ///
    /// Su responsabilidad es trabajar con la información
    /// relacionada con los roles del sistema.
    ///
    /// Esta clase funciona como intermediaria entre:
    /// - la Capa de Presentación
    /// - y la Capa de Datos.
    ///
    /// La Presentación no accede directamente a SQL Server.
    /// </summary>
    public class CN_Rol
    {
        /// <summary>
        /// Objeto de la Capa de Datos utilizado
        /// para obtener la información de los roles.
        /// </summary>
        private CD_Rol obj_cd_rol = new CD_Rol();


        /// <summary>
        /// Obtiene la lista completa de roles registrados
        /// en la base de datos.
        ///
        /// Actualmente los roles son:
        /// 1 = Administrador
        /// 2 = Vendedor
        /// 3 = Repositor
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos Rol.
        /// </returns>
        public List<Rol> Listar()
        {
            /*
             * Llamamos al método Listar()
             * de la Capa de Datos.
             *
             * CD_Rol se encarga de consultar
             * la tabla Rol en SQL Server.
             */
            return obj_cd_rol.Listar();
        }
    }
}