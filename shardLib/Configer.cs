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
        public static string BasePath = Environment.CurrentDirectory;
        public static string ServerPath = BasePath+@"\Server";
        public static string CachePath = BasePath + @"\Cache";
        public static string ClientPath = BasePath + @"\Client";
        public static string LogPath = BasePath+@"\log.txt";
        public static int ChunkSize = 1024 * 1024;
    }
}
