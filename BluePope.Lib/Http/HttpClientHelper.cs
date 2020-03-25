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
using System.Web;

namespace BluePope.Lib.Http
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


            Client.DefaultRequestHeaders.Add("Accept-Language", "ko-KR,en-US");
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
            //Client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=UTF-8");
            //Client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");

            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, HttpContent content)
        {
            var request = new HttpRequestMessage();
            request.Method = method;

            if (request.Method == HttpMethod.Get)
            {
                var data = await content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(data))
                    request.RequestUri = new Uri(url);
                else if (url.IndexOf('?') < 0)
                    request.RequestUri = new Uri($"{url}?{data}");
                else
                    request.RequestUri = new Uri($"{url}&{data}");
            }
            else
            {
                request.RequestUri = new Uri(url);
                request.Content = content;
            }

            var response = await this.Client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<string> GetStringAsync(HttpMethod method, string url, object sendData = null)
        {
            var response = await SendAsync(method, url, new FormUrlEncodedContent(ConvertFormData(sendData)));

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetStringSendJsonAsync(HttpMethod method, string url, object sendData = null)
        {
            var response = await SendAsync(method, url, new StringContent(JsonConvert.SerializeObject(sendData), Encoding.UTF8, "application/json"));

            return await response.Content.ReadAsStringAsync();
        }


        public async Task<HttpFile> GetFileAsync(HttpMethod method, string url, object sendData = null)
        {
            var response = await SendAsync(method, url, new FormUrlEncodedContent(ConvertFormData(sendData)));

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
                    foreach (var item in ConvertFormData(sendData))
                        multipartFormContent.Add(new StringContent(item.Value, Encoding.UTF8, "application/x-www-form-urlencoded"), item.Key);
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

        public List<KeyValuePair<string, string>> ConvertFormData(object obj, string prefix = "")
        {
            var list = new List<KeyValuePair<string, string>>();
            string name;

            if (obj is null)
                return list;

            if (obj.GetType().GetGenericArguments().Length > 0)
            {
                int idx = 0;
                foreach (var item in (IEnumerable<object>)obj)
                {
                    foreach (var prop in item.GetType().GetProperties())
                    {
                        if (string.IsNullOrWhiteSpace(prefix))
                        {
                            name = $"{prop.Name}[{idx}]";
                        }
                        else
                        {
                            name = $"{prefix}[{idx}][{prop.Name}]";
                        }

                        if (prop.PropertyType.GetGenericArguments().Length > 0)
                        {
                            list.AddRange(ConvertFormData(prop.GetValue(item), name));
                        }
                        else
                        {
                            list.Add(new KeyValuePair<string, string>(name, prop.GetValue(item)?.ToString()));
                        }
                    }
                    idx++;
                }
            }
            else
            {
                foreach (var prop in obj.GetType().GetProperties())
                {
                    if (string.IsNullOrWhiteSpace(prefix))
                    {
                        name = prop.Name;
                    }
                    else
                    {
                        name = $"{prefix}[{prop.Name}]";
                    }

                    if (prop.PropertyType.GetGenericArguments().Length > 0)
                    {
                        list.AddRange(ConvertFormData(prop.GetValue(obj), name));
                    }
                    else
                    {
                        list.Add(new KeyValuePair<string, string>(name, prop.GetValue(obj)?.ToString()));
                    }
                }
            }

            return list;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                    this.Handler.Dispose();
                    this.Client.Dispose();
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~HttpClientHelper()
        // {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
