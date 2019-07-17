using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreMvcWeb.Models;
using System.Drawing;
using System.Net;
using System.IO;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using CoreMvcWeb.Services.BatchJob;

namespace CoreMvcWeb.Controllers
{
    /// <summary>
    /// 실험실
    /// </summary>
    public class LabController : Controller
    {
        public LabController()
        {
        }
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

        public IActionResult DeliveryCheck()
        {
            return View();
        }

        public IActionResult GetDeliveryCheckList(string company_code, string invoice_no)
        {
            //selenium 및 크롬을 통한 웹 크롤링

            /* linux 동작시
             * 1. 구글 크롬 설치
             * centos 7 의 경우
             * 1) repo 등록
vi /etc/yum.repos.d/google-chrome.repo

[google-chrome]
name=google-chrome
baseurl=http://dl.google.com/linux/chrome/rpm/stable/x86_64
enabled=1
gpgcheck=1
gpgkey=https://dl-ssl.google.com/linux/linux_signing_key.pub

             * 2) yum install google-chrome-stable
             * yum install http://dev.naver.com/frs/download.php/443/ttf-nanum-coding-2.0-2.noarch.rpm
             * 2. 구글 크롬 드라이버 설치 // 설치된 크롬 버전에 맞는 드라이버 필요
             * 3. 폰트 설치 // 나눔고딕
cd /usr/share/fonts/
mkdir ./nanumfont
cd ./nanumfont
wget http://cdn.naver.com/naver/NanumFont/fontfiles/NanumFont_TTF_ALL.zip
unzip NanumFont_TTF_ALL.zip

             * 4. webdriver에 실행권한 줄것 chmod 777 과 같이..
             */
            var text = string.Empty;

            if (company_code == "EMS")
            {
                var options = new ChromeOptions();
                options.AddArgument("no-sandbox");
                options.AddArgument("headless");
                options.AddArgument("window-size=1920,1080");
                options.AddArgument("disable-gpu");
                options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36");
                options.AddArgument("lang=ko-kr");

                using (var driver = new ChromeDriver(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "webdriver"), options))
                {
                    try
                    {
                        IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30.00));

                        driver.Url = $"https://service.epost.go.kr/trace.RetrieveEmsRigiTraceList.comm?POST_CODE={invoice_no}";
                        driver.Navigate();

                        wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                        var rows = driver.FindElementsByCssSelector(".detail_off tr");

                        foreach(var row in rows)
                        {
                            var cols = row.FindElements(By.CssSelector("td"));

                            for(int i=0; i < cols.Count(); i++)
                            {
                                text += $"!{i} {cols[i].Text}";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        text = ex.Message;
                    }
                    finally
                    {
                        driver.Quit();
                    }
                }
            }

            return Json(new { data = text });
        }
    }
}
