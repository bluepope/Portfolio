using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcWeb.Models.ClientSide
{
    public class TestJsonModel
    {
        public bool isNew { get; set; } = false;
        public bool isEdit { get; set; } = false;
        public bool isDelete { get; set; } = false;


        public string COL1 { get; set; }
        public string COL2 { get; set; }

        public static IList<TestJsonModel> GetList()
        {
            var list = new List<TestJsonModel>();

            list.Add(new TestJsonModel() { COL1 = "K1", COL2 = "VAL1" });
            list.Add(new TestJsonModel() { COL1 = "K2", COL2 = "VAL2" });
            list.Add(new TestJsonModel() { COL1 = "K3", COL2 = "VAL3" });
            list.Add(new TestJsonModel() { COL1 = "K4", COL2 = "VAL4" });
            list.Add(new TestJsonModel() { COL1 = "K5", COL2 = "VAL5" });

            return list;
        }
    }
}
