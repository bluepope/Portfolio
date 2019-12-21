using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using BluePope.Lib.DataBase;
using BluePope.HomeWeb.Models.Board;
using BluePope.HomeWeb.Models.Login;
using Newtonsoft.Json;

namespace BluePope.HomeWeb.Controllers
{
    [Authorize]
    public class BoardController : Controller
    {
        MUserinfo _login;

        public BoardController()//MUserinfo login
        {
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
            ViewData["total_count"] = MBoard.GetCount("GENERAL");
            
            var model = await MBoard.GetListAsync("GENERAL", p);

            return View(model);
        }

        [AllowAnonymous]
        [Route("/board/view")]
        public async Task<IActionResult> ContentsView(int seq, int p) //View()는 메소드로 쓸수 없음 route 로 처리
        {
            ViewData["page"] = p;
            var model = await MBoard.GetAsync("GENERAL", seq);

            if (!(model.REG_USER == _login?.USER_ID))
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

        public IActionResult Write()
        {
            return View();
        }

        public async Task<IActionResult> Save(MBoard input)
        {
            using (var db = new MySqlDapperHelper())
            {
                db.BeginTransaction();

                try
                {
                    //input.TITLE 및 input.CONTENTS 검증 필요
                    //특히 contents 는 xss와 같은 script태그나 img 태그 공격도 체크해야함....

                    input.REG_IP = HttpContext.Connection.RemoteIpAddress.ToString();
                    input.REG_USER = User.Identity.Name; //USER의 고유 ID를 Claim에 저장하고 가져오는 방법????
                    input.REG_USERNAME = User.Identity.Name;

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
