using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.GUI
{
    public partial class PackDataForm : Form
    {
        private PackData data;
        public PackDataForm(PackData data,Color color)
        {
            InitializeComponent();
            listBox1.ForeColor = color;
            dumpButton.ForeColor = color;
            dumpAllButton.ForeColor = color;
            infoLabel.ForeColor = color;
            this.data = data;


        }

        private void HexViewForm_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (var item in data.Items)
            {
                listBox1.Items.Add(item.PackFilename);
            }
            UpdateInfo(0);
            
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInfo(listBox1.SelectedIndex);
        }

        private void UpdateInfo(int index)
        {
            var item = data.Items[index];
            infoLabel.Text = $"Name: {item.PackFilename}\nSize: {item.Data.Length.ToPrettySize()}";
        }

        private void dumpButton_Click(object sender, EventArgs e)
        {
            var item = data.Items[listBox1.SelectedIndex];

            saveFileDialog1.FileName = item.PackFilename;
            if (item.PackFilename.EndsWith(".mfx")) saveFileDialog1.Filter = "Clickteam Extension(*.mfx)|.mfx";
            else if (item.PackFilename.EndsWith(".dll")) saveFileDialog1.Filter = "Clickteam Module(*.dll)|.dll";

            
            saveFileDialog1.InitialDirectory = Path.GetFullPath(Settings.ExtensionPath);
            saveFileDialog1.ShowDialog();

        }

        private void dumpAllButton_Click(object sender, EventArgs e)
        {
            foreach (var item in data.Items)
            {
                item.Dump();
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var item = data.Items[listBox1.SelectedIndex];
            item.Dump(saveFileDialog1.FileName);
            
        }
    }

    
    
}