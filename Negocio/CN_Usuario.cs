using System;
using System.Collections.Generic;
using Datos;
using Entidades;

namespace Negocio
{
    /// <summary>
    /// Clase perteneciente a la Capa de Negocio.
    /// 
    /// Su responsabilidad es aplicar las reglas y validaciones
    /// relacionadas con los usuarios antes de acceder a la base de datos.
    /// 
    /// Esta clase se comunica con la Capa de Datos mediante CD_Usuario.
    /// La Presentación NO debería consultar directamente la base de datos.
    /// </summary>
    public class CN_Usuario
    {
        // Creamos un objeto de la Capa de Datos.
        //
        // CD_Usuario es la clase encargada de acceder
        // a la tabla Usuario de la base de datos.
        //
        // La palabra "private" significa que este objeto
        // solamente puede ser utilizado dentro de CN_Usuario.
        private CD_Usuario obj_cd_usuario = new CD_Usuario();


        /// <summary>
        /// Obtiene la lista completa de usuarios.
        /// 
        /// Este método funciona como intermediario entre
        /// la Presentación y la Capa de Datos.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos Usuario.
        /// </returns>
        public List<Usuario> Listar()
        {
            // Llamamos al método Listar() de la Capa de Datos
            // y devolvemos directamente el resultado.
            return obj_cd_usuario.Listar();
        }

        /// <summary>
        /// Registra un nuevo usuario.
        ///
        /// Antes de acceder a la base de datos,
        /// valida toda la información recibida.
        /// </summary>
        /// <param name="usuario">
        /// Usuario que se desea registrar.
        /// </param>
        /// <returns>
        /// Devuelve el id generado para el nuevo usuario.
        /// </returns>
        public int Registrar(Usuario usuario)
        {
            /*
             * Primero aplicamos todas las validaciones
             * comunes de los datos del usuario.
             */
            ValidarDatosUsuario(usuario);


            /*
             * Trim()
             *
             * Elimina espacios al principio
             * y al final del texto.
             *
             * Normalizamos los datos antes de guardarlos.
             */
            usuario.Nombre =
                usuario.Nombre.Trim();

            usuario.Apellido =
                usuario.Apellido.Trim();

            usuario.Correo =
                usuario.Correo.Trim();


            /*
             * Antes de registrar comprobamos
             * que el correo no esté utilizado
             * por otro usuario.
             */
            if (obj_cd_usuario.ExisteCorreo(
                usuario.Correo))
            {
                throw new ArgumentException(
                    "Ya existe un usuario registrado con ese correo electrónico."
                );
            }


            /*
             * Si todas las validaciones fueron superadas,
             * enviamos el usuario a la Capa de Datos.
             */
            return obj_cd_usuario.Registrar(
                usuario
            );
        }



        /// <summary>
        /// Modifica un usuario existente.
        /// </summary>
        /// <param name="usuario">
        /// Usuario con la información modificada.
        /// </param>
        /// <returns>
        /// true si la modificación fue realizada.
        /// </returns>
        public bool Editar(Usuario usuario)
        {
            /*
             * Para editar necesitamos saber
             * qué usuario será modificado.
             */
            if (usuario == null)
            {
                throw new ArgumentException(
                    "Debe indicar un usuario para editar."
                );
            }


            if (usuario.IdUsuario <= 0)
            {
                throw new ArgumentException(
                    "El usuario seleccionado no es válido."
                );
            }


            // Aplicamos las mismas reglas generales
            // utilizadas al registrar.
            ValidarDatosUsuario(usuario);


            usuario.Nombre =
                usuario.Nombre.Trim();

            usuario.Apellido =
                usuario.Apellido.Trim();

            usuario.Correo =
                usuario.Correo.Trim();


            /*
             * Al editar debemos comprobar que el correo
             * no pertenezca a OTRO usuario.
             *
             * Por eso enviamos usuario.IdUsuario
             * para excluir al usuario que estamos editando.
             *
             * Ejemplo:
             *
             * Silvina tiene:
             * silvina@gmail.com
             *
             * Si editamos solamente su apellido,
             * el sistema no debe interpretar
             * su propio correo como duplicado.
             */
            if (obj_cd_usuario.ExisteCorreo(
                usuario.Correo,
                usuario.IdUsuario))
            {
                throw new ArgumentException(
                    "Ya existe otro usuario registrado con ese correo electrónico."
                );
            }


            return obj_cd_usuario.Editar(
                usuario
            );
        }



