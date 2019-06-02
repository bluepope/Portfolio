using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreMvcWeb.Models;
using CoreMvcWeb.Hubs.Chat;
using Microsoft.AspNetCore.SignalR;

namespace CoreMvcWeb.Controllers
{
    public class ClientSideController : Controller
    {
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
    }
}
