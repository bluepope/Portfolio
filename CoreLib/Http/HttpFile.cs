using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CoreLib.Http
{
    public class HttpFile : IDisposable
    {
        /// <summary>
        /// 전송시 이름
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 파일명
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 파일데이터
        /// </summary>
        public Stream ResponseStream { get; set; }

        /// <summary>
        /// 파일 크기
        /// </summary>
        public long TotalBytesSize { get; set; }

        public HttpFile()
        {

        }

        public HttpFile(string name, string filePath)
        {
            this.Name = name;

            var file = new System.IO.FileInfo(filePath);
            this.FileName = file.Name;
            this.ResponseStream = file.Open(FileMode.Open, FileAccess.Read);
            this.TotalBytesSize = file.Length;
        }

        public HttpFile(string name, string fileName, byte[] fileData)
        {
            this.Name = name;
            this.FileName = fileName;

            this.ResponseStream = new MemoryStream(fileData);
            this.TotalBytesSize = fileData.LongLength;
        }

        public void Dispose()
        {
            this.Name = null;
            this.FileName = null;
            if (this.ResponseStream != null)
                this.ResponseStream.Dispose();
        }

        /// <summary>
        /// 파일 다운로드 저장
        /// </summary>
        /// <param name="path">저장위치</param>
        /// <param name="isOverride">덮어쓰기여부</param>
        /// <param name="progressEvent">nowSize 이벤트</param>
        public async void SaveAs(string path, bool isOverride = false, Action<long> progressEvent = null)
        {
            const int bufferSize = 20480; //20kb
            var totalBytesRead = 0L;
            var buffer = new byte[bufferSize];
            var isMoreToRead = true;

            var fileMode = System.IO.FileMode.CreateNew;

            if (isOverride)
                fileMode = FileMode.Create;

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var fileStream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None, bufferSize, true))
            {
                do
                {
                    var bytesRead = await this.ResponseStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;
                        progressEvent?.Invoke(totalBytesRead);
                        continue;
                    }

                    await fileStream.WriteAsync(buffer, 0, bytesRead);

                    totalBytesRead += bytesRead;
                    progressEvent?.Invoke(totalBytesRead);
                }
                while (isMoreToRead);
            }
        }
    }
}
