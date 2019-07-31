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
        private HttpClientHandler Handler { get; }
        private CookieContainer Cookies { get; }
        private HttpClient Client { get; }

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
            this.Handler.Dispose();
            this.Client.Dispose();
        }

        public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, HttpContent content)
        {
            var request = new HttpRequestMessage()
            {
                Method = method,
                RequestUri = new Uri(url),
                Content = content,
            };

            var response = await this.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            
            return response;
        }

        public async Task<string> GetStringAsync(HttpMethod method, string url, object sendData = null)
        {
            var response = await SendAsync(method, url, new StringContent(JsonConvert.SerializeObject(sendData), Encoding.UTF8, "application/json"));

            return await response.Content.ReadAsStringAsync();
        }


        public async Task<HttpFile> GetFileAsync(HttpMethod method, string url, object sendData = null)
        {
            var response = await SendAsync(method, url, new StringContent(JsonConvert.SerializeObject(sendData), Encoding.UTF8, "application/json"));

            var file = new HttpFile();
            file.TotalBytesSize = response.Content.Headers.ContentLength.GetValueOrDefault(0);
            file.ResponseStream = await response.Content.ReadAsStreamAsync();

            try
            {
                file.FileName = new ContentDisposition(response.Content.Headers.GetValues("content-disposition").FirstOrDefault()).FileName;
            }
            catch
            {
                file.FileName = System.IO.Path.GetFileName(url);
            }

            return file;
        }

        //multipart 텍스트 및 파일 전송
        public async Task<string> SendMultipartAsync(HttpMethod method, string url, object sendData, IList<HttpFile> fileList, Action<long, long> totalProgressEvent = null, Action<string, long, long> fileProgressEvent = null)
        {
            using (var multipartFormContent = new MultipartFormDataContent())
            {
                if (sendData != null)
                {
                    foreach (var prop in sendData.GetType().GetProperties())
                    {
                        var objVal = prop.GetValue(sendData, null);
                        string val = null;

                        if (objVal != null)
                            val = objVal.ToString();

                        multipartFormContent.Add(new StringContent(val, Encoding.UTF8, "application/json"), prop.Name);
                    }
                }
                
                if (fileList?.Count > 0)
                {
                    long totalFileSize = 0;
                    long readLength = 0;

                    foreach (var file in fileList)
                    {
                        totalFileSize += file.TotalBytesSize;

                        var uploadContent = new ProgressableStreamContent(new StreamContent(file.ResponseStream), (now, tot) => {

                            fileProgressEvent?.Invoke(file.FileName, now, tot);
                            totalProgressEvent?.Invoke(readLength + now, totalFileSize);

                            if (now == tot)
                                readLength += now;
                        });

                        multipartFormContent.Add(uploadContent, file.Name, file.FileName);
                    }
                }
                
                return await (await SendAsync(method, url, multipartFormContent)).Content.ReadAsStringAsync();
            }
        }
    }
}
