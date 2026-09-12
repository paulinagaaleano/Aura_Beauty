namespace Entidades
{
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
    }
}
