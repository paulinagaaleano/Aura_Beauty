using System.Configuration;

namespace Datos
{
    public class Conexion
    {
        // Opción 1: Directa y segura para evitar cualquier fallo de lectura en capas
        public static string cadena = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AuraBeautyDB;Integrated Security=true";

        // O si prefieres leerla del App.config de forma segura:
        // public static string cadena = ConfigurationManager.ConnectionStrings["cadena_conexion"].ConnectionString;
    }
}