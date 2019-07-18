using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;

namespace CoreLib.Http
{
    public class HttpHelper
    {
        /// <summary>
        /// 웹 파일 다운로드
        /// </summary>
        /// <param name="url">http://some.com/downloadfile</param>
        /// <param name="saveFileName">if want filename, null is webfilename</param>
        /// <param name="downloadPath">if need filepath, null is /DownloadFiles </param>
        public static string WebDownloadFile(string url, string downloadPath = null, string saveFileName = null)
        {
            using (WebClient client = new WebClient())
            {
                client.OpenRead(url); //최초 접속하여 정보를 읽어옴

                if (string.IsNullOrWhiteSpace(saveFileName))
                {
                    if (client.ResponseHeaders.AllKeys.Any(p => p.ToLower() == "content-disposition"))
                    {
                        saveFileName = new ContentDisposition(client.ResponseHeaders["Content-Disposition"]).FileName;
                    }
                    else
                    {
                        saveFileName = Path.GetFileName(url);
                    }
                }

                if (string.IsNullOrWhiteSpace(downloadPath))
                {
                    downloadPath = Path.Combine(Directory.GetCurrentDirectory(), "DownloadFiles");
                }

                Directory.CreateDirectory(downloadPath);

                string fileFullPath = Path.Combine(downloadPath, saveFileName);

                if (File.Exists(fileFullPath))
                {
                    int cnt = Directory.GetFiles(downloadPath, $"{Path.GetFileNameWithoutExtension(saveFileName)}(*){Path.GetExtension(saveFileName)}").Count() + 1;

                    fileFullPath = Path.Combine(downloadPath, $"{Path.GetFileNameWithoutExtension(saveFileName)}({cnt}){Path.GetExtension(saveFileName)}");
                }

                client.DownloadFile(url, fileFullPath);

                return fileFullPath;
            }
        }
    }
}
