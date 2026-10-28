
USE [AuraBeautyDB];
GO

-- ============================================================
-- PROCEDIMIENTO: SP_Restore_Rechazar
--
-- OBJETIVO:
-- Rechazar una solicitud de restauración y registrar
-- quién la rechazó, cuándo lo hizo y el motivo.
--
-- VALIDACIONES:
-- 1. La solicitud debe existir.
-- 2. El administrador debe existir y tener id_rol = 1.
-- 3. El administrador debe ser distinto del solicitante.
-- 4. La solicitud debe encontrarse en estado Pendiente.
-- 5. El motivo del rechazo es obligatorio.
--
-- IMPORTANTE:
-- Este procedimiento no restaura ni modifica la base de datos
-- operativa. Solo actualiza el registro de la solicitud.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_Restore_Rechazar

    -- Identificador de la solicitud que se quiere rechazar.
    @IdSolicitud INT,

    -- Identificador del administrador que rechaza.
    @IdAutorizador INT,

    -- Motivo por el que se rechaza la restauración.
    @MotivoRechazo NVARCHAR(1000)

AS
BEGIN

    -- Evita mensajes adicionales sobre filas afectadas.
    SET NOCOUNT ON;

    -- Si ocurre un error en una transacción, permite detectar
    -- la situación y revertir los cambios correspondientes.
    SET XACT_ABORT ON;

    -- ========================================================
    -- 1. VALIDAR LOS IDENTIFICADORES
    -- ========================================================

    IF @IdSolicitud IS NULL OR @IdSolicitud <= 0
    BEGIN
        THROW 50041,
              'El identificador de la solicitud no es válido.',
              1;
    END;

    IF @IdAutorizador IS NULL OR @IdAutorizador <= 0
    BEGIN
        THROW 50042,
              'El identificador del administrador no es válido.',
              1;
    END;

    -- ========================================================
    -- 2. VALIDAR EL MOTIVO DEL RECHAZO
    -- No se permite registrar un rechazo sin explicación.
    -- ========================================================

    IF @MotivoRechazo IS NULL
       OR LTRIM(RTRIM(@MotivoRechazo)) = ''
    BEGIN
        THROW 50043,
              'Debe indicar el motivo del rechazo.',
              1;
    END;

    -- ========================================================
    -- 3. VERIFICAR QUE EL USUARIO SEA ADMINISTRADOR
    --
    -- En Aura Beauty, id_rol = 1 corresponde al Administrador.
    -- La autenticación real se verificará también desde C#.
    -- ========================================================

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdAutorizador
          AND id_rol = 1
    )
    BEGIN
        THROW 50044,
              'El usuario no existe o no es Administrador.',
              1;
    END;

    -- ========================================================
    -- 4. INICIAR LA TRANSACCIÓN
    --
    -- Se bloquea la solicitud durante la comprobación para
    -- evitar que dos operaciones resuelvan simultáneamente
    -- la misma solicitud pendiente.
    -- ========================================================

    BEGIN TRY

        BEGIN TRANSACTION;

        -- Variables para recuperar los datos de la solicitud.
        DECLARE @IdSolicitante INT;
        DECLARE @EstadoActual VARCHAR(20);

        -- Obtener el solicitante y el estado actual.
        SELECT
            @IdSolicitante = id_solicitante,
            @EstadoActual = estado
        FROM dbo.RestauracionSolicitud
            WITH (UPDLOCK, HOLDLOCK)
        WHERE id_solicitud = @IdSolicitud;

        -- ====================================================
        -- 5. COMPROBAR QUE LA SOLICITUD EXISTA
        -- ====================================================

        IF @IdSolicitante IS NULL
        BEGIN
            THROW 50045,
                  'La solicitud de restauración no existe.',
                  1;
        END;

        -- ====================================================
        -- 6. IMPEDIR QUE EL SOLICITANTE SE RECHACE A SÍ MISMO
        -- ====================================================

        IF @IdSolicitante = @IdAutorizador
        BEGIN
            THROW 50046,
                  'El solicitante no puede resolver su propia solicitud.',
                  1;
        END;

        -- ====================================================
        -- 7. COMPROBAR QUE LA SOLICITUD SIGA PENDIENTE
        --
        -- Una solicitud autorizada o rechazada no puede
        -- volver a resolverse mediante este procedimiento.
        -- ====================================================

        IF @EstadoActual <> 'Pendiente'
        BEGIN
            THROW 50047,
                  'La solicitud no está pendiente de resolución.',
                  1;
        END;

        -- ====================================================
        -- 8. REGISTRAR EL RECHAZO
        --
        -- Se guarda el administrador que resolvió la solicitud,
        -- la fecha y el motivo.
        --
        -- CONCAT permite conservar las observaciones previas
        -- de la solicitud y agregar el motivo del rechazo.
        -- ====================================================

        UPDATE dbo.RestauracionSolicitud
        SET
            id_autorizador = @IdAutorizador,
            fecha_resolucion = SYSDATETIME(),
            estado = 'Rechazada',
            observaciones =
                LEFT(
                    CONCAT(
                        COALESCE(
                            NULLIF(observaciones, ''),
                            ''
                        ),
                        CASE
                            WHEN NULLIF(observaciones, '') IS NOT NULL
                                THEN N' | '
                            ELSE N''
                        END,
                        N'Motivo del rechazo: ',
                        @MotivoRechazo
                    ),
                    1000
                )
        WHERE id_solicitud = @IdSolicitud;

        -- Confirmar los cambios realizados.
        COMMIT TRANSACTION;

        -- ====================================================
        -- 9. DEVOLVER LOS DATOS ACTUALIZADOS
        -- Permite que C# informe el resultado al administrador.
        -- ====================================================

        SELECT
            id_solicitud AS IdSolicitud,
            id_backup AS IdBackup,
            id_solicitante AS IdSolicitante,
            id_autorizador AS IdAutorizador,
            fecha_solicitud AS FechaSolicitud,
            fecha_resolucion AS FechaResolucion,
            estado AS Estado,
            observaciones AS Observaciones
        FROM dbo.RestauracionSolicitud
        WHERE id_solicitud = @IdSolicitud;

    END TRY
    BEGIN CATCH

        -- Si la transacción no se completó, revertirla.
        IF XACT_STATE() <> 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        -- Devolver el error original a quien llamó al
        -- procedimiento, por ejemplo, a la capa de Datos.
        THROW;

    END CATCH;

END;
GO
