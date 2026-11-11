/*
 AURA BEAUTY - INSTALACION COMPLETA PARA PRUEBAS - 11/11/2026
 FUENTES: TallerII (9).zip + parche de historial persistente.

 IMPORTANTE:
 - REQUIERE SSMS CON Query > SQLCMD Mode ACTIVADO.
 - Diseñado para una INSTANCIA de prueba SIN AuraBeautyDB ni AuraBeautyDB_Admin.
 - NO borra ninguna base existente; si encuentra alguna, detiene el script.
 - NO crea archivos .bak ni ejecuta una restauracion.
 - Los usuarios/credenciales de ejemplo proceden del script original:
   SOLO PARA DEMOSTRACIONES; reemplazar contrasenas antes de cualquier uso real.
 - La persistencia despues de RESTORE exige que C# siga llamando al procedimiento
   AuraBeautyDB_Admin.dbo.SP_SincronizarHistorial, como en el parche anterior.
 - El modulo de restauracion requiere permisos de archivo/SQL y una ruta accesible.

 EJECUCION: SSMS > Query > SQLCMD Mode > Ejecutar script completo.
 */
:ON ERROR EXIT
USE [master];
GO
IF DB_ID(N'AuraBeautyDB') IS NOT NULL OR DB_ID(N'AuraBeautyDB_Admin') IS NOT NULL
BEGIN
    THROW 51000, 'Instalacion detenida: AuraBeautyDB o AuraBeautyDB_Admin ya existe. Ejecute solo en una instancia limpia de pruebas. No se modifico ninguna base.', 1;
END;
GO
CREATE DATABASE [AuraBeautyDB];
GO
USE [AuraBeautyDB];
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO


--===========================================================================
-- 01 - TABLAS, RELACIONES, DATOS DE DEMOSTRACION Y PROCEDIMIENTOS COMERCIALES
--===========================================================================
CREATE TABLE [dbo].[Categoria](
	[Id_categoria] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](100) NULL,
	[descripcion] [nvarchar](255) NULL,
	[imagen] [nvarchar](255) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id_categoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Cliente] Fecha de script: 21/9/2026 21:10:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cliente](
	[Id_cliente] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](100) NULL,
	[apellido] [nvarchar](100) NULL,
	[dni] [int] NULL,
	[fecha_nacimiento] [datetime] NULL,
	[fecha_registro] [datetime] NULL,
	[domicilio] [nvarchar](255) NULL,
	[correo] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id_cliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Producto] Fecha de script: 21/9/2026 21:10:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Producto](
	[Id_producto] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [nvarchar](100) NULL,
	[descripcion] [nvarchar](255) NULL,
	[precio] [decimal](10, 2) NULL,
	[stock] [int] NULL,
	[id_categoria] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id_producto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Rol] Fecha de script: 21/9/2026 21:10:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rol](
	[id_rol] [int] NOT NULL,
	[descripcion] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id_rol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Usuario] Fecha de script: 21/9/2026 21:10:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuario](
	[id_usuario] [int] IDENTITY(1,1) NOT NULL,
	[nombre] [varchar](50) NOT NULL,
	[apellido] [varchar](50) NULL,
	[correo] [varchar](100) NOT NULL,
	[contraseña] [varchar](100) NOT NULL,
	[id_rol] [int] NOT NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[VentaCabecera] Fecha de script: 21/9/2026 21:10:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VentaCabecera](
	[Id_ventaCabecera] [int] IDENTITY(1,1) NOT NULL,
	[id_cliente] [int] NULL,
	[id_usuario] [int] NULL,
	[tipo_factura] [nvarchar](50) NULL,
	[nro_factura] [nvarchar](50) NULL,
	[total] [decimal](10, 2) NULL,
	[fecha_venta] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id_ventaCabecera] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[VentaDetalle] Fecha de script: 21/9/2026 21:10:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VentaDetalle](
	[Id_detalle] [int] IDENTITY(1,1) NOT NULL,
	[id_ventaCabecera] [int] NULL,
	[id_producto] [int] NULL,
	[cantidad] [int] NULL,
	[precio_unitario] [decimal](10, 2) NULL,
	[subtotal] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id_detalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Objeto: Table [dbo].[SolicitudStock] - Gestión de solicitudes justificadas de stock ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SolicitudStock](
    [id_solicitud] [int] IDENTITY(1,1) NOT NULL,
    [id_producto] [int] NOT NULL,
    [id_usuario_solicitante] [int] NOT NULL,
    [tipo_movimiento] [varchar](20) NOT NULL,
    [cantidad] [int] NOT NULL,
    [motivo] [varchar](250) NOT NULL,
    [estado] [varchar](20) NOT NULL
        CONSTRAINT [DF_SolicitudStock_Estado] DEFAULT ('PENDIENTE'),
    [fecha_solicitud] [datetime] NOT NULL
        CONSTRAINT [DF_SolicitudStock_Fecha] DEFAULT (GETDATE()),
    [id_usuario_autorizador] [int] NULL,
    [fecha_resolucion] [datetime] NULL,

    CONSTRAINT [PK_SolicitudStock]
        PRIMARY KEY CLUSTERED ([id_solicitud] ASC),

    CONSTRAINT [CK_SolicitudStock_Tipo]
        CHECK ([tipo_movimiento] IN ('INGRESO','EGRESO')),

    CONSTRAINT [CK_SolicitudStock_Estado]
        CHECK ([estado] IN ('PENDIENTE','APROBADA','RECHAZADA')),

    CONSTRAINT [CK_SolicitudStock_Cantidad]
        CHECK ([cantidad] > 0)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SolicitudStock] WITH CHECK
ADD CONSTRAINT [FK_SolicitudStock_Producto]
FOREIGN KEY([id_producto])
REFERENCES [dbo].[Producto] ([Id_producto])
GO

ALTER TABLE [dbo].[SolicitudStock] CHECK
CONSTRAINT [FK_SolicitudStock_Producto]
GO

ALTER TABLE [dbo].[SolicitudStock] WITH CHECK
ADD CONSTRAINT [FK_SolicitudStock_Solicitante]
FOREIGN KEY([id_usuario_solicitante])
REFERENCES [dbo].[Usuario] ([id_usuario])
GO

ALTER TABLE [dbo].[SolicitudStock] CHECK
CONSTRAINT [FK_SolicitudStock_Solicitante]
GO

ALTER TABLE [dbo].[SolicitudStock] WITH CHECK
ADD CONSTRAINT [FK_SolicitudStock_Autorizador]
FOREIGN KEY([id_usuario_autorizador])
REFERENCES [dbo].[Usuario] ([id_usuario])
GO

ALTER TABLE [dbo].[SolicitudStock] CHECK
CONSTRAINT [FK_SolicitudStock_Autorizador]
GO

SET IDENTITY_INSERT [dbo].[Categoria] ON 

