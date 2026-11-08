
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
        /// Registra el comienzo de la recuperación fuera
        /// de AuraBeautyDB, para conservar la auditoría.
        /// </summary>
        private void IniciarAuditoria(
            Guid idEjecucion,
            int idSolicitud,
            int idAutorizador,
            string origen,
            string preventivo)
        {
            using (SqlConnection cn =
                new SqlConnection(Cadena(BaseAuditoria)))
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO dbo.RestauracionAuditoria
                (
                    IdEjecucion,
                    IdSolicitud,
                    IdAutorizador,
                    RutaBackupOrigen,
                    RutaBackupPreventivo,
                    Estado
                )
                VALUES
                (
                    @Ejecucion, @Solicitud,
                    @Autorizador, @Origen,
                    @Preventivo, 'INICIADA'
                );", cn))
            {
                cmd.Parameters.Add(
                    "@Ejecucion", SqlDbType.UniqueIdentifier
                ).Value = idEjecucion;

                cmd.Parameters.Add(
                    "@Solicitud", SqlDbType.Int
                ).Value = idSolicitud;

                cmd.Parameters.Add(
                    "@Autorizador", SqlDbType.Int
                ).Value = idAutorizador;

                cmd.Parameters.Add(
                    "@Origen", SqlDbType.NVarChar, 4000
                ).Value = origen;

                cmd.Parameters.Add(
                    "@Preventivo", SqlDbType.NVarChar, 4000
                ).Value = preventivo;

                cn.Open();
                cmd.ExecuteNonQuery();
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
        /// Ejecuta la restauración real.
        ///
        /// Devuelve la ruta del respaldo preventivo.
        /// El archivo preventivo NO se elimina.
        /// </summary>

        public string RestaurarOriginal(
            int idSolicitud,
            int idAutorizador,
            string rutaOrigen,
            Action confirmarAutorizacion)


        {


            // La autorización se ejecutará únicamente
            // después de crear y verificar el respaldo preventivo.
            if (confirmarAutorizacion == null)
            {
                throw new ArgumentNullException(
                    "confirmarAutorizacion");
            }

            if (idSolicitud <= 0 || idAutorizador <= 0)
                throw new ArgumentException(
                    "La solicitud y el autorizador " +
                    "deben ser válidos.");

            VerificarEntorno();

            // Identificador único para esta ejecución.
            Guid idEjecucion = Guid.NewGuid();

            // El respaldo preventivo se guarda junto
            // al backup seleccionado, con nombre único.
            string carpeta = Path.GetDirectoryName(rutaOrigen);

            string rutaPreventiva = Path.Combine(
                carpeta,
                "AuraBeautyDB_PRE_RESTORE_" +
                DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff") +
                "_" + idEjecucion.ToString("N") +
                ".bak");

            bool auditoriaCreada = false;
            bool restauracionIniciada = false;

            try
            {
                // 1. Verificar el backup elegido.
                string movimientos =
                    ValidarBackupYPrepararMovimientos(
                        rutaOrigen);

                // 2. Registrar auditoría fuera de AuraBeautyDB.
                IniciarAuditoria(
                    idEjecucion,
                    idSolicitud,
                    idAutorizador,
                    rutaOrigen,
                    rutaPreventiva);

                auditoriaCreada = true;

                // 3. Resguardar el estado ACTUAL de la base.
                Ejecutar(
                    "master",
                    @"BACKUP DATABASE [AuraBeautyDB]
                      TO DISK = @Ruta
                      WITH COPY_ONLY, INIT,
                           CHECKSUM, STATS = 10;",
                    rutaPreventiva);

                // 4. Verificar que el resguardo sea legible.
                Leer(
                    "master",
                    @"RESTORE VERIFYONLY
                      FROM DISK = @Ruta
                      WITH CHECKSUM;",
                    rutaPreventiva);

                ActualizarAuditoria(
                    idEjecucion,
                    "RESGUARDADA");


                // El respaldo preventivo ya fue creado y verificado.
                //
                // Recién ahora se registra la autorización.
                // Si esta operación falla, NO comienza el RESTORE.
                confirmarAutorizacion();


                // 5. Registrar que entramos en la etapa
                //    que puede reemplazar AuraBeautyDB.
                ActualizarAuditoria(
                    idEjecucion,
                    "RESTAURANDO");

                // Cerrar las conexiones inactivas del pool.
                SqlConnection.ClearAllPools();

                restauracionIniciada = true;

                /*
                 * 6. Restaurar AuraBeautyDB.
                 *
                 * SINGLE_USER:
                 * Cierra las conexiones existentes.
                 *
                 * REPLACE:
                 * Permite sustituir la base actual.
                 *
                 * MOVE:
                 * Utiliza las rutas físicas actuales.
                 *
                 * MULTI_USER:
                 * Habilita nuevamente las conexiones.
                 *
                 * Todo se ejecuta desde master.
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

                // 7. Comprobar que la base quedó ONLINE.
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
                        "La base no quedó en estado ONLINE.");
                }

                // 8. Registrar éxito fuera de la base.
                ActualizarAuditoria(
                    idEjecucion,
                    "EXITOSA",
                    "AuraBeautyDB restaurada. " +
                    "Se conserva el respaldo preventivo.");

                return rutaPreventiva;
            }
            catch (Exception ex)
            {
                /*
                 * Si ya comenzó el RESTORE, no podemos
                 * asegurar que la base esté intacta.
                 *
                 * Por eso registramos REVISAR, no
                 * una afirmación incorrecta de FALLIDA.
                 */
                if (restauracionIniciada)
                {
                    try
                    {
                        Ejecutar(
                            "master",
                            @"IF DB_ID(N'AuraBeautyDB') IS NOT NULL
                              ALTER DATABASE [AuraBeautyDB]
                              SET MULTI_USER
                              WITH ROLLBACK IMMEDIATE;",
                            rutaOrigen);
                    }
                    catch
                    {
                        // No ocultar el error original.
                    }
                }

                if (auditoriaCreada)
                {
                    try
                    {
                        ActualizarAuditoria(
                            idEjecucion,
                            restauracionIniciada
                                ? "REVISAR"
                                : "FALLIDA",
                            ex.Message);
                    }
                    catch
                    {
                        // El error de auditoría tampoco
                        // debe ocultar el error principal.
                    }
                }

                throw new InvalidOperationException(
                    "No se pudo completar la recuperación. " +
                    "No la ejecutes nuevamente sin revisar " +
                    "el estado de las bases de datos.\n\n" +
                    "Resguardo previsto: " +
                    rutaPreventiva + "\n\n" +
                    "Detalle: " + ex.Message,
                    ex);
            }
        }
    }
}

