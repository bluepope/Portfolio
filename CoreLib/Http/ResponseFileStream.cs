using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CoreLib.Http
{
    public class ResponseFileStream
    {
        public string FileName;
        public Stream ResponseStream;

        public async void SaveAs(string path, FileMode fileMode = FileMode.CreateNew)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (System.IO.FileStream output = new System.IO.FileStream(path, fileMode))
            {
                await this.ResponseStream.CopyToAsync(output);
            }
        }
    }
}