INSERT [dbo].[Categoria] ([Id_categoria], [nombre], [descripcion], [imagen], [created_at], [updated_at], [deleted_at]) VALUES (1, N'Bases', N'Productos destinados a unificar el tono de la piel, mejorar su apariencia y proporcionar distintos niveles de cobertura y acabado.', NULL, CAST(N'2026-09-14T00:21:08.583' AS DateTime), NULL, NULL)
INSERT [dbo].[Categoria] ([Id_categoria], [nombre], [descripcion], [imagen], [created_at], [updated_at], [deleted_at]) VALUES (2, N'Labiales', N'Aportan color, definición y acabado a los labios, disponibles en diferentes texturas, tonos.', NULL, CAST(N'2026-09-14T00:21:48.680' AS DateTime), NULL, NULL)
INSERT [dbo].[Categoria] ([Id_categoria], [nombre], [descripcion], [imagen], [created_at], [updated_at], [deleted_at]) VALUES (3, N'Sombras de Ojos', N'Productos para realzar y definir la mirada mediante la aplicación de color en los párpados, con variedad de tonos, texturas y acabados.', NULL, CAST(N'2026-09-14T00:22:39.077' AS DateTime), NULL, NULL)
INSERT [dbo].[Categoria] ([Id_categoria], [nombre], [descripcion], [imagen], [created_at], [updated_at], [deleted_at]) VALUES (4, N'Máscaras de Pestañas', N'Productos diseñados para destacar las pestañas, aportando definición, longitud, volumen o curvatura según su formulación.', NULL, CAST(N'2026-09-14T00:23:29.940' AS DateTime), NULL, NULL)
INSERT [dbo].[Categoria] ([Id_categoria], [nombre], [descripcion], [imagen], [created_at], [updated_at], [deleted_at]) VALUES (5, N'Correctores', N'Productos destinados a disimular o neutralizar ojeras, manchas, imperfecciones y diferencias de tonalidad de la piel.', NULL, CAST(N'2026-09-14T00:24:18.010' AS DateTime), NULL, NULL)
INSERT [dbo].[Categoria] ([Id_categoria], [nombre], [descripcion], [imagen], [created_at], [updated_at], [deleted_at]) VALUES (6, N'Iluminadores', N'Productos utilizados para aportar luminosidad y resaltar estratégicamente determinados puntos del rostro mediante distintos acabados y texturas.', NULL, CAST(N'2026-09-14T00:24:46.720' AS DateTime), NULL, NULL)
INSERT [dbo].[Categoria] ([Id_categoria], [nombre], [descripcion], [imagen], [created_at], [updated_at], [deleted_at]) VALUES (7, N'Bronzers', N'Productos destinados a aportar calidez y dimensión al rostro, recreando visualmente un efecto bronceado natural.', NULL, CAST(N'2026-09-14T00:25:13.690' AS DateTime), NULL, NULL)
INSERT [dbo].[Categoria] ([Id_categoria], [nombre], [descripcion], [imagen], [created_at], [updated_at], [deleted_at]) VALUES (8, N'Rubores', N'Productos utilizados para aportar color y vitalidad a las mejillas, contribuyendo a definir y realzar las facciones del rostro.', NULL, CAST(N'2026-09-14T00:25:42.727' AS DateTime), NULL, NULL)
INSERT [dbo].[Categoria] ([Id_categoria], [nombre], [descripcion], [imagen], [created_at], [updated_at], [deleted_at]) VALUES (9, N'Delineadores', N'Productos destinados a definir y realzar el contorno de los ojos mediante trazos de diferente intensidad, grosor y acabado.', NULL, CAST(N'2026-09-14T00:26:00.540' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Categoria] OFF
GO
SET IDENTITY_INSERT [dbo].[Cliente] ON 

INSERT [dbo].[Cliente] ([Id_cliente], [nombre], [apellido], [dni], [fecha_nacimiento], [fecha_registro], [domicilio], [correo]) VALUES (1, N'Maite', N'Lopez', 43995800, CAST(N'2005-06-11T00:00:00.000' AS DateTime), CAST(N'2026-09-14T01:57:00.643' AS DateTime), N'Juni 7201', N'maitelopez@gmail.com')
INSERT [dbo].[Cliente] ([Id_cliente], [nombre], [apellido], [dni], [fecha_nacimiento], [fecha_registro], [domicilio], [correo]) VALUES (2, N'Fernanda', N'Galarza', 22389720, CAST(N'1977-02-23T00:00:00.000' AS DateTime), CAST(N'2026-09-14T15:18:45.413' AS DateTime), N'Moreno 500', N'fergalarza@gmail.com')
INSERT [dbo].[Cliente] ([Id_cliente], [nombre], [apellido], [dni], [fecha_nacimiento], [fecha_registro], [domicilio], [correo]) VALUES (3, N'Amira', N'Moreira', 43218456, CAST(N'2002-09-14T00:00:00.000' AS DateTime), CAST(N'2026-09-14T15:27:00.003' AS DateTime), N'Peru 570', N'amiramo@gmail.com')
INSERT [dbo].[Cliente] ([Id_cliente], [nombre], [apellido], [dni], [fecha_nacimiento], [fecha_registro], [domicilio], [correo]) VALUES (4, N'Luciana', N'Ramirez', 40976205, CAST(N'2009-09-14T00:00:00.000' AS DateTime), CAST(N'2026-09-14T15:28:30.020' AS DateTime), N'Bolivar 1889', N'lucianar@gmail.com')
SET IDENTITY_INSERT [dbo].[Cliente] OFF
GO
SET IDENTITY_INSERT [dbo].[Producto] ON 

INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (1, N'Base Ruby Rose', N'Base líquida de cobertura media y acabado natural.', CAST(18500.00 AS Decimal(10, 2)), 10, 1, CAST(N'2026-09-14T01:12:32.783' AS DateTime), CAST(N'2026-09-14T14:14:17.357' AS DateTime), NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (2, N'Labial Cherry Red', N'Labial de acabado mate, alta pigmentación y cobertura uniforme.', CAST(9500.00 AS Decimal(10, 2)), 1, 2, CAST(N'2026-09-14T01:13:58.633' AS DateTime), CAST(N'2026-09-14T15:19:14.157' AS DateTime), NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (3, N'Base Soft Matte', N'Base líquida de acabado mate y cobertura media, ideal para uso diario.', CAST(18900.00 AS Decimal(10, 2)), 8, 1, CAST(N'2026-09-14T15:43:10.377' AS DateTime), CAST(N'2026-09-14T18:07:23.303' AS DateTime), NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (4, N'Labial Nude Rose', N'Labial cremoso tono rosa nude, de textura suave y acabado satinado.', CAST(9300.00 AS Decimal(10, 2)), 7, 2, CAST(N'2026-09-14T15:43:47.863' AS DateTime), NULL, NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (5, N'Paleta Nude Dreams', N'Paleta de 9 sombras en tonos neutros, con acabados mate y satinados.', CAST(16900.00 AS Decimal(10, 2)), 8, 3, CAST(N'2026-09-14T15:44:17.960' AS DateTime), NULL, NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (6, N'Paleta Pink Glam', N'Paleta de sombras en tonos rosas, malvas y brillantes.', CAST(17500.00 AS Decimal(10, 2)), 6, 3, CAST(N'2026-09-14T15:44:45.430' AS DateTime), NULL, NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (7, N'Máscara Volume Max', N'Máscara negra para aportar volumen y definición a las pestañas.', CAST(11000.00 AS Decimal(10, 2)), 5, 4, CAST(N'2026-09-14T15:45:10.437' AS DateTime), NULL, NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (8, N'Máscara Long Lash', N'Máscara negra de efecto alargador y cepillo de alta precisión.', CAST(12500.00 AS Decimal(10, 2)), 10, 4, CAST(N'2026-09-14T15:45:33.170' AS DateTime), NULL, NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (9, N'Corrector Full Cover', N'Corrector líquido de cobertura alta para ojeras e imperfecciones.', CAST(5200.00 AS Decimal(10, 2)), 13, 5, CAST(N'2026-09-14T15:45:57.827' AS DateTime), CAST(N'2026-09-14T18:45:30.850' AS DateTime), NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (10, N'Corrector Natural Skin', N'Corrector ligero de acabado natural y cobertura media.', CAST(5500.00 AS Decimal(10, 2)), 5, 5, CAST(N'2026-09-14T15:46:24.183' AS DateTime), CAST(N'2026-09-14T18:44:17.453' AS DateTime), NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (11, N'Iluminador Golden Glow', N'Iluminador compacto en tono dorado de acabado luminoso.', CAST(12800.00 AS Decimal(10, 2)), 14, 6, CAST(N'2026-09-14T15:46:49.917' AS DateTime), NULL, NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (12, N'Iluminador Pearl Shine', N'Iluminador perlado de textura suave para puntos de luz del rostro.', CAST(13200.00 AS Decimal(10, 2)), 3, 6, CAST(N'2026-09-14T15:47:12.337' AS DateTime), NULL, NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (13, N'Bronzer Sun Kissed', N'Polvo bronceador de acabado mate para aportar calidez al rostro.', CAST(16500.00 AS Decimal(10, 2)), 8, 7, CAST(N'2026-09-14T15:47:44.850' AS DateTime), CAST(N'2026-09-14T18:44:17.453' AS DateTime), NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (14, N'Bronzer Caramel Glow', N'Bronceador compacto en tono caramelo con acabado ligeramente satinado.', CAST(20000.00 AS Decimal(10, 2)), 8, 7, CAST(N'2026-09-14T15:48:07.473' AS DateTime), NULL, NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (15, N'Rubor Peach Blush', N'Rubor compacto tono durazno de acabado natural.', CAST(7600.00 AS Decimal(10, 2)), 12, 8, CAST(N'2026-09-14T15:48:36.987' AS DateTime), NULL, NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (16, N'Rubor Romantic Pink', N'Rubor rosa suave de textura sedosa y fácil difuminado.', CAST(7600.00 AS Decimal(10, 2)), 8, 8, CAST(N'2026-09-14T15:49:06.003' AS DateTime), NULL, NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (17, N'Delineador Black Precision', N'Delineador líquido negro con punta fina para trazos precisos.', CAST(8100.00 AS Decimal(10, 2)), 6, 9, CAST(N'2026-09-14T15:49:30.433' AS DateTime), NULL, NULL)
INSERT [dbo].[Producto] ([Id_producto], [nombre], [descripcion], [precio], [stock], [id_categoria], [created_at], [updated_at], [deleted_at]) VALUES (18, N'Delineador Brown Define', N'Delineador marrón de acabado mate para un delineado más natural.', CAST(8100.00 AS Decimal(10, 2)), 9, 9, CAST(N'2026-09-14T15:49:56.043' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Producto] OFF
GO
INSERT [dbo].[Rol] ([id_rol], [descripcion]) VALUES (1, N'Administrador')
INSERT [dbo].[Rol] ([id_rol], [descripcion]) VALUES (2, N'Vendedor')
INSERT [dbo].[Rol] ([id_rol], [descripcion]) VALUES (3, N'Repositor')
GO
SET IDENTITY_INSERT [dbo].[Usuario] ON 

INSERT [dbo].[Usuario] ([id_usuario], [nombre], [apellido], [correo], [contraseña], [id_rol], [deleted_at]) VALUES (2, N'Paulina', N'Galeano', N'admin@aura.com', N'1234', 1, NULL)
INSERT [dbo].[Usuario] ([id_usuario], [nombre], [apellido], [correo], [contraseña], [id_rol], [deleted_at]) VALUES (3, N'Juana Maria', N'Perez', N'juanap@aura.com', N'12345', 2, NULL)
INSERT [dbo].[Usuario] ([id_usuario], [nombre], [apellido], [correo], [contraseña], [id_rol], [deleted_at]) VALUES (4, N'Carlos', N'Gomez', N'carlosg@aura.com', N'123456', 3, NULL)
INSERT [dbo].[Usuario] ([id_usuario], [nombre], [apellido], [correo], [contraseña], [id_rol], [deleted_at]) VALUES (5, N'Luana', N'Prueba', N'luana@aura.com', N'12345', 2, CAST(N'2026-09-21T20:36:26.483' AS DateTime))
INSERT [dbo].[Usuario] ([id_usuario], [nombre], [apellido], [correo], [contraseña], [id_rol], [deleted_at]) VALUES (6, N'Marta', N'Sanchez', N'admin2@aura.com', N'1234', 1, CAST(N'2026-09-21T20:59:49.787' AS DateTime))
SET IDENTITY_INSERT [dbo].[Usuario] OFF
GO
SET IDENTITY_INSERT [dbo].[VentaCabecera] ON 

INSERT [dbo].[VentaCabecera] ([Id_ventaCabecera], [id_cliente], [id_usuario], [tipo_factura], [nro_factura], [total], [fecha_venta]) VALUES (1, NULL, 3, N'Factura C', N'V-20260914133017146', CAST(37500.00 AS Decimal(10, 2)), CAST(N'2026-09-14T13:36:55.597' AS DateTime))
INSERT [dbo].[VentaCabecera] ([Id_ventaCabecera], [id_cliente], [id_usuario], [tipo_factura], [nro_factura], [total], [fecha_venta]) VALUES (2, NULL, 3, N'Factura C', N'V-20260914141403530', CAST(18500.00 AS Decimal(10, 2)), CAST(N'2026-09-14T14:14:17.350' AS DateTime))
INSERT [dbo].[VentaCabecera] ([Id_ventaCabecera], [id_cliente], [id_usuario], [tipo_factura], [nro_factura], [total], [fecha_venta]) VALUES (3, 2, 3, N'Factura C', N'V-20260914151857605', CAST(9500.00 AS Decimal(10, 2)), CAST(N'2026-09-14T15:19:14.143' AS DateTime))
INSERT [dbo].[VentaCabecera] ([Id_ventaCabecera], [id_cliente], [id_usuario], [tipo_factura], [nro_factura], [total], [fecha_venta]) VALUES (4, 1, 3, N'Ticket', N'V-20260914180638526', CAST(43300.00 AS Decimal(10, 2)), CAST(N'2026-09-14T18:07:23.290' AS DateTime))
INSERT [dbo].[VentaCabecera] ([Id_ventaCabecera], [id_cliente], [id_usuario], [tipo_factura], [nro_factura], [total], [fecha_venta]) VALUES (5, 3, 3, N'Factura C', N'V-20260914184349385', CAST(38500.00 AS Decimal(10, 2)), CAST(N'2026-09-14T18:44:17.433' AS DateTime))
SET IDENTITY_INSERT [dbo].[VentaCabecera] OFF
GO
SET IDENTITY_INSERT [dbo].[VentaDetalle] ON 

INSERT [dbo].[VentaDetalle] ([Id_detalle], [id_ventaCabecera], [id_producto], [cantidad], [precio_unitario], [subtotal]) VALUES (1, 1, 1, 1, CAST(18500.00 AS Decimal(10, 2)), CAST(18500.00 AS Decimal(10, 2)))
INSERT [dbo].[VentaDetalle] ([Id_detalle], [id_ventaCabecera], [id_producto], [cantidad], [precio_unitario], [subtotal]) VALUES (2, 1, 2, 2, CAST(9500.00 AS Decimal(10, 2)), CAST(19000.00 AS Decimal(10, 2)))
INSERT [dbo].[VentaDetalle] ([Id_detalle], [id_ventaCabecera], [id_producto], [cantidad], [precio_unitario], [subtotal]) VALUES (3, 2, 1, 1, CAST(18500.00 AS Decimal(10, 2)), CAST(18500.00 AS Decimal(10, 2)))
INSERT [dbo].[VentaDetalle] ([Id_detalle], [id_ventaCabecera], [id_producto], [cantidad], [precio_unitario], [subtotal]) VALUES (4, 3, 2, 1, CAST(9500.00 AS Decimal(10, 2)), CAST(9500.00 AS Decimal(10, 2)))
INSERT [dbo].[VentaDetalle] ([Id_detalle], [id_ventaCabecera], [id_producto], [cantidad], [precio_unitario], [subtotal]) VALUES (5, 4, 3, 2, CAST(18900.00 AS Decimal(10, 2)), CAST(37800.00 AS Decimal(10, 2)))
INSERT [dbo].[VentaDetalle] ([Id_detalle], [id_ventaCabecera], [id_producto], [cantidad], [precio_unitario], [subtotal]) VALUES (6, 4, 10, 1, CAST(5500.00 AS Decimal(10, 2)), CAST(5500.00 AS Decimal(10, 2)))
INSERT [dbo].[VentaDetalle] ([Id_detalle], [id_ventaCabecera], [id_producto], [cantidad], [precio_unitario], [subtotal]) VALUES (7, 5, 13, 2, CAST(16500.00 AS Decimal(10, 2)), CAST(33000.00 AS Decimal(10, 2)))
INSERT [dbo].[VentaDetalle] ([Id_detalle], [id_ventaCabecera], [id_producto], [cantidad], [precio_unitario], [subtotal]) VALUES (8, 5, 10, 1, CAST(5500.00 AS Decimal(10, 2)), CAST(5500.00 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[VentaDetalle] OFF
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [UQ__Usuario__2A586E0B4120431C] Fecha de script: 21/9/2026 21:10:59 ******/
ALTER TABLE [dbo].[Usuario] ADD UNIQUE NONCLUSTERED 
(
	[correo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Producto]  WITH CHECK ADD  CONSTRAINT [FK_Producto_Categoria] FOREIGN KEY([id_categoria])
REFERENCES [dbo].[Categoria] ([Id_categoria])
GO
ALTER TABLE [dbo].[Producto] CHECK CONSTRAINT [FK_Producto_Categoria]
GO
ALTER TABLE [dbo].[Usuario]  WITH NOCHECK ADD  CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY([id_rol])
REFERENCES [dbo].[Rol] ([id_rol])
GO
ALTER TABLE [dbo].[Usuario] CHECK CONSTRAINT [FK_Usuario_Rol]
GO
ALTER TABLE [dbo].[VentaCabecera]  WITH CHECK ADD  CONSTRAINT [FK_Venta_Cliente] FOREIGN KEY([id_cliente])
REFERENCES [dbo].[Cliente] ([Id_cliente])
GO
ALTER TABLE [dbo].[VentaCabecera] CHECK CONSTRAINT [FK_Venta_Cliente]
GO
ALTER TABLE [dbo].[VentaCabecera]  WITH CHECK ADD  CONSTRAINT [FK_Venta_Usuario] FOREIGN KEY([id_usuario])
REFERENCES [dbo].[Usuario] ([id_usuario])
GO
ALTER TABLE [dbo].[VentaCabecera] CHECK CONSTRAINT [FK_Venta_Usuario]
GO
ALTER TABLE [dbo].[VentaDetalle]  WITH CHECK ADD  CONSTRAINT [FK_Detalle_Producto] FOREIGN KEY([id_producto])
REFERENCES [dbo].[Producto] ([Id_producto])
GO
ALTER TABLE [dbo].[VentaDetalle] CHECK CONSTRAINT [FK_Detalle_Producto]
GO
ALTER TABLE [dbo].[VentaDetalle]  WITH CHECK ADD  CONSTRAINT [FK_Detalle_Venta] FOREIGN KEY([id_ventaCabecera])
REFERENCES [dbo].[VentaCabecera] ([Id_ventaCabecera])
GO
ALTER TABLE [dbo].[VentaDetalle] CHECK CONSTRAINT [FK_Detalle_Venta]
GO

/* ============================================================
   PROCEDIMIENTOS ALMACENADOS - SOLICITUDES DE STOCK
   ============================================================ */

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_SolicitudStock_Registrar]
    @id_producto INT,
    @id_usuario_solicitante INT,
    @tipo_movimiento VARCHAR(20),
    @cantidad INT,
    @motivo VARCHAR(250)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[SolicitudStock]
    (
        id_producto,
        id_usuario_solicitante,
        tipo_movimiento,
        cantidad,
        motivo
    )
    VALUES
    (
        @id_producto,
        @id_usuario_solicitante,
        @tipo_movimiento,
        @cantidad,
        @motivo
    );

    SELECT CAST(1 AS INT) AS Resultado;
END;
GO


CREATE PROCEDURE [dbo].[SP_SolicitudStock_ListarPendientes]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ss.id_solicitud,
        ss.id_producto,
        p.nombre AS producto,
        p.stock AS stock_actual,
        ss.id_usuario_solicitante,
        u.nombre AS nombre_solicitante,
        u.apellido AS apellido_solicitante,
        ss.tipo_movimiento,
        ss.cantidad,
        ss.motivo,
        ss.estado,
        ss.fecha_solicitud,
        ss.id_usuario_autorizador,
        ss.fecha_resolucion
    FROM [dbo].[SolicitudStock] ss
    INNER JOIN [dbo].[Producto] p
        ON ss.id_producto = p.Id_producto
    INNER JOIN [dbo].[Usuario] u
        ON ss.id_usuario_solicitante = u.id_usuario
    WHERE ss.estado = 'PENDIENTE'
    ORDER BY ss.fecha_solicitud DESC;
END;
GO


CREATE PROCEDURE [dbo].[SP_SolicitudStock_Aprobar]
    @id_solicitud INT,
    @id_usuario_autorizador INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @id_producto INT;
        DECLARE @tipo_movimiento VARCHAR(20);
        DECLARE @cantidad INT;
        DECLARE @stock_actual INT;

        SELECT
            @id_producto = id_producto,
            @tipo_movimiento = tipo_movimiento,
            @cantidad = cantidad
        FROM [dbo].[SolicitudStock] WITH (UPDLOCK, HOLDLOCK)
        WHERE id_solicitud = @id_solicitud
          AND estado = 'PENDIENTE';

        IF @id_producto IS NULL
        BEGIN
            THROW 50001,
                'La solicitud no existe o ya fue resuelta.',
                1;
        END;

        SELECT
            @stock_actual = stock
        FROM [dbo].[Producto] WITH (UPDLOCK, HOLDLOCK)
        WHERE Id_producto = @id_producto;

        IF @stock_actual IS NULL
        BEGIN
            THROW 50002,
                'No se encontró el producto asociado.',
                1;
        END;

        IF @tipo_movimiento = 'INGRESO'
        BEGIN
            UPDATE [dbo].[Producto]
            SET stock = stock + @cantidad
            WHERE Id_producto = @id_producto;
        END
        ELSE IF @tipo_movimiento = 'EGRESO'
        BEGIN
            IF @stock_actual < @cantidad
            BEGIN
                THROW 50003,
                    'No existe stock suficiente para aprobar el egreso.',
                    1;
            END;

            UPDATE [dbo].[Producto]
            SET stock = stock - @cantidad
            WHERE Id_producto = @id_producto;
        END
        ELSE
        BEGIN
            THROW 50004,
                'El tipo de movimiento de la solicitud no es válido.',
                1;
        END;

        UPDATE [dbo].[SolicitudStock]
        SET
            estado = 'APROBADA',
            id_usuario_autorizador = @id_usuario_autorizador,
            fecha_resolucion = GETDATE()
        WHERE id_solicitud = @id_solicitud
          AND estado = 'PENDIENTE';

        COMMIT TRANSACTION;

        SELECT CAST(1 AS INT) AS Resultado;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO


CREATE PROCEDURE [dbo].[SP_SolicitudStock_Rechazar]
    @id_solicitud INT,
    @id_usuario_autorizador INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE [dbo].[SolicitudStock]
        SET
            estado = 'RECHAZADA',
            id_usuario_autorizador = @id_usuario_autorizador,
            fecha_resolucion = GETDATE()
        WHERE id_solicitud = @id_solicitud
          AND estado = 'PENDIENTE';

        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50005,
                'La solicitud no existe o ya fue resuelta.',
                1;
        END;

        COMMIT TRANSACTION;

        SELECT CAST(1 AS INT) AS Resultado;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO


USE [master]
GO
ALTER DATABASE [AuraBeautyDB] SET  READ_WRITE 
GO

-- =============================================
-- PROCEDIMIENTO ALMACENADO: SP_ReporteVentas
-- =============================================
USE AuraBeautyDB;
GO

IF OBJECT_ID('SP_ReporteVentas', 'P') IS NOT NULL
    DROP PROCEDURE SP_ReporteVentas;
GO

CREATE PROCEDURE SP_ReporteVentas
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
        ISNULL(v.nro_factura, 'S/N') AS NroFactura,
        ISNULL(u.nombre + ' ' + u.apellido, 'Sin Vendedor') AS Vendedor,
        ISNULL(v.id_cliente, 'Consumidor Final') AS Cliente,
        v.total AS Total
    FROM VentaCabecera v
    LEFT JOIN Usuario u ON u.id_usuario = v.id_usuario
    WHERE CAST(v.fecha_venta AS DATE) >= CAST(@FechaInicio AS DATE)
      AND CAST(v.fecha_venta AS DATE) <= CAST(@FechaFin AS DATE)
      AND (@IdUsuario IS NULL OR v.id_usuario = @IdUsuario)
    ORDER BY v.fecha_venta DESC;
END;
GO

-- =============================================
-- PROCEDIMIENTO ALMACENADO: SP_ReporteMovimientosStock
-- =============================================
IF OBJECT_ID('SP_ReporteMovimientosStock', 'P') IS NOT NULL
    DROP PROCEDURE SP_ReporteMovimientosStock;
GO

CREATE PROCEDURE SP_ReporteMovimientosStock
(
    @FechaInicio DATETIME,
    @FechaFin DATETIME
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.id_solicitud AS IdSolicitud,
        ISNULL(p.nombre, 'Sin Producto') AS Producto,
        ISNULL(c.nombre, 'Sin Categoría') AS Categoria,
        ISNULL(s.tipo_movimiento, 'N/A') AS TipoMovimiento,
        ISNULL(s.Cantidad, 0) AS Cantidad,
        ISNULL(s.Motivo, '') AS Motivo,
        ISNULL(s.Estado, '') AS Estado,
        s.fecha_solicitud,
        ISNULL(s.id_usuario_solicitante, '') AS Solicitante
    FROM SolicitudStock s
    LEFT JOIN Producto p ON p.Id_producto = s.id_producto
    LEFT JOIN Categoria c ON c.Id_categoria = p.id_categoria
    WHERE CAST(s.fecha_solicitud AS DATE) >= CAST(@FechaInicio AS DATE)
      AND CAST(s.fecha_resolucion AS DATE) <= CAST(@FechaFin AS DATE)
    ORDER BY s.fecha_solicitud DESC;
END;
GO



--===========================================================================
-- 02 - BASE ADMINISTRATIVA Y AUDITORIA
--===========================================================================
USE [master];
GO

/*
AURA BEAUTY - BASE ADMINISTRATIVA

Esta base se conservará aunque se restaure
AuraBeautyDB desde un respaldo anterior.
*/

IF DB_ID(N'AuraBeautyDB_Admin') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE [AuraBeautyDB_Admin]');
END;
GO

USE [AuraBeautyDB_Admin];
GO

IF OBJECT_ID(
    N'dbo.RestauracionAuditoria', N'U'
) IS NULL
BEGIN
    CREATE TABLE dbo.RestauracionAuditoria
    (
        IdEjecucion UNIQUEIDENTIFIER NOT NULL
            PRIMARY KEY,

        IdSolicitud INT NOT NULL,

        IdAutorizador INT NOT NULL,

        RutaBackupOrigen NVARCHAR(4000) NOT NULL,

        RutaBackupPreventivo NVARCHAR(4000) NOT NULL,

        Estado VARCHAR(20) NOT NULL,

        FechaInicio DATETIME2(0) NOT NULL
            DEFAULT SYSDATETIME(),

        FechaFin DATETIME2(0) NULL,

        Detalle NVARCHAR(2000) NULL,

        CONSTRAINT CK_RestauracionAuditoria_Estado
            CHECK
            (
                Estado IN
                (
                    'INICIADA',
                    'RESGUARDADA',
                    'RESTAURANDO',
                    'EXITOSA',
                    'FALLIDA',
                    'REVISAR'
                )
            )
    );
END;
GO

-- Verificación: solo lectura.

SELECT
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'RestauracionAuditoria';
GO



--===========================================================================
-- 03 - COLUMNAS DE IDENTIFICACION DE SOLICITUD EN AUDITORIA
--===========================================================================
USE AuraBeautyDB_Admin;
GO

/*
==================================================
AURA BEAUTY - IDENTIFICACIÓN DE SOLICITUDES

Se agregan datos que permiten distinguir
solicitudes diferentes, incluso si después
de un RESTORE se reutiliza un identificador.

No elimina registros existentes.
No modifica AuraBeautyDB.
No ejecuta ninguna restauración.
==================================================
*/

-- 1. Identificador del respaldo solicitado.

IF COL_LENGTH(
    'dbo.RestauracionAuditoria',
    'IdBackupOrigen'
) IS NULL
BEGIN
    ALTER TABLE dbo.RestauracionAuditoria
    ADD IdBackupOrigen INT NULL;
END;
GO

-- 2. Fecha exacta de creación de la solicitud.

IF COL_LENGTH(
    'dbo.RestauracionAuditoria',
    'FechaSolicitudOrigen'
) IS NULL
BEGIN
    ALTER TABLE dbo.RestauracionAuditoria
    ADD FechaSolicitudOrigen DATETIME2(7) NULL;
END;
GO

/*
3. Índice único.

Impedirá registrar nuevamente una misma
solicitud identificada por los tres campos,
salvo cuando una ejecución previa haya
fallado antes de iniciar la restauración.

Se excluyen por ahora los registros antiguos
que todavía no tienen los nuevos datos.
*/

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_Auditoria_SolicitudOriginal'
      AND object_id =
          OBJECT_ID('dbo.RestauracionAuditoria')
)
BEGIN
    CREATE UNIQUE INDEX UX_Auditoria_SolicitudOriginal
    ON dbo.RestauracionAuditoria
    (
        IdSolicitud,
        IdBackupOrigen,
        FechaSolicitudOrigen
    )
    WHERE IdBackupOrigen IS NOT NULL
      AND FechaSolicitudOrigen IS NOT NULL
      AND Estado <> 'FALLIDA';
END;
GO

-- 4. Verificar las columnas y los registros.

SELECT
    IdSolicitud,
    IdBackupOrigen,
    FechaSolicitudOrigen,
    Estado,
    FechaInicio
FROM dbo.RestauracionAuditoria
ORDER BY FechaInicio DESC;
GO



--===========================================================================
-- 04 - RESERVAS DE PRODUCTOS (TABLA INTERMEDIA)
--===========================================================================
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



--===========================================================================
-- 05 - TIPO TVP PARA CONFIRMAR VENTAS
--===========================================================================
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



--===========================================================================
-- 06 - RESERVAR PRODUCTO
--===========================================================================
USE [AuraBeautyDB]
GO

/****** Objeto: StoredProcedure [dbo].[SP_ReservaProducto_Reservar] Fecha de script: 26/10/2026 10:58:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[SP_ReservaProducto_Reservar]
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



--===========================================================================
-- 07 - CANCELAR RESERVA
--===========================================================================
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

CREATE OR ALTER PROCEDURE [dbo].[SP_ReservaProducto_Cancelar]
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



--===========================================================================
-- 08 - CONFIRMAR VENTA Y RESERVAS
--===========================================================================
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

CREATE OR ALTER PROCEDURE [dbo].[SP_Venta_ConfirmarReservas]
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



--===========================================================================
-- 09 - REPORTES DE ANALISIS DE VENTAS
--===========================================================================
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



--===========================================================================
-- 10 - REGISTROS OPERATIVOS DE BACKUP Y SOLICITUDES
--===========================================================================
USE [AuraBeautyDB];
GO

-- =========================================================
-- 1. HISTORIAL DE COPIAS DE SEGURIDAD
-- Guarda información de cada backup realizado.
-- No almacena los datos de la copia dentro de esta tabla:
-- el archivo .bak se guarda físicamente en el disco.
-- =========================================================

IF OBJECT_ID('dbo.BackupRegistro', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.BackupRegistro
    (
        id_backup INT IDENTITY(1,1) PRIMARY KEY,

        nombre_archivo NVARCHAR(260) NOT NULL,

        ruta_archivo NVARCHAR(500) NOT NULL,

        fecha_creacion DATETIME2 NOT NULL
            DEFAULT SYSDATETIME(),

        id_usuario INT NOT NULL,

        estado VARCHAR(20) NOT NULL,

        observaciones NVARCHAR(1000) NULL,

        CONSTRAINT FK_BackupRegistro_Usuario
            FOREIGN KEY (id_usuario)
            REFERENCES dbo.Usuario(id_usuario),

        CONSTRAINT CK_BackupRegistro_Estado
            CHECK (estado IN ('Exitoso', 'Fallido'))
    );
END;
GO

-- =========================================================
-- 2. SOLICITUDES DE RESTAURACIÓN
-- Registra quién solicita restaurar un backup,
-- quién lo autoriza y el estado de la solicitud.
-- =========================================================

IF OBJECT_ID('dbo.RestauracionSolicitud', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RestauracionSolicitud
    (
        id_solicitud INT IDENTITY(1,1) PRIMARY KEY,

        id_backup INT NOT NULL,

        id_solicitante INT NOT NULL,

        id_autorizador INT NULL,

        fecha_solicitud DATETIME2 NOT NULL
            DEFAULT SYSDATETIME(),

        fecha_resolucion DATETIME2 NULL,

        estado VARCHAR(20) NOT NULL
            DEFAULT 'Pendiente',

        observaciones NVARCHAR(1000) NULL,

        CONSTRAINT FK_Restauracion_Backup
            FOREIGN KEY (id_backup)
            REFERENCES dbo.BackupRegistro(id_backup),

        CONSTRAINT FK_Restauracion_Solicitante
            FOREIGN KEY (id_solicitante)
            REFERENCES dbo.Usuario(id_usuario),

        CONSTRAINT FK_Restauracion_Autorizador
            FOREIGN KEY (id_autorizador)
            REFERENCES dbo.Usuario(id_usuario),

        CONSTRAINT CK_Restauracion_Estado
            CHECK
            (
                estado IN
                (
                    'Pendiente',
                    'Autorizada',
                    'Rechazada',
                    'Exitosa',
                    'Fallida'
                )
            ),

        CONSTRAINT CK_Restauracion_DosUsuarios
            CHECK
            (
                id_autorizador IS NULL
                OR id_autorizador <> id_solicitante
            )
    );
END;
GO



--===========================================================================
-- 11 - REGISTRAR BACKUP
--===========================================================================
USE [AuraBeautyDB];
GO

-- ============================================================
-- PROCEDIMIENTO: SP_Backup_Registrar
--
-- OBJETIVO:
-- Registrar en la tabla BackupRegistro el resultado de una
-- operación de copia de seguridad.
--
-- IMPORTANTE:
-- Este procedimiento NO crea el archivo .bak.
-- El archivo debe generarse primero desde C#.
--
-- SEGURIDAD:
-- Solo permite registrar operaciones realizadas por usuarios
-- cuyo rol sea Administrador (id_rol = 1).
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_Backup_Registrar

    -- Nombre del archivo de copia de seguridad.
    @NombreArchivo NVARCHAR(260),

    -- Ruta física donde se guardó el archivo .bak.
    @RutaArchivo NVARCHAR(500),

    -- Identificador del usuario que realizó la operación.
    @IdUsuario INT,

    -- Resultado de la operación: Exitoso o Fallido.
    @Estado VARCHAR(20),

    -- Mensaje opcional con información adicional.
    @Observaciones NVARCHAR(1000) = NULL

AS
BEGIN

    -- Evita que SQL Server devuelva mensajes adicionales
    -- con la cantidad de filas afectadas.
    SET NOCOUNT ON;

    -- ========================================================
    -- 1. VALIDAR LOS DATOS RECIBIDOS
    -- Comprobamos que los campos obligatorios tengan valores.
    -- ========================================================

    IF @NombreArchivo IS NULL
       OR LTRIM(RTRIM(@NombreArchivo)) = ''
    BEGIN
        THROW 50001,
              'El nombre del archivo es obligatorio.',
              1;
    END;

    IF @RutaArchivo IS NULL
       OR LTRIM(RTRIM(@RutaArchivo)) = ''
    BEGIN
        THROW 50002,
              'La ruta del archivo es obligatoria.',
              1;
    END;

    IF @IdUsuario IS NULL OR @IdUsuario <= 0
    BEGIN
        THROW 50003,
              'El identificador del usuario no es válido.',
              1;
    END;

    -- ========================================================
    -- 2. VALIDAR EL ESTADO DE LA OPERACIÓN
    -- Solo aceptamos los dos estados definidos en la tabla.
    -- ========================================================

    IF @Estado NOT IN ('Exitoso', 'Fallido')
       OR @Estado IS NULL
    BEGIN
        THROW 50004,
              'El estado debe ser Exitoso o Fallido.',
              1;
    END;

    -- ========================================================
    -- 3. VERIFICAR QUE EL USUARIO EXISTA Y SEA ADMINISTRADOR
    --
    -- Se consulta Usuario para comprobar el rol asignado.
    -- Según la configuración actual de Aura Beauty,
    -- id_rol = 1 corresponde al Administrador.
    -- ========================================================

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
    )
    BEGIN
        THROW 50005,
              'El usuario no existe o no tiene rol Administrador.',
              1;
    END;

    -- ========================================================
    -- 4. REGISTRAR EL RESULTADO DEL BACKUP
    --
    -- La fecha se genera automáticamente mediante el DEFAULT
    -- de la columna fecha_creacion de BackupRegistro.
    -- ========================================================

    INSERT INTO dbo.BackupRegistro
    (
        nombre_archivo,
        ruta_archivo,
        id_usuario,
        estado,
        observaciones
    )
    VALUES
    (
        @NombreArchivo,
        @RutaArchivo,
        @IdUsuario,
        @Estado,
        @Observaciones
    );

    -- ========================================================
    -- 5. DEVOLVER EL IDENTIFICADOR DEL REGISTRO CREADO
    --
    -- SCOPE_IDENTITY devuelve el IDENTITY generado por este
    -- INSERT dentro del mismo ámbito.
    -- ========================================================

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdBackup;

END;
GO



--===========================================================================
-- 12 - LISTAR BACKUPS OPERATIVOS
--===========================================================================
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



--===========================================================================
-- 13 - VALIDAR AUTORIZADOR
--===========================================================================
USE AuraBeautyDB;
GO

CREATE OR ALTER PROCEDURE dbo.SP_Backup_ValidarAutorizador
    @Contrasena NVARCHAR(100),
    @IdSolicitante INT,
    @IdAdministradorSesion INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Contrasena IS NULL OR LEN(@Contrasena) = 0
        THROW 51401, 'Ingresá la contraseña.', 1;

    IF @IdSolicitante IS NULL OR @IdSolicitante <= 0
       OR @IdAdministradorSesion IS NULL
       OR @IdAdministradorSesion <= 0
        THROW 51402, 'Los usuarios indicados no son válidos.', 1;

    -- Debe existir un administrador activo en la sesión.
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdAdministradorSesion
          AND id_rol = 1
          AND deleted_at IS NULL
    )
        THROW 51403,
            'La sesión no pertenece a un administrador activo.', 1;

    DECLARE @Coincidencias INT;
    DECLARE @IdAutorizador INT;

    /*
    Versión compatible con la autenticación actual
    de Aura Beauty, que compara contraseñas en texto.

    La comparación BIN2 distingue mayúsculas
    y minúsculas.
    */

    SELECT
        @Coincidencias = COUNT(*),
        @IdAutorizador = MIN(id_usuario)
    FROM dbo.Usuario
    WHERE id_rol = 1
      AND deleted_at IS NULL
      AND contraseña COLLATE Latin1_General_100_BIN2 =
          @Contrasena COLLATE Latin1_General_100_BIN2;

    IF @Coincidencias = 0
        THROW 51404,
            'Contraseña incorrecta.', 1;

    IF @Coincidencias > 1
        THROW 51405,
            'Varios administradores tienen esa contraseña. No es posible identificar al responsable.',
            1;

    IF @IdAutorizador = @IdSolicitante
        THROW 51406,
            'No podés resolver tu propia solicitud.', 1;

    IF @IdAutorizador = @IdAdministradorSesion
        THROW 51407,
            'Debe ingresar la contraseña de otro administrador.',
            1;

    SELECT @IdAutorizador AS IdAutorizador;
END;
GO



--===========================================================================
-- 14 - VALIDAR CLAVE
--===========================================================================
USE [AuraBeautyDB];
GO

/*
===================================================
AURA BEAUTY - VALIDAR CONTRASEÑA DEL ADMINISTRADOR

Este procedimiento:
- Identifica al administrador por su ID de sesión.
- Comprueba que esté activo.
- Valida exclusivamente su contraseña.
- No permite autenticar a otro usuario con esa clave.
- No modifica registros ni contraseñas.
===================================================
*/

CREATE OR ALTER PROCEDURE dbo.SP_Backup_ValidarClaveSesion
    @IdUsuario INT,
    @Contrasena NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    IF @IdUsuario IS NULL OR @IdUsuario <= 0
        THROW 51501,
            'El administrador no es válido.', 1;

    IF @Contrasena IS NULL OR LEN(@Contrasena) = 0
        THROW 51502,
            'Debés ingresar tu contraseña.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
          AND deleted_at IS NULL
    )
        THROW 51503,
            'El usuario no es un administrador activo.', 1;

    /*
    Se utiliza temporalmente el sistema de
    contraseñas existente en Aura Beauty.

    La comparación BIN2 distingue mayúsculas
    y minúsculas.
    */

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
          AND deleted_at IS NULL
          AND contraseña COLLATE Latin1_General_100_BIN2 =
              @Contrasena COLLATE Latin1_General_100_BIN2
    )
        THROW 51504,
            'La contraseña ingresada es incorrecta.', 1;

    SELECT CAST(1 AS BIT) AS ClaveValida;
END;
GO



--===========================================================================
-- 15 - SOLICITAR RESTAURACION
--===========================================================================
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



--===========================================================================
-- 16 - AUTORIZAR RESTAURACION
--===========================================================================
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



--===========================================================================
-- 17 - RECHAZAR RESTAURACION
--===========================================================================
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



--===========================================================================
-- 18 - LISTAR PENDIENTES
--===========================================================================
USE AuraBeautyDB;
GO

/*
=====================================================
AURA BEAUTY
VISIBILIDAD DE SOLICITUDES DE RESTAURACIÓN

Regla:
Un administrador puede consultar las solicitudes
de otros administradores, pero no las propias.

No elimina ni modifica solicitudes existentes.
=====================================================
*/

-- ==============================================
-- 1. SOLICITUDES PENDIENTES
-- ==============================================

CREATE OR ALTER PROCEDURE dbo.SP_Restore_ListarPendientes
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
          AND deleted_at IS NULL
    )
    BEGIN
        THROW 50001,
            'El usuario no es un administrador activo.',
            1;
    END;

    SELECT
        rs.id_solicitud AS IdSolicitud,
        rs.id_backup AS IdBackup,
        b.nombre_archivo AS NombreArchivo,
        b.ruta_archivo AS RutaArchivo,
        rs.id_solicitante AS IdSolicitante,
        u.nombre + ' ' + u.apellido AS Solicitante,
        rs.fecha_solicitud AS FechaSolicitud,
        rs.estado AS Estado,
        rs.observaciones AS Observaciones
    FROM dbo.RestauracionSolicitud AS rs
    INNER JOIN dbo.BackupRegistro AS b
        ON rs.id_backup = b.id_backup
    INNER JOIN dbo.Usuario AS u
        ON rs.id_solicitante = u.id_usuario
    WHERE rs.estado = 'Pendiente'
      AND rs.id_solicitante <> @IdUsuario
    ORDER BY rs.fecha_solicitud ASC;
END;
GO

-- ==============================================
-- 2. HISTORIAL DE SOLICITUDES
-- ==============================================

CREATE OR ALTER PROCEDURE dbo.SP_Restore_ListarHistorial
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
          AND deleted_at IS NULL
    )
    BEGIN
        THROW 50010,
            'Solo un administrador activo puede consultar el historial.',
            1;
    END;

    SELECT
        rs.id_solicitud AS IdSolicitud,
        rs.id_backup AS IdBackup,
        b.nombre_archivo AS NombreArchivo,
        b.fecha_creacion AS FechaBackup,
        rs.id_solicitante AS IdSolicitante,
        CONCAT(us.nombre, ' ', us.apellido) AS Solicitante,
        rs.id_autorizador AS IdAutorizador,
        CASE
            WHEN ua.id_usuario IS NULL THEN NULL
            ELSE CONCAT(ua.nombre, ' ', ua.apellido)
        END AS Autorizador,
        rs.fecha_solicitud AS FechaSolicitud,
        rs.fecha_resolucion AS FechaResolucion,
        rs.estado AS Estado,
        rs.observaciones AS Observaciones
    FROM dbo.RestauracionSolicitud AS rs
    INNER JOIN dbo.BackupRegistro AS b
        ON b.id_backup = rs.id_backup
    INNER JOIN dbo.Usuario AS us
        ON us.id_usuario = rs.id_solicitante
    LEFT JOIN dbo.Usuario AS ua
        ON ua.id_usuario = rs.id_autorizador
    WHERE rs.id_solicitante <> @IdUsuario
    ORDER BY rs.fecha_solicitud DESC;
END;
GO

-- ==============================================
-- 3. CONSULTA INDIVIDUAL DE UNA SOLICITUD
-- ==============================================

CREATE OR ALTER PROCEDURE dbo.SP_Restore_ConsultarEstado
    @IdSolicitud INT,
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @IdSolicitud IS NULL OR @IdSolicitud <= 0
        THROW 50051,
            'El identificador de la solicitud no es válido.',
            1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
          AND deleted_at IS NULL
    )
        THROW 50053,
            'Solo un administrador activo puede consultar el estado.',
            1;

    SELECT
        r.id_solicitud AS IdSolicitud,
        r.id_backup AS IdBackup,
        b.nombre_archivo AS NombreArchivo,
        b.ruta_archivo AS RutaArchivo,
        r.id_solicitante AS IdSolicitante,
        LTRIM(RTRIM(
            CONCAT(us.nombre, ' ', us.apellido)
        )) AS Solicitante,
        r.id_autorizador AS IdAutorizador,
        LTRIM(RTRIM(
            CONCAT(ua.nombre, ' ', ua.apellido)
        )) AS Autorizador,
        r.fecha_solicitud AS FechaSolicitud,
        r.fecha_resolucion AS FechaResolucion,
        r.estado AS Estado,
        r.observaciones AS Observaciones
    FROM dbo.RestauracionSolicitud AS r
    INNER JOIN dbo.BackupRegistro AS b
        ON r.id_backup = b.id_backup
    INNER JOIN dbo.Usuario AS us
        ON r.id_solicitante = us.id_usuario
    LEFT JOIN dbo.Usuario AS ua
        ON r.id_autorizador = ua.id_usuario
    WHERE r.id_solicitud = @IdSolicitud
      AND r.id_solicitante <> @IdUsuario;
END;
GO



--===========================================================================
-- 19 - CONSULTAR ESTADO
--===========================================================================
USE [AuraBeautyDB];
GO

-- ============================================================
-- PROCEDIMIENTO: SP_Restore_ConsultarEstado
--
-- OBJETIVO:
-- Consultar el estado y los datos de una solicitud de
-- restauración de una copia de seguridad.
--
-- INFORMACIÓN DEVUELTA:
-- - Datos de la solicitud.
-- - Backup que se desea restaurar.
-- - Administrador solicitante.
-- - Administrador que autorizó o rechazó.
-- - Fechas y estado actual.
-- - Observaciones registradas.
--
-- IMPORTANTE:
-- Este procedimiento solamente consulta información.
-- No modifica registros ni ejecuta restauraciones.
-- ============================================================

CREATE OR ALTER PROCEDURE dbo.SP_Restore_ConsultarEstado

    -- Identificador de la solicitud que se quiere consultar.
    @IdSolicitud INT,

    -- Identificador del administrador que realiza la consulta.
    @IdUsuario INT

AS
BEGIN

    -- Evita mensajes adicionales sobre filas afectadas.
    SET NOCOUNT ON;

    -- ========================================================
    -- 1. VALIDAR LOS IDENTIFICADORES RECIBIDOS
    -- ========================================================

    IF @IdSolicitud IS NULL OR @IdSolicitud <= 0
    BEGIN
        THROW 50051,
              'El identificador de la solicitud no es válido.',
              1;
    END;

    IF @IdUsuario IS NULL OR @IdUsuario <= 0
    BEGIN
        THROW 50052,
              'El identificador del usuario no es válido.',
              1;
    END;

    -- ========================================================
    -- 2. VERIFICAR QUE EL USUARIO SEA ADMINISTRADOR
    --
    -- En Aura Beauty, id_rol = 1 corresponde al Administrador.
    -- La identidad del usuario también debe validarse desde C#.
    -- ========================================================

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1
    )
    BEGIN
        THROW 50053,
              'Solo un administrador puede consultar el estado.',
              1;
    END;

    -- ========================================================
    -- 3. CONSULTAR LA SOLICITUD
    --
    -- Se relacionan las tablas para obtener información legible
    -- sobre el backup y los usuarios involucrados.
    --
    -- LEFT JOIN permite mostrar la solicitud aunque todavía
    -- no tenga un autorizador asignado.
    -- ========================================================

    SELECT
        -- Identificador único de la solicitud.
        r.id_solicitud AS IdSolicitud,

        -- Identificador del backup solicitado.
        r.id_backup AS IdBackup,

        -- Nombre del archivo de backup.
        b.nombre_archivo AS NombreArchivo,

        -- Ruta física del archivo.
        b.ruta_archivo AS RutaArchivo,

        -- Usuario que solicitó la restauración.
        r.id_solicitante AS IdSolicitante,

        -- Nombre completo del solicitante.
        LTRIM(RTRIM(
            CONCAT(us.nombre, ' ', us.apellido)
        )) AS Solicitante,

        -- Usuario que autorizó o rechazó la solicitud.
        r.id_autorizador AS IdAutorizador,

        -- Nombre completo del autorizador, si existe.
        LTRIM(RTRIM(
            CONCAT(ua.nombre, ' ', ua.apellido)
        )) AS Autorizador,

        -- Fecha en la que se creó la solicitud.
        r.fecha_solicitud AS FechaSolicitud,

        -- Fecha en la que se resolvió, si corresponde.
        r.fecha_resolucion AS FechaResolucion,

        -- Estado actual de la solicitud.
        r.estado AS Estado,

        -- Observaciones o motivo del rechazo.
        r.observaciones AS Observaciones

    FROM dbo.RestauracionSolicitud AS r

    -- Obtener información del backup seleccionado.
    INNER JOIN dbo.BackupRegistro AS b
        ON r.id_backup = b.id_backup

    -- Obtener el nombre del administrador solicitante.
    INNER JOIN dbo.Usuario AS us
        ON r.id_solicitante = us.id_usuario

    -- Obtener el nombre del administrador que resolvió
    -- la solicitud. Puede ser NULL si sigue pendiente.
    LEFT JOIN dbo.Usuario AS ua
        ON r.id_autorizador = ua.id_usuario

    -- Devolver únicamente la solicitud seleccionada.
    WHERE r.id_solicitud = @IdSolicitud;

    -- ========================================================
    -- 4. INTERPRETAR EL RESULTADO
    --
    -- Si la solicitud existe, se devuelve una fila.
    -- Si no existe, se devuelve un conjunto vacío.
    -- No se modifica ningún dato.
    -- ========================================================

END;
GO



--===========================================================================
-- 20 - HISTORIALES PERSISTENTES, TRIGGERS Y SINCRONIZACION
--===========================================================================
/*
 AURA BEAUTY - MIGRACION ADITIVA DE HISTORIALES
 PREVIAMENTE: respaldar AuraBeautyDB y AuraBeautyDB_Admin.
 No ejecuta RESTORE, no borra registros y no crea solicitudes ficticias.
 Ejecutar conectado a la misma instancia LocalDB que utiliza Aura Beauty.
*/
USE [AuraBeautyDB_Admin];
GO
IF OBJECT_ID(N'dbo.BackupHistorial', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.BackupHistorial
 (
  IdRegistro UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_BackupHist_Id DEFAULT NEWID() PRIMARY KEY,
  IdBackupOriginal INT NOT NULL,
  NombreArchivo NVARCHAR(260) NOT NULL,
  RutaArchivo NVARCHAR(500) NOT NULL,
  FechaCreacion DATETIME2(7) NOT NULL,
  IdUsuario INT NOT NULL,
  UsuarioNombre NVARCHAR(260) NULL,
  Estado VARCHAR(20) NOT NULL,
  Observaciones NVARCHAR(1000) NULL,
  FechaArchivado DATETIME2(7) NOT NULL DEFAULT SYSDATETIME()
 );
END;
GO
IF OBJECT_ID(N'dbo.SolicitudHistorial', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.SolicitudHistorial
 (
  IdRegistro UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_SolHist_Id DEFAULT NEWID() PRIMARY KEY,
  IdSolicitudOriginal INT NOT NULL,
  IdBackupOriginal INT NOT NULL,
  FechaSolicitud DATETIME2(7) NOT NULL,
  NombreArchivo NVARCHAR(260) NULL,
  FechaBackup DATETIME2(7) NULL,
  IdSolicitante INT NOT NULL,
  Solicitante NVARCHAR(260) NULL,
  IdAutorizador INT NULL,
  Autorizador NVARCHAR(260) NULL,
  FechaResolucion DATETIME2(7) NULL,
  Estado VARCHAR(20) NOT NULL,
  Observaciones NVARCHAR(1000) NULL,
  FechaArchivado DATETIME2(7) NOT NULL DEFAULT SYSDATETIME()
 );
END;
GO
/* La identidad se compone de ID + fecha + backup: los IDENTITY pueden reiniciarse tras RESTORE. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.SolicitudHistorial') AND name=N'UX_SolHist_Origen')
 CREATE UNIQUE INDEX UX_SolHist_Origen ON dbo.SolicitudHistorial (IdSolicitudOriginal, IdBackupOriginal, FechaSolicitud);
GO
/* El backup también puede reutilizar un ID luego de restaurar. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID(N'dbo.BackupHistorial') AND name=N'IX_BackupHist_Origen')
 CREATE INDEX IX_BackupHist_Origen ON dbo.BackupHistorial (IdBackupOriginal, FechaCreacion);
GO
USE [AuraBeautyDB];
GO
/* Registrar altas y modificaciones de backups mediante snapshots en la BD externa. */
CREATE OR ALTER TRIGGER dbo.TR_BackupRegistro_Historial
ON dbo.BackupRegistro
AFTER INSERT, UPDATE
AS
BEGIN
 SET NOCOUNT ON;
 /* Coincidencia por ID + fecha + ruta; permite reutilización de ID tras una restauración. */
 UPDATE h SET h.NombreArchivo=i.nombre_archivo,
  h.IdUsuario=i.id_usuario,
  h.UsuarioNombre=LTRIM(RTRIM(CONCAT(u.nombre,' ',u.apellido))),
  h.Estado=i.estado, h.Observaciones=i.observaciones,
  h.FechaArchivado=SYSDATETIME()
 FROM AuraBeautyDB_Admin.dbo.BackupHistorial h
 JOIN inserted i ON h.IdBackupOriginal=i.id_backup AND h.FechaCreacion=i.fecha_creacion AND h.RutaArchivo=i.ruta_archivo
 LEFT JOIN dbo.Usuario u ON u.id_usuario=i.id_usuario;
 INSERT AuraBeautyDB_Admin.dbo.BackupHistorial
 (IdBackupOriginal,NombreArchivo,RutaArchivo,FechaCreacion,IdUsuario,UsuarioNombre,Estado,Observaciones)
 SELECT i.id_backup,i.nombre_archivo,i.ruta_archivo,i.fecha_creacion,i.id_usuario,
  LTRIM(RTRIM(CONCAT(u.nombre,' ',u.apellido))),i.estado,i.observaciones
 FROM inserted i LEFT JOIN dbo.Usuario u ON u.id_usuario=i.id_usuario
 WHERE NOT EXISTS
 (SELECT 1 FROM AuraBeautyDB_Admin.dbo.BackupHistorial h WITH (UPDLOCK,HOLDLOCK)
  WHERE h.IdBackupOriginal=i.id_backup AND h.FechaCreacion=i.fecha_creacion AND h.RutaArchivo=i.ruta_archivo);
END;
GO
/* Un alta registra Pendiente; un UPDATE captura Autorizada, Rechazada, Exitosa y Fallida. */
CREATE OR ALTER TRIGGER dbo.TR_RestauracionSolicitud_Historial
ON dbo.RestauracionSolicitud
AFTER INSERT, UPDATE
AS
BEGIN
 SET NOCOUNT ON;
 UPDATE h SET
  h.NombreArchivo=b.nombre_archivo, h.FechaBackup=b.fecha_creacion,
  h.Solicitante=LTRIM(RTRIM(CONCAT(us.nombre,' ',us.apellido))),
  h.IdAutorizador=i.id_autorizador,
  h.Autorizador=CASE WHEN ua.id_usuario IS NULL THEN NULL ELSE LTRIM(RTRIM(CONCAT(ua.nombre,' ',ua.apellido))) END,
  h.FechaResolucion=i.fecha_resolucion,h.Estado=i.estado,
  h.Observaciones=i.observaciones,h.FechaArchivado=SYSDATETIME()
 FROM AuraBeautyDB_Admin.dbo.SolicitudHistorial h
 JOIN inserted i ON h.IdSolicitudOriginal=i.id_solicitud AND h.IdBackupOriginal=i.id_backup AND h.FechaSolicitud=i.fecha_solicitud
 LEFT JOIN dbo.BackupRegistro b ON b.id_backup=i.id_backup
 LEFT JOIN dbo.Usuario us ON us.id_usuario=i.id_solicitante
 LEFT JOIN dbo.Usuario ua ON ua.id_usuario=i.id_autorizador;
 INSERT AuraBeautyDB_Admin.dbo.SolicitudHistorial
 (IdSolicitudOriginal,IdBackupOriginal,FechaSolicitud,NombreArchivo,FechaBackup,IdSolicitante,Solicitante,
  IdAutorizador,Autorizador,FechaResolucion,Estado,Observaciones)
 SELECT i.id_solicitud,i.id_backup,i.fecha_solicitud,b.nombre_archivo,b.fecha_creacion,i.id_solicitante,
  LTRIM(RTRIM(CONCAT(us.nombre,' ',us.apellido))),i.id_autorizador,
  CASE WHEN ua.id_usuario IS NULL THEN NULL ELSE LTRIM(RTRIM(CONCAT(ua.nombre,' ',ua.apellido))) END,
  i.fecha_resolucion,i.estado,i.observaciones
 FROM inserted i
 LEFT JOIN dbo.BackupRegistro b ON b.id_backup=i.id_backup
 LEFT JOIN dbo.Usuario us ON us.id_usuario=i.id_solicitante
 LEFT JOIN dbo.Usuario ua ON ua.id_usuario=i.id_autorizador
 WHERE NOT EXISTS
 (SELECT 1 FROM AuraBeautyDB_Admin.dbo.SolicitudHistorial h WITH (UPDLOCK,HOLDLOCK)
  WHERE h.IdSolicitudOriginal=i.id_solicitud AND h.IdBackupOriginal=i.id_backup AND h.FechaSolicitud=i.fecha_solicitud);
END;
GO
/* Migrar lo que exista HOY, sin inventar filas borradas por backups anteriores. */
INSERT AuraBeautyDB_Admin.dbo.BackupHistorial
 (IdBackupOriginal,NombreArchivo,RutaArchivo,FechaCreacion,IdUsuario,UsuarioNombre,Estado,Observaciones)
SELECT b.id_backup,b.nombre_archivo,b.ruta_archivo,b.fecha_creacion,b.id_usuario,
 LTRIM(RTRIM(CONCAT(u.nombre,' ',u.apellido))),b.estado,b.observaciones
FROM dbo.BackupRegistro b LEFT JOIN dbo.Usuario u ON u.id_usuario=b.id_usuario
WHERE NOT EXISTS
(SELECT 1 FROM AuraBeautyDB_Admin.dbo.BackupHistorial h
 WHERE h.IdBackupOriginal=b.id_backup AND h.FechaCreacion=b.fecha_creacion AND h.RutaArchivo=b.ruta_archivo);
GO
INSERT AuraBeautyDB_Admin.dbo.SolicitudHistorial
 (IdSolicitudOriginal,IdBackupOriginal,FechaSolicitud,NombreArchivo,FechaBackup,IdSolicitante,Solicitante,IdAutorizador,Autorizador,FechaResolucion,Estado,Observaciones)
SELECT r.id_solicitud,r.id_backup,r.fecha_solicitud,b.nombre_archivo,b.fecha_creacion,r.id_solicitante,
 LTRIM(RTRIM(CONCAT(us.nombre,' ',us.apellido))),r.id_autorizador,
 CASE WHEN ua.id_usuario IS NULL THEN NULL ELSE LTRIM(RTRIM(CONCAT(ua.nombre,' ',ua.apellido))) END,
 r.fecha_resolucion,r.estado,r.observaciones
FROM dbo.RestauracionSolicitud r
LEFT JOIN dbo.BackupRegistro b ON b.id_backup=r.id_backup
LEFT JOIN dbo.Usuario us ON us.id_usuario=r.id_solicitante
LEFT JOIN dbo.Usuario ua ON ua.id_usuario=r.id_autorizador
WHERE NOT EXISTS (SELECT 1 FROM AuraBeautyDB_Admin.dbo.SolicitudHistorial h
 WHERE h.IdSolicitudOriginal=r.id_solicitud AND h.IdBackupOriginal=r.id_backup AND h.FechaSolicitud=r.fecha_solicitud);
GO
/* API existente: no cambiamos los nombres ni los alias que consume C#. */
CREATE OR ALTER PROCEDURE dbo.SP_Restore_ListarHistorial @IdUsuario INT
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@IdUsuario AND id_rol=1 AND deleted_at IS NULL)
  THROW 50010,'Solo un administrador activo puede consultar el historial.',1;
 SELECT h.IdSolicitudOriginal AS IdSolicitud,h.IdBackupOriginal AS IdBackup,
  h.NombreArchivo,h.FechaBackup,h.IdSolicitante,h.Solicitante,
  h.IdAutorizador,h.Autorizador,h.FechaSolicitud,h.FechaResolucion,h.Estado,h.Observaciones
 FROM AuraBeautyDB_Admin.dbo.SolicitudHistorial h
 ORDER BY h.FechaSolicitud DESC,h.FechaArchivado DESC;
END;
GO
/* Consulta de auditoría; no sustituye SP_Backup_Listar, que alimenta las acciones sobre backups todavía vigentes. */
CREATE OR ALTER PROCEDURE dbo.SP_Backup_ListarHistorialPermanente @IdUsuario INT
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.Usuario WHERE id_usuario=@IdUsuario AND id_rol=1 AND deleted_at IS NULL)
  THROW 50012,'Solo un administrador activo puede consultar los backups.',1;
 SELECT h.IdBackupOriginal AS IdBackup,h.NombreArchivo,h.RutaArchivo,
  h.FechaCreacion,h.IdUsuario,h.UsuarioNombre AS Usuario,h.Estado,h.Observaciones
 FROM AuraBeautyDB_Admin.dbo.BackupHistorial h
 ORDER BY h.FechaCreacion DESC,h.FechaArchivado DESC;
END;
GO
-- VERIFICACION (NO ejecuta RESTORE)
SELECT COUNT(*) AS BackupsConservados FROM AuraBeautyDB_Admin.dbo.BackupHistorial;
SELECT Estado,COUNT(*) AS Cantidad FROM AuraBeautyDB_Admin.dbo.SolicitudHistorial GROUP BY Estado;
GO

/* API PERSISTENTE: este procedimiento reside en Admin y SOBREVIVE RESTORE de AuraBeautyDB.
   La capa Datos lo llama después de cada operación de escritura. */
USE [AuraBeautyDB_Admin];
GO
CREATE OR ALTER PROCEDURE dbo.SP_SincronizarHistorial
AS
BEGIN
 SET NOCOUNT ON;
 SET XACT_ABORT ON;
 BEGIN TRANSACTION;

 UPDATE h SET h.NombreArchivo=b.nombre_archivo,h.IdUsuario=b.id_usuario,
  h.UsuarioNombre=LTRIM(RTRIM(CONCAT(u.nombre,' ',u.apellido))),
  h.Estado=b.estado,h.Observaciones=b.observaciones,h.FechaArchivado=SYSDATETIME()
 FROM dbo.BackupHistorial h
 JOIN AuraBeautyDB.dbo.BackupRegistro b ON h.IdBackupOriginal=b.id_backup AND h.FechaCreacion=b.fecha_creacion AND h.RutaArchivo=b.ruta_archivo
 LEFT JOIN AuraBeautyDB.dbo.Usuario u ON u.id_usuario=b.id_usuario;
 INSERT dbo.BackupHistorial
 (IdBackupOriginal,NombreArchivo,RutaArchivo,FechaCreacion,IdUsuario,UsuarioNombre,Estado,Observaciones)
 SELECT b.id_backup,b.nombre_archivo,b.ruta_archivo,b.fecha_creacion,b.id_usuario,
  LTRIM(RTRIM(CONCAT(u.nombre,' ',u.apellido))),b.estado,b.observaciones
 FROM AuraBeautyDB.dbo.BackupRegistro b
 LEFT JOIN AuraBeautyDB.dbo.Usuario u ON u.id_usuario=b.id_usuario
 WHERE NOT EXISTS(SELECT 1 FROM dbo.BackupHistorial h WITH (UPDLOCK,HOLDLOCK)
  WHERE h.IdBackupOriginal=b.id_backup AND h.FechaCreacion=b.fecha_creacion AND h.RutaArchivo=b.ruta_archivo);

 UPDATE h SET h.NombreArchivo=b.nombre_archivo,h.FechaBackup=b.fecha_creacion,
  h.Solicitante=LTRIM(RTRIM(CONCAT(us.nombre,' ',us.apellido))),
  h.IdAutorizador=r.id_autorizador,
  h.Autorizador=CASE WHEN ua.id_usuario IS NULL THEN NULL ELSE LTRIM(RTRIM(CONCAT(ua.nombre,' ',ua.apellido))) END,
  h.FechaResolucion=r.fecha_resolucion,h.Estado=r.estado,h.Observaciones=r.observaciones,h.FechaArchivado=SYSDATETIME()
 FROM dbo.SolicitudHistorial h
 JOIN AuraBeautyDB.dbo.RestauracionSolicitud r
  ON h.IdSolicitudOriginal=r.id_solicitud AND h.IdBackupOriginal=r.id_backup AND h.FechaSolicitud=r.fecha_solicitud
 LEFT JOIN AuraBeautyDB.dbo.BackupRegistro b ON b.id_backup=r.id_backup
 LEFT JOIN AuraBeautyDB.dbo.Usuario us ON us.id_usuario=r.id_solicitante
 LEFT JOIN AuraBeautyDB.dbo.Usuario ua ON ua.id_usuario=r.id_autorizador;
 INSERT dbo.SolicitudHistorial
 (IdSolicitudOriginal,IdBackupOriginal,FechaSolicitud,NombreArchivo,FechaBackup,IdSolicitante,Solicitante,IdAutorizador,Autorizador,FechaResolucion,Estado,Observaciones)
 SELECT r.id_solicitud,r.id_backup,r.fecha_solicitud,b.nombre_archivo,b.fecha_creacion,r.id_solicitante,
  LTRIM(RTRIM(CONCAT(us.nombre,' ',us.apellido))),r.id_autorizador,
  CASE WHEN ua.id_usuario IS NULL THEN NULL ELSE LTRIM(RTRIM(CONCAT(ua.nombre,' ',ua.apellido))) END,
  r.fecha_resolucion,r.estado,r.observaciones
 FROM AuraBeautyDB.dbo.RestauracionSolicitud r
 LEFT JOIN AuraBeautyDB.dbo.BackupRegistro b ON b.id_backup=r.id_backup
 LEFT JOIN AuraBeautyDB.dbo.Usuario us ON us.id_usuario=r.id_solicitante
 LEFT JOIN AuraBeautyDB.dbo.Usuario ua ON ua.id_usuario=r.id_autorizador
 WHERE NOT EXISTS(SELECT 1 FROM dbo.SolicitudHistorial h WITH (UPDLOCK,HOLDLOCK)
  WHERE h.IdSolicitudOriginal=r.id_solicitud AND h.IdBackupOriginal=r.id_backup AND h.FechaSolicitud=r.fecha_solicitud);
 COMMIT TRANSACTION;
END;
GO



--===========================================================================
-- 21 - REGISTRAR RESULTADO FINAL DE RESTAURACION (REFERENCIADO POR C#)
--===========================================================================
USE [AuraBeautyDB];
GO
CREATE OR ALTER PROCEDURE dbo.SP_Restore_Registrar
    @IdSolicitud INT,
    @Estado VARCHAR(20),
    @Observaciones NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF @IdSolicitud IS NULL OR @IdSolicitud <= 0
        THROW 51011, 'Solicitud invalida.', 1;
    IF @Estado NOT IN ('Exitosa', 'Fallida')
        THROW 51012, 'Resultado invalido: solo Exitosa o Fallida.', 1;
    IF NOT EXISTS
    (
        SELECT 1 FROM dbo.RestauracionSolicitud WITH (UPDLOCK, HOLDLOCK)
        WHERE id_solicitud = @IdSolicitud
          AND estado IN ('Autorizada', 'Exitosa', 'Fallida')
    )
        THROW 51013, 'No existe la solicitud o no fue autorizada.', 1;
    UPDATE dbo.RestauracionSolicitud
       SET estado = @Estado,
           observaciones = COALESCE(@Observaciones, observaciones),
           fecha_resolucion = COALESCE(fecha_resolucion, SYSDATETIME())
     WHERE id_solicitud = @IdSolicitud;
END;
GO



--===========================================================================
-- 22 - HISTORIAL FINAL: SOLICITUDES Y EJECUCIONES DE AUDITORIA
--===========================================================================
USE [AuraBeautyDB];
GO
CREATE OR ALTER PROCEDURE dbo.SP_Restore_ListarHistorial
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (
        SELECT 1 FROM dbo.Usuario
        WHERE id_usuario = @IdUsuario
          AND id_rol = 1 AND deleted_at IS NULL
    )
        THROW 50010, 'Solo un administrador activo puede consultar el historial.', 1;

    -- Solicitudes persistentes: incluye Pendiente, Autorizada, Rechazada, etc.
    SELECT
        h.IdSolicitudOriginal AS IdSolicitud,
        h.IdBackupOriginal AS IdBackup,
        h.NombreArchivo, h.FechaBackup,
        h.IdSolicitante, h.Solicitante,
        h.IdAutorizador, h.Autorizador,
        h.FechaSolicitud, h.FechaResolucion,
        h.Estado, h.Observaciones
    FROM AuraBeautyDB_Admin.dbo.SolicitudHistorial h

    UNION ALL

    -- Ejecuciones historicas: se presentan como eventos separados.
    SELECT
        a.IdSolicitud,
        a.IdBackupOrigen AS IdBackup,
        CAST(RIGHT(a.RutaBackupOrigen,
            CHARINDEX('\', REVERSE(a.RutaBackupOrigen) + '\') - 1)
            AS NVARCHAR(260)) AS NombreArchivo,
        CAST(NULL AS DATETIME2(7)) AS FechaBackup,
        CAST(NULL AS INT) AS IdSolicitante,
        CAST(NULL AS NVARCHAR(260)) AS Solicitante,
        a.IdAutorizador,
        CAST(CASE WHEN u.id_usuario IS NULL THEN NULL
            ELSE LTRIM(RTRIM(CONCAT(u.nombre, ' ', u.apellido))) END
            AS NVARCHAR(260)) AS Autorizador,
        a.FechaSolicitudOrigen AS FechaSolicitud,
        a.FechaFin AS FechaResolucion,
        CAST(a.Estado AS VARCHAR(20)) AS Estado,
        CAST(a.Detalle AS NVARCHAR(1000)) AS Observaciones
    FROM AuraBeautyDB_Admin.dbo.RestauracionAuditoria a
    LEFT JOIN dbo.Usuario u ON u.id_usuario = a.IdAutorizador
    ORDER BY FechaSolicitud DESC;
END;
GO



--===========================================================================
-- 23 - VALIDACION FINAL (NO MODIFICA INFORMACION)
--===========================================================================
USE [AuraBeautyDB];
GO
SELECT DB_NAME() AS BaseComercial,
    (SELECT COUNT(*) FROM dbo.Usuario) AS Usuarios,
    (SELECT COUNT(*) FROM dbo.Producto) AS Productos,
    (SELECT COUNT(*) FROM dbo.ReservaProducto) AS Reservas,
    (SELECT COUNT(*) FROM dbo.BackupRegistro) AS BackupsOperativos,
    (SELECT COUNT(*) FROM dbo.RestauracionSolicitud) AS SolicitudesOperativas;
SELECT name AS Procedimiento
FROM sys.procedures WHERE name LIKE 'SP_%' ORDER BY name;
USE [AuraBeautyDB_Admin];
GO
SELECT DB_NAME() AS BaseAdministrativa,
    (SELECT COUNT(*) FROM dbo.BackupHistorial) AS BackupsHistoricos,
    (SELECT COUNT(*) FROM dbo.SolicitudHistorial) AS SolicitudesHistoricas,
    (SELECT COUNT(*) FROM dbo.RestauracionAuditoria) AS Ejecuciones;
PRINT 'Instalacion SQL finalizada. Probar login, reservas, ventas, stock y backups desde C#.';
GO

