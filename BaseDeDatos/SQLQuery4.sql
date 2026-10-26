USE [AuraBeautyDB]
GO

/****** Objeto: UserDefinedTableType [dbo].[TVP_DetalleVentaReserva] Fecha de script: 26/10/2026 11:06:06 ******/
CREATE TYPE [dbo].[TVP_DetalleVentaReserva] AS TABLE(
	[IdProducto] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[PrecioUnitario] [decimal](10, 2) NOT NULL,
	PRIMARY KEY CLUSTERED 
(
	[IdProducto] ASC
)WITH (IGNORE_DUP_KEY = OFF),
	CHECK (([Cantidad]>(0))),
	CHECK (([PrecioUnitario]>(0)))
)
GO


