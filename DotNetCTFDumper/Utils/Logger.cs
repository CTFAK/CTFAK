using System;
using System.IO;
using System.Web.UI.WebControls;

namespace DotNetCTFDumper.Utils
{
    public static class Logger
    {
        static StreamWriter _writer;
        public static void Log(string text, bool logToScreen = true,ConsoleColor color = ConsoleColor.White)
        {
            if (_writer == null)
            {
                File.Delete("Dump.log");
                _writer = new StreamWriter("Dump.log", true);
                _writer.AutoFlush = true;

            }
            _writer.WriteLine(text);
            
            if (logToScreen)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ForegroundColor = ConsoleColor.White;
            }


        }
    }
}
