
USE [AuraBeautyDB];
GO

-- ============================================================
-- PROCEDIMIENTO: SP_Restore_Autorizar
--
-- OBJETIVO:
-- Autorizar una solicitud de restauración mediante un segundo
-- administrador, distinto del usuario que hizo la solicitud.
--
-- VALIDACIONES:
-- 1. La solicitud debe existir.
-- 2. La solicitud debe estar Pendiente.
-- 3. El autorizador debe ser un Administrador (id_rol = 1).
-- 4. El autorizador no puede ser el solicitante.
--
-- IMPORTANTE:
-- Este procedimiento registra la autorización.
-- NO ejecuta la restauración física de la base de datos.
-- Las credenciales del autorizador deben validarse previamente
-- desde C# mediante el mecanismo de autenticación del sistema.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_Restore_Autorizar

    -- Identificador de la solicitud que se desea autorizar.
    @IdSolicitud INT,

    -- Identificador del segundo administrador.
    @IdAutorizador INT

AS
BEGIN

    -- Evita mensajes adicionales sobre filas afectadas.
    SET NOCOUNT ON;

    -- Permite revertir los cambios si ocurre un error.
    SET XACT_ABORT ON;

    -- ========================================================
    -- 1. VALIDAR LOS IDENTIFICADORES
    -- ========================================================

    IF @IdSolicitud IS NULL OR @IdSolicitud <= 0
    BEGIN
        THROW 50031,
              'El identificador de la solicitud no es válido.',
              1;
    END;

    IF @IdAutorizador IS NULL OR @IdAutorizador <= 0
    BEGIN
        THROW 50032,
              'El identificador del autorizador no es válido.',
              1;
    END;

    -- ========================================================
    -- 2. VERIFICAR QUE EL AUTORIZADOR SEA ADMINISTRADOR
    --
    -- En Aura Beauty, id_rol = 1 corresponde al Administrador.
    -- Esta comprobación del rol no sustituye la autenticación.
    -- ========================================================

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdAutorizador
          AND id_rol = 1
    )
    BEGIN
        THROW 50033,
              'El autorizador no existe o no es Administrador.',
              1;
    END;

    -- ========================================================
    -- 3. INICIAR UNA TRANSACCIÓN
    --
    -- La transacción agrupa la validación y la actualización.
    -- Se utiliza un bloqueo sobre la solicitud para evitar que
    -- dos operaciones simultáneas la autoricen a la vez.
    -- ========================================================

    BEGIN TRY

        BEGIN TRANSACTION;

        -- Variable para guardar al solicitante.
        DECLARE @IdSolicitante INT;

        -- Variable para guardar el estado actual.
        DECLARE @EstadoActual VARCHAR(20);

        -- Consultar la solicitud y bloquear su fila mientras
        -- se realiza esta operación.
        SELECT
            @IdSolicitante = id_solicitante,
            @EstadoActual = estado
        FROM dbo.RestauracionSolicitud
            WITH (UPDLOCK, HOLDLOCK)
        WHERE id_solicitud = @IdSolicitud;

        -- ====================================================
        -- 4. COMPROBAR QUE LA SOLICITUD EXISTA
        -- Si no se encontró una fila, las variables serán NULL.
        -- ====================================================

        IF @IdSolicitante IS NULL
        BEGIN
            THROW 50034,
                  'La solicitud de restauración no existe.',
                  1;
        END;

        -- ====================================================
        -- 5. IMPEDIR LA AUTOAUTORIZACIÓN
        -- El solicitante y el autorizador deben ser distintos.
        -- ====================================================

        IF @IdSolicitante = @IdAutorizador
        BEGIN
            THROW 50035,
                  'No puede autorizar su propia solicitud.',
                  1;
        END;

        -- ====================================================
        -- 6. COMPROBAR QUE LA SOLICITUD SIGA PENDIENTE
        -- No se pueden autorizar solicitudes ya resueltas.
        -- ====================================================

        IF @EstadoActual <> 'Pendiente'
        BEGIN
            THROW 50036,
                  'La solicitud ya fue resuelta o no está pendiente.',
                  1;
        END;

        -- ====================================================
        -- 7. REGISTRAR LA AUTORIZACIÓN
        --
        -- Se guarda el autorizador, la fecha de resolución
        -- y el nuevo estado de la solicitud.
        -- ====================================================

        UPDATE dbo.RestauracionSolicitud
        SET
            id_autorizador = @IdAutorizador,
            fecha_resolucion = SYSDATETIME(),
            estado = 'Autorizada'
        WHERE id_solicitud = @IdSolicitud;

        -- Confirmar los cambios realizados.
        COMMIT TRANSACTION;

        -- ====================================================
        -- 8. DEVOLVER UNA CONFIRMACIÓN
        -- C# podrá utilizar este resultado para informar
        -- que la solicitud quedó autorizada.
        -- ====================================================

        SELECT
            id_solicitud AS IdSolicitud,
            id_backup AS IdBackup,
            id_solicitante AS IdSolicitante,
            id_autorizador AS IdAutorizador,
            fecha_solicitud AS FechaSolicitud,
            fecha_resolucion AS FechaResolucion,
            estado AS Estado
        FROM dbo.RestauracionSolicitud
        WHERE id_solicitud = @IdSolicitud;

    END TRY
    BEGIN CATCH

        -- Si ocurrió un error y la transacción sigue abierta,
        -- revertir los cambios para no dejar datos incompletos.
        IF XACT_STATE() <> 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        -- Volver a generar el error original para que C#
        -- pueda recibirlo y mostrar un mensaje apropiado.
        THROW;

    END CATCH;

END;
GO
