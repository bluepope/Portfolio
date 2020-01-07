using BluePope.Lib.Crypter;
using BluePope.Lib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.User
{
    public interface IUserInfo
    {
        /// <summary>
        /// 고유 아이디
        /// </summary>
        uint U_ID { get; set; }
        string EMAIL { get; set; }
        string USER_NAME { get; set; }
        string ROLES { get; set; }
        int POINT { get; set; }
        short STATUS { get; set; }

        DateTime REG_DATE { get; set; }
        DateTime? UPDATE_DATE { get; set; }
        DateTime? LAST_LOGIN_DATE { get; set; }
        string REMARK1 { get; set; }

    }
}
