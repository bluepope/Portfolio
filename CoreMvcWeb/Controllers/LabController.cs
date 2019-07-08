using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreMvcWeb.Models;

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

        public IActionResult BarCodeGenerator(string barcode_number)
        {
            if (barcode_number.IsNull() == false)
            {
                try
                {
                    ViewData["barcode_number"] = barcode_number;

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
