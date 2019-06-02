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
                var item = LoginModel.GetLogin(user_id, user_pw);

                if (item == null) //로그인 오류
                    return Redirect("/");

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, item.USER_ID));
                identity.AddClaim(new Claim(ClaimTypes.Name, item.USER_NAME));

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true, AllowRefresh = true });

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