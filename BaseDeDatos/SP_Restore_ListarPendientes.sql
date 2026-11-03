
USE AuraBeautyDB;
GO

/*
=====================================================
AURA BEAUTY
VISIBILIDAD DE SOLICITUDES DE RESTAURACIÓN

Regla:
Un administrador puede consultar las solicitudes
de otros administradores, pero no las propias.

No elimina ni modifica solicitudes existentes.
=====================================================
*/

-- ==============================================
-- 1. SOLICITUDES PENDIENTES
-- ==============================================

CREATE OR ALTER PROCEDURE dbo.SP_Restore_ListarPendientes
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
        THROW 50001,
            'El usuario no es un administrador activo.',
            1;
    END;

    SELECT
        rs.id_solicitud AS IdSolicitud,
        rs.id_backup AS IdBackup,
        b.nombre_archivo AS NombreArchivo,
        b.ruta_archivo AS RutaArchivo,
        rs.id_solicitante AS IdSolicitante,
        u.nombre + ' ' + u.apellido AS Solicitante,
        rs.fecha_solicitud AS FechaSolicitud,
        rs.estado AS Estado,
        rs.observaciones AS Observaciones
    FROM dbo.RestauracionSolicitud AS rs
    INNER JOIN dbo.BackupRegistro AS b
        ON rs.id_backup = b.id_backup
    INNER JOIN dbo.Usuario AS u
        ON rs.id_solicitante = u.id_usuario
    WHERE rs.estado = 'Pendiente'
      AND rs.id_solicitante <> @IdUsuario
    ORDER BY rs.fecha_solicitud ASC;
END;
GO

-- ==============================================
-- 2. HISTORIAL DE SOLICITUDES
-- ==============================================

CREATE OR ALTER PROCEDURE dbo.SP_Restore_ListarHistorial
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
        THROW 50010,
            'Solo un administrador activo puede consultar el historial.',
            1;
    END;

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
    WHERE rs.id_solicitante <> @IdUsuario
    ORDER BY rs.fecha_solicitud DESC;
END;
GO

-- ==============================================
-- 3. CONSULTA INDIVIDUAL DE UNA SOLICITUD
-- ==============================================

CREATE OR ALTER PROCEDURE dbo.SP_Restore_ConsultarEstado
    @IdSolicitud INT,
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @IdSolicitud IS NULL OR @IdSolicitud <= 0
        THROW 50051,
            'El identificador de la solicitud no es válido.',
            1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
          AND deleted_at IS NULL
    )
        THROW 50053,
            'Solo un administrador activo puede consultar el estado.',
            1;

    SELECT
        r.id_solicitud AS IdSolicitud,
        r.id_backup AS IdBackup,
        b.nombre_archivo AS NombreArchivo,
        b.ruta_archivo AS RutaArchivo,
        r.id_solicitante AS IdSolicitante,
        LTRIM(RTRIM(
            CONCAT(us.nombre, ' ', us.apellido)
        )) AS Solicitante,
        r.id_autorizador AS IdAutorizador,
        LTRIM(RTRIM(
            CONCAT(ua.nombre, ' ', ua.apellido)
        )) AS Autorizador,
        r.fecha_solicitud AS FechaSolicitud,
        r.fecha_resolucion AS FechaResolucion,
        r.estado AS Estado,
        r.observaciones AS Observaciones
    FROM dbo.RestauracionSolicitud AS r
    INNER JOIN dbo.BackupRegistro AS b
        ON r.id_backup = b.id_backup
    INNER JOIN dbo.Usuario AS us
        ON r.id_solicitante = us.id_usuario
    LEFT JOIN dbo.Usuario AS ua
        ON r.id_autorizador = ua.id_usuario
    WHERE r.id_solicitud = @IdSolicitud
      AND r.id_solicitante <> @IdUsuario;
END;
GO
