using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcWeb.Models.Common
{
    public class LeftFolderModel
    {
        public LeftFolderModel() { }
        public static LeftFolderModel Create(Action<LeftFolderModel> setting)
        {
            var model = new LeftFolderModel();
            setting.Invoke(model);

            return model;
        }

        public string ControllerName { get; set; }
        public string Name { get; set; }
        public bool isActive { get => this.SubMenuList.Count(p => p.isActive) > 0; }

        public int NotiCount { get => this.SubMenuList.Sum(p => p.NotiCount); }

        public List<LeftMenuModel> SubMenuList = new List<LeftMenuModel>();

    }
}
