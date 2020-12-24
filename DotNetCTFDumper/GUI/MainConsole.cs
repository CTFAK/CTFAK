using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
            textBox1.ForeColor = MainForm.ColorTheme;
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            setLineFormat(1,1);
            textBox1.SelectionStart = textBox1.Text.Length;
            // scroll it automatically
            textBox1.ScrollToCaret();
        }
        
        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref PARAFORMAT lParam);
        const int PFM_SPACEBEFORE = 0x00000040;
        const int PFM_SPACEAFTER  = 0x00000080;
        const int PFM_LINESPACING = 0x00000100;
        const int SCF_SELECTION = 1;
        const int EM_SETPARAFORMAT = 1095;
        private void setLineFormat(byte rule, int space)
        {
            PARAFORMAT fmt = new PARAFORMAT();
            fmt.cbSize = Marshal.SizeOf(fmt);
            fmt.dwMask = PFM_LINESPACING;
            fmt.dyLineSpacing = space;
            fmt.bLineSpacingRule = rule;
            textBox1.SelectAll();
            SendMessage( new HandleRef( textBox1, textBox1.Handle ),
                EM_SETPARAFORMAT,
                SCF_SELECTION,
                ref fmt
            );
        }
        public struct PARAFORMAT
        {
            public int cbSize;
            public uint dwMask;
            public short wNumbering;
            public short wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 32 )]
            public int[] rgxTabs;
            // PARAFORMAT2 from here onwards
            public int dySpaceBefore;
            public int dySpaceAfter;
            public int dyLineSpacing;
            public short sStyle;
            public byte bLineSpacingRule;
            public byte bOutlineLevel;
            public short wShadingWeight;
            public short wShadingStyle;
            public short wNumberingStart;
            public short wNumberingStyle;
            public short wNumberingTab;
            public short wBorderSpace;
            public short wBorderWidth;
            public short wBorders;
        }
    }
}