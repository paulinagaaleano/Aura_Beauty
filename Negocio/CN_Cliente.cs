using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;
using Entidades;
using System.Net.Mail;

namespace Negocio
{
    /// <summary>
    /// Clase de la capa de Negocio encargada de gestionar
    /// las reglas relacionadas con los clientes.
    ///
    /// Esta clase funciona como intermediaria entre
    /// la Presentación y la capa de Datos.
    ///
    /// Aquí se validan:
    /// - DNI.
    /// - Nombre.
    /// - Apellido.
    /// - Domicilio.
    /// - Correo electrónico.
    /// - Fecha de nacimiento.
    /// - Duplicación de DNI.
    ///
    /// No contiene consultas SQL ni controles de Windows Forms.
    /// </summary>
    public class CN_Cliente
    {
        /// <summary>
        /// Objeto de la capa de Datos utilizado para acceder
        /// a la tabla Cliente.
        /// </summary>
        private readonly CD_Cliente cdCliente =
            new CD_Cliente();


        // =========================================================
        // LISTAR
        // =========================================================

        /// <summary>
        /// Obtiene todos los clientes registrados.
        /// </summary>
        public List<Cliente> Listar()
        {
            return cdCliente.Listar();
        }


        // =========================================================
        // BUSCAR
        // =========================================================

        /// <summary>
        /// Busca clientes por DNI o apellido.
        ///
        /// Si el texto está vacío, devuelve todos los clientes.
        /// </summary>
        public List<Cliente> Buscar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return cdCliente.Listar();
            }

