
-- Seleccionar la base de datos del sistema Aura Beauty.
USE AuraBeautyDB;
GO

-- Crear un procedimiento para listar las solicitudes
-- de restauración que todavía están pendientes.
CREATE OR ALTER PROCEDURE dbo.SP_Restore_ListarPendientes
    -- Recibe el ID del administrador que consulta.
    @IdUsuario INT
AS
BEGIN
    -- Evitar mensajes adicionales de filas afectadas.
    SET NOCOUNT ON;

    -- Verificar que el usuario exista y tenga rol administrador.
    -- También se comprueba que su cuenta no esté eliminada.
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
          AND deleted_at IS NULL
    )
    BEGIN
        -- Detener la ejecución si el usuario no está autorizado.
        THROW 50001,
              'El usuario no es un administrador activo.',
              1;
    END;

    -- Obtener las solicitudes pendientes junto con
    -- los datos del respaldo y del administrador solicitante.
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

    -- Relacionar la solicitud con el respaldo elegido.
    INNER JOIN dbo.BackupRegistro AS b
        ON rs.id_backup = b.id_backup

    -- Obtener el nombre del usuario que solicitó restaurar.
    INNER JOIN dbo.Usuario AS u
        ON rs.id_solicitante = u.id_usuario

    -- Mostrar solamente solicitudes pendientes.
    WHERE rs.estado = 'Pendiente'

    -- Ordenar desde la solicitud más antigua.
    ORDER BY rs.fecha_solicitud ASC;
END;
GO
