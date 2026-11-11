
USE [AuraBeautyDB];
GO

-- ============================================================
-- TRIGGER: TR_BackupRegistro_Historial
--
-- Registra altas y modificaciones de backups.
-- ============================================================

CREATE OR ALTER TRIGGER dbo.TR_BackupRegistro_Historial
ON dbo.BackupRegistro
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Actualizar registros históricos existentes.
    UPDATE h
    SET
        h.NombreArchivo = i.nombre_archivo,
        h.IdUsuario = i.id_usuario,
        h.UsuarioNombre =
            LTRIM(RTRIM(CONCAT(u.nombre, ' ', u.apellido))),
        h.Estado = i.estado,
        h.Observaciones = i.observaciones,
        h.FechaArchivado = SYSDATETIME()

    FROM AuraBeautyDB_Admin.dbo.BackupHistorial AS h

    INNER JOIN inserted AS i
        ON h.IdBackupOriginal = i.id_backup
       AND h.FechaCreacion = i.fecha_creacion
       AND h.RutaArchivo = i.ruta_archivo

    LEFT JOIN dbo.Usuario AS u
        ON u.id_usuario = i.id_usuario;

    -- Guardar nuevos registros.
    INSERT INTO AuraBeautyDB_Admin.dbo.BackupHistorial
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
        i.id_backup,
        i.nombre_archivo,
        i.ruta_archivo,
        i.fecha_creacion,
        i.id_usuario,
        LTRIM(RTRIM(CONCAT(u.nombre, ' ', u.apellido))),
        i.estado,
        i.observaciones

    FROM inserted AS i

    LEFT JOIN dbo.Usuario AS u
        ON u.id_usuario = i.id_usuario

    WHERE NOT EXISTS
    (
        SELECT 1
        FROM AuraBeautyDB_Admin.dbo.BackupHistorial AS h
            WITH (UPDLOCK, HOLDLOCK)
        WHERE h.IdBackupOriginal = i.id_backup
          AND h.FechaCreacion = i.fecha_creacion
          AND h.RutaArchivo = i.ruta_archivo
    );
END;
GO

-- ============================================================
-- TRIGGER: TR_RestauracionSolicitud_Historial
--
-- Captura:
-- - Nueva solicitud pendiente.
-- - Solicitud autorizada.
-- - Solicitud rechazada.
-- - Otros cambios de estado.
-- ============================================================

CREATE OR ALTER TRIGGER
dbo.TR_RestauracionSolicitud_Historial

ON dbo.RestauracionSolicitud

AFTER INSERT, UPDATE

AS
BEGIN
    SET NOCOUNT ON;

    -- Actualizar solicitudes históricas existentes.
    UPDATE h
    SET
        h.NombreArchivo = b.nombre_archivo,
        h.FechaBackup = b.fecha_creacion,

        h.Solicitante =
            LTRIM(RTRIM(CONCAT(us.nombre, ' ', us.apellido))),

        h.IdAutorizador = i.id_autorizador,

        h.Autorizador =
            CASE
                WHEN ua.id_usuario IS NULL THEN NULL
                ELSE LTRIM(RTRIM(
                    CONCAT(ua.nombre, ' ', ua.apellido)
                ))
            END,

        h.FechaResolucion = i.fecha_resolucion,
        h.Estado = i.estado,
        h.Observaciones = i.observaciones,
        h.FechaArchivado = SYSDATETIME()

    FROM AuraBeautyDB_Admin.dbo.SolicitudHistorial AS h

    INNER JOIN inserted AS i
        ON h.IdSolicitudOriginal = i.id_solicitud
       AND h.IdBackupOriginal = i.id_backup
       AND h.FechaSolicitud = i.fecha_solicitud

    LEFT JOIN dbo.BackupRegistro AS b
        ON b.id_backup = i.id_backup

    LEFT JOIN dbo.Usuario AS us
        ON us.id_usuario = i.id_solicitante

    LEFT JOIN dbo.Usuario AS ua
        ON ua.id_usuario = i.id_autorizador;

    -- Insertar solicitudes nuevas.
    INSERT INTO AuraBeautyDB_Admin.dbo.SolicitudHistorial
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
        i.id_solicitud,
        i.id_backup,
        i.fecha_solicitud,
        b.nombre_archivo,
        b.fecha_creacion,
        i.id_solicitante,

        LTRIM(RTRIM(
            CONCAT(us.nombre, ' ', us.apellido)
        )),

        i.id_autorizador,

        CASE
            WHEN ua.id_usuario IS NULL THEN NULL
            ELSE LTRIM(RTRIM(
                CONCAT(ua.nombre, ' ', ua.apellido)
            ))
        END,

        i.fecha_resolucion,
        i.estado,
        i.observaciones

    FROM inserted AS i

    LEFT JOIN dbo.BackupRegistro AS b
        ON b.id_backup = i.id_backup

    LEFT JOIN dbo.Usuario AS us
        ON us.id_usuario = i.id_solicitante

    LEFT JOIN dbo.Usuario AS ua
        ON ua.id_usuario = i.id_autorizador

    WHERE NOT EXISTS
    (
        SELECT 1
        FROM AuraBeautyDB_Admin.dbo.SolicitudHistorial AS h
            WITH (UPDLOCK, HOLDLOCK)

        WHERE h.IdSolicitudOriginal = i.id_solicitud
          AND h.IdBackupOriginal = i.id_backup
          AND h.FechaSolicitud = i.fecha_solicitud
    );
END;
GO
