
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
