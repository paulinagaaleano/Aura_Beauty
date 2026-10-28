
USE [AuraBeautyDB];
GO

-- ============================================================
-- PROCEDIMIENTO: SP_Restore_ConsultarEstado
--
-- OBJETIVO:
-- Consultar el estado y los datos de una solicitud de
-- restauración de una copia de seguridad.
--
-- INFORMACIÓN DEVUELTA:
-- - Datos de la solicitud.
-- - Backup que se desea restaurar.
-- - Administrador solicitante.
-- - Administrador que autorizó o rechazó.
-- - Fechas y estado actual.
-- - Observaciones registradas.
--
-- IMPORTANTE:
-- Este procedimiento solamente consulta información.
-- No modifica registros ni ejecuta restauraciones.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_Restore_ConsultarEstado

    -- Identificador de la solicitud que se quiere consultar.
    @IdSolicitud INT,

    -- Identificador del administrador que realiza la consulta.
    @IdUsuario INT

AS
BEGIN

    -- Evita mensajes adicionales sobre filas afectadas.
    SET NOCOUNT ON;

    -- ========================================================
    -- 1. VALIDAR LOS IDENTIFICADORES RECIBIDOS
    -- ========================================================

    IF @IdSolicitud IS NULL OR @IdSolicitud <= 0
    BEGIN
        THROW 50051,
              'El identificador de la solicitud no es válido.',
              1;
    END;

    IF @IdUsuario IS NULL OR @IdUsuario <= 0
    BEGIN
        THROW 50052,
              'El identificador del usuario no es válido.',
              1;
    END;

    -- ========================================================
    -- 2. VERIFICAR QUE EL USUARIO SEA ADMINISTRADOR
    --
    -- En Aura Beauty, id_rol = 1 corresponde al Administrador.
    -- La identidad del usuario también debe validarse desde C#.
    -- ========================================================

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
    )
    BEGIN
        THROW 50053,
              'Solo un administrador puede consultar el estado.',
              1;
    END;

    -- ========================================================
    -- 3. CONSULTAR LA SOLICITUD
    --
    -- Se relacionan las tablas para obtener información legible
    -- sobre el backup y los usuarios involucrados.
    --
    -- LEFT JOIN permite mostrar la solicitud aunque todavía
    -- no tenga un autorizador asignado.
    -- ========================================================

    SELECT
        -- Identificador único de la solicitud.
        r.id_solicitud AS IdSolicitud,

        -- Identificador del backup solicitado.
        r.id_backup AS IdBackup,

        -- Nombre del archivo de backup.
        b.nombre_archivo AS NombreArchivo,

        -- Ruta física del archivo.
        b.ruta_archivo AS RutaArchivo,

        -- Usuario que solicitó la restauración.
        r.id_solicitante AS IdSolicitante,

        -- Nombre completo del solicitante.
        LTRIM(RTRIM(
            CONCAT(us.nombre, ' ', us.apellido)
        )) AS Solicitante,

        -- Usuario que autorizó o rechazó la solicitud.
        r.id_autorizador AS IdAutorizador,

        -- Nombre completo del autorizador, si existe.
        LTRIM(RTRIM(
            CONCAT(ua.nombre, ' ', ua.apellido)
        )) AS Autorizador,

        -- Fecha en la que se creó la solicitud.
        r.fecha_solicitud AS FechaSolicitud,

        -- Fecha en la que se resolvió, si corresponde.
        r.fecha_resolucion AS FechaResolucion,

        -- Estado actual de la solicitud.
        r.estado AS Estado,

        -- Observaciones o motivo del rechazo.
        r.observaciones AS Observaciones

    FROM dbo.RestauracionSolicitud AS r

    -- Obtener información del backup seleccionado.
    INNER JOIN dbo.BackupRegistro AS b
        ON r.id_backup = b.id_backup

    -- Obtener el nombre del administrador solicitante.
    INNER JOIN dbo.Usuario AS us
        ON r.id_solicitante = us.id_usuario

    -- Obtener el nombre del administrador que resolvió
    -- la solicitud. Puede ser NULL si sigue pendiente.
    LEFT JOIN dbo.Usuario AS ua
        ON r.id_autorizador = ua.id_usuario

    -- Devolver únicamente la solicitud seleccionada.
    WHERE r.id_solicitud = @IdSolicitud;

    -- ========================================================
    -- 4. INTERPRETAR EL RESULTADO
    --
    -- Si la solicitud existe, se devuelve una fila.
    -- Si no existe, se devuelve un conjunto vacío.
    -- No se modifica ningún dato.
    -- ========================================================

END;
GO