        /// <summary>
        /// Realiza la baja lógica de un usuario.
        ///
        /// Antes de solicitar la baja a la Capa de Datos,
        /// verifica que la operación no deje al sistema
        /// sin administradores activos.
        /// </summary>
        /// <param name="idUsuario">
        /// Identificador del usuario que se desea dar de baja.
        /// </param>
        /// <returns>
        /// true si la baja lógica fue realizada correctamente.
        /// </returns>
        public bool Eliminar(int idUsuario)
        {
            /*
             * Primero comprobamos que el identificador
             * recibido sea válido.
             */
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar un usuario válido para dar de baja."
                );
            }


            /*
             * Consultamos si el usuario seleccionado
             * es un Administrador activo.
             */
            bool esAdministrador =
                obj_cd_usuario.EsAdministradorActivo(
                    idUsuario
                );


            /*
             * Esta regla solamente debe comprobarse
             * cuando el usuario que se quiere dar
             * de baja es Administrador.
             */
            if (esAdministrador)
            {
                int cantidadAdministradores =
                    obj_cd_usuario.ContarAdministradoresActivos();


                /*
                 * Si solamente queda un administrador,
                 * no permitimos su baja.
                 *
                 * De esta manera garantizamos que
                 * el sistema siempre conserve al menos
                 * un Administrador activo.
                 */
                if (cantidadAdministradores <= 1)
                {
                    throw new ArgumentException(
                        "No es posible dar de baja al último administrador activo del sistema."
                    );
                }
            }


