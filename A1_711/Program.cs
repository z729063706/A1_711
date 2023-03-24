using shardLib;
using System.IO;
using Image = System.Drawing.Image;
namespace client
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string ServerPath = @"C:\Users\72906\Desktop\711\Server";
            string[] files = Directory.GetFiles(ServerPath);
            ApplicationConfiguration.Initialize();
            Application.Run(new clientForm());
        }
    }
}