            return cdCliente.Buscar(
                texto.Trim()
            );
        }


        // =========================================================
        // REGISTRAR
        // =========================================================

        /// <summary>
        /// Registra un nuevo cliente después de comprobar
        /// todas las reglas de negocio.
        /// </summary>
        /// <param name="cliente">
        /// Cliente que se desea registrar.
        /// </param>
        /// <returns>
        /// ID generado para el nuevo cliente.
        /// </returns>
        public int Registrar(Cliente cliente)
        {
            ValidarCliente(cliente);


            // No permitimos registrar dos clientes
            // con el mismo DNI.
            if (
                cdCliente.ExisteDni(
                    cliente.Dni
                )
            )
            {
                throw new Exception(
                    "Ya existe un cliente registrado con ese DNI."
                );
            }


            // Trim elimina espacios innecesarios
            // al principio y al final del texto.
            PrepararTextos(cliente);


            return cdCliente.Registrar(
                cliente
            );
        }


        // =========================================================
        // EDITAR
        // =========================================================

        /// <summary>
        /// Modifica un cliente existente después de validar
        /// todos sus datos.
        /// </summary>
        public bool Editar(Cliente cliente)
        {
            if (cliente == null)
            {
                throw new Exception(
                    "Los datos del cliente no son válidos."
                );
            }


            if (cliente.IdCliente <= 0)
            {
                throw new Exception(
                    "Seleccioná un cliente válido para editar."
                );
            }


            ValidarCliente(cliente);


            // Durante una edición necesitamos comprobar
            // si el DNI pertenece a OTRO cliente.
            //
            // Por eso enviamos el propio IdCliente para
            // excluirlo de la búsqueda.
            if (
                cdCliente.ExisteDni(
                    cliente.Dni,
                    cliente.IdCliente
                )
            )
            {
                throw new Exception(
                    "Ya existe otro cliente registrado con ese DNI."
                );
            }


            PrepararTextos(cliente);


            return cdCliente.Editar(
                cliente
            );
        }


        // =========================================================
        // VALIDACIÓN GENERAL
        // =========================================================

        /// <summary>
        /// Comprueba las reglas de negocio obligatorias
        /// de un cliente.
        /// </summary>
        private void ValidarCliente(
            Cliente cliente
        )
        {
            if (cliente == null)
            {
                throw new Exception(
                    "Los datos del cliente no son válidos."
                );
            }


            // -----------------------------------------------------
            // DNI
            // -----------------------------------------------------

            if (cliente.Dni <= 0)
            {
                throw new Exception(
                    "El DNI es obligatorio y debe ser válido."
                );
            }


            // Un DNI argentino generalmente posee
            // entre 7 y 8 cifras.
            //
            // Utilizamos esta regla para evitar valores
            // claramente incorrectos.
            string dniTexto =
                cliente.Dni.ToString();


            if (
                dniTexto.Length < 7
                ||
                dniTexto.Length > 8
            )
            {
                throw new Exception(
                    "El DNI debe contener entre 7 y 8 dígitos."
                );
            }


            // -----------------------------------------------------
            // NOMBRE
            // -----------------------------------------------------

            if (
                string.IsNullOrWhiteSpace(
                    cliente.Nombre
                )
            )
            {
                throw new Exception(
                    "El nombre del cliente es obligatorio."
                );
            }


            if (
                cliente.Nombre.Trim().Length > 100
            )
            {
                throw new Exception(
                    "El nombre no puede superar los 100 caracteres."
                );
            }


            // -----------------------------------------------------
            // APELLIDO
            // -----------------------------------------------------

            if (
                string.IsNullOrWhiteSpace(
                    cliente.Apellido
                )
            )
            {
                throw new Exception(
                    "El apellido del cliente es obligatorio."
                );
            }


            if (
                cliente.Apellido.Trim().Length > 100
            )
            {
                throw new Exception(
                    "El apellido no puede superar los 100 caracteres."
                );
            }


            // -----------------------------------------------------
            // DOMICILIO
            // -----------------------------------------------------

            if (
                !string.IsNullOrWhiteSpace(
                    cliente.Domicilio
                )
                &&
                cliente.Domicilio.Trim().Length > 255
            )
            {
                throw new Exception(
                    "El domicilio no puede superar los 255 caracteres."
                );
            }


            // -----------------------------------------------------
            // CORREO
            // -----------------------------------------------------

            if (
                !string.IsNullOrWhiteSpace(
                    cliente.Correo
                )
            )
            {
                if (
                    cliente.Correo.Trim().Length > 100
                )
                {
                    throw new Exception(
                        "El correo no puede superar los 100 caracteres."
                    );
                }


                if (
                    !EsCorreoValido(
                        cliente.Correo.Trim()
                    )
                )
                {
                    throw new Exception(
                        "El formato del correo electrónico no es válido."
                    );
                }
            }


            // -----------------------------------------------------
            // FECHA DE NACIMIENTO
            // -----------------------------------------------------

            if (
                cliente.FechaNacimiento.HasValue
            )
            {
                DateTime fecha =
                    cliente.FechaNacimiento.Value.Date;


                if (
                    fecha > DateTime.Today
                )
                {
                    throw new Exception(
                        "La fecha de nacimiento no puede ser futura."
                    );
                }
            }
        }


        // =========================================================
        // VALIDAR CORREO
        // =========================================================

        /// <summary>
        /// Comprueba si una dirección de correo posee
        /// una estructura válida.
        ///
        /// MailAddress pertenece a .NET.
        /// Intentamos crear un objeto MailAddress con el texto.
        ///
        /// Si .NET puede interpretarlo, devuelve true.
        /// Si genera una excepción, devuelve false.
        /// </summary>
        private bool EsCorreoValido(
            string correo
        )
        {
            try
            {
                MailAddress direccion =
                    new MailAddress(correo);


                return
                    direccion.Address ==
                    correo;
            }
            catch
            {
                return false;
            }
        }


        // =========================================================
        // PREPARAR TEXTOS
        // =========================================================

        /// <summary>
        /// Elimina espacios innecesarios antes de enviar
        /// los datos a SQL Server.
        ///
        /// Por ejemplo:
        ///
        /// "  Ana  "
        ///
        /// se transforma en:
        ///
        /// "Ana"
        /// </summary>
        private void PrepararTextos(
            Cliente cliente
        )
        {
            cliente.Nombre =
                cliente.Nombre.Trim();


            cliente.Apellido =
                cliente.Apellido.Trim();


            cliente.Domicilio =
                string.IsNullOrWhiteSpace(
                    cliente.Domicilio
                )
                    ? ""
                    : cliente.Domicilio.Trim();


            cliente.Correo =
                string.IsNullOrWhiteSpace(
                    cliente.Correo
                )
                    ? ""
                    : cliente.Correo.Trim();
        }
    }
}
