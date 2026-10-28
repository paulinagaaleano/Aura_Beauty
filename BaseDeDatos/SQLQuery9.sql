
USE [AuraBeautyDB];
GO

-- ============================================================
-- PROCEDIMIENTO: SP_Backup_Listar
--
-- OBJETIVO:
-- Consultar el historial de copias de seguridad registradas.
--
-- DATOS QUE DEVUELVE:
-- ID, nombre del archivo, ruta, fecha, usuario, estado
-- y observaciones.
--
-- IMPORTANTE:
-- Este procedimiento solo consulta información.
-- No crea, elimina ni restaura archivos.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_Backup_Listar
    -- ID del usuario que solicita la consulta.
    -- La capa de Negocio también deberá validar su sesión y rol.
    @IdUsuario INT
AS
BEGIN
    -- Evita mensajes adicionales sobre filas afectadas.
    SET NOCOUNT ON;

    -- ========================================================
    -- 1. VALIDAR EL IDENTIFICADOR DEL USUARIO
    -- ========================================================

    IF @IdUsuario IS NULL OR @IdUsuario <= 0
    BEGIN
        THROW 50011,
              'El identificador del usuario no es válido.',
              1;
    END;

    -- ========================================================
    -- 2. VERIFICAR QUE EL USUARIO EXISTA Y SEA ADMINISTRADOR
    --
    -- En Aura Beauty, id_rol = 1 corresponde al Administrador.
    -- Esta comprobación complementa, pero no reemplaza,
    -- la validación de sesión y permisos en C#.
    -- ========================================================

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
    )
    BEGIN
        THROW 50012,
              'Solo un administrador puede consultar los backups.',
              1;
    END;

    -- ========================================================
    -- 3. CONSULTAR EL HISTORIAL
    --
    -- Se relaciona BackupRegistro con Usuario para mostrar
    -- el nombre y apellido de quien realizó cada operación.
    --
    -- LEFT JOIN permite conservar el registro del backup
    -- aunque no hubiera datos coincidentes del usuario.
    -- La clave foránea normalmente impide esa situación.
    -- ========================================================

    SELECT
        -- Identificador del registro de backup.
        b.id_backup AS IdBackup,

        -- Nombre del archivo físico.
        b.nombre_archivo AS NombreArchivo,

        -- Ubicación del archivo.
        b.ruta_archivo AS RutaArchivo,

        -- Fecha y hora de registro.
        b.fecha_creacion AS FechaCreacion,

        -- Identificador del usuario responsable.
        b.id_usuario AS IdUsuario,

        -- Nombre completo del usuario.
        LTRIM(RTRIM(
            CONCAT(u.nombre, ' ', u.apellido)
        )) AS Usuario,

        -- Resultado de la operación.
        b.estado AS Estado,

        -- Información adicional o mensaje de error.
        b.observaciones AS Observaciones

    FROM dbo.BackupRegistro AS b

    LEFT JOIN dbo.Usuario AS u
        ON b.id_usuario = u.id_usuario

    -- Mostrar primero las copias más recientes.
    ORDER BY b.fecha_creacion DESC, b.id_backup DESC;

END;
GO
