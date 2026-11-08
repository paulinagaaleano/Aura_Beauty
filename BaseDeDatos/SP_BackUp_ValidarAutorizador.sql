
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
