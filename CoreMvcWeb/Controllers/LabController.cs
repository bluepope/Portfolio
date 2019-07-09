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
        public IActionResult BarCodeGenerator()
        {
            return View();
        }

        public IActionResult GetBarcodeImage(string barcodeNumber, string imageType)
        {
            if (barcodeNumber.IsNull() == false)
            {
                try
                {
                    var barcode = new NetBarcode.Barcode(barcodeNumber, NetBarcode.Type.EAN13, true);

                    if (imageType.ToLower() == "base64")
                    {
                        return Content($"data:image/png;base64, {barcode.GetBase64Image()}");
                    }
                    else //png
                    {
                        return File(barcode.GetByteArray(), "image/png", $"barcode_{barcodeNumber}.png");
                    }
                }
                catch
                {
                }
            }

            return Content("");
        }
    }
}
