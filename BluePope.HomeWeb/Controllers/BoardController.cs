using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using BluePope.Lib.DataBase;
using BluePope.HomeWeb.Models.Board;
using Newtonsoft.Json;
using BluePope.HomeWeb.Models.User;

namespace BluePope.HomeWeb.Controllers
{
    [Authorize]
    public class BoardController : Controller
    {
        readonly IUserInfo _login;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly IDapperHelper _db;

        public BoardController(IDapperHelper db, IUserInfo login, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _login = login;
            _httpContextAccessor = httpContextAccessor;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return Redirect("/board/list");
        }

        [AllowAnonymous]
        public async Task<IActionResult> List(int p)
        {
            if (p < 1)
                p = 1;

            ViewData["page"] = p;
            ViewData["total_count"] = await MBoard.GetCountAsync("GENERAL");
            
            var model = await MBoard.GetListAsync("GENERAL", p);

            return View(model);
        }

        [AllowAnonymous]
        [Route("/board/view")]
        public async Task<IActionResult> ContentsView(int seq, int p) //View()는 메소드로 쓸수 없음 route 로 처리
        {
            ViewData["page"] = p;
            var model = await MBoard.GetAsync("GENERAL", seq);

            if (!(model.REG_UID == _login?.U_ID))
            {
                //HttpContext.Connection.Id
                //동일 세션의 중복 ViewCount를 막기위해 쿠키를 써봄
                var cookieId = "BOARD_GENERAL";

                if (Request.Cookies.ContainsKey(cookieId) == false)
                {
                    Response.Cookies.Append(cookieId, $"[{seq}]", new Microsoft.AspNetCore.Http.CookieOptions() { HttpOnly = true });
                    _ = model.AddViewCount();
                }
                else
                {
                    var list = JsonConvert.DeserializeObject<List<int>>(Request.Cookies[cookieId]);
                    if (list.Any(x => x == seq) == false)
                    {
                        _ = model.AddViewCount();
                        
                        list.Add(seq);
                        Response.Cookies.Append(cookieId, JsonConvert.SerializeObject(list), new Microsoft.AspNetCore.Http.CookieOptions() { HttpOnly = true });
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Write()
        {
            return View();
        }

        [HttpPost]
        [Route("/board/write")]
        public async Task<IActionResult> WritePost(MBoard input)
        {
            using (var db = new MySqlDapperHelper())
            {
                db.BeginTransaction();

                try
                {
                    input.CONTENTS = new Ganss.XSS.HtmlSanitizer().Sanitize(input.CONTENTS);

                    input.REG_UID = _login.U_ID; //USER의 고유 ID를 Claim에 저장하고 가져오는 방법????
                    input.REG_USERNAME = _login.USER_NAME;
                    input.REG_IP = HttpContext.Connection.GetRemoteIpAddress();

                    await input.Insert(db);

                    db.Commit();
                }
                catch(Exception ex)
                {
                    db.Rollback();

                    return Json(new { msg = ex.Message });
                }

                return Json(new { msg = "OK" });
            }
        }
    }
}
