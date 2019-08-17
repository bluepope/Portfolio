using CoreLib.DataBase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreMvcWeb.Models.Lab
{
    public class DeliveryInfoModel
    {
        public string COMPANY_TYPE { get; set; }
        public string INVOICE_NO { get; set; }
        public int SEQ { get; set; }
        public string UPDATE_DATE { get; set; }

        public string REG_USER { get; set; }
        public DateTime REG_DATE { get; set; }
        public string MOVE_TYPE { get; set; }
        public string REMARK1 { get; set; }

        /// <summary>
        /// 상태정보는 마스터정보로 이관해야함
        /// </summary>
        public string STATUS_FLAG { get; set; }


        public async static Task<IList<DeliveryInfoModel>> GetList(string company_type, string invoice_no)
        {
            return await MySqlDapperHelper.RunGetQueryFromXmlAsync<DeliveryInfoModel>("/Sql/Lab.xml", "GetDeliveryList", new {
                company_type = company_type,
                invoice_no = invoice_no,
            });
        }
    }
}