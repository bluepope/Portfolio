using CoreLib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcWeb.Models.Board
{
    #region "Home.BOARD 테이블"
    /*
CREATE TABLE `BOARD` (
	`SEQ` INT(11) NOT NULL AUTO_INCREMENT,
	`BOARD_TYPE` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`TITLE` VARCHAR(200) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`CONTENTS` MEDIUMTEXT NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`DUP_KEY` BIGINT(20) NOT NULL,
	`VIEW_CNT` INT(11) NOT NULL,
	`STATUS_FLAG` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`REG_IP` VARCHAR(20) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`REG_USER` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`REG_USERNAME` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_unicode_ci',
	`REG_DATE` DATETIME NOT NULL,
	PRIMARY KEY (`SEQ`)
)
COLLATE='utf8mb4_unicode_ci'
ENGINE=InnoDB
AUTO_INCREMENT=2
;
    */
    #endregion

    public class BoardModel
    {
        /// <summary>
        /// 중복 입력 방지용
        /// </summary>
        public string DUP_KEY { get; set; }

        public string BOARD_TYPE { get; set; }
        public int SEQ { get; set; }
        public string TITLE { get; set; }
        public string CONTENTS { get; set; }
        public int VIEW_CNT { get; set; }
        public string STATUS_FLAG { get; set; }

        public string REG_IP { get; set; }
        public string REG_USER { get; set; }
        public string REG_USERNAME { get; set; }

        public static BoardModel Get(string board_type, int seq)
        {
            MySqlDapperHelper.RunExecute("UPDATE BOARD SET VIEW_CNT = VIEW_CNT + 1 WHERE BOARD_TYPE = @board_type AND SEQ = @seq", new
            {
                board_type = board_type,
                seq = seq
            });

            return MySqlDapperHelper.RunGetQuery<BoardModel>(@"
SELECT
	A.BOARD_TYPE
	,A.SEQ
	,A.TITLE
	,A.CONTENTS
	,A.DUP_KEY
	,A.VIEW_CNT
	,A.STATUS_FLAG
	,A.REG_IP
	,A.REG_USER
	,A.REG_USERNAME
	,A.REG_DATE
FROM
    BOARD A
WHERE
	A.BOARD_TYPE = @board_type
    AND A.SEQ = @seq
ORDER BY
    A.SEQ DESC
", new
            {
                board_type = board_type,
                seq = seq
            }).FirstOrDefault();
        }
        public static IList<BoardModel> GetList(string board_type, int page = 1, int page_size = 20)
        {
            return MySqlDapperHelper.RunGetQuery<BoardModel>($@"
SELECT
	A.BOARD_TYPE
	,A.SEQ
	,A.TITLE
	,A.VIEW_CNT
	,A.STATUS_FLAG
	,A.REG_IP
	,A.REG_USER
	,A.REG_USERNAME
	,A.REG_DATE
FROM
    BOARD A
WHERE
	A.BOARD_TYPE = @board_type
ORDER BY
    A.SEQ DESC
LIMIT
	{(page - 1) * page_size}, {page_size}
", new
            {
                board_type = board_type
            });
        }

        public int Insert(MySqlDapperHelper db)
        {
            return db.Execute($@"
INSERT INTO BOARD (
	BOARD_TYPE
	,TITLE
	,CONTENTS
	,DUP_KEY
	,VIEW_CNT
	,STATUS_FLAG
	,REG_IP
	,REG_USER
	,REG_USERNAME
	,REG_DATE
)
SELECT
	@BOARD_TYPE
	,@TITLE
	,@CONTENTS
	,@DUP_KEY
	,@VIEW_CNT
	,'Y'
	,@REG_IP
	,@REG_USER
	,@REG_USERNAME
	,now()
", this);
        }

        public DateTime? REG_DATE { get; set; }

        /* 필요한가? 
        public string UPDATE_IP { get; set; }
        public string UPDATE_USER { get; set; }
        public string UPDATE_USERNAME { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        */

    }
}
