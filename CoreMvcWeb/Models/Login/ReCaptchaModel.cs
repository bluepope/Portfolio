using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcWeb.Models.Login
{
    public class ReCaptchaModel
    {
        /*
"success": true,
"challenge_ts": "2019-06-24T07:10:57Z",
"hostname": "localhost",
"score": 0.9,
"action": "login"
"error-codes: ['timeout-or-duplicate']"
*/

        public bool Success { get; set; }

        public string Challenge_TS { get; set; }
        public string HostName { get; set; }
        public float Score { get; set; }
        public string Action { get; set; }

        [JsonProperty(PropertyName = "error-codes")]
        public string[] ErrorCodes {get;set; }
    }
}
