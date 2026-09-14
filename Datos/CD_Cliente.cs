using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using System.Data.SqlClient;

namespace Datos
{
    /// <summary>
    /// Clase de Acceso a Datos correspondiente a los clientes.
    ///
    /// Su responsabilidad es comunicarse directamente con
    /// la tabla Cliente de SQL Server.
    ///
    /// Esta clase puede:
    /// - Listar clientes.
    /// - Buscar clientes por DNI o apellido.
    /// - Registrar clientes.
    /// - Editar clientes.
    ///
    /// No contiene controles de Windows Forms ni reglas
    /// de negocio. Las validaciones se realizarán en CN_Cliente.
    /// </summary>
    public class CD_Cliente
    {
        // =========================================================
        // LISTAR CLIENTES
        // =========================================================

        /// <summary>
        /// Obtiene todos los clientes registrados.
        /// </summary>
        /// <returns>
        /// Lista de objetos Cliente.
        /// </returns>
        public List<Cliente> Listar()
        {
            List<Cliente> lista = new List<Cliente>();

            using (SqlConnection conexion =
                new SqlConnection(Conexion.cadena))
            {
                string consulta = @"
                    SELECT
                        Id_cliente,
                        nombre,
                        apellido,
                        dni,
                        fecha_nacimiento,
                        fecha_registro,
                        domicilio,
                        correo
                    FROM Cliente
                    ORDER BY apellido, nombre;";

                SqlCommand comando =
                    new SqlCommand(consulta, conexion);

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(MapearCliente(lector));
                    }
                }
            }

