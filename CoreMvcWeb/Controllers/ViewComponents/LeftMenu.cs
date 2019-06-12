using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcWeb.Controllers.ViewComponents
{
    public class LeftMenu : ViewComponent
    {
        public LeftMenu()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(PathString requestPath, string controllerName, string actionName)
        {
            return await Task.Run(() => { return View("Default", $"{requestPath} / {controllerName} / {actionName}"); });
        }
    }
}
