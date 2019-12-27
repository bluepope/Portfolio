using BluePope.HomeWeb.Models.User;
using BluePope.Lib.Crypter;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.Login
{
    public class MLoginUser : IUserinfo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 고유 아이디
        /// </summary>
        public uint UNIQUE_ID { get; set; }
        /// <summary>
        /// 아이디
        /// </summary>
        public string USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public DateTime REG_DATE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string ROLES { get; set; }
        public string EMAIL { get; set; }

        public MLoginUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated == false)
                return;

            this.USER_NAME = _httpContextAccessor.HttpContext.User.Identity.Name;
        }
    }
}