            return lista;
        }


        // =========================================================
        // BUSCAR CLIENTES
        // =========================================================

        /// <summary>
        /// Busca clientes por DNI o por apellido.
        ///
        /// El mismo texto recibido puede coincidir con cualquiera
        /// de esos dos datos.
        /// </summary>
        /// <param name="texto">
        /// DNI o parte del apellido que se desea buscar.
        /// </param>
        /// <returns>
        /// Lista de clientes coincidentes.
        /// </returns>
        public List<Cliente> Buscar(string texto)
        {
            List<Cliente> lista = new List<Cliente>();

            using (SqlConnection conexion =
                new SqlConnection(Conexion.cadena))
            {
                string consulta = @"
                    SELECT
                        Id_cliente,
                        nombre,
                        apellido,
                        dni,
                        fecha_nacimiento,
                        fecha_registro,
                        domicilio,
                        correo
                    FROM Cliente
                    WHERE
                        apellido LIKE @texto
                        OR CONVERT(NVARCHAR(20), dni) LIKE @texto
                    ORDER BY apellido, nombre;";

                SqlCommand comando =
                    new SqlCommand(consulta, conexion);

                comando.Parameters.AddWithValue(
                    "@texto",
                    "%" + texto + "%"
                );

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(MapearCliente(lector));
                    }
                }
            }

            return lista;
        }


        // =========================================================
        // REGISTRAR CLIENTE
        // =========================================================

        /// <summary>
        /// Registra un nuevo cliente en SQL Server.
        ///
        /// La fecha de registro se genera automáticamente
        /// mediante GETDATE().
        /// </summary>
        /// <param name="cliente">
        /// Cliente que se desea registrar.
        /// </param>
        /// <returns>
        /// ID generado para el nuevo cliente.
        /// </returns>
        public int Registrar(Cliente cliente)
        {
            using (SqlConnection conexion =
                new SqlConnection(Conexion.cadena))
            {
                string consulta = @"
                    INSERT INTO Cliente
                    (
                        nombre,
                        apellido,
                        dni,
                        fecha_nacimiento,
                        fecha_registro,
                        domicilio,
                        correo
                    )
                    VALUES
                    (
                        @nombre,
                        @apellido,
                        @dni,
                        @fechaNacimiento,
                        GETDATE(),
                        @domicilio,
                        @correo
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                SqlCommand comando =
                    new SqlCommand(consulta, conexion);


                comando.Parameters.AddWithValue(
                    "@nombre",
                    (object)cliente.Nombre ?? DBNull.Value
                );


                comando.Parameters.AddWithValue(
                    "@apellido",
                    (object)cliente.Apellido ?? DBNull.Value
                );


                comando.Parameters.AddWithValue(
                    "@dni",
                    cliente.Dni
                );


                // Si no existe fecha de nacimiento,
                // se envía DBNull.Value a SQL Server.
                comando.Parameters.AddWithValue(
                    "@fechaNacimiento",
                    cliente.FechaNacimiento.HasValue
                        ? (object)cliente.FechaNacimiento.Value
                        : DBNull.Value
                );


                comando.Parameters.AddWithValue(
                    "@domicilio",
                    (object)cliente.Domicilio ?? DBNull.Value
                );


                comando.Parameters.AddWithValue(
                    "@correo",
                    (object)cliente.Correo ?? DBNull.Value
                );


                conexion.Open();


                object resultado =
                    comando.ExecuteScalar();


                return Convert.ToInt32(resultado);
            }
        }


        // =========================================================
        // EDITAR CLIENTE
        // =========================================================

        /// <summary>
        /// Modifica los datos de un cliente existente.
        ///
        /// No modifica la fecha de registro porque esa fecha
        /// representa el momento en que el cliente ingresó
        /// originalmente al sistema.
        /// </summary>
        /// <param name="cliente">
        /// Cliente con los nuevos datos.
        /// </param>
        /// <returns>
        /// true si SQL Server modificó el registro.
        /// </returns>
        public bool Editar(Cliente cliente)
        {
            using (SqlConnection conexion =
                new SqlConnection(Conexion.cadena))
            {
                string consulta = @"
                    UPDATE Cliente
                    SET
                        nombre = @nombre,
                        apellido = @apellido,
                        dni = @dni,
                        fecha_nacimiento = @fechaNacimiento,
                        domicilio = @domicilio,
                        correo = @correo
                    WHERE Id_cliente = @idCliente;";

                SqlCommand comando =
                    new SqlCommand(consulta, conexion);


                comando.Parameters.AddWithValue(
                    "@nombre",
                    (object)cliente.Nombre ?? DBNull.Value
                );


                comando.Parameters.AddWithValue(
                    "@apellido",
                    (object)cliente.Apellido ?? DBNull.Value
                );


                comando.Parameters.AddWithValue(
                    "@dni",
                    cliente.Dni
                );


                comando.Parameters.AddWithValue(
                    "@fechaNacimiento",
                    cliente.FechaNacimiento.HasValue
                        ? (object)cliente.FechaNacimiento.Value
                        : DBNull.Value
                );


                comando.Parameters.AddWithValue(
                    "@domicilio",
                    (object)cliente.Domicilio ?? DBNull.Value
                );


                comando.Parameters.AddWithValue(
                    "@correo",
                    (object)cliente.Correo ?? DBNull.Value
                );


                comando.Parameters.AddWithValue(
                    "@idCliente",
                    cliente.IdCliente
                );


                conexion.Open();


                int filasAfectadas =
                    comando.ExecuteNonQuery();


                return filasAfectadas > 0;
            }
        }


        // =========================================================
        // COMPROBAR DNI
        // =========================================================

        /// <summary>
        /// Comprueba si ya existe un cliente registrado
        /// con determinado DNI.
        ///
        /// El parámetro idClienteExcluir permite ignorar
        /// al propio cliente durante una edición.
        /// </summary>
        public bool ExisteDni(
            int dni,
            int idClienteExcluir = 0
        )
        {
            using (SqlConnection conexion =
                new SqlConnection(Conexion.cadena))
            {
                string consulta = @"
                    SELECT COUNT(*)
                    FROM Cliente
                    WHERE dni = @dni
                      AND Id_cliente <> @idClienteExcluir;";


                SqlCommand comando =
                    new SqlCommand(consulta, conexion);


                comando.Parameters.AddWithValue(
                    "@dni",
                    dni
                );


                comando.Parameters.AddWithValue(
                    "@idClienteExcluir",
                    idClienteExcluir
                );


                conexion.Open();


                int cantidad =
                    Convert.ToInt32(
                        comando.ExecuteScalar()
                    );


                return cantidad > 0;
            }
        }


        // =========================================================
        // MAPEAR CLIENTE
        // =========================================================

        /// <summary>
        /// Convierte una fila obtenida mediante SqlDataReader
        /// en un objeto Cliente.
        ///
        /// Este proceso se denomina mapeo:
        /// cada columna de SQL Server se asigna a una
        /// propiedad de la entidad Cliente.
        /// </summary>
        private Cliente MapearCliente(
            SqlDataReader lector
        )
        {
            Cliente cliente = new Cliente();


            cliente.IdCliente =
                Convert.ToInt32(
                    lector["Id_cliente"]
                );


            cliente.Nombre =
                lector["nombre"] == DBNull.Value
                    ? ""
                    : lector["nombre"].ToString();


            cliente.Apellido =
                lector["apellido"] == DBNull.Value
                    ? ""
                    : lector["apellido"].ToString();


            cliente.Dni =
                lector["dni"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(
                        lector["dni"]
                    );


            cliente.FechaNacimiento =
                lector["fecha_nacimiento"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(
                        lector["fecha_nacimiento"]
                    );


            cliente.FechaRegistro =
                lector["fecha_registro"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(
                        lector["fecha_registro"]
                    );


            cliente.Domicilio =
                lector["domicilio"] == DBNull.Value
                    ? ""
                    : lector["domicilio"].ToString();


            cliente.Correo =
                lector["correo"] == DBNull.Value
                    ? ""
                    : lector["correo"].ToString();


            return cliente;
        }
    }
}
