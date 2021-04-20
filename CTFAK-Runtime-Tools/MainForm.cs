using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using CTFAK_Runtime_Tools.RuntimeParsers;

namespace CTFAK_Runtime_Tools
{
    public partial class MainForm : Form
    {
        private List<Process> _processes = new List<Process>();
        public Timer updateDataTimer = new Timer();
        public RuntimeCTFGame NewGame;

        public MainForm()
        {
            InitializeComponent();
            currentFrameInfo.Text = "";
            updateDataTimer.Interval = 500;
            updateDataTimer.Tick += new EventHandler(updateData);
            
            
        }

        private void updateProcesses_Click(object sender, EventArgs e)
        {
            procList.Items.Clear();
            UpdateProcs();
        }

        private void testBtn1_Click(object sender, EventArgs e)
        {
            NewGame = new RuntimeCTFGame(Process.GetProcessesByName(procList.SelectedItem.ToString())[0]);
            NewGame.ProcessAttached += (object data) => { Message("Attaching reader to process");};
            NewGame.DataReadingFinished += (object data) =>
            {
                Message("Basic GameData reading finished");
                this.Text = "CTFAK-RUNTIME: " + (data as RuntimeGameData)?.Name.Value;
            };
            
            NewGame.Read();
            updateDataTimer.Enabled = true;
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            procList.Items.Clear();
            foreach (Process process in _processes)
            {
                if (process.ProcessName.Contains(searchBox.Text))
                {
                    try
                    {
                        procList.Items.Add($"{process.ProcessName}");
                    }
                    catch{}

                }
            }
        }

        private void UpdateProcs()
        {
            var procs = Process.GetProcesses();
            _processes.Clear();
            foreach (Process process in procs)
            {
                try
                {
                    procList.Items.Add($"{process.ProcessName}");
                    _processes.Add(process);
                }
                catch
                {
                    
                }
            }
        }

        void updateData(object sender, EventArgs eventArgs)
        {
            var frame = NewGame.GetCurrentFrame();
            currentFrameInfo.Text = "Current Frame:\r\n";
            currentFrameInfo.Text += $"Name: {frame.Name}\r\n";
            currentFrameInfo.Text += $"Size: {frame.Width}x{frame.Height}\r\n";
        }

        public void Message(object msg)
        {
            msg = CTFAK.Utils.Helper.GetCurrentTime() + " " + msg;
            Console.WriteLine(msg);
            richTextBox1.AppendText($"{msg}\r\n");
            richTextBox1.ScrollToCaret();
        }
    }
}