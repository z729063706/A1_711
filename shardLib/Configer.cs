using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shardLib
{
    public class Configer
    {
        public static string localhost = "127.0.0.1";
        public static string serverIP = localhost;
        public static string cacheIP = localhost;
        public static int serverPort = 8088; 
        public static int cachePort = 8089;
        public static string ServerPath = @"C:\Users\72906\Desktop\711\Server";
        public static string CachePath = @"C:\Users\72906\Desktop\711\Cache";
        public static int ChunkSize = 2 * 1024;
    }
}
