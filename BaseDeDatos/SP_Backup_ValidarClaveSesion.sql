
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
