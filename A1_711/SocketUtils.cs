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

namespace client
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

        public static object SendMessage(string serverAddress, int port, object obj, int timeoutMilliseconds = 5000)
        {
            object responseObject = null;
            try
            {
                using (var client = new TcpClient())
                {
                    // 连接到服务器
                    if (client.ConnectAsync(serverAddress, port).Wait(timeoutMilliseconds))
                    {
                        using (var stream = client.GetStream())
                        {
                            var message = new SocketMessage
                            {
                                ObjectTypeString = obj.GetType().AssemblyQualifiedName,
                                Data = SerializeObject(obj)
                            };
                            var buffer = SerializeObject(message);
                            stream.Write(buffer, 0, buffer.Length);

                            // 接收服务器返回的数据
                            var responseBuffer = new byte[client.ReceiveBufferSize];
                            var readTask = stream.ReadAsync(responseBuffer, 0, client.ReceiveBufferSize);
                            if (readTask.Wait(timeoutMilliseconds))
                            {
                                int bytesRead = readTask.Result;
                                if (bytesRead > 0)
                                {
                                    var responseMessage = (SocketMessage)DeserializeObject(responseBuffer.Take(bytesRead).ToArray(), typeof(SocketMessage));
                                    //responseObject = DeserializeObject(responseMessage.Data, Type.GetType(responseMessage.ObjectTypeString));
                                    responseObject = responseMessage.toObject();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Read from server timed out.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Connection to server timed out.");
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return responseObject;
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
