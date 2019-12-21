using BluePope.Lib.Crypter;
using BluePope.Lib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.User
{
    public class MUserinfo : IUserinfo
    {
        /// <summary>
        /// 고유 아이디
        /// </summary>
        public uint UNIQUE_ID { get; set; }
        /// <summary>
        /// 아이디
        /// </summary>
        public string USER_ID { get; set; }
        public string PASSWORD { get; set; }
        public string USER_NAME { get; set; }
        public DateTime REG_DATE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string ROLES { get; set; }
        public string EMAIL { get; set; }

        public async static Task<MUserinfo> GetLogin(string user_id, string pw)
        {
            var model = (await MySqlDapperHelper.Instance.GetQueryFromXmlAsync<MUserinfo>("Sql/User.xml", "GetUserInfoAll", new { user_id = user_id.ToLower() })).FirstOrDefault();

            if (model == null)
            {
                throw new Exception("id 없음");
            }
            else
            {
                var pwhash = HMacSha256.GetHMac(pw.Trim(), model.UNIQUE_ID.ToString());

                if (model.PASSWORD != pwhash)
                {
                    throw new Exception("패스워드가 틀렸습니다");
                }

                return model;
            }
        }
    }
}
