
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Datos
{
    /// <summary>
    /// Restaura AuraBeautyDB sobre su nombre original.
    ///
    /// Antes de modificar la base:
    /// - Verifica el backup solicitado.
    /// - Crea un respaldo preventivo COPY_ONLY.
    /// - Verifica el respaldo preventivo.
    /// - Registra la operación en AuraBeautyDB_Admin.
    ///
    /// Si una verificación previa falla, no ejecuta RESTORE.
    /// </summary>
    public class CD_RestauracionSegura
    {


        /// <summary>
        /// Representa los datos necesarios para
        /// identificar una solicitud de restauración.
        ///
        /// Permite distinguir solicitudes que
        /// tengan el mismo número después de
        /// recuperar una copia anterior de la base.
        /// </summary>
        public sealed class IdentidadSolicitudPendiente
        {
            /// <summary>
            /// Identificador del respaldo seleccionado.
            /// </summary>
            public int IdBackup { get; set; }

            /// <summary>
            /// Fecha exacta en la que se creó
            /// la solicitud de restauración.
            /// </summary>
            public DateTime FechaSolicitud { get; set; }

            /// <summary>
            /// Ruta completa del archivo .bak.
            /// </summary>
            public string RutaArchivo { get; set; }
        }

        private const string BaseOriginal = "AuraBeautyDB";
        private const string BaseAuditoria = "AuraBeautyDB_Admin";

        /// <summary>
        /// Construye una conexión al catálogo indicado,
        /// conservando el servidor y la autenticación
        /// de la conexión original de Aura Beauty.
        /// </summary>
        private string Cadena(string catalogo)
        {
            SqlConnectionStringBuilder datos =
                new SqlConnectionStringBuilder(Conexion.cadena);

            datos.InitialCatalog = catalogo;
            datos.Pooling = false;

            return datos.ConnectionString;
        }


        /// <summary>
        /// Comprueba que la restauración se realizará
        /// sobre la instancia ORIGINAL de Aura Beauty.
        ///
        /// Verifica:
        /// 1. El servidor configurado.
        /// 2. La base de datos de destino.
        /// 3. Las rutas físicas de los archivos.
        /// 4. La existencia de la auditoría externa.
        ///
        /// No crea, modifica ni restaura datos.
        /// </summary>
        public void VerificarEntorno()
        {
            // ===========================================
            // 1. VALIDAR CONEXIÓN ORIGINAL
            // ===========================================

            SqlConnectionStringBuilder configuracion =
                new SqlConnectionStringBuilder(
                    Conexion.cadena);

            if (!string.Equals(
                configuracion.DataSource,
                @"(localdb)\MSSQLLocalDB",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Restauración bloqueada.\n\n" +
                    "El servidor configurado no es " +
                    "la instancia original de Aura Beauty.");
            }

            if (!string.Equals(
                configuracion.InitialCatalog,
                "AuraBeautyDB",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Restauración bloqueada.\n\n" +
                    "La base configurada no es AuraBeautyDB.");
            }

            // ===========================================
            // 2. VERIFICAR ARCHIVOS FÍSICOS
            // ===========================================

            using (SqlConnection conexion =
                new SqlConnection(Cadena("master")))
            using (SqlCommand comando =
                new SqlCommand(@"
                    SELECT CASE WHEN

                    EXISTS
                    (
                        SELECT 1
                        FROM sys.master_files
                        WHERE database_id =
                            DB_ID(N'AuraBeautyDB')
                          AND type = 0
                          AND physical_name =
                            N'C:\Users\brend\AuraBeautyDB.mdf'
                    )

                    AND EXISTS
                    (
                        SELECT 1
                        FROM sys.master_files
                        WHERE database_id =
                            DB_ID(N'AuraBeautyDB')
                          AND type = 1
                          AND physical_name =
                            N'C:\Users\brend\AuraBeautyDB_log.ldf'
                    )

                    AND EXISTS
                    (
                        SELECT 1
                        FROM sys.master_files
                        WHERE database_id =
                            DB_ID(N'AuraBeautyDB_Admin')
                          AND type = 0
                          AND physical_name =
                            N'C:\Users\brend\AuraBeautyDB_Admin.mdf'
                    )

                    AND EXISTS
                    (
                        SELECT 1
                        FROM sys.master_files
                        WHERE database_id =
                            DB_ID(N'AuraBeautyDB_Admin')
                          AND type = 1
                          AND physical_name =
                            N'C:\Users\brend\AuraBeautyDB_Admin_log.ldf'
                    )

                    THEN 1 ELSE 0 END;", conexion))
            {
                conexion.Open();

                int resultado =
                    Convert.ToInt32(
                        comando.ExecuteScalar());

                if (resultado != 1)
                {
                    throw new InvalidOperationException(
                        "Restauración bloqueada.\n\n" +
                        "La ubicación de las bases de datos " +
                        "no coincide con la configuración " +
                        "original de Aura Beauty.");
                }
            }

            // ===========================================
            // 3. VERIFICAR AUDITORÍA EXTERNA
            // ===========================================

            using (SqlConnection conexion =
                new SqlConnection(Cadena(BaseAuditoria)))
            using (SqlCommand comando =
                new SqlCommand(@"
                    SELECT OBJECT_ID(
                        N'dbo.RestauracionAuditoria',
                        N'U');", conexion))
            {
                conexion.Open();

                object resultado = comando.ExecuteScalar();

                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    throw new InvalidOperationException(
                        "No existe la tabla de auditoría " +
                        "en AuraBeautyDB_Admin.");
                }
            }
        }


        /// <summary>
        /// Obtiene el respaldo de una solicitud pendiente.
        /// SQL Server comprueba que pertenezca a otro
        /// administrador y que el respaldo sea exitoso.
        ///
        /// No modifica solicitudes.
        /// </summary>
        public string ObtenerRutaPendiente(
            int idSolicitud,
            int idAdministrador)
        {
            using (SqlConnection cn =
                new SqlConnection(Cadena(BaseOriginal)))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT b.ruta_archivo
                FROM dbo.RestauracionSolicitud AS r
                INNER JOIN dbo.BackupRegistro AS b
                    ON b.id_backup = r.id_backup
                WHERE r.id_solicitud = @Solicitud
                  AND r.id_solicitante <> @Administrador
                  AND r.estado = 'Pendiente'
                  AND b.estado = 'Exitoso';", cn))
            {
                cmd.Parameters.Add(
                    "@Solicitud", SqlDbType.Int
                ).Value = idSolicitud;

                cmd.Parameters.Add(
                    "@Administrador", SqlDbType.Int
                ).Value = idAdministrador;

                cn.Open();

                object resultado = cmd.ExecuteScalar();

                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    throw new InvalidOperationException(
                        "La solicitud ya no está pendiente " +
                        "o el respaldo no es válido.");
                }

                return Convert.ToString(resultado);
            }
        }

        /// <summary>
        /// Ejecuta una consulta y devuelve su resultado.
        /// Las consultas RESTORE se ejecutan desde master.
        /// </summary>
        private DataTable Leer(
            string catalogo,
            string consulta,
            string ruta = null)
        {
            DataTable tabla = new DataTable();

            using (SqlConnection cn =
                new SqlConnection(Cadena(catalogo)))
            using (SqlCommand cmd =
                new SqlCommand(consulta, cn))
            {
                cmd.CommandTimeout = 0;

                if (ruta != null)
                {
                    cmd.Parameters.Add(
                        "@Ruta", SqlDbType.NVarChar, 4000
                    ).Value = ruta;
                }

                using (SqlDataAdapter adaptador =
                    new SqlDataAdapter(cmd))
                {
                    adaptador.Fill(tabla);
                }
            }

            return tabla;
        }

        /// <summary>
        /// Ejecuta un comando SQL sin devolver filas.
        /// </summary>
        private void Ejecutar(
            string catalogo,
            string consulta,
            string ruta)
        {
            using (SqlConnection cn =
                new SqlConnection(Cadena(catalogo)))
            using (SqlCommand cmd =
                new SqlCommand(consulta, cn))
            {
                cmd.CommandTimeout = 0;

                cmd.Parameters.Add(
                    "@Ruta", SqlDbType.NVarChar, 4000
                ).Value = ruta;

                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Escapa un literal utilizado para las
        /// cláusulas MOVE del comando RESTORE.
        /// </summary>
        private static string Literal(string valor)
        {
            return "N'" + valor.Replace("'", "''") + "'";
        }

        /// <summary>
        /// Comprueba el respaldo y prepara el movimiento
        /// de los archivos lógicos hacia los archivos
        /// físicos actuales de AuraBeautyDB.
        ///
        /// Solo admite un archivo D y uno L.
        /// </summary>
        public string ValidarBackupYPrepararMovimientos(
            string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo) ||
                !Path.IsPathRooted(rutaArchivo) ||
                !string.Equals(
                    Path.GetExtension(rutaArchivo),
                    ".bak",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "La ruta del respaldo debe ser " +
                    "absoluta y terminar en .bak.");
            }

            // Verificar la cabecera del respaldo.
            DataTable cabecera = Leer(
                "master",
                "RESTORE HEADERONLY FROM DISK = @Ruta;",
                rutaArchivo);

            if (cabecera.Rows.Count != 1)
            {
                throw new InvalidOperationException(
                    "El archivo debe contener un único " +
                    "conjunto de respaldo.");
            }

            DataRow registro = cabecera.Rows[0];

            string baseEnBackup =
                Convert.ToString(registro["DatabaseName"]);

            int tipoBackup =
                Convert.ToInt32(registro["BackupType"]);

            if (!string.Equals(
                baseEnBackup,
                BaseOriginal,
                StringComparison.OrdinalIgnoreCase) ||
                tipoBackup != 1)
            {
                throw new InvalidOperationException(
                    "El archivo no contiene una copia " +
                    "completa de AuraBeautyDB.");
            }

            // Comprobar que SQL Server pueda leer el backup.
            Leer(
                "master",
                "RESTORE VERIFYONLY FROM DISK = @Ruta " +
                "WITH CHECKSUM;",
                rutaArchivo);

            // Obtener los nombres lógicos del .bak.
            DataTable archivos = Leer(
                "master",
                "RESTORE FILELISTONLY FROM DISK = @Ruta;",
                rutaArchivo);

            // Consultar los archivos físicos de la base actual.
            DataTable actuales = Leer(
                "master",
                @"SELECT type_desc, physical_name
                  FROM sys.master_files
                  WHERE database_id = DB_ID(N'AuraBeautyDB')
                  ORDER BY file_id;");

            if (archivos.Rows.Count != 2 ||
                actuales.Rows.Count != 2)
            {
                throw new InvalidOperationException(
                    "La estructura de archivos no es " +
                    "compatible con la restauración automática.");
            }

            string logicoDatos = null;
            string logicoLog = null;
            string fisicoDatos = null;
            string fisicoLog = null;

            foreach (DataRow fila in archivos.Rows)
            {
                string tipo = Convert.ToString(fila["Type"]);

                if (tipo == "D")
                    logicoDatos =
                        Convert.ToString(fila["LogicalName"]);
                else if (tipo == "L")
                    logicoLog =
                        Convert.ToString(fila["LogicalName"]);
                else
                    throw new InvalidOperationException(
                        "El respaldo contiene un tipo " +
                        "de archivo no admitido.");
            }

            foreach (DataRow fila in actuales.Rows)
            {
                string tipo =
                    Convert.ToString(fila["type_desc"]);

                if (tipo == "ROWS")
                    fisicoDatos =
                        Convert.ToString(fila["physical_name"]);
                else if (tipo == "LOG")
                    fisicoLog =
                        Convert.ToString(fila["physical_name"]);
            }

            if (string.IsNullOrWhiteSpace(logicoDatos) ||
                string.IsNullOrWhiteSpace(logicoLog) ||
                string.IsNullOrWhiteSpace(fisicoDatos) ||
                string.IsNullOrWhiteSpace(fisicoLog))
            {
                throw new InvalidOperationException(
                    "No se pudieron identificar " +
                    "correctamente los archivos MDF y LDF.");
            }

            return
                "MOVE " + Literal(logicoDatos) +
                " TO " + Literal(fisicoDatos) + ", " +
                "MOVE " + Literal(logicoLog) +
                " TO " + Literal(fisicoLog);
        }

        /// <summary>
        /// Registra el inicio de la recuperación en
        /// AuraBeautyDB_Admin, fuera de la base que
        /// eventualmente será restaurada.
        ///
        /// El índice único impedirá iniciar otra
        /// ejecución no fallida con la misma identidad.
        /// </summary>
        private void IniciarAuditoria(
            Guid idEjecucion,
            int idSolicitud,
            int idAutorizador,
            string origen,
            string preventivo,
            int idBackupOrigen,
            DateTime fechaSolicitudOrigen)
        {
            using (SqlConnection conexion =
                new SqlConnection(Cadena(BaseAuditoria)))
            using (SqlCommand comando = new SqlCommand(@"
                INSERT INTO dbo.RestauracionAuditoria
                (
                    IdEjecucion,
                    IdSolicitud,
                    IdAutorizador,
                    RutaBackupOrigen,
                    RutaBackupPreventivo,
                    IdBackupOrigen,
                    FechaSolicitudOrigen,
                    Estado
                )
                VALUES
                (
                    @Ejecucion,
                    @Solicitud,
                    @Autorizador,
                    @Origen,
                    @Preventivo,
                    @IdBackupOrigen,
                    @FechaSolicitudOrigen,
                    'INICIADA'
                );", conexion))
            {
                comando.Parameters.Add(
                    "@Ejecucion",
                    SqlDbType.UniqueIdentifier
                ).Value = idEjecucion;

                comando.Parameters.Add(
                    "@Solicitud",
                    SqlDbType.Int
                ).Value = idSolicitud;

                comando.Parameters.Add(
                    "@Autorizador",
                    SqlDbType.Int
                ).Value = idAutorizador;

                comando.Parameters.Add(
                    "@Origen",
                    SqlDbType.NVarChar,
                    4000
                ).Value = origen;

                comando.Parameters.Add(
                    "@Preventivo",
                    SqlDbType.NVarChar,
                    4000
                ).Value = preventivo;

                comando.Parameters.Add(
                    "@IdBackupOrigen",
                    SqlDbType.Int
                ).Value = idBackupOrigen;

                SqlParameter fecha =
                    comando.Parameters.Add(
                        "@FechaSolicitudOrigen",
                        SqlDbType.DateTime2);

                fecha.Scale = 7;
                fecha.Value = fechaSolicitudOrigen;

                conexion.Open();

                comando.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Actualiza la bitácora externa.
        /// REVISAR significa que el estado definitivo
        /// debe comprobarse antes de reintentar.
        /// </summary>
        private void ActualizarAuditoria(
            Guid idEjecucion,
            string estado,
            string detalle = null)
        {
            using (SqlConnection cn =
                new SqlConnection(Cadena(BaseAuditoria)))
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE dbo.RestauracionAuditoria
                SET Estado = @Estado,
                    Detalle = @Detalle,
                    FechaFin =
                        CASE
                            WHEN @Estado IN
                                ('EXITOSA','FALLIDA','REVISAR')
                            THEN SYSDATETIME()
                            ELSE NULL
                        END
                WHERE IdEjecucion = @Ejecucion;", cn))
            {
                cmd.Parameters.Add(
                    "@Ejecucion", SqlDbType.UniqueIdentifier
                ).Value = idEjecucion;

                cmd.Parameters.Add(
                    "@Estado", SqlDbType.VarChar, 20
                ).Value = estado;

                cmd.Parameters.Add(
                    "@Detalle", SqlDbType.NVarChar, 2000
                ).Value = string.IsNullOrEmpty(detalle)
                    ? (object)DBNull.Value
                    : detalle.Substring(
                        0, Math.Min(2000, detalle.Length));

                cn.Open();

                if (cmd.ExecuteNonQuery() != 1)
                {
                    throw new InvalidOperationException(
                        "No se pudo actualizar la auditoría.");
                }
            }
        }

        /// <summary>
        /// Restaura AuraBeautyDB conservando su nombre.
        ///
        /// Secuencia:
        /// 1. Verificar el entorno SQL.
        /// 2. Verificar la identidad de la solicitud.
        /// 3. Comprobar que no fue procesada.
        /// 4. Validar el respaldo solicitado.
        /// 5. Registrar el inicio en la auditoría externa.
        /// 6. Crear y verificar el respaldo preventivo.
        /// 7. Registrar la autorización.
        /// 8. Restaurar AuraBeautyDB.
        /// 9. Comprobar que quede ONLINE.
        /// 10. Registrar el resultado definitivo.
        ///
        /// Si la autorización pudo haberse registrado,
        /// cualquier error posterior queda en REVISAR.
        /// </summary>
        public string RestaurarOriginal(
            int idSolicitud,
            int idAutorizador,
            string rutaOrigen,
            int idBackupOrigen,
            DateTime fechaSolicitudOrigen,
            Action confirmarAutorizacion)
        {
            if (idSolicitud <= 0 ||
                idAutorizador <= 0 ||
                idBackupOrigen <= 0)
            {
                throw new ArgumentException(
                    "Los identificadores no son válidos.");
            }

            if (fechaSolicitudOrigen == DateTime.MinValue)
            {
                throw new ArgumentException(
                    "La fecha de la solicitud no es válida.");
            }

            if (confirmarAutorizacion == null)
            {
                throw new ArgumentNullException(
                    "confirmarAutorizacion");
            }

            // Comprobar que el servidor es el permitido.
            VerificarEntorno();

            // Rechazar solicitudes ya procesadas.
            VerificarNoProcesada(
                idSolicitud,
                idBackupOrigen,
                fechaSolicitudOrigen);

            // Comprobar el backup antes de modificar datos.
            string movimientos =
                ValidarBackupYPrepararMovimientos(
                    rutaOrigen);

            Guid idEjecucion = Guid.NewGuid();

            string carpeta =
                Path.GetDirectoryName(rutaOrigen);

            if (string.IsNullOrWhiteSpace(carpeta))
            {
                throw new InvalidOperationException(
                    "No se pudo determinar la carpeta " +
                    "del respaldo seleccionado.");
            }

            // Cada resguardo preventivo tiene un nombre
            // exclusivo para conservar copias anteriores.
            string rutaPreventiva = Path.Combine(
                carpeta,
                "AuraBeautyDB_PRE_RESTORE_" +
                DateTime.UtcNow.ToString(
                    "yyyyMMdd_HHmmss_fff") +
                "_" + idEjecucion.ToString("N") +
                ".bak");

            bool auditoriaCreada = false;
            bool autorizacionIntentada = false;
            bool restauracionIniciada = false;

            try
            {
                // ========================================
                // 1. REGISTRAR INICIO
                // ========================================

                /*
                 * El índice único externo impide
                 * insertar otra ejecución activa,
                 * exitosa o pendiente de revisión
                 * con la misma identidad.
                 */
                IniciarAuditoria(
                    idEjecucion,
                    idSolicitud,
                    idAutorizador,
                    rutaOrigen,
                    rutaPreventiva,
                    idBackupOrigen,
                    fechaSolicitudOrigen);

                auditoriaCreada = true;

                // ========================================
                // 2. RESPALDAR EL ESTADO ACTUAL
                // ========================================

                Ejecutar(
                    "master",
                    @"BACKUP DATABASE [AuraBeautyDB]
                      TO DISK = @Ruta
                      WITH COPY_ONLY,
                           INIT,
                           CHECKSUM,
                           STATS = 10;",
                    rutaPreventiva);

                // ========================================
                // 3. VERIFICAR EL RESPALDO PREVENTIVO
                // ========================================

                Leer(
                    "master",
                    @"RESTORE VERIFYONLY
                      FROM DISK = @Ruta
                      WITH CHECKSUM;",
                    rutaPreventiva);

                ActualizarAuditoria(
                    idEjecucion,
                    "RESGUARDADA");

                // ========================================
                // 4. REGISTRAR AUTORIZACIÓN
                // ========================================

                /*
                 * IMPORTANTE:
                 *
                 * Marcamos el intento ANTES de llamar
                 * al procedimiento de autorización.
                 *
                 * Si SQL registra la autorización pero
                 * ocurre un error de comunicación al
                 * devolver el resultado, no podemos
                 * considerarla simplemente fallida.
                 */
                autorizacionIntentada = true;

                confirmarAutorizacion();

                // ========================================
                // 5. COMENZAR RESTAURACIÓN
                // ========================================

                ActualizarAuditoria(
                    idEjecucion,
                    "RESTAURANDO");

                SqlConnection.ClearAllPools();

                restauracionIniciada = true;

                /*
                 * SINGLE_USER:
                 * Desconecta otras sesiones de la base.
                 *
                 * RESTORE WITH REPLACE:
                 * Reemplaza AuraBeautyDB con el
                 * respaldo seleccionado.
                 *
                 * MOVE:
                 * Conserva las rutas físicas actuales.
                 *
                 * MULTI_USER:
                 * Vuelve a permitir conexiones.
                 */
                string restauracion =
                    @"ALTER DATABASE [AuraBeautyDB]
                        SET SINGLE_USER
                        WITH ROLLBACK IMMEDIATE;

                      RESTORE DATABASE [AuraBeautyDB]
                      FROM DISK = @Ruta
                      WITH REPLACE,
                           RECOVERY,
                           CHECKSUM, " +
                           movimientos + @",
                           STATS = 10;

                      ALTER DATABASE [AuraBeautyDB]
                        SET MULTI_USER;";

                Ejecutar(
                    "master",
                    restauracion,
                    rutaOrigen);

                SqlConnection.ClearAllPools();

                // ========================================
                // 6. VERIFICAR ESTADO DE LA BASE
                // ========================================

                DataTable resultado = Leer(
                    "master",
                    @"SELECT state_desc
                      FROM sys.databases
                      WHERE name = N'AuraBeautyDB';");

                if (resultado.Rows.Count != 1 ||
                    Convert.ToString(
                        resultado.Rows[0]["state_desc"]
                    ) != "ONLINE")
                {
                    throw new InvalidOperationException(
                        "La base restaurada no quedó " +
                        "en estado ONLINE.");
                }

                // ========================================
                // 7. REGISTRAR FINALIZACIÓN
                // ========================================

                ActualizarAuditoria(
                    idEjecucion,
                    "EXITOSA",
                    "AuraBeautyDB restaurada. " +
                    "Respaldo preventivo conservado.");

                return rutaPreventiva;
            }
            catch (Exception ex)
            {
                /*
                 * Si comenzó el RESTORE, intentamos
                 * restablecer el acceso a la base.
                 *
                 * No ejecutamos otro RESTORE ni
                 * recuperamos automáticamente el
                 * respaldo preventivo.
                 */
                if (restauracionIniciada)
                {
                    try
                    {
                        Ejecutar(
                            "master",
                            @"IF DB_ID(N'AuraBeautyDB')
                                IS NOT NULL
                              ALTER DATABASE [AuraBeautyDB]
                              SET MULTI_USER
                              WITH ROLLBACK IMMEDIATE;",
                            rutaOrigen);
                    }
                    catch
                    {
                        // Conservar el error principal.
                    }
                }

                if (auditoriaCreada)
                {
                    try
                    {
                        /*
                         * Antes de intentar autorizar:
                         * FALLIDA admite una nueva
                         * evaluación posterior.
                         *
                         * Desde que se intenta autorizar:
                         * REVISAR bloquea reintentos
                         * automáticos, incluso si el
                         * RESTORE todavía no comenzó.
                         */
                        string estadoError =
                            autorizacionIntentada ||
                            restauracionIniciada
                                ? "REVISAR"
                                : "FALLIDA";

                        ActualizarAuditoria(
                            idEjecucion,
                            estadoError,
                            ex.Message);
                    }
                    catch
                    {
                        // No ocultar el error original.
                    }
                }

                throw new InvalidOperationException(
                    "No se pudo completar la recuperación. " +
                    "No vuelvas a autorizar esta solicitud " +
                    "sin revisar la auditoría externa.\n\n" +
                    "Respaldo preventivo previsto:\n" +
                    rutaPreventiva + "\n\n" +
                    "Detalle: " + ex.Message,
                    ex);
            }
        }

        

        /// <summary>
        /// Recupera desde SQL Server los datos reales
        /// de una solicitud todavía pendiente.
        ///
        /// El solicitante debe ser distinto del
        /// administrador que autoriza.
        ///
        /// La fecha procede de la base de datos,
        /// no del reloj de la computadora.
        /// </summary>
        public IdentidadSolicitudPendiente
            ObtenerIdentidadSolicitudPendiente(
                int idSolicitud,
                int idAdministrador)
        {
            using (SqlConnection conexion =
                new SqlConnection(Cadena(BaseOriginal)))
            using (SqlCommand comando = new SqlCommand(@"
                SELECT
                    r.id_backup AS IdBackup,
                    r.fecha_solicitud AS FechaSolicitud,
                    b.ruta_archivo AS RutaArchivo
                FROM dbo.RestauracionSolicitud AS r
                INNER JOIN dbo.BackupRegistro AS b
                    ON b.id_backup = r.id_backup
                WHERE r.id_solicitud = @IdSolicitud
                  AND r.id_solicitante <> @IdAdministrador
                  AND r.estado = 'Pendiente'
                  AND b.estado = 'Exitoso';",
                conexion))
            {
                comando.Parameters.Add(
                    "@IdSolicitud", SqlDbType.Int
                ).Value = idSolicitud;

                comando.Parameters.Add(
                    "@IdAdministrador", SqlDbType.Int
                ).Value = idAdministrador;

                conexion.Open();

                using (SqlDataReader lector =
                    comando.ExecuteReader())
                {
                    if (!lector.Read())
                    {
                        throw new InvalidOperationException(
                            "La solicitud ya no está pendiente " +
                            "o su respaldo no es válido.");
                    }

                    if (lector["FechaSolicitud"] ==
                        DBNull.Value)
                    {
                        throw new InvalidOperationException(
                            "La solicitud no tiene una fecha válida.");
                    }

                    return new IdentidadSolicitudPendiente
                    {
                        IdBackup = Convert.ToInt32(
                            lector["IdBackup"]),

                        FechaSolicitud = Convert.ToDateTime(
                            lector["FechaSolicitud"]),

                        RutaArchivo = Convert.ToString(
                            lector["RutaArchivo"])
                    };
                }
            }
        }

        /// <summary>
        /// Impide repetir una recuperación que ya tiene
        /// una ejecución no fallida en la auditoría.
        ///
        /// La verificación utiliza:
        /// - Identificador de solicitud.
        /// - Identificador del respaldo.
        /// - Fecha exacta de solicitud.
        ///
        /// El índice único de SQL Server sigue siendo
        /// la protección definitiva frente a dos
        /// inserciones simultáneas.
        /// </summary>
        public void VerificarNoProcesada(
            int idSolicitud,
            int idBackup,
            DateTime fechaSolicitud)
        {
            using (SqlConnection conexion =
                new SqlConnection(Cadena(BaseAuditoria)))
            using (SqlCommand comando = new SqlCommand(@"
                SELECT TOP (1) Estado
                FROM dbo.RestauracionAuditoria
                WHERE IdSolicitud = @IdSolicitud
                  AND IdBackupOrigen = @IdBackup
                  AND FechaSolicitudOrigen = @FechaSolicitud
                  AND Estado <> 'FALLIDA';",
                conexion))
            {
                comando.Parameters.Add(
                    "@IdSolicitud", SqlDbType.Int
                ).Value = idSolicitud;

                comando.Parameters.Add(
                    "@IdBackup", SqlDbType.Int
                ).Value = idBackup;

                SqlParameter parametroFecha =
                    comando.Parameters.Add(
                        "@FechaSolicitud",
                        SqlDbType.DateTime2);

                parametroFecha.Scale = 7;
                parametroFecha.Value = fechaSolicitud;

                conexion.Open();

                object estado = comando.ExecuteScalar();

                if (estado != null &&
                    estado != DBNull.Value)
                {
                    throw new InvalidOperationException(
                        "Esta solicitud ya tiene una " +
                        "recuperación registrada con estado: " +
                        Convert.ToString(estado) + ".\n\n" +
                        "No se ejecutará nuevamente. " +
                        "Revisá la auditoría externa.");
                }
            }
        }
    }
}
