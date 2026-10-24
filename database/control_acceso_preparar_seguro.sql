-- Preparacion local no destructiva para las pruebas de gestion de usuarios.
-- El destino esta fijado a control_acceso_pruebas; no cambiarlo a una base con datos reales.
-- Crea tablas faltantes, conserva filas existentes y reemplaza solo las rutinas indicadas.

CREATE DATABASE IF NOT EXISTS `control_acceso_pruebas`
  CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE `control_acceso_pruebas`;

CREATE TABLE IF NOT EXISTS `tipo_usuario` (
  `TIP_CVE_TIPOUSUARIO` int NOT NULL AUTO_INCREMENT,
  `TIP_DESCRIPCION` varchar(20) NOT NULL,
  PRIMARY KEY (`TIP_CVE_TIPOUSUARIO`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS `usuario` (
  `USU_CVE_USUARIO` int NOT NULL AUTO_INCREMENT,
  `USU_NOMBRE` varchar(20) NOT NULL,
  `USU_APELLIDO_PATERNO` varchar(20) NOT NULL,
  `USU_APELLIDO_MATERNO` varchar(20) NOT NULL,
  `USU_USUARIO` varchar(20) NOT NULL,
  `USU_CONTRASENA` varchar(255) NOT NULL,
  `USU_RUTA` varchar(200) NOT NULL,
  `TIP_CVE_TIPOUSUARIO` int NOT NULL,
  PRIMARY KEY (`USU_CVE_USUARIO`),
  KEY `TIP_CVE_TIPOUSUARIO` (`TIP_CVE_TIPOUSUARIO`),
  CONSTRAINT `usuario_ibfk_1` FOREIGN KEY (`TIP_CVE_TIPOUSUARIO`)
    REFERENCES `tipo_usuario` (`TIP_CVE_TIPOUSUARIO`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Aumentar la columna conserva sus valores y permite guardar hashes.
ALTER TABLE `usuario` MODIFY `USU_CONTRASENA` varchar(255) NOT NULL;

-- Agregar un rol genérico únicamente si la tabla todavía no tiene ninguno.
INSERT INTO `tipo_usuario` (`TIP_CVE_TIPOUSUARIO`, `TIP_DESCRIPCION`)
SELECT 1, 'Usuario'
WHERE NOT EXISTS (SELECT 1 FROM `tipo_usuario`);

CREATE OR REPLACE VIEW `vwRptUsuario` AS
SELECT U.`USU_CVE_USUARIO` AS `Clave`,
       CONCAT(U.`USU_NOMBRE`, ' ', U.`USU_APELLIDO_PATERNO`, ' ', U.`USU_APELLIDO_MATERNO`) AS `Nombre`,
       U.`USU_USUARIO` AS `Usuario`,
       U.`USU_RUTA` AS `Foto`,
       CONCAT(T.`TIP_CVE_TIPOUSUARIO`, '-', T.`TIP_DESCRIPCION`) AS `Rol`
FROM `usuario` U
JOIN `tipo_usuario` T ON U.`TIP_CVE_TIPOUSUARIO` = T.`TIP_CVE_TIPOUSUARIO`;

CREATE OR REPLACE VIEW `vwTipoUsuario` AS
SELECT T.`TIP_CVE_TIPOUSUARIO` AS `clave`,
       CONCAT(T.`TIP_CVE_TIPOUSUARIO`, '-', T.`TIP_DESCRIPCION`) AS `descripcion`
FROM `tipo_usuario` T;

-- Reemplazar procedimientos no elimina filas de las tablas.
DROP PROCEDURE IF EXISTS `spDelUsuario`;
DROP PROCEDURE IF EXISTS `spInsUsuario`;
DROP PROCEDURE IF EXISTS `spUpdUsuario`;
DROP PROCEDURE IF EXISTS `spValidarAcceso`;

DELIMITER $$

CREATE PROCEDURE `spDelUsuario`(IN clave INT)
BEGIN
    DELETE FROM `usuario` WHERE `USU_CVE_USUARIO` = clave;
    SELECT IF(ROW_COUNT() > 0, 0, 1);
END$$

CREATE PROCEDURE `spInsUsuario`(
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
        SELECT 1 FROM `usuario`
        WHERE `USU_NOMBRE` = nombre
          AND `USU_APELLIDO_PATERNO` = paterno
          AND `USU_APELLIDO_MATERNO` = materno
    ) THEN
        SELECT 1;
    ELSEIF EXISTS (SELECT 1 FROM `usuario` WHERE `USU_USUARIO` = usuario) THEN
        SELECT 2;
    ELSEIF NOT EXISTS (
        SELECT 1 FROM `tipo_usuario` WHERE `TIP_CVE_TIPOUSUARIO` = tipo
    ) THEN
        SELECT 3;
    ELSE
        INSERT INTO `usuario` (
            `USU_NOMBRE`, `USU_APELLIDO_PATERNO`, `USU_APELLIDO_MATERNO`,
            `USU_USUARIO`, `USU_CONTRASENA`, `USU_RUTA`, `TIP_CVE_TIPOUSUARIO`
        ) VALUES (nombre, paterno, materno, usuario, contrasena, ruta, tipo);
        SELECT IF(ROW_COUNT() > 0, 0, 99);
    END IF;
END$$

CREATE PROCEDURE `spUpdUsuario`(
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
    IF NOT EXISTS (SELECT 1 FROM `usuario` WHERE `USU_CVE_USUARIO` = clave) THEN
        SELECT 1;
    ELSEIF EXISTS (
        SELECT 1 FROM `usuario`
        WHERE `USU_NOMBRE` = nombre
          AND `USU_APELLIDO_PATERNO` = paterno
          AND `USU_APELLIDO_MATERNO` = materno
          AND `USU_CVE_USUARIO` <> clave
    ) THEN
        SELECT 2;
    ELSEIF EXISTS (
        SELECT 1 FROM `usuario`
        WHERE `USU_USUARIO` = usuario AND `USU_CVE_USUARIO` <> clave
    ) THEN
        SELECT 3;
    ELSEIF NOT EXISTS (
        SELECT 1 FROM `tipo_usuario` WHERE `TIP_CVE_TIPOUSUARIO` = tipo
    ) THEN
        SELECT 4;
    ELSE
        UPDATE `usuario`
        SET `USU_NOMBRE` = nombre,
            `USU_APELLIDO_PATERNO` = paterno,
            `USU_APELLIDO_MATERNO` = materno,
            `USU_USUARIO` = usuario,
            `USU_CONTRASENA` = COALESCE(NULLIF(pwd, ''), `USU_CONTRASENA`),
            `USU_RUTA` = ruta,
            `TIP_CVE_TIPOUSUARIO` = tipo
        WHERE `USU_CVE_USUARIO` = clave;
        SELECT IF(ROW_COUNT() > 0, 0, 99);
    END IF;
END$$

CREATE PROCEDURE `spValidarAcceso`(IN usuario VARCHAR(20))
BEGIN
    IF EXISTS (SELECT 1 FROM `usuario` U WHERE U.`USU_USUARIO` = usuario) THEN
        SELECT 1 AS `ban`,
               CONCAT(U.`USU_NOMBRE`, ' ', U.`USU_APELLIDO_PATERNO`, ' ', U.`USU_APELLIDO_MATERNO`) AS `usu_nombre_completo`,
               U.`USU_RUTA` AS `usu_ruta`,
               U.`USU_USUARIO` AS `usu_usuario`,
               T.`TIP_DESCRIPCION` AS `tip_descripcion`,
               U.`USU_CONTRASENA` AS `password_hash`
        FROM `usuario` U
        JOIN `tipo_usuario` T ON U.`TIP_CVE_TIPOUSUARIO` = T.`TIP_CVE_TIPOUSUARIO`
        WHERE U.`USU_USUARIO` = usuario
        LIMIT 1;
    ELSE
        SELECT 0 AS `ban`;
    END IF;
END$$

DELIMITER ;
