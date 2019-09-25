using CoreLib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcWeb.Models.Board
{
    public class BoardPgModel
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
        public DateTime? REG_DATE { get; set; }

        /* 필요한가? 
        public string UPDATE_IP { get; set; }
        public string UPDATE_USER { get; set; }
        public string UPDATE_USERNAME { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        */

        public static BoardModel Get(string board_type, int seq)
        {
            return PgSqlDapperHelper.RunGetQueryFromXml<BoardModel>("Sql/Board_pg.xml", "GetBoard", new
            {
                board_type = board_type,
                seq = seq
            }).FirstOrDefault();
        }

        public static async Task<BoardModel> GetAsync(string board_type, int seq)
        {
            var model = await PgSqlDapperHelper.RunGetQueryFromXmlAsync<BoardModel>("Sql/Board_pg.xml", "GetBoard", new
            {
                board_type = board_type,
                seq = seq
            });
            
            return model.FirstOrDefault();
        }

        public static int GetCount(string board_type)
        {
            return PgSqlDapperHelper.RunGetQueryFromXml<int>("Sql/Board_pg.xml", "GetBoardCount", new
            {
                board_type = board_type,
            }).FirstOrDefault();
            
        }

        public static IEnumerable<BoardModel> GetList(string board_type, int page = 1, int page_size = 20)
        {
            //데이터가 많아지면 LIMIT가 느려질수 있다고함, WHERE 로 모집합을 줄이고 LIMIT를 걸어야한다고....
            var sql = PgSqlDapperHelper.GetSqlFromXml("Sql/Board_pg.xml", "GetBoardList");
            var limit = $" LIMIT {(page - 1) * page_size}, {page_size}";

            return PgSqlDapperHelper.RunGetQuery<BoardModel>(sql + limit, new
            {
                board_type = board_type,
            });
        }

        public static async Task<IEnumerable<BoardModel>> GetListAsync(string board_type, int page = 1, int page_size = 20)
        {
            //데이터가 많아지면 LIMIT가 느려질수 있다고함, WHERE 로 모집합을 줄이고 LIMIT를 걸어야한다고....
            var sql = PgSqlDapperHelper.GetSqlFromXml("Sql/Board_pg.xml", "GetBoardList");
            var limit = $" LIMIT {(page - 1) * page_size}, {page_size}";

            return await PgSqlDapperHelper.RunGetQueryAsync<BoardModel>(sql, new
            {
                board_type = board_type,
            });
        }

        public int AddViewCount()
        {
            return PgSqlDapperHelper.RunExecuteFromXml("Sql/Board_pg.xml", "UpdateViewCount", this);
        }

        public int Insert(PgSqlDapperHelper db)
        {
            return db.ExecuteFromXml("Sql/Board_pg.xml", "InsertBoard", this);
        }
    }
}
