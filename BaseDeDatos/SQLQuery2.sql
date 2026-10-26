USE [AuraBeautyDB]
GO

/****** Objeto: StoredProcedure [dbo].[SP_ReservaProducto_Reservar] Fecha de script: 26/10/2026 10:58:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER   PROCEDURE [dbo].[SP_ReservaProducto_Reservar]
    @IdOperacion UNIQUEIDENTIFIER,
    @IdProducto INT,
    @IdUsuario INT,
    @Cantidad INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @IdOperacion IS NULL
       OR @IdOperacion =
          '00000000-0000-0000-0000-000000000000'
        THROW 51001, 'La operación no es válida.', 1;

    IF @IdProducto <= 0 OR @IdProducto IS NULL
        THROW 51002, 'El producto no es válido.', 1;

    IF @IdUsuario <= 0 OR @IdUsuario IS NULL
        THROW 51003, 'El vendedor no es válido.', 1;

    IF @Cantidad <= 0 OR @Cantidad IS NULL
        THROW 51004, 'La cantidad debe ser positiva.', 1;

    DECLARE @Ahora DATETIME2(0);
    DECLARE @Vencimiento DATETIME2(0);
    DECLARE @Stock INT;
    DECLARE @ReservadoOtros BIGINT;
    DECLARE @IdReserva INT;
    DECLARE @UsuarioReserva INT;
    DECLARE @Estado VARCHAR(12);

    BEGIN TRY
        BEGIN TRANSACTION;

        /*
        Bloquear el producto antes de consultar
        reservas y calcular disponibilidad.

        Todas las operaciones de reserva sobre
        el mismo producto deben esperar su turno.
        */
        SELECT @Stock = stock
        FROM dbo.Producto WITH (UPDLOCK, HOLDLOCK)
        WHERE Id_producto = @IdProducto
          AND deleted_at IS NULL;

        IF @Stock IS NULL
            THROW 51005,
                'El producto no existe o está inactivo.', 1;

        SET @Ahora = SYSUTCDATETIME();
        SET @Vencimiento = DATEADD(MINUTE, 10, @Ahora);

        /*
        No permitir reutilizar un carrito que
        ya tiene alguna reserva confirmada.
        */
        IF EXISTS
        (
            SELECT 1
            FROM dbo.ReservaProducto WITH (UPDLOCK, HOLDLOCK)
            WHERE IdOperacion = @IdOperacion
              AND Estado = 'CONFIRMADA'
        )
            THROW 51007,
                'Esta operación ya fue confirmada.', 1;

        /*
        Buscar la reserva del mismo carrito
        y producto, incluso si fue cancelada.
        */
        SELECT
            @IdReserva = IdReservaProducto,
            @UsuarioReserva = IdUsuario,
            @Estado = Estado
        FROM dbo.ReservaProducto WITH (UPDLOCK, HOLDLOCK)
        WHERE IdOperacion = @IdOperacion
          AND IdProducto = @IdProducto;

        IF @IdReserva IS NOT NULL
           AND @UsuarioReserva <> @IdUsuario
            THROW 51006,
                'La reserva pertenece a otro vendedor.', 1;

        /*
        Restar solamente las reservas ACTIVAS
        y NO VENCIDAS de otras operaciones.
        */
        SELECT @ReservadoOtros =
            ISNULL(SUM(CAST(Cantidad AS BIGINT)), 0)
        FROM dbo.ReservaProducto WITH (UPDLOCK, HOLDLOCK)
        WHERE IdProducto = @IdProducto
          AND Estado = 'ACTIVA'
          AND FechaVencimientoUtc > @Ahora
          AND IdOperacion <> @IdOperacion;

        IF CAST(@Stock AS BIGINT) - @ReservadoOtros
           < @Cantidad
            THROW 51008,
                'No hay unidades suficientes para reservar.', 1;

        IF @IdReserva IS NULL
        BEGIN
            INSERT INTO dbo.ReservaProducto
            (
                IdOperacion,
                IdProducto,
                IdUsuario,
                Cantidad,
                Estado,
                FechaReservaUtc,
                FechaVencimientoUtc,
                FechaActualizacionUtc,
                IdVentaCabecera
            )
            VALUES
            (
                @IdOperacion,
                @IdProducto,
                @IdUsuario,
                @Cantidad,
                'ACTIVA',
                @Ahora,
                @Vencimiento,
                @Ahora,
                NULL
            );

            SET @IdReserva =
                CONVERT(INT, SCOPE_IDENTITY());
        END
        ELSE
        BEGIN
            /*
            Permitir reactivar reservas CANCELADAS
            o VENCIDAS, siempre que todavía haya stock.

            La misma fila se reutiliza por el índice
            único (IdOperacion, IdProducto).
            */
            UPDATE dbo.ReservaProducto
            SET Cantidad = @Cantidad,
                Estado = 'ACTIVA',
                FechaReservaUtc = @Ahora,
                FechaVencimientoUtc = @Vencimiento,
                FechaActualizacionUtc = @Ahora,
                IdVentaCabecera = NULL
            WHERE IdReservaProducto = @IdReserva
              AND Estado <> 'CONFIRMADA';
        END;

        COMMIT TRANSACTION;

        SELECT
            @IdReserva AS IdReservaProducto,
            @IdOperacion AS IdOperacion,
            @IdProducto AS IdProducto,
            @Cantidad AS CantidadReservada,
            @Stock AS StockFisico,
            @ReservadoOtros AS ReservadoPorOtros,
            CAST(
                CAST(@Stock AS BIGINT)
                - @ReservadoOtros
                - @Cantidad AS INT
            ) AS DisponibleRestante,
            @Vencimiento AS FechaVencimientoUtc,
            'ACTIVA' AS Estado;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO


