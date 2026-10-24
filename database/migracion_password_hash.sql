-- Ejecutar una sola vez sobre una base existente para conservar sus usuarios.
-- Las contraseñas antiguas se convertirán a hash tras el primer inicio de sesión correcto.

ALTER TABLE usuario
    MODIFY USU_CONTRASENA VARCHAR(255) NOT NULL;

DROP PROCEDURE IF EXISTS spInsUsuario;
DROP PROCEDURE IF EXISTS spUpdUsuario;
DROP PROCEDURE IF EXISTS spValidarAcceso;

DELIMITER $$

CREATE PROCEDURE spInsUsuario(
    IN nombre VARCHAR(20),
    IN paterno VARCHAR(20),
    IN materno VARCHAR(20),
    IN usuario VARCHAR(20),
    IN contrasena VARCHAR(255),
    IN ruta VARCHAR(200),
    IN tipo INT
)
BEGIN
    IF EXISTS (
        SELECT 1 FROM usuario
        WHERE USU_NOMBRE = nombre
          AND USU_APELLIDO_PATERNO = paterno
          AND USU_APELLIDO_MATERNO = materno
    ) THEN
        SELECT 1;
    ELSEIF EXISTS (SELECT 1 FROM usuario WHERE USU_USUARIO = usuario) THEN
        SELECT 2;
    ELSEIF NOT EXISTS (
        SELECT 1 FROM tipo_usuario WHERE TIP_CVE_TIPOUSUARIO = tipo
    ) THEN
        SELECT 3;
    ELSE
        INSERT INTO usuario (
            USU_NOMBRE, USU_APELLIDO_PATERNO, USU_APELLIDO_MATERNO,
            USU_USUARIO, USU_CONTRASENA, USU_RUTA, TIP_CVE_TIPOUSUARIO
        ) VALUES (
            nombre, paterno, materno, usuario, contrasena, ruta, tipo
        );
        SELECT IF(ROW_COUNT() > 0, 0, 99);
    END IF;
END$$

CREATE PROCEDURE spUpdUsuario(
    IN clave INT,
    IN nombre VARCHAR(20),
    IN paterno VARCHAR(20),
    IN materno VARCHAR(20),
    IN usuario VARCHAR(20),
    IN pwd VARCHAR(255),
    IN ruta VARCHAR(200),
    IN tipo INT
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM usuario WHERE USU_CVE_USUARIO = clave) THEN
        SELECT 1;
    ELSEIF EXISTS (
        SELECT 1 FROM usuario
        WHERE USU_NOMBRE = nombre
          AND USU_APELLIDO_PATERNO = paterno
          AND USU_APELLIDO_MATERNO = materno
          AND USU_CVE_USUARIO <> clave
    ) THEN
        SELECT 2;
    ELSEIF EXISTS (
        SELECT 1 FROM usuario
        WHERE USU_USUARIO = usuario AND USU_CVE_USUARIO <> clave
    ) THEN
        SELECT 3;
    ELSEIF NOT EXISTS (
        SELECT 1 FROM tipo_usuario WHERE TIP_CVE_TIPOUSUARIO = tipo
    ) THEN
        SELECT 4;
    ELSE
        UPDATE usuario
        SET USU_NOMBRE = nombre,
            USU_APELLIDO_PATERNO = paterno,
            USU_APELLIDO_MATERNO = materno,
            USU_USUARIO = usuario,
            USU_CONTRASENA = COALESCE(NULLIF(pwd, ''), USU_CONTRASENA),
            USU_RUTA = ruta,
            TIP_CVE_TIPOUSUARIO = tipo
        WHERE USU_CVE_USUARIO = clave;
        SELECT IF(ROW_COUNT() > 0, 0, 99);
    END IF;
END$$

CREATE PROCEDURE spValidarAcceso(IN usuario VARCHAR(20))
BEGIN
    IF EXISTS (SELECT 1 FROM usuario u WHERE u.USU_USUARIO = usuario) THEN
        SELECT 1 AS ban,
               CONCAT(u.USU_NOMBRE, ' ', u.USU_APELLIDO_PATERNO, ' ', u.USU_APELLIDO_MATERNO) AS usu_nombre_completo,
               u.USU_RUTA AS usu_ruta,
               u.USU_USUARIO AS usu_usuario,
               t.TIP_DESCRIPCION AS tip_descripcion,
               u.USU_CONTRASENA AS password_hash
        FROM usuario u
        JOIN tipo_usuario t ON u.TIP_CVE_TIPOUSUARIO = t.TIP_CVE_TIPOUSUARIO
        WHERE u.USU_USUARIO = usuario
        LIMIT 1;
    ELSE
        SELECT 0 AS ban;
    END IF;
END$$

DELIMITER ;
