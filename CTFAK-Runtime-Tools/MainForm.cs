using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace CTFAK_Runtime_Tools
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void updateProcesses_Click(object sender, EventArgs e)
        {
            procList.Items.Clear();
            var procs = Process.GetProcesses();
            foreach (Process process in procs)
            {
                try
                {
                    procList.Items.Add($"{process.ProcessName}-{process.Id}");
                }
                catch
                {
                    
                }
            }
        }

        private void testBtn1_Click(object sender, EventArgs e)
        {
            // Native.OpenProcess(0x0020, false, Process.GetProcessesByName(procList.SelectedItem.ToString())[0].Id);
        }
    }
}