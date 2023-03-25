using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace shardLib
{
    public class SocketUtils
    {
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

                            var responseBuffer = new byte[client.ReceiveBufferSize];
                            var readTask = stream.ReadAsync(responseBuffer, 0, client.ReceiveBufferSize);
                            if (readTask.Wait(timeoutMilliseconds))
                            {
                                int bytesRead = readTask.Result;
                                if (bytesRead > 0)
                                {
                                    var responseMessage = (SocketMessage)DeserializeObject(responseBuffer.Take(bytesRead).ToArray(), typeof(SocketMessage));
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
        public static List<Files> GetFileList(string serverAddress, int port, int timeoutMilliseconds = 5000)
        {
            string req = "GetFileList";
            var response = (List<Files>)SendMessage(serverAddress, port, req, timeoutMilliseconds);
            return response;
        }
    }
}
