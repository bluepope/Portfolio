using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.ClientSide
{
    public class MTestJson
    {
        public bool isNew { get; set; } = false;
        public bool isEdit { get; set; } = false;
        public bool isDelete { get; set; } = false;


        public string COL1 { get; set; }
        public string COL2 { get; set; }

        public static IList<MTestJson> GetList()
        {
            var list = new List<MTestJson>();

            list.Add(new MTestJson() { COL1 = "K1", COL2 = "VAL1" });
            list.Add(new MTestJson() { COL1 = "K2", COL2 = "VAL2" });
            list.Add(new MTestJson() { COL1 = "K3", COL2 = "VAL3" });
            list.Add(new MTestJson() { COL1 = "K4", COL2 = "VAL4" });
            list.Add(new MTestJson() { COL1 = "K5", COL2 = "VAL5" });

            return list;
        }
    }
}