            /*
             * Si todas las reglas se cumplen,
             * solicitamos a la Capa de Datos
             * que realice la baja lógica.
             */
            return obj_cd_usuario.Eliminar(
                idUsuario
            );
        }



        /// <summary>
        /// Valida los datos generales de un usuario.
        ///
        /// Este método es privado porque únicamente
        /// se utiliza dentro de CN_Usuario.
        /// </summary>
        /// <param name="usuario">
        /// Usuario cuyos datos serán comprobados.
        /// </param>
        private void ValidarDatosUsuario(
            Usuario usuario)
        {
            /*
             * null
             *
             * Significa que el objeto no existe
             * o no contiene una referencia válida.
             */
            if (usuario == null)
            {
                throw new ArgumentException(
                    "Los datos del usuario son obligatorios."
                );
            }


            /*
             * string.IsNullOrWhiteSpace(...)
             *
             * Devuelve true cuando un texto:
             *
             * - es null,
             * - está vacío,
             * - o contiene únicamente espacios.
             */
            if (string.IsNullOrWhiteSpace(
                usuario.Nombre))
            {
                throw new ArgumentException(
                    "El nombre es obligatorio."
                );
            }


            if (string.IsNullOrWhiteSpace(
                usuario.Apellido))
            {
                throw new ArgumentException(
                    "El apellido es obligatorio."
                );
            }


            if (string.IsNullOrWhiteSpace(
                usuario.Correo))
            {
                throw new ArgumentException(
                    "El correo electrónico es obligatorio."
                );
            }


            /*
             * Validación básica del formato del correo.
             */
            string correo =
                usuario.Correo.Trim();


            if (!correo.Contains("@") ||
                !correo.Contains("."))
            {
                throw new ArgumentException(
                    "El formato del correo electrónico no es válido."
                );
            }


            if (string.IsNullOrWhiteSpace(
                usuario.Contraseña))
            {
                throw new ArgumentException(
                    "La contraseña es obligatoria."
                );
            }


            /*
             * El rol debe tener un identificador válido.
             *
             * Actualmente tenemos:
             *
             * 1 = Administrador
             * 2 = Vendedor
             * 3 = Repositor
             */
            if (usuario.IdRol <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar un rol para el usuario."
                );
            }
        }


        /// <summary>
        /// Valida los datos ingresados para iniciar sesión.
        /// 
        /// Primero verifica que los campos sean válidos.
        /// Si las validaciones se cumplen, consulta la Capa de Datos
        /// para comprobar si existe un usuario con esas credenciales.
        /// </summary>
        /// <param name="correo">
        /// Correo electrónico ingresado por el usuario.
        /// </param>
        /// <param name="contraseña">
        /// Contraseña ingresada por el usuario.
        /// </param>
        /// <returns>
        /// Devuelve un objeto Usuario si las credenciales son correctas.
        /// Devuelve null si no existe un usuario con esos datos.
        /// </returns>
        public Usuario ValidarLogin(string correo, string contraseña)
        {
            /*
             * string.IsNullOrWhiteSpace(...)
             *
             * Es un método predefinido de C#.
             *
             * Devuelve true cuando el texto:
             * - es null,
             * - está vacío "",
             * - o contiene solamente espacios.
             *
             * Ejemplos:
             *
             * ""       → true
             * "   "    → true
             * null     → true
             * "Silvina" → false
             */

            if (string.IsNullOrWhiteSpace(correo))
            {
                /*
                 * throw new ArgumentException(...)
                 *
                 * "throw" significa lanzar una excepción.
                 *
                 * Una excepción representa una situación
                 * que impide continuar normalmente.
                 *
                 * ArgumentException se utiliza cuando
                 * un argumento recibido por un método
                 * no cumple con las condiciones esperadas.
                 */
                throw new ArgumentException(
                    "El correo electrónico es obligatorio."
                );
            }


            if (string.IsNullOrWhiteSpace(contraseña))
            {
                throw new ArgumentException(
                    "La contraseña es obligatoria."
                );
            }


            /*
             * Trim()
             *
             * Es un método predefinido de string.
             *
             * Elimina los espacios que puedan existir
             * al principio y al final del texto.
             *
             * Ejemplo:
             *
             * "  usuario@gmail.com  "
             *
             * se transforma en:
             *
             * "usuario@gmail.com"
             */
            correo = correo.Trim();


            /*
             * Validamos de manera básica el formato del correo.
             *
             * Para este proyecto no necesitamos una validación
             * extremadamente compleja.
             *
             * Comprobamos que exista:
             * - un símbolo @
             * - y un punto .
             */
            if (!correo.Contains("@") || !correo.Contains("."))
            {
                /*
                 * El símbolo ! significa negación.
                 *
                 * Ejemplo:
                 *
                 * correo.Contains("@")
                 *
                 * puede devolver true o false.
                 *
                 * Si devuelve false:
                 *
                 * !false
                 *
                 * se transforma en true.
                 */
                throw new ArgumentException(
                    "El formato del correo electrónico no es válido."
                );
            }


            /*
             * No usamos Trim() en la contraseña.
             *
             * Esto es intencional.
             *
             * Una contraseña podría contener espacios
             * como parte válida de la misma.
             *
             * Si elimináramos esos espacios, estaríamos
             * modificando el dato ingresado por el usuario.
             */


            /*
             * Una vez superadas las validaciones,
             * llamamos a la Capa de Datos.
             *
             * CD_Usuario.ValidarLogin(...)
             * consulta la base de datos.
             *
             * Puede devolver:
             *
             * - un objeto Usuario → credenciales correctas
             * - null → no existe coincidencia
             */
            Usuario usuario =
                obj_cd_usuario.ValidarLogin(
                    correo,
                    contraseña
                );


            /*
             * return
             *
             * Finaliza el método y devuelve un resultado.
             *
             * En este caso devuelve el objeto Usuario
             * obtenido desde la Capa de Datos.
             */
            return usuario;
        }
    }
}