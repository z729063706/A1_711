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
            ApplicationConfiguration.Initialize();
            Application.Run(new clientForm());
        }
    }
}