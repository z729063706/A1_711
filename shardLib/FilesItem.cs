using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shardLib
{
    public class Files
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime Update { get; set; }
        public string DownloadStatus { get; set; }
        public string Path { get; set; }
        public byte[] Tb { get; set; }
        public Files()
        {
        }

        public Files(string path, long size, DateTime update, Image tb, string downloadStatus = "To be download")
        {
            Path = path;
            Name = path.Split('\\')[path.Split('\\').Length - 1];
            Size = size;
            Update = update;
            DownloadStatus = downloadStatus;
            Tb = ImageToByteArray(tb);
        }
        public static string sizeToString(long sz)
        {
            {
                if (sz < 1024)
                {
                    return sz.ToString() + "B";
                }
                else if (sz < 1024 * 1024)
                {
                    return (sz / 1024).ToString() + "KB";
                }
                else if (sz < 1024 * 1024 * 1024)
                {
                    return (sz / 1024 / 1024).ToString() + "MB";
                }
                else
                {
                    return (sz / 1024 / 1024 / 1024).ToString() + "GB";
                }
            }
        }
        public static byte[] ImageToByteArray(Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }

        public static Image ByteArrayToImage(byte[] byteArray)
        {
            using (var memoryStream = new MemoryStream(byteArray))
            {
                return Image.FromStream(memoryStream);
            }
        }
        static bool ThumbnailCallback()
        {
            return false;
        }
        public static List<Files> getFiles(string serverpath)
        {
            List<Files> files = new List<Files>();

            foreach (string file in Directory.GetFiles(serverpath))
            {
                //string pics = ["png", "jpg", "tiff", "bmp", "ico"];
                try
                { 
                    Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                    /*Image image = Image.FromFile(file);
                    Image tb = image.GetThumbnailImage(32, 32, myCallback, IntPtr.Zero);
                    FileInfo fileinfo = new FileInfo(file);
                    Files tmp = new Files(file, fileinfo.Length, fileinfo.LastWriteTime, tb);
                    files.Add(tmp);*/
                    using (Image image = Image.FromFile(file))
                    {
                        using (Image tb = image.GetThumbnailImage(32, 32, myCallback, IntPtr.Zero))
                        {
                            FileInfo fileinfo = new FileInfo(file);
                            Files tmp = new Files(file, fileinfo.Length, fileinfo.LastWriteTime, tb);
                            files.Add(tmp);
                            tb.Dispose();
                        }
                        image.Dispose();
                    }
                }
                catch
                {
                    continue;
                }
               
            }
            return files;
        }
    }
}
