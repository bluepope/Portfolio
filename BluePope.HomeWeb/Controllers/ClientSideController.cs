using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using BluePope.HomeWeb.Models.ClientSide;
using Newtonsoft.Json;

namespace BluePope.HomeWeb.Controllers
{
    public class ClientSideController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClientSideController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return Redirect("/clientside/vue");
        }

        public IActionResult Vue()
        {
            return View();
        }

        public IActionResult AjaxTest()
        {
            return View();
        }

        public IActionResult AxiosTest()
        {
            return View();
        }

        [RequestSizeLimit(52428800)] //50MB
        //[DisableRequestSizeLimit] --unlimited
        //axios 는 [FromBody] 해줘야함
        public async Task<IActionResult> SaveAjaxData(MTestJson input, string json_data)
        {
            try
            {
                if (json_data.IsNull() == false)
                {
                    input = JsonConvert.DeserializeObject<MTestJson>(json_data);
                }

                /*
                 * 파일 저장위치는 저장 테스트용이며
                 * 실제 저장시는 파일명 중복 검증 및 업로드 파일형식 (exe, js 등) 제한하여야합니다
                 * 파일명 중복문제 -> DB를 통한 filename 및 fullpath 분리관리로 해결 등
                 */

                foreach (var file in Request.Form.Files)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadFiles", fileName); //파일 저장위치는 소스와 분리해야하나 일단 테스트용으로 놔둠;;

                        Directory.CreateDirectory(Path.GetDirectoryName(filePath)); //exist 하고 없으면 create 함
                        
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                }

                return Json(new { msg = "OK" });
            }
            catch(Exception ex)
            {
                return Json(new { msg = ex.Message });
            }

        }

        [HttpGet]
        public IActionResult GetTestData()
        {
            return Json(MTestJson.GetList());
        }

        public IActionResult SaveTestData(IList<MTestJson> input)
        {
            //여기서 검증 및 저장 작업
            if (!ModelState.IsValid)
            {
                return Json(new { msg = "잘못된 접근 입니다" });
            }

            return Json(new { msg = "OK" });
        }

        public IActionResult DragAndDropMultipleupload()
        {
            return View();
        }
    }
}
