using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace DotNetCTFDumper.GUI
{
    public partial class ErrorLogBox : Form
    {
        public ErrorLogBox(Exception e)
        {
            InitializeComponent();
            textBox1.Text += $"{e.Message}\r\n\r\n\r\n";
            StackTrace st = new StackTrace(true);
            string stackIndent = "";
            for(int i =0; i< st.FrameCount; i++ )
            {
                // Note that at this level, there are four
                // stack frames, one for each method invocation.
                StackFrame sf = st.GetFrame(i);
                var filename = Path.GetFileNameWithoutExtension(sf.GetFileName());
                if (filename == null) filename = "UnknownFile";
                textBox1.Text +=
                    $" {(filename)} : {sf.GetMethod()}: Line {sf.GetFileLineNumber()}\r\n\r\n";
                    stackIndent += "  ";
            }

            //Console.ReadKey();
        }
    }
}