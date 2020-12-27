using System;
using System.IO;
using System.Web.UI.WebControls;
using DotNetCTFDumper.GUI;

namespace DotNetCTFDumper.Utils
{
    public static class Logger
    {
        static StreamWriter _writer;
        public static void Log(string text, bool logToScreen = true,ConsoleColor color = ConsoleColor.White, bool logToConsole=true)
        {
            if (_writer == null)
            {
                File.Delete("NewLog.log");
                _writer = new StreamWriter("NewLog.log", true);
                _writer.AutoFlush = true;

            }
            _writer.WriteLine(Helper.GetCurrentTime()+ text);
            
            if (logToScreen)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(Helper.GetCurrentTime()+text);
                Console.ForegroundColor = ConsoleColor.White;
            }
            if(logToConsole) MainConsole.Message(text);
                


        }
    }
}
