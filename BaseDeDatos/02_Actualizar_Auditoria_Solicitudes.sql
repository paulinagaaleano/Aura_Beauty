
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
