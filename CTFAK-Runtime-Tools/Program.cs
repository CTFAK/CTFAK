using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders.Events;
using CTFAK.Utils;
using CTFAK_Runtime_Tools.IO;
using CTFAK_Runtime_Tools.RuntimeParsers;

namespace CTFAK_Runtime_Tools
{
    internal class Program
    {
        private const bool useGUI=false;
        public static void Main(string[] args)
        {
            if (useGUI)
            {
                var form = new MainForm();
                Application.Run(form);  
            }
            else
            {
                // var startInfo = new ProcessStartInfo()
                // {
                    // FileName = @"E:\Application 1.exe"
                // };
                // ProcDumpParser.Init(@"C:\Users\ivani\Desktop\fnaf.DMP");
                var proc = Process.GetProcessesByName("Application 1")[0];//opening process by name
                var runtimeStream = new RuntimeStream(proc);//creating my new stream
                var runtimeReader = new ByteReader(runtimeStream);//create an instance of my advanced reader
                var runtimeWriter = new ByteWriter(runtimeStream);//create an instance of my advanced writer
                var newOffset = FindPAMUAddr(proc, runtimeReader);
                Console.WriteLine("PAMU OFFSET: "+newOffset.ToString("X2"));
               runtimeReader.Seek(newOffset); 
               var newData = new RuntimeGameData();
               newData.Read(runtimeReader);
               
            }
        }

        public static int FindPAMUAddr(Process proc, ByteReader reader)
        {
            const int magicOffset = 0x000AC9AC;
            var procBase = proc.Modules[0].BaseAddress;
            var address = IntPtr.Add(procBase, magicOffset);
            reader.Seek(address.ToInt32());
            return reader.ReadInt32();
        }

       
    }
}