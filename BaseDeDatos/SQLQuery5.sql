USE [AuraBeautyDB]
GO

/****** Objeto: StoredProcedure [dbo].[SP_Venta_ConfirmarReservas] Fecha de script: 26/10/2026 11:00:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*
=========================================================
AURA BEAUTY - CONFIRMACIÓN DE VENTA CON RESERVAS

Procedimiento NUEVO.

No reemplaza el procedimiento de reservas.
No modifica la estructura de las tablas existentes.

La venta, el stock y las reservas se actualizan
dentro de una única transacción.
=========================================================
*/

ALTER   PROCEDURE [dbo].[SP_Venta_ConfirmarReservas]
    @IdOperacion UNIQUEIDENTIFIER,
    @IdUsuario INT,
    @IdCliente INT = NULL,
    @TipoFactura NVARCHAR(50),
    @NroFactura NVARCHAR(50),
    @Detalles dbo.TVP_DetalleVentaReserva READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @IdVenta INT;
    DECLARE @Ahora DATETIME2(0);
    DECLARE @Total DECIMAL(18,2);

    DECLARE @CantidadProductos INT;
    DECLARE @FilasAfectadas INT;

    DECLARE @IdProducto INT;
    DECLARE @Cantidad INT;
    DECLARE @Precio DECIMAL(10,2);

    DECLARE @Stock INT;
    DECLARE @PrecioActual DECIMAL(10,2);
    DECLARE @ReservadoOtros BIGINT;

    DECLARE @CantidadReserva INT;
    DECLARE @EstadoReserva VARCHAR(12);
    DECLARE @VencimientoReserva DATETIME2(0);

    DECLARE @ResultadoBloqueo INT;
    DECLARE @Recurso NVARCHAR(255);

    -- ================================================
    -- 1. VALIDACIONES INICIALES
    -- ================================================

    IF @IdOperacion IS NULL
       OR @IdOperacion =
          '00000000-0000-0000-0000-000000000000'
        THROW 51201,
            'La operación no es válida.', 1;

    IF @IdUsuario IS NULL OR @IdUsuario <= 0
        THROW 51202,
            'El vendedor no es válido.', 1;

    IF NULLIF(LTRIM(RTRIM(@TipoFactura)), '') IS NULL
        THROW 51203,
            'Debe indicar el tipo de factura.', 1;

    IF NULLIF(LTRIM(RTRIM(@NroFactura)), '') IS NULL
        THROW 51204,
            'Debe indicar el número de comprobante.', 1;

    SELECT @CantidadProductos = COUNT(*)
    FROM @Detalles;

    IF @CantidadProductos = 0
        THROW 51205,
            'El carrito no tiene productos.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 2
          AND deleted_at IS NULL
    )
        THROW 51206,
            'El usuario no es un vendedor activo.', 1;

    -- El total se calcula a partir de los detalles.
    SELECT @Total =
        SUM(
            CAST(Cantidad AS DECIMAL(18,2))
            * PrecioUnitario
        )
    FROM @Detalles;

    IF @Total IS NULL
       OR @Total <= 0
       OR @Total > 99999999.99
        THROW 51207,
            'El total de la venta no es válido.', 1;

    BEGIN TRY

        BEGIN TRANSACTION;

        -- ============================================
        -- 2. EVITAR DOS CONFIRMACIONES DEL MISMO CARRITO
        -- ============================================

        SET @Recurso =
            N'AuraVenta:' +
            CONVERT(NVARCHAR(36), @IdOperacion);

        EXEC @ResultadoBloqueo =
            sys.sp_getapplock
                @Resource = @Recurso,
                @LockMode = 'Exclusive',
                @LockOwner = 'Transaction',
                @LockTimeout = 15000;

        IF @ResultadoBloqueo < 0
            THROW 51208,
                'No se pudo bloquear la operación de venta.',
                1;

        -- Una operación no puede pertenecer
        -- simultáneamente a dos vendedores.

        IF EXISTS
        (
            SELECT 1
            FROM dbo.ReservaProducto
            WHERE IdOperacion = @IdOperacion
              AND IdUsuario <> @IdUsuario
        )
            THROW 51209,
                'La operación pertenece a otro vendedor.',
                1;

        -- Si la venta ya se confirmó anteriormente,
        -- devolvemos el mismo identificador.
        -- NO volvemos a descontar stock.

        SELECT TOP (1)
            @IdVenta = IdVentaCabecera
        FROM dbo.ReservaProducto
        WHERE IdOperacion = @IdOperacion
          AND IdUsuario = @IdUsuario
          AND Estado = 'CONFIRMADA'
          AND IdVentaCabecera IS NOT NULL;

        IF @IdVenta IS NOT NULL
        BEGIN
            COMMIT TRANSACTION;

            SELECT
                @IdVenta AS IdVentaCabecera,
                CAST(1 AS BIT) AS YaRegistrada;

            RETURN;
        END;

        -- ============================================
        -- 3. EVITAR COMPROBANTES DUPLICADOS
        -- ============================================

        SET @Recurso =
            N'AuraFactura:' + @NroFactura;

        EXEC @ResultadoBloqueo =
            sys.sp_getapplock
                @Resource = @Recurso,
                @LockMode = 'Exclusive',
                @LockOwner = 'Transaction',
                @LockTimeout = 15000;

        IF @ResultadoBloqueo < 0
            THROW 51210,
                'No se pudo verificar el comprobante.',
                1;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.VentaCabecera WITH (UPDLOCK, HOLDLOCK)
            WHERE nro_factura = @NroFactura
        )
            THROW 51211,
                'Ya existe una venta con ese número de comprobante.',
                1;

        -- ============================================
        -- 4. BLOQUEAR Y VALIDAR LOS PRODUCTOS
        -- ============================================

        /*
        Se recorren por ID para mantener un orden
        estable de bloqueo entre confirmaciones.

        El procedimiento Reservar también bloquea
        la fila del producto antes de actualizar
        una reserva.
        */

        DECLARE cursorProductos CURSOR LOCAL FAST_FORWARD
        FOR
            SELECT
                IdProducto,
                Cantidad,
                PrecioUnitario
            FROM @Detalles
            ORDER BY IdProducto;

        OPEN cursorProductos;

        FETCH NEXT FROM cursorProductos
        INTO @IdProducto, @Cantidad, @Precio;

        WHILE @@FETCH_STATUS = 0
        BEGIN

            SET @Stock = NULL;
            SET @PrecioActual = NULL;

            SELECT
                @Stock = stock,
                @PrecioActual = precio
            FROM dbo.Producto WITH (UPDLOCK, HOLDLOCK)
            WHERE Id_producto = @IdProducto
              AND deleted_at IS NULL;

            IF @Stock IS NULL
                THROW 51212,
                    'Un producto ya no está disponible.',
                    1;

            IF @PrecioActual IS NULL
               OR @PrecioActual <> @Precio
                THROW 51213,
                    'Cambió el precio de un producto. Actualizá el carrito.',
                    1;

            IF @Stock < @Cantidad
                THROW 51214,
                    'No hay stock suficiente para completar la venta.',
                    1;

            FETCH NEXT FROM cursorProductos
            INTO @IdProducto, @Cantidad, @Precio;
        END;

        CLOSE cursorProductos;
        DEALLOCATE cursorProductos;

        SET @Ahora = SYSUTCDATETIME();

        -- ============================================
        -- 5. COMPROBAR RESERVAS VIGENTES
        -- ============================================

        /*
        Cada producto de la venta debe tener
        una reserva activa, no vencida y con
        exactamente la misma cantidad.
        */

        IF EXISTS
        (
            SELECT 1
            FROM @Detalles AS D
            LEFT JOIN dbo.ReservaProducto AS R
                WITH (UPDLOCK, HOLDLOCK)
                ON R.IdOperacion = @IdOperacion
               AND R.IdProducto = D.IdProducto
               AND R.IdUsuario = @IdUsuario
            WHERE R.IdReservaProducto IS NULL
               OR R.Estado <> 'ACTIVA'
               OR R.FechaVencimientoUtc <= @Ahora
               OR R.Cantidad <> D.Cantidad
        )
            THROW 51215,
                'Las reservas no coinciden con el carrito o están vencidas.',
                1;

        /*
        También impedimos confirmar una operación
        que tiene otros productos reservados
        pero ausentes del carrito enviado.
        */

        IF EXISTS
        (
            SELECT 1
            FROM dbo.ReservaProducto AS R
                WITH (UPDLOCK, HOLDLOCK)
            WHERE R.IdOperacion = @IdOperacion
              AND R.IdUsuario = @IdUsuario
              AND R.Estado = 'ACTIVA'
              AND NOT EXISTS
              (
                  SELECT 1
                  FROM @Detalles AS D
                  WHERE D.IdProducto = R.IdProducto
              )
        )
            THROW 51216,
                'El contenido del carrito cambió. Actualizá la venta.',
                1;

        -- ============================================
        -- 6. COMPROBAR RESERVAS DE OTROS VENDEDORES
        -- ============================================

        /*
        Aunque el carrito tenga sus reservas,
        volvemos a comprobar que las existencias
        físicas alcancen para nuestra compra
        y las reservas vigentes de terceros.

        Esto protege también frente a modificaciones
        de stock realizadas por otros módulos.
        */

        IF EXISTS
        (
            SELECT 1
            FROM @Detalles AS D
            INNER JOIN dbo.Producto AS P
                ON P.Id_producto = D.IdProducto
            OUTER APPLY
            (
                SELECT
                    ISNULL(
                        SUM(CAST(R.Cantidad AS BIGINT)),
                        0
                    ) AS Reservado
                FROM dbo.ReservaProducto AS R
                    WITH (UPDLOCK, HOLDLOCK)
                WHERE R.IdProducto = D.IdProducto
                  AND R.Estado = 'ACTIVA'
                  AND R.FechaVencimientoUtc > @Ahora
                  AND R.IdOperacion <> @IdOperacion
            ) AS O
            WHERE CAST(P.stock AS BIGINT)
                  - O.Reservado < D.Cantidad
        )
            THROW 51217,
                'El stock disponible cambió. Revisá el carrito.',
                1;

        -- ============================================
        -- 7. REGISTRAR CABECERA
        -- ============================================

        INSERT INTO dbo.VentaCabecera
        (
            id_cliente,
            id_usuario,
            tipo_factura,
            nro_factura,
            total,
            fecha_venta
        )
        VALUES
        (
            @IdCliente,
            @IdUsuario,
            @TipoFactura,
            @NroFactura,
            @Total,
            GETDATE()
        );

        SET @IdVenta =
            CAST(SCOPE_IDENTITY() AS INT);

        -- ============================================
        -- 8. REGISTRAR DETALLES
        -- ============================================

        INSERT INTO dbo.VentaDetalle
        (
            id_ventaCabecera,
            id_producto,
            cantidad,
            precio_unitario,
            subtotal
        )
        SELECT
            @IdVenta,
            D.IdProducto,
            D.Cantidad,
            D.PrecioUnitario,
            CAST(
                D.Cantidad * D.PrecioUnitario
                AS DECIMAL(10,2)
            )
        FROM @Detalles AS D;

        SET @FilasAfectadas = @@ROWCOUNT;

        IF @FilasAfectadas <> @CantidadProductos
            THROW 51218,
                'No se registraron todos los detalles.',
                1;

        -- ============================================
        -- 9. DESCONTAR STOCK DEFINITIVAMENTE
        -- ============================================

        UPDATE P
        SET
            P.stock = P.stock - D.Cantidad,
            P.updated_at = GETDATE()
        FROM dbo.Producto AS P
        INNER JOIN @Detalles AS D
            ON D.IdProducto = P.Id_producto
        WHERE P.deleted_at IS NULL
          AND P.stock >= D.Cantidad;

        SET @FilasAfectadas = @@ROWCOUNT;

        IF @FilasAfectadas <> @CantidadProductos
            THROW 51219,
                'No se pudo actualizar todo el stock.',
                1;

        -- ============================================
        -- 10. CONFIRMAR TODAS LAS RESERVAS
        -- ============================================

        UPDATE dbo.ReservaProducto
        SET
            Estado = 'CONFIRMADA',
            IdVentaCabecera = @IdVenta,
            FechaActualizacionUtc = @Ahora
        WHERE IdOperacion = @IdOperacion
          AND IdUsuario = @IdUsuario
          AND Estado = 'ACTIVA'
          AND FechaVencimientoUtc > @Ahora;

        SET @FilasAfectadas = @@ROWCOUNT;

        IF @FilasAfectadas <> @CantidadProductos
            THROW 51220,
                'No se pudieron confirmar todas las reservas.',
                1;

        -- ============================================
        -- 11. CONFIRMAR TRANSACCIÓN
        -- ============================================

        COMMIT TRANSACTION;

        SELECT
            @IdVenta AS IdVentaCabecera,
            CAST(0 AS BIT) AS YaRegistrada;

    END TRY
    BEGIN CATCH

        -- Cerramos el cursor si ocurre un error
        -- mientras se validan los productos.

        IF CURSOR_STATUS('local', 'cursorProductos') >= 0
            CLOSE cursorProductos;

        IF CURSOR_STATUS('local', 'cursorProductos') > -3
            DEALLOCATE cursorProductos;

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH;
END;
GO


