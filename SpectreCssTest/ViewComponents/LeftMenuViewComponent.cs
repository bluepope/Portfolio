using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpectreCssTest.ViewComponents
{
    public class LeftMenuViewComponent : ViewComponent
    {
        //static List<MLeftMenu> _leftmenu = null;

        public LeftMenuViewComponent()
        {
        }

        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var controller = ViewContext.RouteData.Values["Controller"] as string;
            var action = ViewContext.RouteData.Values["Action"] as string;

            return await Task.Run(() => { return View(); });
        }
    }
}
