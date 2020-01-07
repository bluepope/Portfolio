-- --------------------------------------------------------
-- 호스트:                          192.168.0.200
-- 서버 버전:                        10.4.10-MariaDB-1:10.4.10+maria~bionic - mariadb.org binary distribution
-- 서버 OS:                        debian-linux-gnu
-- HeidiSQL 버전:                  10.3.0.5771
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- 테이블 home.t_board 구조 내보내기
CREATE TABLE IF NOT EXISTS `t_board` (
  `SEQ` int(11) NOT NULL AUTO_INCREMENT,
  `BOARD_TYPE` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `TITLE` varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  `CONTENTS` mediumtext COLLATE utf8mb4_unicode_ci NOT NULL,
  `VIEW_CNT` int(11) unsigned NOT NULL DEFAULT 0,
  `STATUS` tinyint(4) NOT NULL DEFAULT 0,
  `REG_IP` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `REG_UID` int(10) unsigned NOT NULL DEFAULT 0,
  `REG_USERNAME` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `REG_DATE` datetime NOT NULL,
  PRIMARY KEY (`SEQ`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 내보낼 데이터가 선택되어 있지 않습니다.

-- 테이블 home.t_user 구조 내보내기
CREATE TABLE IF NOT EXISTS `t_user` (
  `U_ID` int(11) unsigned NOT NULL,
  `EMAIL` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `PASSWORD` varchar(64) COLLATE utf8mb4_unicode_ci NOT NULL,
  `USER_NAME` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `REG_DATE` datetime NOT NULL,
  `UPDATE_DATE` datetime DEFAULT NULL,
  `ROLES` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `POINT` int(11) NOT NULL DEFAULT 0,
  `STATUS` tinyint(4) NOT NULL DEFAULT 0,
  `LAST_LOGIN_DATE` datetime DEFAULT NULL,
  `REMARK1` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`U_ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 내보낼 데이터가 선택되어 있지 않습니다.

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
