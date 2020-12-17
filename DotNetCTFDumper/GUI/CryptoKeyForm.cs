using System;
using System.Drawing;
using System.Windows.Forms;
using Be.Windows.Forms;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.GUI
{
    public partial class CryptoKeyForm : Form
    {
        private string rawKey;
        public CryptoKeyForm(string data,Color color)
        {
            InitializeComponent();
            hexBox1.ForeColor = color;
            hexBox1.InfoForeColor = Color.FromArgb(color.R/2, color.G/2, color.B/2);
            hexBox1.SelectionForeColor=Color.FromArgb(color.R, color.G, color.B);
            hexBox1.SelectionBackColor=Color.FromArgb(color.R/4, color.G/4, color.B/4);
            hexBox1.ShadowSelectionColor=Color.FromArgb(150,color.R/4, color.G/4, color.B/4);
            plusButton.ForeColor = color;
            minusButton.ForeColor = color;
            charBox.ForeColor = color;
            rawKey = data;
        }

        private void CryptoKeyForm_Load(object sender, EventArgs e)
        {
            try
            {
                var previewKey = Decryption.MakeKeyFromBytes(rawKey, (byte) int.Parse((charBox.Text)));
                hexBox1.ByteProvider=new DynamicByteProvider(previewKey);
                
            }
            catch
            {
                // ignored
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var previewKey = Decryption.MakeKeyFromBytes(rawKey, (byte) int.Parse((charBox.Text)));
                hexBox1.ByteProvider=new DynamicByteProvider(previewKey);
                
            }
            catch
            {
                
            }
        }

        private void plusButton_Click(object sender, EventArgs e)
        {
            charBox.Text = ((byte)int.Parse((charBox.Text))+1).ToString();
        }

        private void minusButton_Click(object sender, EventArgs e)
        {
            charBox.Text = ((byte)int.Parse((charBox.Text))-1).ToString();
        }
    }

    
    
}