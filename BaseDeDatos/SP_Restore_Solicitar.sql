
USE [AuraBeautyDB];
GO

-- ============================================================
-- PROCEDIMIENTO: SP_Restore_Solicitar
--
-- OBJETIVO:
-- Registrar una solicitud de restauración de un backup.
--
-- FUNCIONAMIENTO:
-- 1. Recibe el backup seleccionado y el usuario solicitante.
-- 2. Comprueba que el usuario sea administrador.
-- 3. Comprueba que el backup exista y haya sido registrado
--    como exitoso.
-- 4. Crea una solicitud con estado "Pendiente".
--
-- IMPORTANTE:
-- Este procedimiento NO restaura la base de datos.
-- Solo registra la solicitud para su autorización posterior.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_Restore_Solicitar

    -- Identificador de la copia que se desea restaurar.
    @IdBackup INT,

    -- Administrador que solicita la restauración.
    @IdSolicitante INT,

    -- Motivo opcional de la solicitud.
    @Observaciones NVARCHAR(1000) = NULL

AS
BEGIN

    -- Evita mensajes adicionales de filas afectadas.
    SET NOCOUNT ON;

    -- ========================================================
    -- 1. VALIDAR LOS IDENTIFICADORES RECIBIDOS
    -- ========================================================

    IF @IdBackup IS NULL OR @IdBackup <= 0
    BEGIN
        THROW 50021,
              'El identificador del backup no es válido.',
              1;
    END;

    IF @IdSolicitante IS NULL OR @IdSolicitante <= 0
    BEGIN
        THROW 50022,
              'El identificador del solicitante no es válido.',
              1;
    END;

    -- ========================================================
    -- 2. COMPROBAR QUE EL SOLICITANTE SEA ADMINISTRADOR
    --
    -- En Aura Beauty, id_rol = 1 corresponde al Administrador.
    -- La autenticación de la sesión también se debe verificar
    -- en C#; este parámetro no prueba por sí solo la identidad.
    -- ========================================================

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdSolicitante
          AND id_rol = 1
    )
    BEGIN
        THROW 50023,
              'Solo un administrador puede solicitar una restauración.',
              1;
    END;

    -- ========================================================
    -- 3. VERIFICAR QUE EL BACKUP ESTÉ REGISTRADO COMO EXITOSO
    --
    -- Esto comprueba el estado guardado en la tabla.
    -- Más adelante también verificaremos que el archivo exista
    -- y que SQL Server pueda leerlo antes de restaurar.
    -- ========================================================

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.BackupRegistro
        WHERE id_backup = @IdBackup
          AND estado = 'Exitoso'
    )
    BEGIN
        THROW 50024,
              'El backup no existe o no está registrado como exitoso.',
              1;
    END;

    -- ========================================================
    -- 4. CREAR LA SOLICITUD DE RESTAURACIÓN
    --
    -- El estado se establece como "Pendiente".
    -- id_autorizador y fecha_resolucion quedan NULL hasta
    -- que otro administrador intervenga.
    -- ========================================================

    INSERT INTO dbo.RestauracionSolicitud
    (
        id_backup,
        id_solicitante,
        estado,
        observaciones
    )
    VALUES
    (
        @IdBackup,
        @IdSolicitante,
        'Pendiente',
        @Observaciones
    );

    -- ========================================================
    -- 5. DEVOLVER EL ID DE LA SOLICITUD CREADA
    --
    -- C# podrá utilizar este identificador para consultar
    -- el estado o gestionar la autorización posteriormente.
    -- ========================================================

    SELECT
        CAST(SCOPE_IDENTITY() AS INT) AS IdSolicitud;

END;
GO
