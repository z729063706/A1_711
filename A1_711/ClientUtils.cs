using shardLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace client
{
    public class ClientUtils
    {
        public static string cacheIP = Configer.cacheIP;
        public static int cachePort = Configer.cachePort;
        public static string clientPath = Configer.ClientPath;
        //从socket获取文件列表
        public static List<Files> GetFileList(int serverPort)
        {
            string request = "GetFileList";
            List<Files> F = null;
            F = (List<Files>)SocketUtils.SendMessage("127.0.0.1", serverPort, request);
            return F;
        }
        public static List<string> GetSplites(string filename)
        {
            List<string> req = new();
            req.Add("GetSplites");
            req.Add(filename);
            List<string> F = null;
            F = (List<string>)SocketUtils.SendMessage(cacheIP, cachePort, req);
            return F;
        }
        public static byte[] GetSplit(string splitname)
        {
            List<string> req = new();
            req.Add("GetSplit");
            req.Add(splitname);
            byte[] F = null;
            F = (byte[])SocketUtils.SendMessage(cacheIP, cachePort, req);
            return F;
        }
        public static async Task GetSplitSync(string splitname)
        {
            if (!Directory.Exists(Configer.ClientTmpPath))
            {
                Directory.CreateDirectory(Configer.ClientTmpPath);
            }
            List<string> req = new();
            req.Add("GetSplit");
            req.Add(splitname);
            byte[] F = null;
            F = (byte[])SocketUtils.SendMessage(cacheIP, cachePort, req);
            File.WriteAllBytes(Configer.ClientTmpPath + @"\" + splitname, F);
        }
        public static async Task DownloadFile(List<String> splites, string filepath)
        {
            string filename = filepath.Split('\\').Last();
            string filePath = clientPath + @"\" + filename;
            byte[] file = new byte[0];
            foreach (string split in splites)
            {
                byte[] splitData = GetSplit(split);
                file = file.Concat(splitData).ToArray();
            }
            /*List<Task> downloadTasks = new List<Task>();
            foreach (string s in splites)
            {
                downloadTasks.Add(GetSplitSync(s));
            }
            await Task.WhenAll(downloadTasks);*/
            /*foreach (string s in splites)
            {

                byte[] splitData = Configer.ClientTmpPath + @"\" + s
                file = file.Concat(splitData).ToArray();
            }*/
            System.IO.File.WriteAllBytes(filePath, file);
            return;
        }
    }
}
