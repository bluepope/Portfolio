using BluePope.HomeWeb.Models.User;
using BluePope.Lib.Crypter;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.Login
{
    public class MLoginUser : IUserInfo
    {
        public uint U_ID { get; set; }
        public string EMAIL { get; set; }
        public string USER_NAME { get; set; }
        public string ROLES { get; set; }
        public int POINT { get; set; }
        public short STATUS { get; set; }
        public DateTime REG_DATE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public DateTime? LAST_LOGIN_DATE { get; set; }
        public string REMARK1 { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public MLoginUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated == false)
                return;

            var claimsPrincipal = _httpContextAccessor.HttpContext.User as ClaimsPrincipal;

            var modelBase = (IUserInfo)this;

            foreach (var claim in claimsPrincipal.Claims)
            {
                var property = modelBase.GetType().GetProperty(claim.Type);
                if (property != null)
                {
                    property.SetValue(this, claim.Value);
                }
            }
        }
    }
}
