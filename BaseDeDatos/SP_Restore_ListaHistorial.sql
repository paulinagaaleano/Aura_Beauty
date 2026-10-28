-- Crear o actualizar el procedimiento que consulta
-- el historial completo de solicitudes de restauración.

USE AuraBeautyDB;
GO

CREATE OR ALTER PROCEDURE dbo.SP_Restore_ListarHistorial
    @IdUsuario INT
AS
BEGIN
    -- Evitar mensajes adicionales de filas afectadas.
    SET NOCOUNT ON;

    -- Verificar que el usuario que consulta sea
    -- un administrador activo.
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

    -- Recuperar todas las solicitudes y sus datos relacionados.
    SELECT
        rs.id_solicitud AS IdSolicitud,
        rs.id_backup AS IdBackup,
        b.nombre_archivo AS NombreArchivo,
        b.fecha_creacion AS FechaBackup,
        rs.id_solicitante AS IdSolicitante,
        CONCAT(us.nombre, ' ', us.apellido) AS Solicitante,
        rs.id_autorizador AS IdAutorizador,
        CASE
            WHEN ua.id_usuario IS NULL THEN NULL
            ELSE CONCAT(ua.nombre, ' ', ua.apellido)
        END AS Autorizador,
        rs.fecha_solicitud AS FechaSolicitud,
        rs.fecha_resolucion AS FechaResolucion,
        rs.estado AS Estado,
        rs.observaciones AS Observaciones
    FROM dbo.RestauracionSolicitud AS rs
    INNER JOIN dbo.BackupRegistro AS b
        ON b.id_backup = rs.id_backup
    INNER JOIN dbo.Usuario AS us
        ON us.id_usuario = rs.id_solicitante
    LEFT JOIN dbo.Usuario AS ua
        ON ua.id_usuario = rs.id_autorizador

    -- Mostrar primero las solicitudes más recientes.
    ORDER BY rs.fecha_solicitud DESC;
END;
GO

