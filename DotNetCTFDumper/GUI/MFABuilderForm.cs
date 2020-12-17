using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DotNetCTFDumper.MMFParser.Translation;

namespace DotNetCTFDumper.GUI
{
    public partial class MFABuilderForm : Form
    {
        public MFABuilderForm(Color color)
        {
            InitializeComponent();
            foreach (Control control in Controls)
            {
                control.BackColor=Color.Black;
                control.ForeColor = color;
                if (control is Button)
                {
                    control.BackColor=Color.FromArgb(30,30,30);
                }
            }
            Pame2Mfa.TranslatingFrame += OnFrameTranslation;

        }

        public void OnFrameTranslation(object name)
        {
            logBox.AppendText($"Translating Frame: {name}\r\n");
        }

        private void DumpButton_Click(object sender, EventArgs e)
        {
            MFAGenerator.BuildMFA();
        }
    }
}