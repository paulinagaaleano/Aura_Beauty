// Importa las clases de la capa Datos para poder acceder a CD_Usuario.
using Datos;

// Importa las entidades del dominio, por ejemplo Usuario y Rol.
using Entidades;
using System.Collections.Generic;

namespace Negocio
{
    /// <summary>
    /// Clase de la capa de Negocio encargada de gestionar
    /// las reglas y operaciones relacionadas con los usuarios.
    /// 
    /// Esta clase actúa como intermediaria entre la capa de Presentación
    /// y la capa de Datos.
    /// </summary>
    public class CN_Usuario
    {
        // Instancia de la clase de acceso a datos de Usuario.
        // Se utiliza para consultar, registrar o modificar información
        // almacenada en la base de datos.
        private CD_Usuario obj_cd_usuario = new CD_Usuario();

        /// <summary>
        /// Obtiene la lista completa de usuarios.
        /// 
        /// La capa de Negocio solicita los datos a CD_Usuario
        /// y devuelve una colección de objetos Usuario.
        /// </summary>
        /// <returns>Lista de usuarios registrados.</returns>
        public List<Usuario> Listar()
        {
            return obj_cd_usuario.Listar();
        }
    }
}