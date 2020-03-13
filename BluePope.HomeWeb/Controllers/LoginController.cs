using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BluePope.HomeWeb.Models.Login;
using BluePope.HomeWeb.Models.User;
using BluePope.Lib.DataBase;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BluePope.HomeWeb.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        IUserInfo _user;
        IDapperHelper _db;

        public LoginController(IUserInfo loginUser, IDapperHelper db)
        {
            _user = loginUser;
            _db = db;
        }

        [AllowAnonymous]
        public IActionResult Index() => Redirect("/login/login");

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["sitekey"] = MReCaptcha.SiteKey;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/login/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPost(string email, string password, string token, bool remember = false)
        {
            if (email.IsNull() || password.IsNull())
            {
                return Json(new { msg = "이메일 또는 비밀번호를 확인해주세요" });
            }

            if (User.Identity != null && User.Identity.IsAuthenticated == true)
                Redirect("/");

            try
            {
                /*
                var verify = await MReCaptcha.RecaptchaVerify(token);

                if (verify.Success == false || verify.Score < 0.3F)
                {
                    return Json(new { msg = string.Join(',', verify.ErrorCodes) });
                }
                */

                var login = await MUserinfo.GetLoginAsync(email, password);

                if (login == null) //로그인 오류
                    return Redirect("/");

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, login.U_ID.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, login.EMAIL));
                identity.AddClaim(new Claim(ClaimTypes.Role, login.ROLES.IsNull("")));
                identity.AddClaim(WebExtention.CustomClaimType.Email, login.EMAIL, typeof(string).ToString());
                identity.AddClaim(WebExtention.CustomClaimType.UserName, login.USER_NAME, typeof(string).ToString());
                identity.AddClaim(WebExtention.CustomClaimType.NextCheckTime, DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"), typeof(DateTime).ToString());

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = remember, //로그인 쿠키 영속성 (브라우저 종료시 유지) 여부
                    ExpiresUtc = DateTime.UtcNow.AddDays(3), //3일간 미접속시 쿠키 만료
                    AllowRefresh = true, //갱신여부
                });

                return Json(new { msg = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message });
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/login/signup")]
        public async Task<IActionResult> SignUpPost([FromForm] MUserinfo input, string password2)
        {
            if (input.PASSWORD.Trim() != password2.Trim())
                return Json(new { msg = "비밀번호와 비밀번호 확인을 일치시켜주세요" });

            try
            {
                _db.BeginTransaction();

                var r = await input.SignUpAsync(_db);

                if (r < 1)
                    throw new Exception("회원 등록 오류");

                _db.Commit();
            }
            catch (Exception ex)
            {
                _db.Rollback();

                return Json(new { msg = ex.Message });
            }

            return await LoginPost(input.EMAIL, password2, null, false);
        }
    }
}