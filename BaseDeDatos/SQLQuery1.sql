USE [AuraBeautyDB]
GO

/****** Objeto: Table [dbo].[ReservaProducto] Fecha de script: 26/10/2026 11:02:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ReservaProducto](
	[IdReservaProducto] [int] IDENTITY(1,1) NOT NULL,
	[IdOperacion] [uniqueidentifier] NOT NULL,
	[IdProducto] [int] NOT NULL,
	[IdUsuario] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[Estado] [varchar](12) NOT NULL,
	[FechaReservaUtc] [datetime2](0) NOT NULL,
	[FechaVencimientoUtc] [datetime2](0) NOT NULL,
	[FechaActualizacionUtc] [datetime2](0) NOT NULL,
	[IdVentaCabecera] [int] NULL,
 CONSTRAINT [PK_ReservaProducto] PRIMARY KEY CLUSTERED 
(
	[IdReservaProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ReservaProducto] ADD  CONSTRAINT [DF_ReservaProducto_Estado]  DEFAULT ('ACTIVA') FOR [Estado]
GO

ALTER TABLE [dbo].[ReservaProducto] ADD  CONSTRAINT [DF_ReservaProducto_FechaReserva]  DEFAULT (sysutcdatetime()) FOR [FechaReservaUtc]
GO

ALTER TABLE [dbo].[ReservaProducto] ADD  CONSTRAINT [DF_ReservaProducto_FechaActualizacion]  DEFAULT (sysutcdatetime()) FOR [FechaActualizacionUtc]
GO

ALTER TABLE [dbo].[ReservaProducto]  WITH CHECK ADD  CONSTRAINT [FK_ReservaProducto_Producto] FOREIGN KEY([IdProducto])
REFERENCES [dbo].[Producto] ([Id_producto])
GO

ALTER TABLE [dbo].[ReservaProducto] CHECK CONSTRAINT [FK_ReservaProducto_Producto]
GO

ALTER TABLE [dbo].[ReservaProducto]  WITH CHECK ADD  CONSTRAINT [FK_ReservaProducto_Usuario] FOREIGN KEY([IdUsuario])
REFERENCES [dbo].[Usuario] ([id_usuario])
GO

ALTER TABLE [dbo].[ReservaProducto] CHECK CONSTRAINT [FK_ReservaProducto_Usuario]
GO

ALTER TABLE [dbo].[ReservaProducto]  WITH CHECK ADD  CONSTRAINT [FK_ReservaProducto_Venta] FOREIGN KEY([IdVentaCabecera])
REFERENCES [dbo].[VentaCabecera] ([Id_ventaCabecera])
GO

ALTER TABLE [dbo].[ReservaProducto] CHECK CONSTRAINT [FK_ReservaProducto_Venta]
GO

ALTER TABLE [dbo].[ReservaProducto]  WITH CHECK ADD  CONSTRAINT [CK_ReservaProducto_Cantidad] CHECK  (([Cantidad]>(0)))
GO

ALTER TABLE [dbo].[ReservaProducto] CHECK CONSTRAINT [CK_ReservaProducto_Cantidad]
GO

ALTER TABLE [dbo].[ReservaProducto]  WITH CHECK ADD  CONSTRAINT [CK_ReservaProducto_Confirmacion] CHECK  (([Estado]='CONFIRMADA' AND [IdVentaCabecera] IS NOT NULL OR [Estado]<>'CONFIRMADA' AND [IdVentaCabecera] IS NULL))
GO

ALTER TABLE [dbo].[ReservaProducto] CHECK CONSTRAINT [CK_ReservaProducto_Confirmacion]
GO

ALTER TABLE [dbo].[ReservaProducto]  WITH CHECK ADD  CONSTRAINT [CK_ReservaProducto_Estado] CHECK  (([Estado]='VENCIDA' OR [Estado]='CANCELADA' OR [Estado]='CONFIRMADA' OR [Estado]='ACTIVA'))
GO

ALTER TABLE [dbo].[ReservaProducto] CHECK CONSTRAINT [CK_ReservaProducto_Estado]
GO

ALTER TABLE [dbo].[ReservaProducto]  WITH CHECK ADD  CONSTRAINT [CK_ReservaProducto_Fechas] CHECK  (([FechaVencimientoUtc]>[FechaReservaUtc]))
GO

ALTER TABLE [dbo].[ReservaProducto] CHECK CONSTRAINT [CK_ReservaProducto_Fechas]
GO


