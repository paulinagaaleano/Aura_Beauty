

USE [AuraBeautyDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- PROCEDIMIENTO: SP_ReporteVentasDetallesAnalisis
-- Obtiene el detalle de las ventas de un período.
-- Permite filtrar opcionalmente por vendedor.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[SP_ReporteVentasDetallesAnalisis]
(
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @IdUsuario INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.Id_ventaCabecera AS IdVenta,
        v.fecha_venta AS FechaVenta,
        v.id_usuario AS IdUsuario,
        d.id_producto AS IdProducto,
        d.cantidad AS Cantidad,
        d.subtotal AS Subtotal
    FROM VentaCabecera v
    INNER JOIN VentaDetalle d
        ON d.id_ventaCabecera = v.Id_ventaCabecera
    WHERE CAST(v.fecha_venta AS DATE) >= CAST(@FechaInicio AS DATE)
      AND CAST(v.fecha_venta AS DATE) <= CAST(@FechaFin AS DATE)
      AND (@IdUsuario IS NULL OR v.id_usuario = @IdUsuario)
    ORDER BY v.fecha_venta ASC;
END;
GO

-- =============================================
-- PROCEDIMIENTO: SP_ReporteVentasAnalisis
-- Obtiene las ventas de un período determinado.
-- Permite filtrar opcionalmente por vendedor.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[SP_ReporteVentasAnalisis]
(
    @FechaInicio DATETIME,
    @FechaFin DATETIME,
    @IdUsuario INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.Id_ventaCabecera AS IdVenta,
        v.fecha_venta AS FechaVenta,
        v.total AS Total,
        v.id_usuario AS IdUsuario
    FROM VentaCabecera v
    WHERE CAST(v.fecha_venta AS DATE) >= CAST(@FechaInicio AS DATE)
      AND CAST(v.fecha_venta AS DATE) <= CAST(@FechaFin AS DATE)
      AND (@IdUsuario IS NULL OR v.id_usuario = @IdUsuario)
    ORDER BY v.fecha_venta ASC;
END;
GO
