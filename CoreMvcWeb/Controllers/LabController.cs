using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreMvcWeb.Models;
using System.Drawing;

namespace CoreMvcWeb.Controllers
{
    /// <summary>
    /// 실험실
    /// </summary>
    public class LabController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 바코드 제네레이터
        /// </summary>
        /// <param name="barcode_number"></param>
        /// <returns></returns>
        public IActionResult BarCodeGenerator(string barcode_number)
        {
            if (barcode_number.IsNull() == false)
            {
                try
                {
                    ViewData["barcode_number"] = barcode_number;

                    //바코드는 System.Drawing.Common 을 사용하는데 Core 2.2 기준으로 현재 CrossPlatform 지원 안됨
                    //core 3.0에서 해결될 것으로 기대 중

                    //이를 해결하기 위해 Microsoft.Windows.Compatibility 를 Nuget 에서 설치
                    // centos 기준으로 다음 작업 필요
                    // yum install -y epel-release 
                    // yum whatprovides libgdiplus --최신버전 검색
                    // yum install -y libgdiplus-2.10-10.el7.x86_64 --19.07.08 기준 최신

                    var barcode = new NetBarcode.Barcode(barcode_number, NetBarcode.Type.EAN13, true);

                    //base64 로 return
                    ViewData["barcode_image"] = barcode.GetBase64Image();
                }
                catch
                {

                }
            }

            return View();
        }

        public IActionResult GetBarcodeImage(string barcode_number)
        {
            if (barcode_number.IsNull() == false)
            {
                try
                {
                    var barcode = new NetBarcode.Barcode(barcode_number, NetBarcode.Type.EAN13, true);

                    return File(barcode.GetByteArray(), "image/png", $"barcode_{barcode_number}.png");
                }
                catch
                {
                }
            }

            return null;
        }
    }
}
