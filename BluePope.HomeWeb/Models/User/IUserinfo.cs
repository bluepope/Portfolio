﻿using BluePope.Lib.Crypter;
using BluePope.Lib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.User
{
    public interface IUserinfo
    {
        /// <summary>
        /// 고유 아이디
        /// </summary>
        public uint UNIQUE_ID { get; set; }
        /// <summary>
        /// 아이디
        /// </summary>
        public string USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string EMAIL { get; set; }
        public DateTime REG_DATE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string ROLES { get; set; }

    }
}