using System;
using System.Windows.Forms;

namespace NetMFAPatcher.GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.GameInfo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ImageBox = new System.Windows.Forms.CheckBox();
            this.SoundBox = new System.Windows.Forms.CheckBox();
            this.ChunkBox = new System.Windows.Forms.CheckBox();
            this.ImagesBar = new System.Windows.Forms.ProgressBar();
            this.ImagesLabel = new System.Windows.Forms.Label();
            this.SoundsLabel = new System.Windows.Forms.Label();
            this.SoundBar = new System.Windows.Forms.ProgressBar();
            this.FolderBTN = new System.Windows.Forms.Button();
            this.MFABtn = new System.Windows.Forms.Button();
            this.SoundsButton = new System.Windows.Forms.Button();
            this.ImagesButton = new System.Windows.Forms.Button();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.cryptKeyBtn = new System.Windows.Forms.Button();
            this.showHexBtn = new System.Windows.Forms.Button();
            this.dumpSortedBtn = new System.Windows.Forms.Button();
            this.ChunkCombo = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveChunkBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHexBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.packDataBtn = new System.Windows.Forms.Button();
            this.ChunkCombo.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Select File";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "fnaf3.exe";
            this.openFileDialog1.Filter = "CTF Executable|*.exe";
            this.openFileDialog1.InitialDirectory = "E:\\";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.BackColor = System.Drawing.Color.Black;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.treeView1.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.treeView1.Location = new System.Drawing.Point(642, 9);
            this.treeView1.Margin = new System.Windows.Forms.Padding(0);
            this.treeView1.MaximumSize = new System.Drawing.Size(286, 900);
            this.treeView1.MinimumSize = new System.Drawing.Size(286, 489);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(286, 489);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_AfterDblClick);
            this.treeView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeView1_RightClick);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.BackColor = System.Drawing.Color.Black;
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.listBox1.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.IntegralHeight = false;
            this.listBox1.ItemHeight = 14;
            this.listBox1.Location = new System.Drawing.Point(459, 12);
            this.listBox1.Margin = new System.Windows.Forms.Padding(0);
            this.listBox1.MaximumSize = new System.Drawing.Size(180, 50000);
            this.listBox1.MinimumSize = new System.Drawing.Size(180, 234);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(180, 281);
            this.listBox1.TabIndex = 5;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // GameInfo
            // 
            this.GameInfo.AutoSize = true;
            this.GameInfo.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.GameInfo.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.GameInfo.Location = new System.Drawing.Point(9, 55);
            this.GameInfo.Margin = new System.Windows.Forms.Padding(5, 0, 3, 0);
            this.GameInfo.Name = "GameInfo";
            this.GameInfo.Size = new System.Drawing.Size(182, 45);
            this.GameInfo.TabIndex = 3;
            this.GameInfo.Text = "GameInfo will appear here\r\nSemenLine\r\nLine3\r\n";
            this.GameInfo.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.label1.Location = new System.Drawing.Point(119, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "CTFDumper 0.1.1 Debug";
            // 
            // ImageBox
            // 
            this.ImageBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ImageBox.ForeColor = System.Drawing.Color.Lime;
            this.ImageBox.Location = new System.Drawing.Point(9, 239);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(104, 24);
            this.ImageBox.TabIndex = 5;
            this.ImageBox.Text = "Dump Images";
            this.ImageBox.UseVisualStyleBackColor = true;
            this.ImageBox.Visible = false;
            this.ImageBox.CheckedChanged += new System.EventHandler(this.ImageBox_CheckedChanged);
            // 
            // SoundBox
            // 
            this.SoundBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SoundBox.ForeColor = System.Drawing.Color.Lime;
            this.SoundBox.Location = new System.Drawing.Point(9, 269);
            this.SoundBox.Name = "SoundBox";
            this.SoundBox.Size = new System.Drawing.Size(104, 24);
            this.SoundBox.TabIndex = 6;
            this.SoundBox.Text = "Dump Sounds";
            this.SoundBox.UseVisualStyleBackColor = true;
            this.SoundBox.Visible = false;
            this.SoundBox.CheckedChanged += new System.EventHandler(this.SoundBox_CheckedChanged);
            // 
            // ChunkBox
            // 
            this.ChunkBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChunkBox.ForeColor = System.Drawing.Color.Lime;
            this.ChunkBox.Location = new System.Drawing.Point(9, 299);
            this.ChunkBox.Name = "ChunkBox";
            this.ChunkBox.Size = new System.Drawing.Size(104, 24);
            this.ChunkBox.TabIndex = 7;
            this.ChunkBox.Text = "Export Chunks\r\n";
            this.ChunkBox.UseVisualStyleBackColor = true;
            this.ChunkBox.Visible = false;
            this.ChunkBox.CheckedChanged += new System.EventHandler(this.ChunkBox_CheckedChanged);
            // 
            // ImagesBar
            // 
            this.ImagesBar.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImagesBar.BackColor = System.Drawing.Color.Black;
            this.ImagesBar.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.ImagesBar.Location = new System.Drawing.Point(190, 367);
            this.ImagesBar.Name = "ImagesBar";
            this.ImagesBar.Size = new System.Drawing.Size(126, 23);
            this.ImagesBar.Step = 2;
            this.ImagesBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ImagesBar.TabIndex = 8;
            this.ImagesBar.Visible = false;
            // 
            // ImagesLabel
            // 
            this.ImagesLabel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImagesLabel.BackColor = System.Drawing.Color.Transparent;
            this.ImagesLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ImagesLabel.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.ImagesLabel.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.ImagesLabel.Location = new System.Drawing.Point(322, 367);
            this.ImagesLabel.Name = "ImagesLabel";
            this.ImagesLabel.Size = new System.Drawing.Size(126, 24);
            this.ImagesLabel.TabIndex = 9;
            this.ImagesLabel.Text = "0/0";
            this.ImagesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ImagesLabel.Visible = false;
            // 
            // SoundsLabel
            // 
            this.SoundsLabel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SoundsLabel.BackColor = System.Drawing.Color.Black;
            this.SoundsLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SoundsLabel.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.SoundsLabel.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.SoundsLabel.Location = new System.Drawing.Point(322, 415);
            this.SoundsLabel.Name = "SoundsLabel";
            this.SoundsLabel.Size = new System.Drawing.Size(126, 24);
            this.SoundsLabel.TabIndex = 11;
            this.SoundsLabel.Text = "0/0";
            this.SoundsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SoundsLabel.Visible = false;
            // 
            // SoundBar
            // 
            this.SoundBar.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SoundBar.BackColor = System.Drawing.Color.Black;
            this.SoundBar.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.SoundBar.Location = new System.Drawing.Point(190, 415);
            this.SoundBar.Name = "SoundBar";
            this.SoundBar.Size = new System.Drawing.Size(126, 23);
            this.SoundBar.Step = 2;
            this.SoundBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.SoundBar.TabIndex = 10;
            this.SoundBar.Visible = false;
            // 
            // FolderBTN
            // 
            this.FolderBTN.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FolderBTN.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.FolderBTN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FolderBTN.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.FolderBTN.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.FolderBTN.Location = new System.Drawing.Point(190, 453);
            this.FolderBTN.Name = "FolderBTN";
            this.FolderBTN.Size = new System.Drawing.Size(83, 42);
            this.FolderBTN.TabIndex = 12;
            this.FolderBTN.Text = "Open Dump Folder";
            this.FolderBTN.UseVisualStyleBackColor = false;
            this.FolderBTN.Visible = false;
            this.FolderBTN.Click += new System.EventHandler(this.FolderBTN_Click);
            // 
            // MFABtn
            // 
            this.MFABtn.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MFABtn.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.MFABtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MFABtn.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.MFABtn.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.MFABtn.Location = new System.Drawing.Point(101, 453);
            this.MFABtn.Name = "MFABtn";
            this.MFABtn.Size = new System.Drawing.Size(83, 42);
            this.MFABtn.TabIndex = 13;
            this.MFABtn.Text = "Generate MFA";
            this.MFABtn.UseVisualStyleBackColor = false;
            this.MFABtn.Visible = false;
            this.MFABtn.Click += new System.EventHandler(this.MFABtn_Click);
            // 
            // SoundsButton
            // 
            this.SoundsButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SoundsButton.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.SoundsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SoundsButton.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.SoundsButton.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.SoundsButton.Location = new System.Drawing.Point(101, 405);
            this.SoundsButton.Name = "SoundsButton";
            this.SoundsButton.Size = new System.Drawing.Size(83, 42);
            this.SoundsButton.TabIndex = 14;
            this.SoundsButton.Text = "Dump Sounds";
            this.SoundsButton.UseVisualStyleBackColor = false;
            this.SoundsButton.Visible = false;
            this.SoundsButton.Click += new System.EventHandler(this.SoundsButton_Click);
            // 
            // ImagesButton
            // 
            this.ImagesButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImagesButton.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.ImagesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ImagesButton.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.ImagesButton.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.ImagesButton.Location = new System.Drawing.Point(101, 357);
            this.ImagesButton.Name = "ImagesButton";
            this.ImagesButton.Size = new System.Drawing.Size(83, 42);
            this.ImagesButton.TabIndex = 15;
            this.ImagesButton.Text = "Dump\r\nImages";
            this.ImagesButton.UseVisualStyleBackColor = false;
            this.ImagesButton.Visible = false;
            this.ImagesButton.Click += new System.EventHandler(this.ImagesButton_Click);
            // 
            // loadingLabel
            // 
            this.loadingLabel.BackColor = System.Drawing.Color.Transparent;
            this.loadingLabel.Font = new System.Drawing.Font("Courier New", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.loadingLabel.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.loadingLabel.Location = new System.Drawing.Point(119, 12);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(335, 91);
            this.loadingLabel.TabIndex = 16;
            this.loadingLabel.Text = "Loading...";
            this.loadingLabel.Click += new System.EventHandler(this.loadingLabel_Click);
            // 
            // cryptKeyBtn
            // 
            this.cryptKeyBtn.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cryptKeyBtn.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.cryptKeyBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cryptKeyBtn.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.cryptKeyBtn.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.cryptKeyBtn.Location = new System.Drawing.Point(12, 405);
            this.cryptKeyBtn.Name = "cryptKeyBtn";
            this.cryptKeyBtn.Size = new System.Drawing.Size(83, 42);
            this.cryptKeyBtn.TabIndex = 17;
            this.cryptKeyBtn.Text = "Crypto Key";
            this.cryptKeyBtn.UseVisualStyleBackColor = false;
            this.cryptKeyBtn.Visible = false;
            this.cryptKeyBtn.Click += new System.EventHandler(this.cryptKeyBtn_Click);
            // 
            // showHexBtn
            // 
            this.showHexBtn.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showHexBtn.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.showHexBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showHexBtn.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.showHexBtn.Location = new System.Drawing.Point(-171, 191);
            this.showHexBtn.Name = "showHexBtn";
            this.showHexBtn.Size = new System.Drawing.Size(180, 29);
            this.showHexBtn.TabIndex = 18;
            this.showHexBtn.Text = "Show hex";
            this.showHexBtn.UseVisualStyleBackColor = false;
            this.showHexBtn.Visible = false;
            this.showHexBtn.Click += new System.EventHandler(this.ShowHex_Click);
            // 
            // dumpSortedBtn
            // 
            this.dumpSortedBtn.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dumpSortedBtn.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.dumpSortedBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dumpSortedBtn.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.dumpSortedBtn.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.dumpSortedBtn.Location = new System.Drawing.Point(12, 453);
            this.dumpSortedBtn.Name = "dumpSortedBtn";
            this.dumpSortedBtn.Size = new System.Drawing.Size(83, 42);
            this.dumpSortedBtn.TabIndex = 19;
            this.dumpSortedBtn.Text = "Dump Sorted";
            this.dumpSortedBtn.UseVisualStyleBackColor = false;
            this.dumpSortedBtn.Visible = false;
            this.dumpSortedBtn.Click += new System.EventHandler(this.dumpSortedBtn_Click);
            // 
            // ChunkCombo
            // 
            this.ChunkCombo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.saveChunkBtn, this.viewHexBtn});
            this.ChunkCombo.Name = "Save";
            this.ChunkCombo.Size = new System.Drawing.Size(124, 48);
            this.ChunkCombo.Opening += new System.ComponentModel.CancelEventHandler(this.ChunkCombo_Opening);
            this.ChunkCombo.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ChunkCombo_ItemSelected);
            // 
            // saveChunkBtn
            // 
            this.saveChunkBtn.Name = "saveChunkBtn";
            this.saveChunkBtn.Size = new System.Drawing.Size(123, 22);
            this.saveChunkBtn.Text = "Save";
            // 
            // viewHexBtn
            // 
            this.viewHexBtn.Name = "viewHexBtn";
            this.viewHexBtn.Size = new System.Drawing.Size(123, 22);
            this.viewHexBtn.Text = "View Hex";
            // 
            // packDataBtn
            // 
            this.packDataBtn.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.packDataBtn.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.packDataBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.packDataBtn.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.packDataBtn.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.packDataBtn.Location = new System.Drawing.Point(12, 357);
            this.packDataBtn.Name = "packDataBtn";
            this.packDataBtn.Size = new System.Drawing.Size(83, 42);
            this.packDataBtn.TabIndex = 20;
            this.packDataBtn.Text = "Pack Data";
            this.packDataBtn.UseVisualStyleBackColor = false;
            this.packDataBtn.Visible = false;
            this.packDataBtn.Click += new System.EventHandler(this.packDataBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(943, 507);
            this.Controls.Add(this.packDataBtn);
            this.Controls.Add(this.dumpSortedBtn);
            this.Controls.Add(this.showHexBtn);
            this.Controls.Add(this.cryptKeyBtn);
            this.Controls.Add(this.loadingLabel);
            this.Controls.Add(this.ImagesButton);
            this.Controls.Add(this.SoundsButton);
            this.Controls.Add(this.MFABtn);
            this.Controls.Add(this.FolderBTN);
            this.Controls.Add(this.SoundsLabel);
            this.Controls.Add(this.SoundBar);
            this.Controls.Add(this.ImagesLabel);
            this.Controls.Add(this.ImagesBar);
            this.Controls.Add(this.ChunkBox);
            this.Controls.Add(this.SoundBox);
            this.Controls.Add(this.ImageBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GameInfo);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 15);
            this.Name = "MainForm";
            this.Text = "DotNetCTFDumper";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ChunkCombo.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button packDataBtn;

        private System.Windows.Forms.ToolStripMenuItem saveChunkBtn;
        private System.Windows.Forms.ToolStripMenuItem viewHexBtn;

        private System.Windows.Forms.ContextMenuStrip ChunkCombo;

        private System.Windows.Forms.Button dumpSortedBtn;

        private System.Windows.Forms.Button showHexBtn;

        private System.Windows.Forms.Button cryptKeyBtn;

        private System.Windows.Forms.Label loadingLabel;

        private System.Windows.Forms.Button ImagesButton;

        private System.Windows.Forms.Button SoundsButton;

        private System.Windows.Forms.Button FolderBTN;
        private System.Windows.Forms.Button MFABtn;

        private System.Windows.Forms.ProgressBar SoundBar;
        private System.Windows.Forms.Label SoundsLabel;

        private System.Windows.Forms.Label ImagesLabel;

        private System.Windows.Forms.ProgressBar ImagesBar;

        private System.Windows.Forms.CheckBox ChunkBox;
        private System.Windows.Forms.CheckBox ImageBox;
        private System.Windows.Forms.CheckBox SoundBox;

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label GameInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TreeView treeView1;

        #endregion
    }
}