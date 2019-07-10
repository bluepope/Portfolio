using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcWeb.Models.Common
{
    public class LeftMenuModel
    {
        public LeftMenuModel() { }
        public static LeftMenuModel Create(Action<LeftMenuModel> setting)
        {
            var model = new LeftMenuModel();
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
