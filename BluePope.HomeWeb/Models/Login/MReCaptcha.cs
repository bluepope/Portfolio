using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Models.Login
{
    public class MReCaptcha
    {
        /*
"success": true,
"challenge_ts": "2019-06-24T07:10:57Z",
"hostname": "localhost",
"score": 0.9,
"action": "login"
"error-codes: ['timeout-or-duplicate']"
*/
        public static string SiteKey { get; set; }
        public static string PrivateKey { get; set; }

        public bool Success { get; set; }

        public string Challenge_TS { get; set; }
        public string HostName { get; set; }
        public float Score { get; set; }
        public string Action { get; set; }

        [JsonProperty(PropertyName = "error-codes")]
        public List<string> ErrorCodes {get;set; }

        public static async Task<MReCaptcha> RecaptchaVerify(string recaptchaToken)
        {
            string url = $"https://www.google.com/recaptcha/api/siteverify?secret={MReCaptcha.PrivateKey}&response={recaptchaToken}";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var json = await httpClient.GetStringAsync(url);
                    return JsonConvert.DeserializeObject<MReCaptcha>(json);

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

    }
}
