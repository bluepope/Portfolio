using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLib.DataBase
{
    public class MySqlDapperHelper : IDisposable
    {
        static Dictionary<string, SqlManager> _sqlManager = new Dictionary<string, SqlManager>();
        MySqlConnection _conn = null;
        MySqlTransaction _trans = null;

        /// <summary>
        /// DB 계정
        /// </summary>
        public enum DataSourceName
        {
            Main
        }

        /// <summary>
        /// Transaction 을 위한 생성자
        /// </summary>
        public MySqlDapperHelper()
        {
            _conn = new MySqlConnection(ConnectionStringModel.Instance.ConnectionString);
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

        #region 1회성 쿼리를 위한 static method 영역
        public static IList<T> RunGetQuery<T>(string sql, object param)
        {
            using (var conn = new MySqlConnection(ConnectionStringModel.Instance.ConnectionString))
            {
                try
                {
                    return Dapper.SqlMapper.Query<T>(conn, sql, param, null, true, null, null).ToList<T>();
                }
                catch (Exception ex)
                {
                    ErrorSqlLog(sql, JsonConvert.SerializeObject(param), ex.Message);
                    throw ex;
                }
            }
        }

        public static IList<T> RunGetQueryXml<T>(string xmlPath, string sqlId, object param)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return RunGetQuery<T>(_sqlManager[xmlPath].GetSql(sqlId), param);
        }

        public static int RunExecute(string sql, object param)
        {
            using (var conn = new MySqlConnection(ConnectionStringModel.Instance.ConnectionString))
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

        public static int RunExecuteXml(string xmlPath, string sqlId, object param)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return RunExecute(_sqlManager[xmlPath].GetSql(sqlId), param);
        }
        #endregion

        #region instance 용 영역
        public IList<T> GetQuery<T>(string sql, object param)
        {
            using (var conn = new MySqlConnection(ConnectionStringModel.Instance.ConnectionString))
            {
                try
                {
                    return Dapper.SqlMapper.Query<T>(conn, sql, param, null, true, null, null).ToList<T>();
                }
                catch (Exception ex)
                {
                    ErrorSqlLog(sql, JsonConvert.SerializeObject(param), ex.Message);
                    throw ex;
                }
            }
        }

        public IList<T> GetQueryXml<T>(string xmlPath, string sqlId, object param)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return this.GetQuery<T>(_sqlManager[xmlPath].GetSql(sqlId), param);
        }

        public int Execute(string sql, object param)
        {
            using (var conn = new MySqlConnection(ConnectionStringModel.Instance.ConnectionString))
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

        public int ExecuteXml(string xmlPath, string sqlId, object param)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return this.Execute(_sqlManager[xmlPath].GetSql(sqlId), param);
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
