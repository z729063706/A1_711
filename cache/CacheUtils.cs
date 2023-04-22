using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using shardLib;

namespace cache
{
    internal class CacheUtils
    {
        private static TcpListener _listener;
        public static DateTime TT = DateTime.Now;
        public static List<Files> cacheFiles = new List<Files>();
        public static Dictionary<string, List<string>> imageCache = new Dictionary<string, List<string>>();
        public static int serverPort = Configer.serverPort;
        public static string serverIP = Configer.serverIP;
        public static string cachePath = Configer.CachePath;

        public static byte[] HexToByte(string filePath)
        {
            string hexString = File.ReadAllText(filePath);
            int byteCount = hexString.Length / 2;
            byte[] buffer = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
            {
                buffer[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return buffer;
        }
        public static DateTime GetFileRefreshDate()
        {
            DateTime t = DateTime.Now;
            try
            {
                string req = "GetFileRefreshDate";
                t = (DateTime)SocketUtils.SendMessage(serverIP, serverPort, req);
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }            
            return t;
        }
        public static void GetFileList(NetworkStream stream, DateTime t)
        {
            if (t != GetFileRefreshDate() || cacheFiles.Count == 0)
            {
                cacheFiles = SocketUtils.GetFileList(serverIP, serverPort);
                TT = GetFileRefreshDate();
                imageCache = GetFileSplit();
            }
            var message = new SocketMessage(cacheFiles);
            stream.Write(SocketUtils.SerializeObject(message), 0, SocketUtils.SerializeObject(message).Length);
            var thisForm = cacheForm.GetInstance();
            thisForm.Invoke(new Action(() => thisForm.addLog("Forward the Cached File List at " + t.ToString())));
        }
        public static void GetSplites(NetworkStream stream, string filename)
        {
            List<string> splites = new List<string>();
            if (imageCache.ContainsKey(filename))
            {
                splites = imageCache[filename];
            }
            long inCacheByte = 0;
            foreach (string split in splites)
            {
                string splitPath = cachePath + @"\" + split;
                if (File.Exists(splitPath))
                {
                    FileInfo fileInfo = new FileInfo(splitPath);                    
                    inCacheByte += fileInfo.Length;
                }
            }
            inCacheByte = inCacheByte / 2;
            long totalByte = 0;
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            foreach (Files file in cacheFiles)
            {
                if (file.Path == filename)
                {
                    totalByte = file.Size;
                    break;
                }
            }
            //TODO: Add a counter to get splites in cache
            var message = new SocketMessage(splites);
            stream.Write(SocketUtils.SerializeObject(message), 0, SocketUtils.SerializeObject(message).Length);
            var thisForm = cacheForm.GetInstance();
            thisForm.Invoke(new Action(() => thisForm.addLog("response: "+ (double)(inCacheByte / totalByte) * 100 +"% of file " + filenameWithoutExtension + " was constructed with the cached data")));
            //thisForm.Invoke(new Action(() => thisForm.addLog("Downloading " + filename )));
            //thisForm.Invoke(new Action(() => thisForm.addLog("In cache size: " + inCacheByte.ToString() + "/" + totalByte.ToString() + " cache rate:" + (double)(inCacheByte / totalByte) * 100 + "% Please allow about 10 second for it" )));
        }
        public static void GetSplitFromServer(string splitname)
        {
            List<string> req = new List<string>();
            req.Add("GetSplitFromServer");
            req.Add(splitname);
            byte[] split = (byte[])SocketUtils.SendMessage(serverIP, serverPort, req);
            string splitPath = cachePath + @"\" + splitname;
            File.WriteAllBytes(splitPath, split);
            var thisForm = cacheForm.GetInstance();
            thisForm.Invoke(new Action(() => thisForm.addFile(splitname)));
            thisForm.Invoke(new Action(() => thisForm.addCount() ));
            return;
        }
        public static void GetSplit(NetworkStream stream, string splitname)
        {
            byte[] data = null;
            if (!File.Exists(cachePath + @"\" + splitname))
            {
                GetSplitFromServer(splitname);
            }
            data = HexToByte(cachePath + @"\" + splitname);
            var message = new SocketMessage(data);
            var serializedMessage = SocketUtils.SerializeObject(message);
            stream.Write(serializedMessage, 0, serializedMessage.Length);
        }
        public static Dictionary<string, List<string>> GetFileSplit()
        {
            string req = "GetFileSplit";
            return (Dictionary<string, List<string>>)SocketUtils.SendMessage(serverIP, serverPort, req);
        }
        




        public static void StartServer(int port)
        {
            Task.Run(() =>
            {
                var thisForm = cacheForm.GetInstance();
                _listener = new TcpListener(IPAddress.Any, port);
                _listener.Start();
                thisForm.Invoke(new Action(() => thisForm.addLog("Cache Started!")));
                while (true)
                {
                    try
                    {
                        using (var client = _listener.AcceptTcpClient())
                        using (var stream = client.GetStream())
                        {
                            var buffer = new byte[client.ReceiveBufferSize];
                            int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);

                            var message = (SocketMessage)SocketUtils.DeserializeObject(buffer.Take(bytesRead).ToArray(), typeof(SocketMessage));
                            var receivedObject = SocketUtils.DeserializeObject(message.Data, Type.GetType(message.ObjectTypeString));
                            if (Type.GetType(message.ObjectTypeString) == typeof(String))
                            {
                                if ((String)receivedObject == "GetFileList")
                                {
                                    GetFileList(stream, TT);
                                    thisForm.Invoke(new Action(() => thisForm.addLog("user request: filelist at " + DateTime.Now.ToString() )));
                                }
                            }
                            else if(Type.GetType(message.ObjectTypeString) == typeof(List<string>))
                            {
                                List<string> req = (List<string>)receivedObject;
                                if (req[0] == "GetSplites")
                                {
                                    GetSplites(stream, req[1]);
                                    thisForm.Invoke(new Action(() => thisForm.addLog("user request: file " + req[1] +" at " + DateTime.Now.ToString() )));
                                }
                                else if (req[0] == "GetSplit")
                                {
                                    GetSplit(stream, req[1]);
                                }
                            }
                        }
                    }
                    catch (SocketException)
                    {
                        break;
                    }

                }
            });
        }
        public static void StopServer(int port)
        {
            _listener.Stop();
            var thisForm = cacheForm.GetInstance();
            thisForm.Invoke(new Action(() => thisForm.addLog("Cache Stopped!")));
        }
    }
}
