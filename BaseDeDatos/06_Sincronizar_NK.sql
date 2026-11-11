
USE [AuraBeautyDB_Admin];
GO

CREATE OR ALTER PROCEDURE dbo.SP_SincronizarHistorial
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    -- ========================================================
    -- 1. ACTUALIZAR BACKUPS YA ARCHIVADOS
    -- ========================================================

    UPDATE h
    SET
        h.NombreArchivo = b.nombre_archivo,
        h.IdUsuario = b.id_usuario,
        h.UsuarioNombre =
            LTRIM(RTRIM(CONCAT(u.nombre, ' ', u.apellido))),
        h.Estado = b.estado,
        h.Observaciones = b.observaciones,
        h.FechaArchivado = SYSDATETIME()

    FROM dbo.BackupHistorial AS h

    INNER JOIN AuraBeautyDB.dbo.BackupRegistro AS b
        ON h.IdBackupOriginal = b.id_backup
       AND h.FechaCreacion = b.fecha_creacion
       AND h.RutaArchivo = b.ruta_archivo

    LEFT JOIN AuraBeautyDB.dbo.Usuario AS u
        ON u.id_usuario = b.id_usuario;

    -- ========================================================
    -- 2. AGREGAR BACKUPS QUE NO ESTEN ARCHIVADOS
    -- ========================================================

    INSERT INTO dbo.BackupHistorial
    (
        IdBackupOriginal,
        NombreArchivo,
        RutaArchivo,
        FechaCreacion,
        IdUsuario,
        UsuarioNombre,
        Estado,
        Observaciones
    )
    SELECT
        b.id_backup,
        b.nombre_archivo,
        b.ruta_archivo,
        b.fecha_creacion,
        b.id_usuario,
        LTRIM(RTRIM(CONCAT(u.nombre, ' ', u.apellido))),
        b.estado,
        b.observaciones

    FROM AuraBeautyDB.dbo.BackupRegistro AS b

    LEFT JOIN AuraBeautyDB.dbo.Usuario AS u
        ON u.id_usuario = b.id_usuario

    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.BackupHistorial AS h
            WITH (UPDLOCK, HOLDLOCK)
        WHERE h.IdBackupOriginal = b.id_backup
          AND h.FechaCreacion = b.fecha_creacion
          AND h.RutaArchivo = b.ruta_archivo
    );

    -- ========================================================
    -- 3. ACTUALIZAR SOLICITUDES YA ARCHIVADAS
    -- ========================================================

    UPDATE h
    SET
        h.NombreArchivo = b.nombre_archivo,
        h.FechaBackup = b.fecha_creacion,

        h.Solicitante =
            LTRIM(RTRIM(CONCAT(
                us.nombre, ' ', us.apellido
            ))),

        h.IdAutorizador = r.id_autorizador,

        h.Autorizador =
            CASE
                WHEN ua.id_usuario IS NULL THEN NULL
                ELSE LTRIM(RTRIM(CONCAT(
                    ua.nombre, ' ', ua.apellido
                )))
            END,

        h.FechaResolucion = r.fecha_resolucion,
        h.Estado = r.estado,
        h.Observaciones = r.observaciones,
        h.FechaArchivado = SYSDATETIME()

    FROM dbo.SolicitudHistorial AS h

    INNER JOIN AuraBeautyDB.dbo.RestauracionSolicitud AS r
        ON h.IdSolicitudOriginal = r.id_solicitud
       AND h.IdBackupOriginal = r.id_backup
       AND h.FechaSolicitud = r.fecha_solicitud

    LEFT JOIN AuraBeautyDB.dbo.BackupRegistro AS b
        ON b.id_backup = r.id_backup

    LEFT JOIN AuraBeautyDB.dbo.Usuario AS us
        ON us.id_usuario = r.id_solicitante

    LEFT JOIN AuraBeautyDB.dbo.Usuario AS ua
        ON ua.id_usuario = r.id_autorizador;

    -- ========================================================
    -- 4. AGREGAR NUEVAS SOLICITUDES
    -- ========================================================

    INSERT INTO dbo.SolicitudHistorial
    (
        IdSolicitudOriginal,
        IdBackupOriginal,
        FechaSolicitud,
        NombreArchivo,
        FechaBackup,
        IdSolicitante,
        Solicitante,
        IdAutorizador,
        Autorizador,
        FechaResolucion,
        Estado,
        Observaciones
    )
    SELECT
        r.id_solicitud,
        r.id_backup,
        r.fecha_solicitud,
        b.nombre_archivo,
        b.fecha_creacion,
        r.id_solicitante,

        LTRIM(RTRIM(CONCAT(
            us.nombre, ' ', us.apellido
        ))),

        r.id_autorizador,

        CASE
            WHEN ua.id_usuario IS NULL THEN NULL
            ELSE LTRIM(RTRIM(CONCAT(
                ua.nombre, ' ', ua.apellido
            )))
        END,

        r.fecha_resolucion,
        r.estado,
        r.observaciones

    FROM AuraBeautyDB.dbo.RestauracionSolicitud AS r

    LEFT JOIN AuraBeautyDB.dbo.BackupRegistro AS b
        ON b.id_backup = r.id_backup

    LEFT JOIN AuraBeautyDB.dbo.Usuario AS us
        ON us.id_usuario = r.id_solicitante

    LEFT JOIN AuraBeautyDB.dbo.Usuario AS ua
        ON ua.id_usuario = r.id_autorizador

    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.SolicitudHistorial AS h
            WITH (UPDLOCK, HOLDLOCK)
        WHERE h.IdSolicitudOriginal = r.id_solicitud
          AND h.IdBackupOriginal = r.id_backup
          AND h.FechaSolicitud = r.fecha_solicitud
    );

    COMMIT TRANSACTION;
END;
GO
