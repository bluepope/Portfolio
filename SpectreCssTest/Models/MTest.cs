using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpectreCssTest.Models
{
    public class MTest
    {
        public string Col1 { get; set; }
        public int Col2 { get; set; }

        public List<MTestSub> SubList { get; set; }

        public static MTest Get()
        {
            var model = new MTest();
            model.Col1 = "aaa";
            model.Col2 = 1;

            model.SubList = new List<MTestSub>();
            model.SubList.Add(new MTestSub() { Sub1 = "s1", Sub2 = 11 });
            model.SubList.Add(new MTestSub() { Sub1 = "s2", Sub2 = 12 });

            return model;
        }

        public static List<MTest> GetList()
        {
            var model = new List<MTest>();

            model.Add(MTest.Get());
            model.Add(MTest.Get());

            return model;
        }
    }
}
