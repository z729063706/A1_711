using System;
using System.Collections.Generic;
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
        public static DateTime T = DateTime.Now;
        public static List<Files> cacheFiles = new List<Files>();
        public static Dictionary<string, List<string>> imageCache = new Dictionary<string, List<string>>();
        public static int serverPort = Configer.serverPort;
        public static string serverIP = Configer.serverIP;
        public static string cachePath = Configer.CachePath;

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
            if (t != GetFileRefreshDate() || cacheFiles == null)
            {
                cacheFiles = SocketUtils.GetFileList(serverIP, serverPort);
                T = GetFileRefreshDate();
            }
            var message = new SocketMessage(cacheFiles);
            stream.Write(SocketUtils.SerializeObject(message), 0, SocketUtils.SerializeObject(message).Length);
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

                            var message = (SocketMessage)SocketUtils.DeserializeObject(buffer.Take(bytesRead).ToArray(), typeof(SocketMessage));
                            var receivedObject = SocketUtils.DeserializeObject(message.Data, Type.GetType(message.ObjectTypeString));
                            if (Type.GetType(message.ObjectTypeString) == typeof(String))
                            {
                                if ((String)receivedObject == "GetFileList")
                                {
                                    GetFileList(stream, T);
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
        }
    }
}
