using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.Common
{
    public class MLeftMenu
    {
        public MLeftMenu() { }
        public static MLeftMenu Create(Action<MLeftMenu> setting)
        {
            var model = new MLeftMenu();
            setting.Invoke(model);

            return model;
        }

        public string ActionName { get; set; }
        public string Name { get; set; }
        public bool isActive { get; set; }

        public int NotiCount { get; set; }

        public void setActive(bool flag) => this.isActive = flag;
        public void setNotiCount(int notiCount) => this.NotiCount = notiCount;
    }
}
