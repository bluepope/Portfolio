using BluePope.HomeWeb.Models.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Newtonsoft.Json;

using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Services.Server
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        public CustomCookieAuthenticationEvents()
        {
        }

        public async override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
            context.HttpContext.Response.StatusCode = 401;
            context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
            
            var json = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new 
            {
                message = "권한이 없습니다",
                redirectUrl = context.Request.Path
            }));

            await context.HttpContext.Response.Body.WriteAsync(json, 0, json.Length);

            //return base.RedirectToAccessDenied(context);
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
                var u_id = userPrincipal.GetClaim(ClaimTypes.NameIdentifier).Value.ToUint();
                var email = userPrincipal.GetClaim(ClaimTypes.Name).Value;

                var model = await MUserinfo.GetAsync(u_id);

                if (model?.EMAIL == email && model?.STATUS >= 0) //로그인 사용자에게 문제가 없다면
                {
                    var claimType = WebExtention.GetCustomClaimTypeString(WebExtention.CustomClaimType.NextCheckTime);
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