using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Services.Server
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        public CustomCookieAuthenticationEvents()
        {
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            //재검증
            var refreshMin = 15;
            var userPrincipal = context.Principal;

            var nextCheckDate = userPrincipal.GetClaimValue(WebExtention.CustomClaimType.NextCheckTime);

            //Console.WriteLine(nextCheckDate);

            if (string.IsNullOrWhiteSpace(nextCheckDate) || DateTime.ParseExact(nextCheckDate, "yyyyMMddHHmmss", CultureInfo.CurrentCulture) < DateTime.Now)
            {
                var claimType = WebExtention.GetCustomClaimTypeString(WebExtention.CustomClaimType.NextCheckTime);

                if (1==1) //로그인 사용자에게 문제가 없다면
                {
                    var claimValue = DateTime.Now.AddMinutes(refreshMin).ToString("yyyyMMddHHmmss");
                    var checkClaim = userPrincipal.GetClaim(claimType);
                    var identity = (userPrincipal.Identity as ClaimsIdentity);
                    identity.RemoveClaim(checkClaim);
                    identity.AddClaim(new Claim(claimType, claimValue));
                    await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
                }
                else //사용자가 검증 로직을 통과하지 못했다면?
                {
                    //강제 로그아웃 처리
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
        }
    }
}