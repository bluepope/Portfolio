using Microsoft.AspNetCore.Mvc;
using CoreMvcWeb.Models.Board;
using Microsoft.AspNetCore.Authorization;
using CoreLib.DataBase;
using System;

namespace CoreMvcWeb.Controllers
{
    [Authorize]
    public class BoardController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/board/list");
        }

        public IActionResult List(int p)
        {
            if (p < 1)
                p = 1;

            ViewData["page"] = p;
            ViewData["total_count"] = BoardModel.GetCount("GENERAL");
            
            var model = BoardModel.GetList("GENERAL", p);

            return View(model);
        }

        [Route("/board/view")]
        public IActionResult ContentsView(int seq, int p) //View()는 메소드로 쓸수 없음 route 로 처리
        {
            ViewData["page"] = p;
            return View(BoardModel.Get("GENERAL", seq));
        }

        public IActionResult Write()
        {
            return View();
        }

        public IActionResult Save(BoardModel input)
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

                    input.Insert(db);

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
