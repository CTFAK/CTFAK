using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace DotNetCTFDumper.GUI
{
    public partial class MainConsole : Form
    {
        public static MainConsole inst;
        public MainConsole()
        {
            inst = this;
            InitializeComponent();
            this.Closing += (a,b) =>
            {
                Environment.Exit(0);
            };
        }

        public static void Message(string msg)
        {
            var date = DateTime.Now;
            inst.textBox1.AppendText(msg.Length > 0
                ? $"[{date.Hour,2}:{date.Minute,2}:{date.Second,2}:{date.Millisecond,3}] {msg}\r\n"
                : "\r\n");
        }
    }
}