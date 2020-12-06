using System.ComponentModel;

namespace NetMFAPatcher.GUI
{
    partial class PackDataForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackDataForm));
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.dumpButton = new System.Windows.Forms.Button();
            this.dumpAllButton = new System.Windows.Forms.Button();
            this.infoLabel = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.SystemColors.WindowText;
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.listBox1.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {"PackFile1", "PackFile2", "PackFile3", "PackFile4", "PackFile5", "PackFile6"});
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(198, 390);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // dumpButton
            // 
            this.dumpButton.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.dumpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dumpButton.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.dumpButton.Location = new System.Drawing.Point(204, 105);
            this.dumpButton.Name = "dumpButton";
            this.dumpButton.Size = new System.Drawing.Size(143, 37);
            this.dumpButton.TabIndex = 1;
            this.dumpButton.Text = "Dump";
            this.dumpButton.UseVisualStyleBackColor = false;
            this.dumpButton.Click += new System.EventHandler(this.dumpButton_Click);
            // 
            // dumpAllButton
            // 
            this.dumpAllButton.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.dumpAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dumpAllButton.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.dumpAllButton.Location = new System.Drawing.Point(204, 148);
            this.dumpAllButton.Name = "dumpAllButton";
            this.dumpAllButton.Size = new System.Drawing.Size(143, 37);
            this.dumpAllButton.TabIndex = 2;
            this.dumpAllButton.Text = "Dump All";
            this.dumpAllButton.UseVisualStyleBackColor = false;
            this.dumpAllButton.Click += new System.EventHandler(this.dumpAllButton_Click);
            // 
            // infoLabel
            // 
            this.infoLabel.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.infoLabel.Location = new System.Drawing.Point(204, 14);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(142, 91);
            this.infoLabel.TabIndex = 3;
            this.infoLabel.Text = "Name: PackFile1.mvx\r\nSize: 5 MB\r\n";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.InitialDirectory = "C:\\Windows";
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // PackDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(646, 390);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.dumpAllButton);
            this.Controls.Add(this.dumpButton);
            this.Controls.Add(this.listBox1);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Name = "PackDataForm";
            this.Text = "Pack Files";
            this.Load += new System.EventHandler(this.HexViewForm_Load);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.SaveFileDialog saveFileDialog1;

        private System.Windows.Forms.Label infoLabel;

        private System.Windows.Forms.Button dumpAllButton;
        private System.Windows.Forms.Button dumpButton;

        private System.Windows.Forms.ListBox listBox1;

        #endregion
    }
}