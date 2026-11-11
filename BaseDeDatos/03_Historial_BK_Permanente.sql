
USE [AuraBeautyDB_Admin];
GO

-- ============================================================
-- AURA BEAUTY
-- HISTORIAL PERMANENTE DE BACKUPS Y RESTAURACIONES
--
-- Estas tablas se guardan en una base administrativa
-- independiente de AuraBeautyDB.
--
-- OBJETIVO:
-- Evitar que el historial desaparezca cuando se restaura
-- una versión anterior de la base comercial.
-- ============================================================

-- ============================================================
-- 1. HISTORIAL DE BACKUPS
-- ============================================================

IF OBJECT_ID(N'dbo.BackupHistorial', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BackupHistorial
    (
        IdRegistro UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT DF_BackupHist_Id
            DEFAULT NEWID()
            PRIMARY KEY,

        IdBackupOriginal INT NOT NULL,

        NombreArchivo NVARCHAR(260) NOT NULL,

        RutaArchivo NVARCHAR(500) NOT NULL,

        FechaCreacion DATETIME2(7) NOT NULL,

        IdUsuario INT NOT NULL,

        UsuarioNombre NVARCHAR(260) NULL,

        Estado VARCHAR(20) NOT NULL,

        Observaciones NVARCHAR(1000) NULL,

        FechaArchivado DATETIME2(7) NOT NULL
            DEFAULT SYSDATETIME()
    );
END;
GO

-- ============================================================
-- 2. HISTORIAL DE SOLICITUDES DE RESTAURACION
--
-- Conserva solicitudes pendientes, autorizadas,
-- rechazadas y cualquier otro estado registrado.
-- ============================================================

IF OBJECT_ID(N'dbo.SolicitudHistorial', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SolicitudHistorial
    (
        IdRegistro UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT DF_SolHist_Id
            DEFAULT NEWID()
            PRIMARY KEY,

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

        FechaArchivado DATETIME2(7) NOT NULL
            DEFAULT SYSDATETIME()
    );
END;
GO

-- ============================================================
-- 3. INDICES
--
-- Los identificadores de las tablas comerciales pueden
-- reutilizarse después de una restauración.
--
-- Por eso, no identificamos una solicitud histórica
-- solamente por su IdSolicitudOriginal.
-- ============================================================

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.SolicitudHistorial')
      AND name = N'UX_SolHist_Origen'
)
BEGIN
    CREATE UNIQUE INDEX UX_SolHist_Origen
    ON dbo.SolicitudHistorial
    (
        IdSolicitudOriginal,
        IdBackupOriginal,
        FechaSolicitud
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.BackupHistorial')
      AND name = N'IX_BackupHist_Origen'
)
BEGIN
    CREATE INDEX IX_BackupHist_Origen
    ON dbo.BackupHistorial
    (
        IdBackupOriginal,
        FechaCreacion
    );
END;
GO
