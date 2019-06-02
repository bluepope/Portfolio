using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcWeb.Models.Login
{
    public class LoginModel
    {
        public int USER_SEQ { get; set; }
        public string USER_ID { get; set; }
        public string USER_NAME { get; set; }

        public static LoginModel GetLogin(string id, string pw)
        {
            if (id == pw)
            {
                return new LoginModel() { USER_SEQ = 1, USER_ID = id, USER_NAME = "테스트 유저" };
            }

            throw new Exception("로그인 오류!");
        }
    }
}
