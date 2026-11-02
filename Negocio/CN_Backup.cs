
using System;
using System.Data;
using System.IO;
using Datos;
using Entidades;

namespace Negocio
{
    /// <summary>
    /// Capa de Negocio del módulo Backup.
    /// Valida las operaciones y coordina la Capa de Datos.
    /// </summary>
    public class CN_Backup
    {
        // Objeto que permite acceder a los métodos de la Capa Datos.
        private readonly CD_Backup objCD_Backup = new CD_Backup();

        // Objeto utilizado para validar las credenciales de usuarios.
        private readonly CN_Usuario objCN_Usuario = new CN_Usuario();

        // ID del rol Administrador en Aura Beauty.
        private const int ROL_ADMINISTRADOR = 1;

        /// <summary>
        /// Crea un respaldo real y registra el resultado.
        /// La ruta completa se recibe desde la interfaz.
        /// </summary>
        public int CrearBackup(
            int idUsuario,
            string rutaArchivo,
            string observaciones = null)
        {
            // Verificar que el usuario sea válido.
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "El usuario indicado no es válido.");
            }

            // Verificar que se haya seleccionado una ruta.
            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new ArgumentException(
                    "Debés seleccionar dónde guardar el respaldo.");
            }

            // Verificar que la ruta sea absoluta.
            if (!Path.IsPathRooted(rutaArchivo))
            {
                throw new ArgumentException(
                    "La ruta del respaldo debe ser absoluta.");
            }

            // Obtener el nombre del archivo desde la ruta.
            string nombreArchivo = Path.GetFileName(rutaArchivo);

            // Verificar que la extensión sea .bak.
            if (!string.Equals(
                Path.GetExtension(nombreArchivo),
                ".bak",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "El archivo debe tener extensión .bak.");
            }

            // Intentar crear el archivo físico de respaldo.
            try
            {
                // Llamar a la Capa Datos para ejecutar BACKUP DATABASE.
                objCD_Backup.CrearBackupReal(rutaArchivo);
            }
            catch (Exception ex)
            {
                // Si falla el respaldo, intentar registrar el fallo.
                try
                {
                    RegistrarBackup(
                        nombreArchivo,
                        rutaArchivo,
                        idUsuario,
                        "Fallido",
                        "Error al crear el respaldo: " + ex.Message);
                }
                catch
                {
                    // Si también falla el registro, conservar
                    // la excepción original del respaldo.
                }

                // Informar el error a la interfaz.
                throw new Exception(
                    "No se pudo crear el respaldo. " +
                    "Verificá la ruta y los permisos de SQL Server. " +
                    "Detalle: " + ex.Message,
                    ex);
            }

            // Registrar el respaldo como exitoso después de crearlo.
            // Si falla este registro, el archivo podría existir igualmente.
            return RegistrarBackup(
                nombreArchivo,
                rutaArchivo,
                idUsuario,
                "Exitoso",
                observaciones);
        }

        /// <summary>
        /// Registra el resultado de un respaldo en la base de datos.
        /// No crea el archivo físico.
        /// </summary>
        public int RegistrarBackup(
            string nombreArchivo,
            string rutaArchivo,
            int idUsuario,
            string estado,
            string observaciones)
        {
            // Validar el nombre del archivo.
            if (string.IsNullOrWhiteSpace(nombreArchivo))
            {
                throw new ArgumentException(
                    "El nombre del archivo es obligatorio.");
            }

            // Validar la ruta.
            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new ArgumentException(
                    "La ruta del archivo es obligatoria.");
            }

            // Validar el usuario.
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "El usuario indicado no es válido.");
            }

            // Comprobar que el estado sea uno de los permitidos.
            if (estado != "Exitoso" && estado != "Fallido")
            {
                throw new ArgumentException(
                    "El estado debe ser Exitoso o Fallido.");
            }

            // Enviar los datos a la Capa Datos.
            return objCD_Backup.RegistrarBackup(
                nombreArchivo.Trim(),
                rutaArchivo.Trim(),
                idUsuario,
                estado,
                observaciones);
        }

        /// <summary>
        /// Obtiene el historial de respaldos.
        /// </summary>
        public DataTable ListarBackups(int idUsuario)
        {
            // Validar el identificador del usuario.
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "Debe indicar un usuario válido.");
            }

            // Consultar los respaldos mediante la Capa Datos.
            return objCD_Backup.ListarBackups(idUsuario);
        }

        /// <summary>
        /// Registra una solicitud de restauración pendiente.
        /// </summary>
        public int SolicitarRestauracion(
            int idBackup,
            int idSolicitante,
            string observaciones)
        {
            // Validar el respaldo seleccionado.
            if (idBackup <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar un respaldo válido.");
            }

            // Validar el solicitante.
            if (idSolicitante <= 0)
            {
                throw new ArgumentException(
                    "El solicitante no es válido.");
            }

            // Registrar la solicitud en SQL Server.
            return objCD_Backup.SolicitarRestauracion(
                idBackup,
                idSolicitante,
                observaciones);
        }

        /// <summary>
        /// Autoriza una restauración mediante otro administrador.
        /// </summary>
        public DataTable AutorizarRestauracion(
            int idSolicitud,
            int idSolicitante,
            string correoAutorizador,
            string contraseñaAutorizador)
        {
            // Validar la solicitud.
            if (idSolicitud <= 0)
            {
                throw new ArgumentException(
                    "La solicitud seleccionada no es válida.");
            }

            // Validar el solicitante.
            if (idSolicitante <= 0)
            {
                throw new ArgumentException(
                    "El solicitante no es válido.");
            }

            // Validar las credenciales ingresadas.
            if (string.IsNullOrWhiteSpace(correoAutorizador) ||
                string.IsNullOrWhiteSpace(contraseñaAutorizador))
            {
                throw new ArgumentException(
                    "Ingresá el correo y la contraseña del administrador.");
            }

            // Autenticar al segundo administrador.
            Usuario autorizador = objCN_Usuario.ValidarLogin(
                correoAutorizador,
                contraseñaAutorizador);

            // Verificar que las credenciales sean correctas.
            if (autorizador == null)
            {
                throw new ArgumentException(
                    "Las credenciales del administrador no son correctas.");
            }

            // Comprobar que tenga el rol Administrador.
            if (autorizador.IdRol != ROL_ADMINISTRADOR)
            {
                throw new ArgumentException(
                    "Solo otro administrador puede autorizar la restauración.");
            }

            // Impedir que el solicitante se autorice a sí mismo.
            if (autorizador.IdUsuario == idSolicitante)
            {
                throw new ArgumentException(
                    "No puede autorizar su propia solicitud.");
            }

            // Registrar la autorización mediante SQL Server.
            // El procedimiento almacenado también verifica la identidad
            // del solicitante real de la solicitud.
            DataTable resultadoAutorizacion =
                objCD_Backup.AutorizarRestauracion(
                    idSolicitud,
                    autorizador.IdUsuario);

            try
            {
                // Obtener el archivo .bak asociado a la solicitud autorizada.
                string rutaBackup =
                    objCD_Backup.ObtenerRutaBackupAutorizado(idSolicitud);

                // Restaurar únicamente en la base de prueba.
                objCD_Backup.RestaurarBackupEnBasePrueba2(rutaBackup);
            }
            catch (Exception ex)
            {
                // Registrar el fallo de la restauración.
                try
                {
                    objCD_Backup.RegistrarResultadoRestauracion(
                        idSolicitud,
                        "Fallida",
                        ex.Message);
                }
                catch (Exception exRegistro)
                {
                    throw new Exception(
                        "La restauración falló y no se pudo registrar el resultado. " +
                        "Error de restauración: " + ex.Message +
                        ". Error de registro: " + exRegistro.Message,
                        exRegistro);
                }

                throw new Exception(
                    "La solicitud fue autorizada, pero la restauración de prueba falló: " +
                    ex.Message,
                    ex);
            }

            // Registrar que la restauración finalizó correctamente.
            objCD_Backup.RegistrarResultadoRestauracion(
                idSolicitud,
                "Exitosa",
                "Restauración de prueba completada correctamente.");

            // Devolver los datos de la autorización al formulario.
            return resultadoAutorizacion;
        }


        /// <summary>
        /// Valida el ID del usuario y solicita el historial a la capa de datos.
        /// </summary>
        public DataTable ListarHistorialRestauraciones(int idUsuario)
        {
            // Verificamos que se haya recibido un ID válido.
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "El ID del usuario no es válido.");
            }

            // Delegamos la consulta a la capa de datos.
            return objCD_Backup.ListarHistorialRestauraciones(idUsuario);
        }


        /// <summary>
        /// Obtiene las solicitudes de restauración pendientes.
        ///
        /// Comprueba que se haya recibido un ID de usuario válido
        /// y delega la consulta a la capa Datos.
        /// La validación del rol administrador también se realiza
        /// en el procedimiento almacenado de SQL Server.
        /// </summary>
        /// <param name="idUsuario">
        /// ID del administrador que consulta las solicitudes.
        /// </param>
        /// <returns>
        /// Una tabla con las solicitudes pendientes.
        /// </returns>
        public DataTable ListarSolicitudesPendientes(int idUsuario)
        {
            // Evitar consultar si no se recibió un ID válido.
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "El ID del usuario administrador no es válido.");
            }

            // Delegar la consulta a la capa Datos.
            return objCD_Backup.ListarSolicitudesPendientes(idUsuario);
        }


        /// <summary>
        /// Rechaza una solicitud mediante un administrador diferente.
        /// </summary>
        public DataTable RechazarRestauracion(
            int idSolicitud,
            int idSolicitante,
            string correoAutorizador,
            string contraseñaAutorizador,
            string motivoRechazo)
        {
            // Validar la solicitud.
            if (idSolicitud <= 0)
            {
                throw new ArgumentException(
                    "La solicitud seleccionada no es válida.");
            }

            // Validar el solicitante.
            if (idSolicitante <= 0)
            {
                throw new ArgumentException(
                    "El solicitante no es válido.");
            }

            // Exigir un motivo para el rechazo.
            if (string.IsNullOrWhiteSpace(motivoRechazo))
            {
                throw new ArgumentException(
                    "Debe indicar el motivo del rechazo.");
            }

            // Autenticar al administrador que resolverá la solicitud.
            Usuario autorizador = objCN_Usuario.ValidarLogin(
                correoAutorizador,
                contraseñaAutorizador);

            // Verificar las credenciales.
            if (autorizador == null)
            {
                throw new ArgumentException(
                    "Las credenciales del administrador no son correctas.");
            }

            // Comprobar el rol.
            if (autorizador.IdRol != ROL_ADMINISTRADOR)
            {
                throw new ArgumentException(
                    "Solo un administrador puede rechazar la solicitud.");
            }

            // Impedir que el solicitante resuelva su propia solicitud.
            if (autorizador.IdUsuario == idSolicitante)
            {
                throw new ArgumentException(
                    "El solicitante no puede rechazar su propia solicitud.");
            }

            // Enviar el rechazo a SQL Server.
            return objCD_Backup.RechazarRestauracion(
                idSolicitud,
                autorizador.IdUsuario,
                motivoRechazo.Trim());
        }

        /// <summary>
        /// Consulta el estado de una solicitud de restauración.
        /// </summary>
        public DataTable ConsultarEstadoRestauracion(
            int idSolicitud,
            int idUsuario)
        {
            // Validar la solicitud.
            if (idSolicitud <= 0)
            {
                throw new ArgumentException(
                    "La solicitud seleccionada no es válida.");
            }

            // Validar el usuario.
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "El usuario indicado no es válido.");
            }

            // Consultar el estado mediante la Capa Datos.
            return objCD_Backup.ConsultarEstadoRestauracion(
                idSolicitud,
                idUsuario);
        }

        /// <summary>
        /// Ejecuta una restauración de prueba en AuraBeautyDB_Prueba2.
        /// No reemplaza la base original AuraBeautyDB.
        /// </summary>
        /// <param name="rutaArchivo">Ruta completa del archivo .bak.</param>
        public void RestaurarBackupEnBasePrueba(string rutaArchivo)
        {
            // Validar que se haya indicado una ruta.
            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new ArgumentException(
                    "Debés indicar la ruta del archivo de backup.");
            }

            // Comprobar que la ruta sea absoluta.
            if (!Path.IsPathRooted(rutaArchivo))
            {
                throw new ArgumentException(
                    "La ruta del backup debe ser absoluta.");
            }

            // Verificar que el archivo exista.
            if (!File.Exists(rutaArchivo))
            {
                throw new FileNotFoundException(
                    "No se encontró el archivo de backup.",
                    rutaArchivo);
            }

            // Verificar que sea un archivo .bak.
            if (!string.Equals(
                Path.GetExtension(rutaArchivo),
                ".bak",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "El archivo seleccionado debe tener extensión .bak.");
            }

            // Delegar la restauración a la capa de Datos.
            objCD_Backup.RestaurarBackupEnBasePrueba2(rutaArchivo);
        }

    }
}
