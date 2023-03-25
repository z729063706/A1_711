using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using shardLib;
using System.Security.Cryptography;

namespace server
{
    internal class ServerUtils
    {
        private static TcpListener _listener;
        public static string serverSplitPath = Configer.ServerPath+ @"\splits";
        public static List<Files> cacheFiles = new List<Files>();
        public static Dictionary<string, List<string>> imageCache = new Dictionary<string, List<string>>();
        public static byte[] SerializeObject(object obj)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(obj, options);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        public static object DeserializeObject(byte[] data, Type type)
        {
            string jsonString = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize(jsonString, type);
        }
        //返回文件列表
        public static void GetFileList(NetworkStream stream)
        {
            var message = new SocketMessage(serverForm.F);
            stream.Write(SerializeObject(message), 0, SerializeObject(message).Length);
        }
        public static void GetFileRefreshDate(NetworkStream stream)
        {
            var message = new SocketMessage(serverForm.T);
            stream.Write(SerializeObject(message), 0, SerializeObject(message).Length);
        }
        public static void GetFileSplit(NetworkStream stream)
        {
            var message = new SocketMessage(imageCache);
            stream.Write(SerializeObject(message), 0, SerializeObject(message).Length);
        }
        public static string ByteToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        public static string GetMd5(byte[] input, int length)
        {
            using MD5 md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(input, 0, length);
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                s.Append(hashBytes[i].ToString("x2"));
            }
            return s.ToString();
        }

        public static async Task<Dictionary<string, List<string>>> SpiltFiles(List<Files> F)
        {
            int chuckSize = Configer.ChunkSize;
            Dictionary<string, List<string>> ImageCache = imageCache;
            foreach (Files f in F)
            {
                if (ImageCache.ContainsKey(f.Path))
                {
                    continue;
                }
                string imagePath = f.Path;
                using FileStream imageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                int chunkIndex = 0;
                byte[] buffer = new byte[chuckSize];
                int bytesRead;
                List<string> chunkHashs = new List<string>();
                while ((bytesRead = await imageStream.ReadAsync(buffer, 0, chuckSize)) > 0)
                {
                    string chunkHash = GetMd5(buffer, bytesRead);
                    string outputPath = Path.Combine(serverSplitPath, $"{chunkHash}");
                    chunkHashs.Add(chunkHash);
                    File.WriteAllText(outputPath,ByteToHex(buffer));
                    chunkIndex++;
                }
                imageCache.Add(imagePath, chunkHashs);
            }
            imageCache = ImageCache;
            return ImageCache;
        }
        public static async void SpiltInit()
        {
            string cacheJson = serverSplitPath + @"\spilt.json";            
            if (File.Exists(serverSplitPath))
            {
                string json = File.ReadAllText(serverSplitPath);
                imageCache = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
            }
            else
            {
                cacheFiles = serverForm.F;
                imageCache = await SpiltFiles(cacheFiles);
            }
        }
        public static void SaveCacheToFile(Dictionary<string, List<string>> cache, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);
            string json = JsonSerializer.Serialize(cache);
            string filePath = Path.Combine(outputDirectory, "spilt.json");
            File.WriteAllText(filePath, json);
        }




        public static void StartServer(int port)
        {
            Task.Run(() =>
            {
                _listener = new TcpListener(IPAddress.Any, port);
                _listener.Start();
                while (true)
                {
                    try 
                    {
                        using (var client = _listener.AcceptTcpClient())
                        using (var stream = client.GetStream())
                        {
                            var buffer = new byte[client.ReceiveBufferSize];
                            int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);

                            var message = (SocketMessage)DeserializeObject(buffer.Take(bytesRead).ToArray(), typeof(SocketMessage));
                            var receivedObject = DeserializeObject(message.Data, Type.GetType(message.ObjectTypeString));
                            Console.WriteLine("Received object: " + receivedObject);
                            if(Type.GetType(message.ObjectTypeString) == typeof(String))
                            {
                                if ((String)receivedObject == "GetFileList")
                                {
                                    GetFileList(stream);
                                    //serverForm.addLog("GetFileList");
                                }
                                else if ((String)receivedObject == "GetFileRefreshDate")
                                {
                                    GetFileRefreshDate(stream);
                                }
                                else if ((String)receivedObject == "GetFileSplit")
                                {
                                    GetFileSplit(stream);
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
            SaveCacheToFile(imageCache, serverSplitPath);
        }
    }
}
