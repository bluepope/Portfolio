using CoreLib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcWeb.Models
{
    public class TestModel
    {
        public string COL1 { get; set; }
        public string COL2 { get; set; }

        public static IList<TestModel> GetList(int col1)
        {
            return MySqlDapperHelper.RunGetQueryFromXml<TestModel>("Sql/Home.xml", "GetTestData", new { COL1 = col1 });
        }
    }
}
