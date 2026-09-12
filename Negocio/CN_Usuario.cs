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