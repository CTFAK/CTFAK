using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Launcher
{
    public partial class Form1 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();
            if (File.Exists(dataFilename)) LoadSettings();
             MouseDown += (o, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };
            FormClosing += (a,b) =>
            {
                SaveSettings();
            };
            KeyDown += (o, e) =>
              {
                  if (e.KeyCode == Keys.Escape)
                  {
                      SaveSettings();
                      Environment.Exit(0);
                  }

              };
        }
        

        private void launchButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
            bool success = NativeMethods.CreateProcess(gameName.Text, null,
                IntPtr.Zero, IntPtr.Zero, false,
                ProcessCreationFlags.CREATE_SUSPENDED,
                IntPtr.Zero, null, ref si, out pi);
            Injector.Inject((uint)pi.dwProcessId, "ModLoader\\CTFAK.Modloader.dll");
            Thread.Sleep(200);
            
            NativeMethods.ResumeThread(pi.hThread);
            

            
            Environment.Exit(0);
        }
        const string dataFilename = "ModLoader\\launcher.dat";
        void SaveSettings()
        {
            Settings c = new Settings();
            c.gameName = gameName.Text;
            FileStream f = File.Create(dataFilename);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(f, c);
            f.Close();
        }
        void LoadSettings()
        {
            Settings c = new Settings();
            Stream s = File.Open(dataFilename,FileMode.Open);
            BinaryFormatter b = new BinaryFormatter();
            c = (Settings)b.Deserialize(s);
            gameName.Text = c.gameName;
            s.Close();
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Environment.Exit(0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
    [Serializable]
    public class Settings
    {
        public string gameName;
        public bool autoStart;
    }
}
