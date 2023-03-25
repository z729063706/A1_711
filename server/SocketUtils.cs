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

namespace server
{
    internal class SocketUtils
    {
        private static TcpListener _listener;
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
                                }
                                else if ((String)receivedObject == "GetFileRefreshDate")
                                {
                                    GetFileRefreshDate(stream);
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
