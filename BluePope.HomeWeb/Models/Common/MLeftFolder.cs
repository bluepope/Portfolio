using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.Common
{
    public class MLeftFolder
    {
        public MLeftFolder() { }
        public static MLeftFolder Create(Action<MLeftFolder> setting)
        {
            var model = new MLeftFolder();
            setting.Invoke(model);

            return model;
        }

        public string ControllerName { get; set; }
        public string Name { get; set; }
        public bool isActive { get => this.SubMenuList.Count(p => p.isActive) > 0; }

        public int NotiCount { get => this.SubMenuList.Sum(p => p.NotiCount); }

        public List<MLeftMenu> SubMenuList = new List<MLeftMenu>();

    }
}
