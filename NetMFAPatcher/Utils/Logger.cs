using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.Utils
{
    public static class Logger
    {
        static StreamWriter writer;
        public static void Log(string text, bool logToScreen = true,ConsoleColor color = ConsoleColor.White)
        {
            if (writer == null)
            {
                File.Delete("Dump.log");
                writer = new StreamWriter("Dump.log", true);
                writer.AutoFlush = true;

            }
            writer.WriteLine(text);
            
            if (logToScreen)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ForegroundColor = ConsoleColor.White;

            }

        }
    }
}
