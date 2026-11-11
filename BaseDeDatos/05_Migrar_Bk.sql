
USE [AuraBeautyDB];
GO

-- ============================================================
-- MIGRAR BACKUPS EXISTENTES
-- ============================================================

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
    b.id_backup,
    b.nombre_archivo,
    b.ruta_archivo,
    b.fecha_creacion,
    b.id_usuario,
    LTRIM(RTRIM(CONCAT(u.nombre, ' ', u.apellido))),
    b.estado,
    b.observaciones

FROM dbo.BackupRegistro AS b

LEFT JOIN dbo.Usuario AS u
    ON u.id_usuario = b.id_usuario

WHERE NOT EXISTS
(
    SELECT 1
    FROM AuraBeautyDB_Admin.dbo.BackupHistorial AS h
    WHERE h.IdBackupOriginal = b.id_backup
      AND h.FechaCreacion = b.fecha_creacion
      AND h.RutaArchivo = b.ruta_archivo
);
GO

-- ============================================================
-- MIGRAR SOLICITUDES EXISTENTES
-- ============================================================

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
    r.id_solicitud,
    r.id_backup,
    r.fecha_solicitud,
    b.nombre_archivo,
    b.fecha_creacion,
    r.id_solicitante,

    LTRIM(RTRIM(
        CONCAT(us.nombre, ' ', us.apellido)
    )),

    r.id_autorizador,

    CASE
        WHEN ua.id_usuario IS NULL THEN NULL
        ELSE LTRIM(RTRIM(
            CONCAT(ua.nombre, ' ', ua.apellido)
        ))
    END,

    r.fecha_resolucion,
    r.estado,
    r.observaciones

FROM dbo.RestauracionSolicitud AS r

LEFT JOIN dbo.BackupRegistro AS b
    ON b.id_backup = r.id_backup

LEFT JOIN dbo.Usuario AS us
    ON us.id_usuario = r.id_solicitante

LEFT JOIN dbo.Usuario AS ua
    ON ua.id_usuario = r.id_autorizador

WHERE NOT EXISTS
(
    SELECT 1
    FROM AuraBeautyDB_Admin.dbo.SolicitudHistorial AS h
    WHERE h.IdSolicitudOriginal = r.id_solicitud
      AND h.IdBackupOriginal = r.id_backup
      AND h.FechaSolicitud = r.fecha_solicitud
);
GO
