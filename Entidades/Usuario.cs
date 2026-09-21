using System;

namespace Entidades
{
    /// <summary>
    /// Representa a un usuario del sistema Aura Beauty.
    ///
    /// Esta clase pertenece a la Capa de Entidades.
    ///
    /// Su función es transportar la información de un usuario
    /// entre las distintas capas del sistema.
    ///
    /// No contiene consultas SQL,
    /// no contiene reglas de negocio
    /// y no contiene elementos visuales.
    /// </summary>
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public int IdRol { get; set; }

        // Propiedad extra para asociar el objeto Rol completo cuando lo necesites consultar
        public Rol oRol { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
