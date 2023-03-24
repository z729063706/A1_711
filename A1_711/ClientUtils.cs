using shardLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    public class ClientUtils
    {
        //从socket获取文件列表
        public static List<Files> GetFileList(int serverPort)
        {
            string request = "GetFileList";
            List<Files> F = null;
            F = (List<Files>)SocketUtils.SendMessage("127.0.0.1", serverPort, request);
            return F;
        }

    }
}
