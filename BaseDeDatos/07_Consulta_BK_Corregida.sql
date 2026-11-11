
USE [AuraBeautyDB];
GO

-- ============================================================
-- SP_Restore_ListarHistorial
--
-- Devuelve TODAS las solicitudes registradas
-- en el historial administrativo.
--
-- No filtra por estado.
-- No excluye solicitudes del administrador actual.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_Restore_ListarHistorial
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar administrador activo.
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
          AND deleted_at IS NULL
    )
    BEGIN
        THROW 50010,
            'Solo un administrador activo puede consultar el historial.',
            1;
    END;

    -- Consultar el historial permanente.
    SELECT
        h.IdSolicitudOriginal AS IdSolicitud,

        h.IdBackupOriginal AS IdBackup,

        h.NombreArchivo,

        h.FechaBackup,

        h.IdSolicitante,

        h.Solicitante,

        h.IdAutorizador,

        h.Autorizador,

        h.FechaSolicitud,

        h.FechaResolucion,

        h.Estado,

        h.Observaciones

    FROM AuraBeautyDB_Admin.dbo.SolicitudHistorial AS h

    ORDER BY
        h.FechaSolicitud DESC,
        h.FechaArchivado DESC;
END;
GO

-- ============================================================
-- SP_Backup_ListarHistorialPermanente
--
-- Consulta histórica de todos los backups archivados.
--
-- No reemplaza SP_Backup_Listar, utilizado para operar
-- sobre los backups vigentes de AuraBeautyDB.
-- ============================================================

CREATE OR ALTER PROCEDURE
dbo.SP_Backup_ListarHistorialPermanente
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
          AND deleted_at IS NULL
    )
    BEGIN
        THROW 50012,
            'Solo un administrador activo puede consultar los backups.',
            1;
    END;

    SELECT
        h.IdBackupOriginal AS IdBackup,
        h.NombreArchivo,
        h.RutaArchivo,
        h.FechaCreacion,
        h.IdUsuario,
        h.UsuarioNombre AS Usuario,
        h.Estado,
        h.Observaciones

    FROM AuraBeautyDB_Admin.dbo.BackupHistorial AS h

    ORDER BY
        h.FechaCreacion DESC,
        h.FechaArchivado DESC;
END;
GO
