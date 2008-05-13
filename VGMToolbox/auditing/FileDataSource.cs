using System.IO;

namespace VGMToolbox.auditing
{
    public class FileDataSource : ICSharpCode.SharpZipLib.Zip.IStaticDataSource
    {
        private string fileName;

        public FileDataSource(string fileName)
        {
            this.fileName = fileName;
        }
        public Stream GetSource()
        {
            return new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }
    }
}
