using BluePope.HomeWeb.Models.Login;
using BluePope.Lib.Crypter;
using BluePope.Lib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.User
{
    public class MUserinfo : IUserInfo
    {
        /// <summary>
        /// 고유 아이디
        /// </summary>
        public uint U_ID { get; set; }
        /// <summary>
        /// 아이디
        /// </summary>
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }

        public string USER_NAME { get; set; }
        public string ROLES { get; set; }
        public int POINT { get; set; }
        public short STATUS { get; set; }

        public DateTime REG_DATE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public DateTime? LAST_LOGIN_DATE { get; set; }
        public string REMARK1 { get; set; }

        static string EmailReplace(string email)
        {
            if (email.IsNull())
                return null;

            var emailArr = email.Split('@');
            emailArr[0] = emailArr[0].Replace(".", "");

            return string.Join('@', emailArr);
        }

        public async static Task<MUserinfo> GetAsync(uint u_id)
        {
            return (await MySqlDapperHelper.Instance.GetQueryFromXmlAsync<MUserinfo>("User.xml", "GetUserInfo", new { u_id = u_id })).FirstOrDefault();
        }

        public async static Task<MUserinfo> GetLoginAsync(string email, string pw)
        {
            email = EmailReplace(email);

            var model = (await MySqlDapperHelper.Instance.GetQueryFromXmlAsync<MUserinfo>("User.xml", "GetLoginInfo", new { email = email.ToLower() })).FirstOrDefault();

            if (model == null)
            {
                throw new Exception("id 없음");
            }
            else
            {
                var pwhash = HMacSha256.GetHMac(pw.Trim(), model.U_ID.ToString());

                if (model.PASSWORD != pwhash)
                {
                    throw new Exception("패스워드가 틀렸습니다");
                }

                return model;
            }
        }

        internal async Task<int> SignUpAsync(IDapperHelper db)
        {
            EMAIL = EmailReplace(EMAIL);

            if (EMAIL.IsNull()) throw new Exception("이메일을 정확히 입력해주세요");
            if (PASSWORD.IsNull()) throw new Exception("비밀번호를 정확히 입력해주세요");
            if (USER_NAME.IsNull()) throw new Exception("닉네임을 정확히 입력해주세요");

            EMAIL = EMAIL.Trim();
            PASSWORD = PASSWORD.Trim();
            USER_NAME = USER_NAME.Trim();

            if (PASSWORD.Length < 8) throw new Exception("비밀번호를 8자이상 입력해주세요");

            //이메일 중복 확인
            if ((await db.GetQueryFromXmlAsync<int>("User.xml", "GetCheckEmail", this)).FirstOrDefault() > 0)
                throw new Exception("이미 사용중인 이메일 입니다");

            await CheckUserNameAsync(db);

            //U_ID 가져옴
            this.U_ID = (await db.GetQueryFromXmlAsync<uint>("User.xml", "GetSignUp_Uid", new { })).FirstOrDefault();

            //PASSWORD 암호화
            PASSWORD = HMacSha256.GetHMac(PASSWORD, U_ID.ToString());

            //Insert
            return await db.ExecuteFromXmlAsync("User.xml", "InsertSignUp", this);
        }

        public async Task CheckUserNameAsync(IDapperHelper db)
        {
            if (USER_NAME.IndexOfAny(new char[] { '　' }) > -1) throw new Exception("사용할수 없는 문자가 존재합니다");
            if (USER_NAME.In("관리자", "운영자")) throw new Exception("사용할수 없는 닉네임이 존재합니다");

            //사용중인 UserName 확인
            if ((await db.GetQueryFromXmlAsync<int>("User.xml", "GetCheckUserName", this)).FirstOrDefault() > 0)
                throw new Exception("이미 사용중인 닉네임 입니다");
        }
    }
}
