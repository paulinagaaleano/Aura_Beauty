USE [AuraBeautyDB]
GO

/****** Objeto: StoredProcedure [dbo].[SP_ReservaProducto_Cancelar] Fecha de script: 26/10/2026 11:00:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*
========================================================
AURA BEAUTY
SP_ReservaProducto_Cancelar

Permite:
- Liberar un producto del carrito.
- Liberar todos los productos del carrito.

No modifica Producto.stock.
No registra ni elimina ventas.
No borra reservas: conserva su historial.

@IdProducto:
NULL = cancelar todas las reservas del carrito.
Un ID = cancelar solamente ese producto.
========================================================
*/

ALTER   PROCEDURE [dbo].[SP_ReservaProducto_Cancelar]
    @IdOperacion UNIQUEIDENTIFIER,
    @IdUsuario INT,
    @IdProducto INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @IdOperacion IS NULL
       OR @IdOperacion =
          '00000000-0000-0000-0000-000000000000'
    BEGIN
        THROW 51101,
            'La operación de venta no es válida.', 1;
    END;

    IF @IdUsuario IS NULL OR @IdUsuario <= 0
    BEGIN
        THROW 51102,
            'El vendedor no es válido.', 1;
    END;

    IF @IdProducto IS NOT NULL AND @IdProducto <= 0
    BEGIN
        THROW 51103,
            'El producto no es válido.', 1;
    END;

    DECLARE @CantidadLiberada INT = 0;
    DECLARE @RegistrosCancelados INT = 0;

    BEGIN TRY

        BEGIN TRANSACTION;

        /*
        Verificamos que el carrito no pertenezca
        a otro vendedor.
        */

        IF EXISTS
        (
            SELECT 1
            FROM dbo.ReservaProducto WITH (UPDLOCK, HOLDLOCK)
            WHERE IdOperacion = @IdOperacion
              AND IdUsuario <> @IdUsuario
        )
        BEGIN
            THROW 51104,
                'La operación pertenece a otro vendedor.',
                1;
        END;

        /*
        Sumamos las unidades de las reservas
        que todavía están vigentes.
        */

        SELECT
            @CantidadLiberada =
                ISNULL(SUM(Cantidad), 0)
        FROM dbo.ReservaProducto WITH (UPDLOCK, HOLDLOCK)
        WHERE IdOperacion = @IdOperacion
          AND IdUsuario = @IdUsuario
          AND Estado = 'ACTIVA'
          AND FechaVencimientoUtc > SYSUTCDATETIME()
          AND
          (
              @IdProducto IS NULL
              OR IdProducto = @IdProducto
          );

        /*
        Cancelamos las reservas activas.

        Una reserva ya confirmada no se toca.
        Una reserva ya cancelada tampoco.
        */

        UPDATE dbo.ReservaProducto
        SET
            Estado = 'CANCELADA',
            FechaActualizacionUtc = SYSUTCDATETIME()
        WHERE IdOperacion = @IdOperacion
          AND IdUsuario = @IdUsuario
          AND Estado = 'ACTIVA'
          AND
          (
              @IdProducto IS NULL
              OR IdProducto = @IdProducto
          );

        SET @RegistrosCancelados = @@ROWCOUNT;

        COMMIT TRANSACTION;

        /*
        Devolvemos el resultado a C#.
        No se modifica el stock físico.
        */

        SELECT
            @IdOperacion AS IdOperacion,
            @CantidadLiberada AS UnidadesLiberadas,
            @RegistrosCancelados AS ReservasCanceladas,
            'CANCELACION_PROCESADA' AS Resultado;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH;
END;
GO


