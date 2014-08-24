SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

CREATE SCHEMA IF NOT EXISTS `LD30` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci ;
USE `LD30` ;

-- -----------------------------------------------------
-- Table `LD30`.`message`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `LD30`.`message` ;

CREATE TABLE IF NOT EXISTS `LD30`.`message` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `message` VARCHAR(300) NOT NULL,
  `regards` VARCHAR(100) NOT NULL,
  `time` BIGINT NOT NULL,
  `views` INT NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `LD30`.`guestbook`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `LD30`.`guestbook` ;

CREATE TABLE IF NOT EXISTS `LD30`.`guestbook` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(100) NOT NULL,
  `whereis` VARCHAR(100) NOT NULL,
  `time` BIGINT NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
