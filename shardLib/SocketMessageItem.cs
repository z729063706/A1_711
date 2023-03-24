using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace shardLib
{

    public class SocketMessage
    {
        public string ObjectTypeString { get; set; }
        public byte[] Data { get; set; }

        public SocketMessage() { }

        public SocketMessage(object obj)
        {
            ObjectTypeString = obj.GetType().AssemblyQualifiedName;
            Data = SerializeObject(obj);
        }

        private byte[] SerializeObject(object obj)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(jsonString);
        }
        public object toObject()
        {
            string jsonString = Encoding.UTF8.GetString(Data);
            return JsonConvert.DeserializeObject(jsonString, Type.GetType(ObjectTypeString));
        }
    }
}
