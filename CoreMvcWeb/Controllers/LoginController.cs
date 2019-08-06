using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreMvcWeb.Models.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoreMvcWeb.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return Redirect("/login/login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("/login/login")]
        public async Task<IActionResult> GetLogin(string user_id, string user_pw)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated == true)
                Redirect("/");

            try
            {
                var login = UserinfoModel.GetLogin(user_id, user_pw);

                if (login == null) //로그인 오류
                    return Redirect("/");

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, login.USER_ID));
                identity.AddClaim(new Claim(ClaimTypes.Name, login.USER_NAME));
                identity.AddClaim(new Claim(ClaimTypes.Role, "ADMIN"));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties {
                    IsPersistent = false, //로그인 쿠키 영속성 (브라우저 종료시 유지) 여부
                    ExpiresUtc = DateTime.UtcNow.AddDays(7), //7일간 미접속시 쿠키 만료
                    AllowRefresh = true, //갱신여부
                });

                return Json(new { msg = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message });
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }
    }
}