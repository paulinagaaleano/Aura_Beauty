
USE [AuraBeautyDB];
GO

CREATE OR ALTER PROCEDURE dbo.SP_Restore_ListarHistorial
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar administrador activo.
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

    -- Historial de solicitudes conservadas.
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

    FROM AuraBeautyDB_Admin.dbo.SolicitudHistorial h

    UNION ALL

    -- Ejecuciones que sobreviven en la auditoría.
    SELECT
        a.IdSolicitud,
        a.IdBackupOrigen AS IdBackup,

        CAST(a.RutaBackupOrigen AS NVARCHAR(260))
            AS NombreArchivo,

        CAST(NULL AS DATETIME2(7)) AS FechaBackup,

        CAST(NULL AS INT) AS IdSolicitante,

        CAST(NULL AS NVARCHAR(260)) AS Solicitante,

        a.IdAutorizador,

        CAST(
            LTRIM(RTRIM(CONCAT(u.nombre, ' ', u.apellido)))
            AS NVARCHAR(260)
        ) AS Autorizador,

        a.FechaSolicitudOrigen AS FechaSolicitud,
        a.FechaFin AS FechaResolucion,

        CAST(a.Estado AS VARCHAR(20)) AS Estado,

        CAST(a.Detalle AS NVARCHAR(1000)) AS Observaciones

    FROM AuraBeautyDB_Admin.dbo.RestauracionAuditoria a

    LEFT JOIN dbo.Usuario u
        ON u.id_usuario = a.IdAutorizador

    ORDER BY FechaSolicitud DESC;
END;
GO
