using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    /// <summary>
    /// Capa de Datos del módulo Backup.
    ///
    /// Su responsabilidad es comunicarse con SQL Server
    /// para ejecutar los procedimientos almacenados.
    ///
    /// Esta clase no decide qué permisos tiene un usuario
    /// ni diseña la interfaz gráfica.
    /// </summary>
    public class CD_Backup
    {
        // Guarda la conexión con la base de datos.
        // Se reutiliza la cadena de conexión del proyecto.
        private readonly string cadenaConexion = Conexion.cadena;


        /// <summary>
        /// Crea el archivo físico de respaldo de AuraBeautyDB.
        /// Este método no registra el resultado en BackupRegistro.
        /// Esa operación se realiza mediante RegistrarBackup.
        /// </summary>
        /// <param name="rutaArchivo">
        /// Ruta completa seleccionada para guardar el archivo .bak.
        /// </param>
        public void CrearBackupReal(string rutaArchivo)
        {
            // Comprobar que se haya indicado una ruta.
            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new ArgumentException(
                    "La ruta del respaldo es obligatoria.");
            }

            // Verificar que la ruta sea absoluta.
            if (!Path.IsPathRooted(rutaArchivo))
            {
                throw new ArgumentException(
                    "La ruta del respaldo debe ser absoluta.");
            }

            // Verificar que el archivo tenga extensión .bak.
            if (!string.Equals(
                Path.GetExtension(rutaArchivo),
                ".bak",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "El archivo de respaldo debe tener extensión .bak.");
            }

            // Comando que solicita a SQL Server un respaldo completo.
            string consulta = @"
                BACKUP DATABASE [AuraBeautyDB]
                TO DISK = @RutaArchivo
                WITH INIT, CHECKSUM, STATS = 10;";

            // Crear una conexión con la base de datos.
            using (SqlConnection conexion =
                new SqlConnection(cadenaConexion))
            {
                // Preparar el comando de respaldo.
                using (SqlCommand comando =
                    new SqlCommand(consulta, conexion))
                {
                    // Enviar la ruta como parámetro SQL.
                    comando.Parameters.Add(
                        "@RutaArchivo",
                        SqlDbType.NVarChar,
                        4000).Value = rutaArchivo;

                    // El respaldo puede tardar más que una consulta normal.
                    comando.CommandTimeout = 0;

                    // Abrir la conexión con SQL Server.
                    conexion.Open();

                    // Ejecutar el respaldo y esperar a que termine.
                    comando.ExecuteNonQuery();
                }
            }

        }

        public int RegistrarBackup(
            string nombreArchivo,
            string rutaArchivo,
            int idUsuario,
            string estado,
            string observaciones)
        {
            // Crear una conexión con SQL Server.
            using (SqlConnection conexion =
                   new SqlConnection(cadenaConexion))
            {
                // Indicar qué procedimiento almacenado se ejecutará.
                using (SqlCommand comando =
                       new SqlCommand(
                           "SP_Backup_Registrar", conexion))
                {
                    // Especificar que el comando es un procedimiento.
                    comando.CommandType = CommandType.StoredProcedure;

                    // Enviar el nombre del archivo.
                    comando.Parameters.Add(
                        "@NombreArchivo",
                        SqlDbType.NVarChar, 260).Value = nombreArchivo;

                    // Enviar la ruta física del archivo.
                    comando.Parameters.Add(
                        "@RutaArchivo",
                        SqlDbType.NVarChar, 500).Value = rutaArchivo;

                    // Enviar el identificador del administrador.
                    comando.Parameters.Add(
                        "@IdUsuario",
                        SqlDbType.Int).Value = idUsuario;

                    // Enviar el estado del backup.
                    comando.Parameters.Add(
                        "@Estado",
                        SqlDbType.VarChar, 20).Value = estado;

                    // Si no hay observaciones, enviar NULL a SQL Server.
                    comando.Parameters.Add(
                        "@Observaciones",
                        SqlDbType.NVarChar, 1000).Value =
                        string.IsNullOrWhiteSpace(observaciones)
                            ? (object)DBNull.Value
                            : observaciones;

                    // Abrir la conexión antes de ejecutar el comando.
                    conexion.Open();

                    // Ejecutar el procedimiento y recuperar
                    // el IdBackup que devuelve su SELECT final.
                    object resultado = comando.ExecuteScalar();

                    // Comprobar que SQL Server haya devuelto un ID.
                    if (resultado == null || resultado == DBNull.Value)
                    {
                        throw new Exception(
                            "No se recibió el identificador del backup.");
                    }

                    // Convertir el ID devuelto a un entero.
                    return Convert.ToInt32(resultado);
                }
            }
        }

        /// <summary>
        /// Obtiene el historial de backups registrados.
        ///
        /// Devuelve una tabla de datos que después podrá
        /// mostrarse en un DataGridView.
        /// </summary>
        public DataTable ListarBackups(int idUsuario)
        {
            // Crear la tabla que almacenará los resultados.
            DataTable tabla = new DataTable();

            // Crear la conexión con SQL Server.
            using (SqlConnection conexion =
                   new SqlConnection(cadenaConexion))
            {
                // Preparar el procedimiento de consulta.
                using (SqlCommand comando =
                       new SqlCommand(
                           "SP_Backup_Listar", conexion))
                {
                    // Indicar que se ejecutará un procedimiento.
                    comando.CommandType = CommandType.StoredProcedure;

                    // Enviar el ID del administrador que consulta.
                    comando.Parameters.Add(
                        "@IdUsuario",
                        SqlDbType.Int).Value = idUsuario;

                    // El adaptador ejecuta la consulta y llena la tabla.
                    using (SqlDataAdapter adaptador =
                           new SqlDataAdapter(comando))
                    {
                        adaptador.Fill(tabla);
                    }
                }
            }

            // Devolver los resultados a la capa de Negocio.
            return tabla;
        }

        /// <summary>
        /// Registra una solicitud de restauración.
        ///
        /// La solicitud queda pendiente de autorización.
        /// Este método no ejecuta una restauración real.
        /// </summary>
        public int SolicitarRestauracion(
            int idBackup,
            int idSolicitante,
            string observaciones)
        {
            // Crear la conexión con SQL Server.
            using (SqlConnection conexion =
                   new SqlConnection(cadenaConexion))
            {
                // Preparar el procedimiento almacenado.
                using (SqlCommand comando =
                       new SqlCommand(
                           "SP_Restore_Solicitar", conexion))
                {
                    // Indicar que se ejecutará un procedimiento.
                    comando.CommandType = CommandType.StoredProcedure;

                    // Enviar el ID del backup seleccionado.
                    comando.Parameters.Add(
                        "@IdBackup",
                        SqlDbType.Int).Value = idBackup;

                    // Enviar el ID del administrador solicitante.
                    comando.Parameters.Add(
                        "@IdSolicitante",
                        SqlDbType.Int).Value = idSolicitante;

                    // Enviar observaciones o NULL si están vacías.
                    comando.Parameters.Add(
                        "@Observaciones",
                        SqlDbType.NVarChar, 1000).Value =
                        string.IsNullOrWhiteSpace(observaciones)
                            ? (object)DBNull.Value
                            : observaciones;

                    // Abrir la conexión.
                    conexion.Open();

                    // Ejecutar el procedimiento y recuperar
                    // el identificador de la solicitud creada.
                    object resultado = comando.ExecuteScalar();

                    // Comprobar que se haya generado un ID.
                    if (resultado == null || resultado == DBNull.Value)
                    {
                        throw new Exception(
                            "No se recibió el identificador de la solicitud.");
                    }

                    // Devolver el ID a la capa de Negocio.
                    return Convert.ToInt32(resultado);
                }
            }
        }

        /// <summary>
        /// Autoriza una solicitud de restauración.
        ///
        /// La autenticación real del segundo administrador
        /// deberá verificarse antes desde la capa de Negocio.
        /// </summary>
        public DataTable AutorizarRestauracion(
            int idSolicitud,
            int idAutorizador)
        {
            // Ejecutar el procedimiento y devolver los datos
            // de la solicitud que quedó autorizada.
            return EjecutarProcedimientoRestauracion(
                "SP_Restore_Autorizar",
                idSolicitud,
                idAutorizador,
                null);
        }

        /// <summary>
        /// Rechaza una solicitud y registra el motivo.
        /// </summary>
        public DataTable RechazarRestauracion(
            int idSolicitud,
            int idAutorizador,
            string motivoRechazo)
        {
            // Ejecutar el procedimiento de rechazo.
            return EjecutarProcedimientoRestauracion(
                "SP_Restore_Rechazar",
                idSolicitud,
                idAutorizador,
                motivoRechazo);
        }

        /// <summary>
        /// Método auxiliar para ejecutar los procedimientos
        /// de autorización y rechazo.
        ///
        /// Centraliza la apertura de conexión y la lectura
        /// del resultado para evitar repetir código.
        /// </summary>
        private DataTable EjecutarProcedimientoRestauracion(
            string nombreProcedimiento,
            int idSolicitud,
            int idAutorizador,
            string motivoRechazo)
        {
            // Tabla donde se guardará el resultado devuelto.
            DataTable tabla = new DataTable();

            // Crear la conexión.
            using (SqlConnection conexion =
                   new SqlConnection(cadenaConexion))
            {
                // Crear el comando con el procedimiento recibido.
                using (SqlCommand comando =
                       new SqlCommand(nombreProcedimiento, conexion))
                {
                    // Configurar el tipo de comando.
                    comando.CommandType = CommandType.StoredProcedure;

                    // Enviar el identificador de la solicitud.
                    comando.Parameters.Add(
                        "@IdSolicitud",
                        SqlDbType.Int).Value = idSolicitud;

                    // Enviar el identificador del autorizador.
                    comando.Parameters.Add(
                        "@IdAutorizador",
                        SqlDbType.Int).Value = idAutorizador;

                    // El procedimiento de rechazo requiere el motivo.
                    // El de autorización no tiene ese parámetro.
                    if (nombreProcedimiento == "SP_Restore_Rechazar")
                    {
                        comando.Parameters.Add(
                            "@MotivoRechazo",
                            SqlDbType.NVarChar, 1000).Value =
                            string.IsNullOrWhiteSpace(motivoRechazo)
                                ? (object)DBNull.Value
                                : motivoRechazo;
                    }

                    // Ejecutar el procedimiento y llenar la tabla.
                    using (SqlDataAdapter adaptador =
                           new SqlDataAdapter(comando))
                    {
                        adaptador.Fill(tabla);
                    }
                }
            }

            // Devolver los datos a la capa de Negocio.
            return tabla;
        }


        /// <summary>
        /// Obtiene las solicitudes de restauración que están pendientes.
        ///
        /// Ejecuta el procedimiento almacenado SP_Restore_ListarPendientes,
        /// que verifica los permisos del administrador y devuelve
        /// los datos de las solicitudes para mostrarlos en la interfaz.
        /// </summary>
        /// <param name="idUsuario">
        /// Identificador del administrador que consulta las solicitudes.
        /// </param>
        /// <returns>
        /// Un DataTable con las solicitudes de restauración pendientes.
        /// </returns>
        public DataTable ListarSolicitudesPendientes(int idUsuario)
        {
            // Crear una tabla en memoria para guardar los resultados SQL.
            DataTable tabla = new DataTable();

            // Abrir la conexión y preparar la ejecución del procedimiento.
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(
                "SP_Restore_ListarPendientes", conexion))
            {
                // Indicar que se ejecutará un procedimiento almacenado.
                comando.CommandType = CommandType.StoredProcedure;

                // Enviar el ID del administrador que realiza la consulta.
                comando.Parameters.Add("@IdUsuario", SqlDbType.Int).Value =
                    idUsuario;

                // El adaptador ejecuta la consulta y carga los resultados
                // directamente en el DataTable.
                using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                {
                    adaptador.Fill(tabla);
                }
            }

            // Devolver las solicitudes obtenidas desde SQL Server.
            return tabla;
        }


        /// <summary>
        /// Obtiene el historial completo de solicitudes de restauración.
        /// El procedimiento verifica que el usuario sea un administrador activo.
        /// </summary>
        public DataTable ListarHistorialRestauraciones(int idUsuario)
        {
            // Creamos una tabla para recibir los resultados.
            DataTable tabla = new DataTable();

            // Abrimos la conexión y preparamos la llamada al procedimiento.
            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            using (SqlCommand comando = new SqlCommand(
                "SP_Restore_ListarHistorial", conexion))
            {
                // Indicamos que vamos a ejecutar un procedimiento almacenado.
                comando.CommandType = CommandType.StoredProcedure;

                // Enviamos el ID del usuario que consulta el historial.
                comando.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;

                // Ejecutamos la consulta y cargamos el resultado en la tabla.
                using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                {
                    adaptador.Fill(tabla);
                }
            }

            // Devolvemos el historial obtenido.
            return tabla;
        }



        /// <summary>
        /// Consulta el estado y los datos de una solicitud.
        /// </summary>
        public DataTable ConsultarEstadoRestauracion(
            int idSolicitud,
            int idUsuario)
        {
            // Crear la tabla que recibirá los datos.
            DataTable tabla = new DataTable();

            // Crear la conexión con SQL Server.
            using (SqlConnection conexion =
                   new SqlConnection(cadenaConexion))
            {
                // Preparar el procedimiento de consulta.
                using (SqlCommand comando =
                       new SqlCommand(
                           "SP_Restore_ConsultarEstado", conexion))
                {
                    // Indicar que el comando es un procedimiento.
                    comando.CommandType = CommandType.StoredProcedure;

                    // Enviar el ID de la solicitud.
                    comando.Parameters.Add(
                        "@IdSolicitud",
                        SqlDbType.Int).Value = idSolicitud;

                    // Enviar el ID del administrador que consulta.
                    comando.Parameters.Add(
                        "@IdUsuario",
                        SqlDbType.Int).Value = idUsuario;

                    // Ejecutar la consulta y cargar los resultados.
                    using (SqlDataAdapter adaptador =
                           new SqlDataAdapter(comando))
                    {
                        adaptador.Fill(tabla);
                    }
                }
            }

            // Devolver la información consultada.
            return tabla;
        }

        /// <summary>
        /// Obtiene la ruta del backup asociado a una solicitud que ya fue autorizada.
        /// La consulta vuelve a comprobar el estado directamente en SQL Server.
        /// </summary>
        public DataTable ObtenerBackupParaRestaurar(int idSolicitud)
        {
            DataTable tabla = new DataTable();

            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(@"
                SELECT r.id_solicitud AS IdSolicitud,
                       r.estado AS Estado,
                       b.id_backup AS IdBackup,
                       b.nombre_archivo AS NombreArchivo,
                       b.ruta_archivo AS RutaArchivo
                FROM dbo.RestauracionSolicitud AS r
                INNER JOIN dbo.BackupRegistro AS b
                    ON b.id_backup = r.id_backup
                WHERE r.id_solicitud = @IdSolicitud
                  AND r.estado = 'Autorizada';", conexion))
            {
                comando.Parameters.Add("@IdSolicitud", SqlDbType.Int).Value = idSolicitud;
                using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                {
                    adaptador.Fill(tabla);
                }
            }

            if (tabla.Rows.Count != 1)
            {
                throw new InvalidOperationException(
                    "La solicitud no existe, no está autorizada o no tiene un backup asociado.");
            }

            return tabla;
        }

        /// <summary>
        /// Obtiene la ruta física del archivo .bak asociado a una solicitud
        /// que ya fue autorizada. Reutiliza la consulta que comprueba el
        /// estado de autorización en SQL Server.
        /// </summary>
        /// <param name="idSolicitud">Identificador de la solicitud autorizada.</param>
        /// <returns>Ruta completa del archivo .bak.</returns>
        public string ObtenerRutaBackupAutorizado(int idSolicitud)
        {
            if (idSolicitud <= 0)
            {
                throw new ArgumentException(
                    "El identificador de la solicitud no es válido.",
                    nameof(idSolicitud));
            }

            // Este método devuelve únicamente datos de solicitudes autorizadas.
            DataTable datos = ObtenerBackupParaRestaurar(idSolicitud);

            if (datos == null ||
                datos.Rows.Count != 1 ||
                !datos.Columns.Contains("RutaArchivo"))
            {
                throw new InvalidOperationException(
                    "No se pudo obtener la ruta del backup autorizado.");
            }

            string rutaArchivo = Convert.ToString(
                datos.Rows[0]["RutaArchivo"]);

            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new InvalidOperationException(
                    "La solicitud autorizada no tiene una ruta de backup registrada.");
            }

            return rutaArchivo;
        }

        /// <summary>
        /// Registra el resultado final de una restauración ya autorizada.
        /// </summary>
        public void RegistrarResultadoRestauracion(
            int idSolicitud, string estado, string observaciones)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            using (SqlCommand comando = new SqlCommand(
                "dbo.SP_Restore_RegistrarResultado", conexion))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.Add("@IdSolicitud", SqlDbType.Int).Value = idSolicitud;
                comando.Parameters.Add("@Estado", SqlDbType.VarChar, 20).Value = estado;
                string detalle = string.IsNullOrWhiteSpace(observaciones)
                    ? null
                    : (observaciones.Length > 1000
                        ? observaciones.Substring(0, 1000)
                        : observaciones);
                comando.Parameters.Add("@Observaciones", SqlDbType.NVarChar, 1000).Value =
                    detalle == null ? (object)DBNull.Value : detalle;
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Restaura un backup autorizado en una base de datos de recuperación
        /// independiente. Nunca restaura sobre AuraBeautyDB.
        /// Cada solicitud utiliza un nombre de base diferente.
        /// </summary>
        /// <summary>
        /// Sobrecarga para pruebas manuales desde la interfaz.
        /// Genera un identificador positivo para que la base de prueba tenga nombre propio.
        /// Para solicitudes autorizadas debe usarse la sobrecarga que recibe idSolicitud.
        /// </summary>
        public string RestaurarBackupEnBasePrueba2(string rutaArchivo)
        {
            int idPrueba = (int)(DateTime.UtcNow.Ticks % 2000000000L) + 1;
            return RestaurarBackupEnBasePrueba2(rutaArchivo, idPrueba);
        }

        public string RestaurarBackupEnBasePrueba2(string rutaArchivo, int idSolicitud)
        {
            if (idSolicitud <= 0)
                throw new ArgumentException("El identificador de solicitud no es válido.");

            if (string.IsNullOrWhiteSpace(rutaArchivo) || !Path.IsPathRooted(rutaArchivo))
                throw new ArgumentException("La ruta del backup debe ser absoluta.");

            if (!string.Equals(Path.GetExtension(rutaArchivo), ".bak", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("El archivo debe tener extensión .bak.");

            // La ruta del .bak debe ser accesible para el SERVICIO de SQL Server,
            // no solamente para el usuario que ejecuta la aplicación.
            SqlConnectionStringBuilder constructor = new SqlConnectionStringBuilder(cadenaConexion);
            constructor.InitialCatalog = "master";
            string cadenaMaster = constructor.ConnectionString;

            // Nombre controlado y único para cada solicitud de restauración.
            string nombreBase = "AuraBeautyDB_Recuperacion_" + idSolicitud;
            string directorioDatos = null;

            // Primero intentamos obtener la carpeta predeterminada de datos de la instancia.
            using (SqlConnection conexion = new SqlConnection(cadenaMaster))
            {
                conexion.Open();

                using (SqlCommand comandoRuta = new SqlCommand(@"
                    SELECT CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS NVARCHAR(4000));", conexion))
                {
                    object valor = comandoRuta.ExecuteScalar();
                    if (valor != null && valor != DBNull.Value)
                        directorioDatos = Convert.ToString(valor);
                }

                // Alternativa para instalaciones que no informan InstanceDefaultDataPath.
                if (string.IsNullOrWhiteSpace(directorioDatos))
                {
                    using (SqlCommand comandoFallback = new SqlCommand(@"
                        SELECT TOP (1) physical_name
                        FROM sys.master_files
                        WHERE database_id = DB_ID('master') AND file_id = 1;", conexion))
                    {
                        object rutaMaster = comandoFallback.ExecuteScalar();
                        if (rutaMaster == null || rutaMaster == DBNull.Value)
                            throw new InvalidOperationException(
                                "SQL Server no informó una carpeta de datos y no se pudo determinar una ruta alternativa.");

                        directorioDatos = Path.GetDirectoryName(Convert.ToString(rutaMaster));
                    }
                }

                if (string.IsNullOrWhiteSpace(directorioDatos))
                    throw new InvalidOperationException(
                        "No se pudo determinar la carpeta de datos de SQL Server.");

                // No se sobrescribe ni se elimina una base existente.
                using (SqlCommand comprobar = new SqlCommand(
                    "SELECT COUNT(*) FROM sys.databases WHERE name = @NombreBase;", conexion))
                {
                    comprobar.Parameters.Add("@NombreBase", SqlDbType.NVarChar, 128).Value = nombreBase;
                    if (Convert.ToInt32(comprobar.ExecuteScalar()) != 0)
                        throw new InvalidOperationException(
                            "Ya existe la base de recuperación " + nombreBase +
                            ". No se sobrescribió ni se eliminó ninguna base.");
                }
            }

            // Consultar los nombres lógicos reales dentro del .bak. Así no dependemos
            // de que ambos equipos tengan los mismos nombres lógicos de archivos.
            DataTable archivosBackup = new DataTable();
            using (SqlConnection conexion = new SqlConnection(cadenaMaster))
            using (SqlCommand comando = new SqlCommand(
                "RESTORE FILELISTONLY FROM DISK = @RutaArchivo;", conexion))
            using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
            {
                comando.Parameters.Add("@RutaArchivo", SqlDbType.NVarChar, 4000).Value = rutaArchivo;
                comando.CommandTimeout = 0;
                conexion.Open();
                adaptador.Fill(archivosBackup);
            }

            if (!archivosBackup.Columns.Contains("LogicalName") ||
                !archivosBackup.Columns.Contains("Type") || archivosBackup.Rows.Count == 0)
            {
                throw new InvalidOperationException(
                    "SQL Server no pudo leer los archivos internos del backup.");
            }

            List<string> movimientos = new List<string>();
            int numeroDatos = 0;
            int numeroLogs = 0;

            foreach (DataRow archivo in archivosBackup.Rows)
            {
                string nombreLogico = Convert.ToString(archivo["LogicalName"]);
                string tipo = Convert.ToString(archivo["Type"]);

                if (string.IsNullOrWhiteSpace(nombreLogico))
                    continue;

                string nombreFisico;
                if (tipo == "L")
                {
                    numeroLogs++;
                    nombreFisico = nombreBase +
                        (numeroLogs == 1 ? "_log.ldf" : "_log" + numeroLogs + ".ldf");
                }
                else if (tipo == "D")
                {
                    numeroDatos++;
                    nombreFisico = nombreBase +
                        (numeroDatos == 1 ? ".mdf" : "_datos" + numeroDatos + ".ndf");
                }
                else
                {
                    // Los tipos de archivo especiales no se mueven como MDF/LDF.
                    continue;
                }

                string destino = Path.Combine(directorioDatos, nombreFisico);
                string logicoEscapado = nombreLogico.Replace("'", "''");
                string destinoEscapado = destino.Replace("'", "''");
                movimientos.Add("MOVE N'" + logicoEscapado + "' TO N'" + destinoEscapado + "'");
            }

            if (movimientos.Count == 0 || numeroDatos == 0 || numeroLogs == 0)
                throw new InvalidOperationException(
                    "El backup no contiene los archivos de datos y registro esperados.");

            string consulta = "RESTORE DATABASE [" + nombreBase + "] " +
                "FROM DISK = @RutaArchivo WITH " + string.Join(", ", movimientos) +
                ", RECOVERY, CHECKSUM, STATS = 10;";

            using (SqlConnection conexion = new SqlConnection(cadenaMaster))
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.Add("@RutaArchivo", SqlDbType.NVarChar, 4000).Value = rutaArchivo;
                comando.CommandTimeout = 0;
                conexion.Open();
                comando.ExecuteNonQuery();
            }

            return nombreBase;
        }

    }
}
