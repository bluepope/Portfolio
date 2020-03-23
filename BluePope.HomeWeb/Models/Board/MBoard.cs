using BluePope.Lib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.Board
{
    public class MBoard
    {
        public string BOARD_TYPE { get; set; }
        public int SEQ { get; set; }
        public string TITLE { get; set; }
        public string CONTENTS { get; set; }
        public uint VIEW_CNT { get; set; }
        public short STATUS { get; set; }

        public uint REG_UID { get; set; }
        public string REG_IP { get; set; }
        public string REG_USERNAME { get; set; }
        public DateTime? REG_DATE { get; set; }

        /* 필요한가? 
        public string UPDATE_IP { get; set; }
        public string UPDATE_USER { get; set; }
        public string UPDATE_USERNAME { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        */

        public static async Task<MBoard> Get(string board_type, int seq)
        {
            return (await MySqlDapperHelper.Instance.GetQueryFromXmlAsync<MBoard>("Board.xml", "GetBoard", new
            {
                board_type = board_type,
                seq = seq
            })).FirstOrDefault();
        }

        public static async Task<MBoard> GetAsync(string board_type, int seq)
        {
            var model = await MySqlDapperHelper.Instance.GetQueryFromXmlAsync<MBoard>("Board.xml", "GetBoard", new
            {
                board_type = board_type,
                seq = seq
            });
            
            return model.FirstOrDefault();
        }

        public static async Task<int> GetCountAsync(string board_type)
        {
            return (await MySqlDapperHelper.Instance.GetQueryFromXmlAsync<int>("Board.xml", "GetBoardCount", new
            {
                board_type = board_type,
            })).FirstOrDefault();
            
        }

        public static async Task<IEnumerable<MBoard>> GetList(string board_type, int page = 1, int page_size = 20)
        {
            //데이터가 많아지면 LIMIT가 느려질수 있다고함, WHERE 로 모집합을 줄이고 LIMIT를 걸어야한다고....
            var sql = MySqlDapperHelper.GetSqlFromXml("Board.xml", "GetBoardList");
            var limit = $" LIMIT {(page - 1) * page_size}, {page_size}";

            return await MySqlDapperHelper.Instance.GetQueryAsync<MBoard>(sql + limit, new
            {
                board_type = board_type,
            });
        }

        public static async Task<IEnumerable<MBoard>> GetListAsync(string board_type, int page = 1, int page_size = 20)
        {
            //데이터가 많아지면 LIMIT가 느려질수 있다고함, WHERE 로 모집합을 줄이고 LIMIT를 걸어야한다고....
            var sql = MySqlDapperHelper.GetSqlFromXml("Board.xml", "GetBoardList");
            var limit = $" LIMIT {(page - 1) * page_size}, {page_size}";

            return await MySqlDapperHelper.Instance.GetQueryAsync<MBoard>(sql + limit, new
            {
                board_type = board_type,
            });
        }

        public async Task<int> AddViewCount()
        {
            return await MySqlDapperHelper.Instance.ExecuteFromXmlAsync("Board.xml", "UpdateViewCount", this);
        }

        public async Task<int> Insert(MySqlDapperHelper db)
        {
            return await db.ExecuteFromXmlAsync("Board.xml", "InsertBoard", this);
        }
    }
}
