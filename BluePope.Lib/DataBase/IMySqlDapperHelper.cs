using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluePope.Lib.DataBase
{
    public interface IMySqlDapperHelper
    {
        /// <summary>
        /// 트랜잭션 시작
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 트랜잭션 롤백
        /// </summary>
        void Rollback();

        /// <summary>
        /// 트랜잭션 커밋
        /// </summary>
        void Commit();

        Task<IEnumerable<T>> GetQueryAsync<T>(string sql, object param);

        Task<IEnumerable<T>> GetQueryFromXmlAsync<T>(string xmlPath, string sqlId, object param);

        Task<int> ExecuteAsync(string sql, object param);
        Task<int> ExecuteFromXmlAsync(string xmlPath, string sqlId, object param);
    }
}
