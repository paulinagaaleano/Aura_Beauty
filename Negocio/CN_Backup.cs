
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
        /// Autoriza una solicitud de restauración desde
        /// la cuenta del administrador que inició sesión.
        ///
        /// Requiere ingresar nuevamente su contraseña
        /// para confirmar esta operación sensible.
        ///
        /// Reglas:
        /// - La solicitud debe ser válida.
        /// - El administrador de sesión debe ser distinto
        ///   del solicitante.
        /// - La contraseña debe corresponder al
        ///   administrador que inició sesión.
        /// - SQL Server comprueba que la solicitud
        ///   todavía esté pendiente.
        ///
        /// Se conserva la restauración de prueba
        /// implementada actualmente en Aura Beauty.
        /// </summary>
        public DataTable AutorizarRestauracion(
            int idSolicitud,
            int idSolicitante,
            int idAdministradorSesion,
            string contraseñaAdministrador)
        {
            // ============================================
            // 1. VALIDAR LOS IDENTIFICADORES
            // ============================================

            if (idSolicitud <= 0)
            {
                throw new ArgumentException(
                    "La solicitud seleccionada no es válida.");
            }

            if (idSolicitante <= 0 ||
                idAdministradorSesion <= 0)
            {
                throw new ArgumentException(
                    "No se pudo identificar a los administradores.");
            }

            // ============================================
            // 2. IMPEDIR LA AUTOAUTORIZACIÓN
            // ============================================

            if (idSolicitante == idAdministradorSesion)
            {
                throw new InvalidOperationException(
                    "No podés autorizar tu propia solicitud.");
            }

            // ============================================
            // 3. VALIDAR CONTRASEÑA
            // ============================================

            if (string.IsNullOrEmpty(
                contraseñaAdministrador))
            {
                throw new ArgumentException(
                    "Ingresá tu contraseña para autorizar.");
            }

            if (contraseñaAdministrador.Length > 100)
            {
                throw new ArgumentException(
                    "La contraseña supera la longitud permitida.");
            }

            // ============================================
            // 4. AUTENTICAR AL ADMINISTRADOR DE SESIÓN
            // ============================================

            bool claveValida =
                objCD_Backup.ValidarClaveSesion(
                    idAdministradorSesion,
                    contraseñaAdministrador);

            if (!claveValida)
            {
                throw new InvalidOperationException(
                    "No se pudo verificar la contraseña.");
            }

            // ============================================
            // 5. REGISTRAR LA AUTORIZACIÓN
            // ============================================

            /*
             * El autorizador es siempre quien inició
             * sesión en Aura Beauty.
             *
             * No buscamos ni utilizamos otra identidad.
             *
             * SP_Restore_Autorizar verifica nuevamente
             * el estado Pendiente y que el solicitante
             * real no sea este administrador.
             */
            DataTable resultadoAutorizacion =
                objCD_Backup.AutorizarRestauracion(
                    idSolicitud,
                    idAdministradorSesion);

            // ============================================
            // 6. RESTAURACIÓN DE PRUEBA ACTUAL
            // ============================================

            /*
             * Por ahora se mantiene el método original.
             *
             * El cambio para restaurar AuraBeautyDB
             * sobre su nombre original será el paso
             * siguiente, después del resguardo previo.
             */
            try
            {
                string rutaBackup =
                    objCD_Backup.ObtenerRutaBackupAutorizado(
                        idSolicitud);

                objCD_Backup.RestaurarBackupEnBasePrueba2(
                    rutaBackup);
            }
            catch (Exception ex)
            {
                // Registrar que la restauración falló.
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
                        "La restauración falló y tampoco " +
                        "se pudo registrar el resultado. " +
                        "Error de restauración: " +
                        ex.Message +
                        ". Error de registro: " +
                        exRegistro.Message,
                        exRegistro);
                }

                throw new Exception(
                    "La solicitud fue autorizada, " +
                    "pero la restauración de prueba falló: " +
                    ex.Message,
                    ex);
            }

            // ============================================
            // 7. REGISTRAR EL RESULTADO
            // ============================================

            objCD_Backup.RegistrarResultadoRestauracion(
                idSolicitud,
                "Exitosa",
                "Restauración de prueba completada correctamente.");

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
        /// Rechaza una solicitud desde la cuenta del
        /// administrador que tiene iniciada la sesión.
        ///
        /// Requiere su propia contraseña y un motivo
        /// obligatorio de rechazo.
        ///
        /// No permite rechazar solicitudes propias.
        /// </summary>
        public DataTable RechazarRestauracion(
            int idSolicitud,
            int idSolicitante,
            int idAdministradorSesion,
            string contraseñaAdministrador,
            string motivoRechazo)
        {
            // ============================================
            // 1. IDENTIFICADORES
            // ============================================

            if (idSolicitud <= 0)
            {
                throw new ArgumentException(
                    "La solicitud seleccionada no es válida.");
            }

            if (idSolicitante <= 0 ||
                idAdministradorSesion <= 0)
            {
                throw new ArgumentException(
                    "No se pudo identificar a los administradores.");
            }

            // ============================================
            // 2. IMPEDIR EL RECHAZO PROPIO
            // ============================================

            if (idSolicitante == idAdministradorSesion)
            {
                throw new InvalidOperationException(
                    "No podés rechazar tu propia solicitud.");
            }

            // ============================================
            // 3. MOTIVO OBLIGATORIO
            // ============================================

            if (string.IsNullOrWhiteSpace(motivoRechazo))
            {
                throw new ArgumentException(
                    "Debés indicar el motivo del rechazo.");
            }

            if (motivoRechazo.Trim().Length > 1000)
            {
                throw new ArgumentException(
                    "El motivo del rechazo supera " +
                    "los 1000 caracteres permitidos.");
            }

            // ============================================
            // 4. CONTRASEÑA DEL ADMINISTRADOR EN SESIÓN
            // ============================================

            if (string.IsNullOrEmpty(
                contraseñaAdministrador))
            {
                throw new ArgumentException(
                    "Ingresá tu contraseña para rechazar.");
            }

            if (contraseñaAdministrador.Length > 100)
            {
                throw new ArgumentException(
                    "La contraseña supera la longitud permitida.");
            }

            bool claveValida =
                objCD_Backup.ValidarClaveSesion(
                    idAdministradorSesion,
                    contraseñaAdministrador);

            if (!claveValida)
            {
                throw new InvalidOperationException(
                    "No se pudo verificar la contraseña.");
            }

            // ============================================
            // 5. REGISTRAR EL RECHAZO
            // ============================================

            /*
             * Se registra como responsable al
             * administrador que inició sesión.
             *
             * SP_Restore_Rechazar comprueba que
             * la solicitud exista, esté Pendiente
             * y no haya sido creada por él.
             */
            return objCD_Backup.RechazarRestauracion(
                idSolicitud,
                idAdministradorSesion,
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



        /// <summary>
        /// Autoriza una restauración y recupera AuraBeautyDB
        /// sobre su nombre original.
        ///
        /// Orden de seguridad:
        /// 1. Validar administrador y contraseña.
        /// 2. Comprobar que la solicitud esté pendiente.
        /// 3. Verificar el archivo de respaldo.
        /// 4. Crear y verificar el respaldo preventivo.
        /// 5. Registrar la autorización.
        /// 6. Ejecutar la restauración definitiva.
        ///
        /// El historial de recuperación se registra
        /// en AuraBeautyDB_Admin.
        /// </summary>
        public string AutorizarYRestaurarOriginal(
            int idSolicitud,
            int idSolicitante,
            int idAdministradorSesion,
            string contraseña)
        {
            // ===========================================
            // 1. VALIDACIONES GENERALES
            // ===========================================

            if (idSolicitud <= 0 ||
                idSolicitante <= 0 ||
                idAdministradorSesion <= 0)
            {
                throw new ArgumentException(
                    "Los identificadores no son válidos.");
            }

            if (idSolicitante == idAdministradorSesion)
            {
                throw new InvalidOperationException(
                    "No podés autorizar tu propia solicitud.");
            }

            if (string.IsNullOrEmpty(contraseña))
            {
                throw new ArgumentException(
                    "Ingresá tu contraseña.");
            }

            // ===========================================
            // 2. VALIDAR ADMINISTRADOR EN SESIÓN
            // ===========================================

            bool claveValida =
                objCD_Backup.ValidarClaveSesion(
                    idAdministradorSesion,
                    contraseña);

            if (!claveValida)
            {
                throw new InvalidOperationException(
                    "La contraseña es incorrecta.");
            }

            // ===========================================
            // 3. PREPARAR RESTAURACIÓN
            // ===========================================

            CD_RestauracionSegura restaurador =
                new CD_RestauracionSegura();

            // La base administrativa debe existir.
            restaurador.VerificarEntorno();

            // Obtener el respaldo de una solicitud pendiente.
            string rutaOrigen =
                restaurador.ObtenerRutaPendiente(
                    idSolicitud,
                    idAdministradorSesion);

            // Verificar estructura y validez del respaldo
            // antes de iniciar modificaciones.
            restaurador.ValidarBackupYPrepararMovimientos(
                rutaOrigen);

            // ===========================================
            // 4. EJECUTAR CIRCUITO SEGURO
            // ===========================================

            /*
             * RestaurarOriginal ejecutará:
             *
             * A. Crear el respaldo preventivo.
             * B. Verificar ese respaldo.
             * C. Ejecutar la función de autorización.
             * D. Restaurar la base original.
             *
             * La autorización no ocurre antes del
             * respaldo preventivo.
             */

            return restaurador.RestaurarOriginal(
                idSolicitud,
                idAdministradorSesion,
                rutaOrigen,
                () =>
                {
                    // Volver a verificar que la solicitud
                    // continúa pendiente y que el archivo
                    // asociado sigue siendo el mismo.

                    string rutaActual =
                        restaurador.ObtenerRutaPendiente(
                            idSolicitud,
                            idAdministradorSesion);

                    if (!string.Equals(
                        rutaOrigen,
                        rutaActual,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "El respaldo asociado a la solicitud " +
                            "cambió durante el proceso.");
                    }

                    // Recién ahora registrar la autorización.
                    objCD_Backup.AutorizarRestauracion(
                        idSolicitud,
                        idAdministradorSesion);
                });
        }



    }
}
