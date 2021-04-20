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
        private const bool useGUI=true;
        public static void Main(string[] args)
        {
            if (useGUI)
            {
                var form = new MainForm();
                Application.Run(form);  
            }
            else
            {
               
                var proc = Process.GetProcessesByName("ucn")[0];
                var game = new RuntimeCTFGame(proc);
               

            }
        }

        

        

       
    }
}