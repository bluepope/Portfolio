using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Http
{
    /// <summary>
    /// http client 를 이용한 up / down
    /// 비동기 upload / download progress 를 추가 개발해야함
    /// </summary>
    public class HttpClientHelper : IDisposable
    {
        private HttpClientHandler Handler { get; set; }
        private CookieContainer Cookies { get; set; }
        private HttpClient Client { get; set; }

        public HttpClientHelper()
        {
            this.Cookies = new CookieContainer();
            this.Handler = new HttpClientHandler() { CookieContainer = this.Cookies };
            this.Client = new HttpClient(this.Handler);

            this.Client.DefaultRequestHeaders.Add("Accept-Language", "ko-KR,en-US");
            this.Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
        }

        public void Dispose()
        {
            this.Cookies = null;
            this.Handler.Dispose();
            this.Client.Dispose();
        }

        public async Task<HttpResponseMessage> GetAsyncGetResponse(string url, object jsonObject = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(url);

            if (jsonObject != null)
            {
                if (url.IndexOf("?") < 0)
                    sb.Append("?");

                foreach (var prop in jsonObject.GetType().GetProperties())
                {
                    var objVal = prop.GetValue(jsonObject, null);
                    string val = string.Empty;

                    if (objVal != null)
                        val = objVal.ToString();

                    sb.AppendFormat("&{0}={1}", prop.Name, System.Net.WebUtility.UrlEncode(val));
                }
            }

            var response = await this.Client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<string> GetAsync(string url, object jsonObject = null)
        {
            var response = await GetAsyncGetResponse(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<ResponseFileStream> GetAsyncAndGetFile(string url, object jsonObject = null)
        {
            var response = await GetAsyncGetResponse(url);
            response.EnsureSuccessStatusCode();

            var r = new ResponseFileStream();
            r.ResponseStream = await response.Content.ReadAsStreamAsync();
            try
            {
                r.FileName = new ContentDisposition(response.Content.Headers.GetValues("content-disposition").FirstOrDefault()).FileName;
            }
            catch
            {
                r.FileName = System.IO.Path.GetFileName(url);
            }

            return r;
        }

        public async Task<HttpResponseMessage> PostAsyncGetResponse(string url, object jsonObject = null)
        {
            var uri = new Uri(url);

            StringContent sendObj = null;
            
            if (jsonObject != null)
                sendObj = new StringContent(JsonConvert.SerializeObject(jsonObject), Encoding.UTF8, "application/json");

            var response = await this.Client.PostAsync(uri, sendObj);

            response.EnsureSuccessStatusCode();

            return response;
        }

        //post 전송
        public async Task<string> PostAsync(string url, object jsonObject = null)
        {
            var response = await PostAsyncGetResponse(url, jsonObject);
             
            return await response.Content.ReadAsStringAsync();
        }

        //파일 다운로드
        public async Task<ResponseFileStream> PostAsyncAndGetFile(string url, object jsonObject = null)
        {
            var response = await PostAsyncGetResponse(url, jsonObject);

            var r = new ResponseFileStream();
            r.ResponseStream = await response.Content.ReadAsStreamAsync();

            try
            {
                r.FileName = new ContentDisposition(response.Content.Headers.GetValues("content-disposition").FirstOrDefault()).FileName;
            }
            catch
            {
                r.FileName = System.IO.Path.GetFileName(url);
            }

            return r;
        }


        //multipart 텍스트 및 파일 전송
        public async Task<string> PostMultipartAsync(string url, object jsonObject, Dictionary<string, string> filePathList)
        {
            var uri = new Uri(url);

            using (var content = new MultipartFormDataContent())
            {
                if (jsonObject != null)
                {
                    foreach (var prop in jsonObject.GetType().GetProperties())
                    {
                        var objVal = prop.GetValue(jsonObject, null);
                        string val = null;

                        if (objVal != null)
                            val = objVal.ToString();

                        content.Add(new StringContent(val), prop.Name);
                    }
                }
                
                if (filePathList != null && filePathList.Count > 0)
                {
                    foreach(var filePath in filePathList)
                    {
                        var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath.Value));
                        content.Add(fileContent, filePath.Key, Path.GetFileName(filePath.Value));
                    }
                }

                var response = this.Client.PostAsync(uri, content).Result;
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
