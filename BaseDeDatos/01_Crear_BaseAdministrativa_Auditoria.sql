
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
