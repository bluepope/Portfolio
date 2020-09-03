using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluePope.Lib.DataBase
{
    public class MySqlDapperHelper : IDapperHelper, IDisposable
    {
        public static string ConnectionString { get; set; }
        static Dictionary<string, SqlManager> _sqlManager = new Dictionary<string, SqlManager>();
        MySqlConnection _conn = null;
        MySqlTransaction _trans = null;

        static MySqlDapperHelper _instance = null;
        public static MySqlDapperHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MySqlDapperHelper();

                return _instance;
            }
        }

        /// <summary>
        /// Transaction 을 위한 생성자
        /// </summary>
        public MySqlDapperHelper()
        {
            _conn = new MySqlConnection(ConnectionString);
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

        public static string GetSqlFromXml(string xmlPath, string sqlId)
        {
            if (_sqlManager.ContainsKey(xmlPath) == false)
                _sqlManager[xmlPath] = new SqlManager(xmlPath);

            return _sqlManager[xmlPath].GetSql(sqlId);
        }

        #region instance 용 영역
        public async Task<IEnumerable<T>> GetQueryAsync<T>(string sql, object param)
        {
            try
            {
                return await Dapper.SqlMapper.QueryAsync<T>(_conn, sql, param, _trans);
            }
            catch (Exception ex)
            {
                ErrorSqlLog(sql, JsonConvert.SerializeObject(param), ex.Message);
                throw ex;
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
            try
            {
                return await Dapper.SqlMapper.ExecuteAsync(_conn, sql, param, _trans);
            }
            catch (Exception ex)
            {
                ErrorSqlLog(sql, JsonConvert.SerializeObject(param), ex.Message);
                throw ex;
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

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                    _trans?.Rollback();
                    _trans?.Dispose();

                    _conn?.Dispose();
                    _conn = null;
                    _trans = null;
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~MySqlDapperHelper()
        // {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
