
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



