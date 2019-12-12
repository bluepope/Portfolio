using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BluePope.Lib.Http
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

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                    ResponseStream?.Dispose();
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                this.Name = null;
                this.FileName = null;

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~HttpFile()
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
