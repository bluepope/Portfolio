using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.DataBase
{
    public class SqlDapperHelper : IDisposable
    {
        public static string ConnectionString { get; set; }
        static Dictionary<string, SqlManager> _sqlManager = new Dictionary<string, SqlManager>();
        SqlConnection _conn = null;
        SqlTransaction _trans = null;

        /// <summary>
        /// Transaction 을 위한 생성자
        /// </summary>
        public SqlDapperHelper()
        {
            _conn = new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// 트랜잭션 시작
        /// </summary>
        public void BeginTransaction()
        {
            if (_conn.State == System.Data.ConnectionState.Closed)
                _conn.Open();

            _trans = _conn.BeginTransaction();
        }

        /// <summary>
        /// 트랜잭션 롤백
        /// </summary>
        public void Rollback()
        {
            _trans.Rollback();
            _trans = null;

            if (_conn.State != System.Data.ConnectionState.Closed)
                _conn.Close();
        }

        /// <summary>
        /// 트랜잭션 커밋
        /// </summary>
        public void Commit()
        {
            _trans.Commit();
            _trans = null;

            if (_conn.State != System.Data.ConnectionState.Closed)
                _conn.Close();
        }

        /// <summary>
        /// using 을 위한 Dispose
        /// </summary>
        public void Dispose()
        {
            if (_trans != null)
            {
                _trans.Rollback();

                _trans.Dispose();
                _trans = null;
            }

            if (_conn.State != System.Data.ConnectionState.Closed)
                _conn.Close();

            _conn.Dispose();
            _conn = null;
        }

        public static string GetSqlFromXml(string xmlPath, string sqlId)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return _sqlManager[xmlPath].GetSql(sqlId);
        }
        #region 1회성 쿼리를 위한 static method 영역
        public static IEnumerable<T> RunGetQuery<T>(string sql, object param)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    return Dapper.SqlMapper.Query<T>(conn, sql, param, null, true, null, null);
                }
                catch (Exception ex)
                {
                    ErrorSqlLog(sql, JsonConvert.SerializeObject(param), ex.Message);
                    throw ex;
                }
            }
        }

        public static IEnumerable<T> RunGetQueryFromXml<T>(string xmlPath, string sqlId, object param)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return RunGetQuery<T>(_sqlManager[xmlPath].GetSql(sqlId), param);
        }

        public static int RunExecute(string sql, object param)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    return Dapper.SqlMapper.Execute(conn, sql, param, null, null, null);
                }
                catch (Exception ex)
                {
                    ErrorSqlLog(sql, JsonConvert.SerializeObject(param), ex.Message);
                    throw ex;
                }
            }
        }

        public static int RunExecuteFromXml(string xmlPath, string sqlId, object param)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return RunExecute(_sqlManager[xmlPath].GetSql(sqlId), param);
        }

        
        public static async Task<IEnumerable<T>> RunGetQueryAsync<T>(string sql, object param)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    return (await Dapper.SqlMapper.QueryAsync<T>(conn, sql, param));
                }
                catch (Exception ex)
                {
                    ErrorSqlLog(sql, JsonConvert.SerializeObject(param), ex.Message);
                    throw ex;
                }
            }
        }

        public static async Task<IEnumerable<T>> RunGetQueryFromXmlAsync<T>(string xmlPath, string sqlId, object param)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return await RunGetQueryAsync<T>(_sqlManager[xmlPath].GetSql(sqlId), param);
        }

        public static async Task<int> RunExecuteAsync(string sql, object param)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    return await Dapper.SqlMapper.ExecuteAsync(conn, sql, param);
                }
                catch (Exception ex)
                {
                    ErrorSqlLog(sql, JsonConvert.SerializeObject(param), ex.Message);
                    throw ex;
                }
            }
        }

        public static async Task<int> RunExecuteFromXmlAsync(string xmlPath, string sqlId, object param)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return await RunExecuteAsync(_sqlManager[xmlPath].GetSql(sqlId), param);
        }
        #endregion

        #region instance 용 영역
        public IEnumerable<T> GetQuery<T>(string sql, object param)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    return Dapper.SqlMapper.Query<T>(conn, sql, param, null, true, null, null);
                }
                catch (Exception ex)
                {
                    ErrorSqlLog(sql, JsonConvert.SerializeObject(param), ex.Message);
                    throw ex;
                }
            }
        }

        public IEnumerable<T> GetQueryFromXml<T>(string xmlPath, string sqlId, object param)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return this.GetQuery<T>(_sqlManager[xmlPath].GetSql(sqlId), param);
        }

        public int Execute(string sql, object param)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    return Dapper.SqlMapper.Execute(conn, sql, param, null, null, null);
                }
                catch (Exception ex)
                {
                    ErrorSqlLog(sql, JsonConvert.SerializeObject(param), ex.Message);
                    throw ex;
                }
            }
        }

        public int ExecuteFromXml(string xmlPath, string sqlId, object param)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return this.Execute(_sqlManager[xmlPath].GetSql(sqlId), param);
        }

        public async Task<IEnumerable<T>> GetQueryAsync<T>(string sql, object param)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    return (await Dapper.SqlMapper.QueryAsync<T>(conn, sql, param));
                }
                catch (Exception ex)
                {
                    ErrorSqlLog(sql, JsonConvert.SerializeObject(param), ex.Message);
                    throw ex;
                }
            }
        }

        public async Task<IEnumerable<T>> GetQueryFromXmlAsync<T>(string xmlPath, string sqlId, object param)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return await this.GetQueryAsync<T>(_sqlManager[xmlPath].GetSql(sqlId), param);
        }

        public async Task<int> ExecuteAsync(string sql, object param)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    return await Dapper.SqlMapper.ExecuteAsync(conn, sql, param);
                }
                catch (Exception ex)
                {
                    ErrorSqlLog(sql, JsonConvert.SerializeObject(param), ex.Message);
                    throw ex;
                }
            }
        }

        public async Task<int> ExecuteFromXmlAsync(string xmlPath, string sqlId, object param)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return await this.ExecuteAsync(_sqlManager[xmlPath].GetSql(sqlId), param);
        }
        #endregion

        private static void ErrorSqlLog(string error_sql, string param_json, string error_msg)
        {
            string log = $@"
Error Msg-------------------------
{error_msg}
----------------------------------
SQL------------------------------- 
{error_sql}
----------------------------------
Params----------------------------
{param_json}
----------------------------------
";

            //Log 저장 어디에?
        }
    }
}
