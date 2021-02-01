namespace CTFAK.GUI
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
            this.imageBar = new System.Windows.Forms.ProgressBar();
            this.imageLabel = new System.Windows.Forms.Label();
            this.soundLabel = new System.Windows.Forms.Label();
            this.soundBar = new System.Windows.Forms.ProgressBar();
            this.FolderBTN = new System.Windows.Forms.Button();
            this.soundsButton = new System.Windows.Forms.Button();
            this.imagesButton = new System.Windows.Forms.Button();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.ChunkCombo = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveChunkBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHexBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.previewFrameBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.musicsButton = new System.Windows.Forms.Button();
            this.musicBar = new System.Windows.Forms.ProgressBar();
            this.musicLabel = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.mainTab = new System.Windows.Forms.TabPage();
            this.mfaTab = new System.Windows.Forms.TabPage();
            this.dumpMFAButton = new System.Windows.Forms.Button();
            this.mfaLogBox = new System.Windows.Forms.TextBox();
            this.packDataTab = new System.Windows.Forms.TabPage();
            this.infoLabel = new System.Windows.Forms.Label();
            this.dumpAllPackButton = new System.Windows.Forms.Button();
            this.dumpPackButton = new System.Windows.Forms.Button();
            this.packDataListBox = new System.Windows.Forms.ListBox();
            this.objViewerTab = new System.Windows.Forms.TabPage();
            this.dumpSelectedBtn = new System.Windows.Forms.Button();
            this.objViewerInfo = new System.Windows.Forms.Label();
            this.imageViewerPlayAnim = new System.Windows.Forms.Button();
            this.imageViewPictureBox = new System.Windows.Forms.PictureBox();
            this.objTreeView = new System.Windows.Forms.TreeView();
            this.soundViewTab = new System.Windows.Forms.TabPage();
            this.stopSoundBtn = new System.Windows.Forms.Button();
            this.soundList = new System.Windows.Forms.TreeView();
            this.playSoundBtn = new System.Windows.Forms.Button();
            this.cryptKeyTab = new System.Windows.Forms.TabPage();
            this.minusCharButton = new System.Windows.Forms.Button();
            this.plusCharBtn = new System.Windows.Forms.Button();
            this.hexBox1 = new Be.Windows.Forms.HexBox();
            this.charBox = new System.Windows.Forms.TextBox();
            this.pluginTab = new System.Windows.Forms.TabPage();
            this.activatePluginBtn = new System.Windows.Forms.Button();
            this.pluginsList = new System.Windows.Forms.ListBox();
            this.settingsTab = new System.Windows.Forms.TabPage();
            this.langComboBox = new System.Windows.Forms.ComboBox();
            this.langLabel = new System.Windows.Forms.Label();
            this.colorLabel = new System.Windows.Forms.Label();
            this.updateSettings = new System.Windows.Forms.Button();
            this.colorBox = new System.Windows.Forms.TextBox();
            this.packDataDialog = new System.Windows.Forms.SaveFileDialog();
            this.ChunkCombo.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.mainTab.SuspendLayout();
            this.mfaTab.SuspendLayout();
            this.packDataTab.SuspendLayout();
            this.objViewerTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.imageViewPictureBox)).BeginInit();
            this.soundViewTab.SuspendLayout();
            this.cryptKeyTab.SuspendLayout();
            this.pluginTab.SuspendLayout();
            this.settingsTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.button1.Location = new System.Drawing.Point(8, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Select File";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "CTF Executable|*.exe|CTF Android|*.apk";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.Black;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Right;
            this.treeView1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.treeView1.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.treeView1.Location = new System.Drawing.Point(646, 3);
            this.treeView1.Margin = new System.Windows.Forms.Padding(0);
            this.treeView1.MaximumSize = new System.Drawing.Size(500, 900);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(286, 473);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_AfterDblClick);
            this.treeView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeView1_RightClick);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.Black;
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.listBox1.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.listBox1.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.IntegralHeight = false;
            this.listBox1.ItemHeight = 17;
            this.listBox1.Location = new System.Drawing.Point(346, 3);
            this.listBox1.Margin = new System.Windows.Forms.Padding(0);
            this.listBox1.MaximumSize = new System.Drawing.Size(300, 50000);
            this.listBox1.MinimumSize = new System.Drawing.Size(180, 234);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(300, 473);
            this.listBox1.TabIndex = 5;
            // 
            // GameInfo
            // 
            this.GameInfo.AutoSize = true;
            this.GameInfo.BackColor = System.Drawing.Color.Transparent;
            this.GameInfo.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.GameInfo.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.GameInfo.Location = new System.Drawing.Point(8, 36);
            this.GameInfo.Margin = new System.Windows.Forms.Padding(5, 0, 3, 0);
            this.GameInfo.Name = "GameInfo";
            this.GameInfo.Size = new System.Drawing.Size(182, 105);
            this.GameInfo.TabIndex = 3;
            this.GameInfo.Text = "GameInfo will appear here\r\nLine2\r\nLine3\r\nLine4\r\nLine5\r\nLine6\r\n\r\n";
            this.GameInfo.Visible = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.label1.Location = new System.Drawing.Point(6, 457);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "CTFDumper 0.1.1 Debug";
            // 
            // imageBar
            // 
            this.imageBar.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.imageBar.BackColor = System.Drawing.Color.Black;
            this.imageBar.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.imageBar.Location = new System.Drawing.Point(97, 326);
            this.imageBar.Name = "imageBar";
            this.imageBar.Size = new System.Drawing.Size(126, 24);
            this.imageBar.Step = 2;
            this.imageBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.imageBar.TabIndex = 8;
            this.imageBar.Visible = false;
            // 
            // imageLabel
            // 
            this.imageLabel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.imageLabel.BackColor = System.Drawing.Color.Transparent;
            this.imageLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.imageLabel.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.imageLabel.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.imageLabel.Location = new System.Drawing.Point(229, 326);
            this.imageLabel.Name = "imageLabel";
            this.imageLabel.Size = new System.Drawing.Size(126, 24);
            this.imageLabel.TabIndex = 9;
            this.imageLabel.Text = "0/0";
            this.imageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.imageLabel.Visible = false;
            // 
            // soundLabel
            // 
            this.soundLabel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.soundLabel.BackColor = System.Drawing.Color.Black;
            this.soundLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.soundLabel.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.soundLabel.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.soundLabel.Location = new System.Drawing.Point(229, 374);
            this.soundLabel.Name = "soundLabel";
            this.soundLabel.Size = new System.Drawing.Size(126, 24);
            this.soundLabel.TabIndex = 11;
            this.soundLabel.Text = "0/0";
            this.soundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.soundLabel.Visible = false;
            // 
            // soundBar
            // 
            this.soundBar.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.soundBar.BackColor = System.Drawing.Color.Black;
            this.soundBar.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.soundBar.Location = new System.Drawing.Point(97, 374);
            this.soundBar.Name = "soundBar";
            this.soundBar.Size = new System.Drawing.Size(126, 23);
            this.soundBar.Step = 2;
            this.soundBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.soundBar.TabIndex = 10;
            this.soundBar.Visible = false;
            // 
            // FolderBTN
            // 
            this.FolderBTN.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FolderBTN.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.FolderBTN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FolderBTN.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.FolderBTN.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.FolderBTN.Location = new System.Drawing.Point(8, 268);
            this.FolderBTN.Name = "FolderBTN";
            this.FolderBTN.Size = new System.Drawing.Size(83, 42);
            this.FolderBTN.TabIndex = 12;
            this.FolderBTN.Text = "Open Dump Folder";
            this.FolderBTN.UseVisualStyleBackColor = false;
            this.FolderBTN.Visible = false;
            this.FolderBTN.Click += new System.EventHandler(this.FolderBTN_Click);
            // 
            // soundsButton
            // 
            this.soundsButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.soundsButton.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.soundsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.soundsButton.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.soundsButton.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.soundsButton.Location = new System.Drawing.Point(8, 364);
            this.soundsButton.Name = "soundsButton";
            this.soundsButton.Size = new System.Drawing.Size(83, 42);
            this.soundsButton.TabIndex = 14;
            this.soundsButton.Text = "Dump Sounds";
            this.soundsButton.UseVisualStyleBackColor = false;
            this.soundsButton.Visible = false;
            this.soundsButton.Click += new System.EventHandler(this.soundsButton_Click);
            // 
            // imagesButton
            // 
            this.imagesButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.imagesButton.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.imagesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.imagesButton.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.imagesButton.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.imagesButton.Location = new System.Drawing.Point(8, 316);
            this.imagesButton.Name = "imagesButton";
            this.imagesButton.Size = new System.Drawing.Size(83, 42);
            this.imagesButton.TabIndex = 15;
            this.imagesButton.Text = "Dump\r\nImages";
            this.imagesButton.UseVisualStyleBackColor = false;
            this.imagesButton.Visible = false;
            this.imagesButton.Click += new System.EventHandler(this.imagesButton_Click);
            // 
            // loadingLabel
            // 
            this.loadingLabel.BackColor = System.Drawing.Color.Transparent;
            this.loadingLabel.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.loadingLabel.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.loadingLabel.Location = new System.Drawing.Point(4, 10);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(110, 23);
            this.loadingLabel.TabIndex = 16;
            this.loadingLabel.Text = "Loading...";
            // 
            // ChunkCombo
            // 
            this.ChunkCombo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {this.saveChunkBtn, this.viewHexBtn, this.previewFrameBtn});
            this.ChunkCombo.Name = "Save";
            this.ChunkCombo.Size = new System.Drawing.Size(152, 70);
            this.ChunkCombo.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ChunkCombo_ItemSelected);
            // 
            // saveChunkBtn
            // 
            this.saveChunkBtn.Name = "saveChunkBtn";
            this.saveChunkBtn.Size = new System.Drawing.Size(151, 22);
            this.saveChunkBtn.Text = "Save";
            // 
            // viewHexBtn
            // 
            this.viewHexBtn.Name = "viewHexBtn";
            this.viewHexBtn.Size = new System.Drawing.Size(151, 22);
            this.viewHexBtn.Text = "View Hex";
            // 
            // previewFrameBtn
            // 
            this.previewFrameBtn.Name = "previewFrameBtn";
            this.previewFrameBtn.Size = new System.Drawing.Size(151, 22);
            this.previewFrameBtn.Text = "Preview Frame";
            // 
            // musicsButton
            // 
            this.musicsButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.musicsButton.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.musicsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.musicsButton.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.musicsButton.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.musicsButton.Location = new System.Drawing.Point(8, 412);
            this.musicsButton.Name = "musicsButton";
            this.musicsButton.Size = new System.Drawing.Size(83, 42);
            this.musicsButton.TabIndex = 21;
            this.musicsButton.Text = "Dump Musics";
            this.musicsButton.UseVisualStyleBackColor = false;
            this.musicsButton.Visible = false;
            this.musicsButton.Click += new System.EventHandler(this.musicsButton_Click);
            // 
            // musicBar
            // 
            this.musicBar.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.musicBar.BackColor = System.Drawing.Color.Black;
            this.musicBar.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.musicBar.Location = new System.Drawing.Point(97, 422);
            this.musicBar.Name = "musicBar";
            this.musicBar.Size = new System.Drawing.Size(126, 23);
            this.musicBar.Step = 2;
            this.musicBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.musicBar.TabIndex = 22;
            this.musicBar.Visible = false;
            // 
            // musicLabel
            // 
            this.musicLabel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.musicLabel.BackColor = System.Drawing.Color.Black;
            this.musicLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.musicLabel.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.musicLabel.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.musicLabel.Location = new System.Drawing.Point(229, 422);
            this.musicLabel.Name = "musicLabel";
            this.musicLabel.Size = new System.Drawing.Size(126, 24);
            this.musicLabel.TabIndex = 23;
            this.musicLabel.Text = "0/0";
            this.musicLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.musicLabel.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.AllowDrop = true;
            this.tabControl1.Controls.Add(this.mainTab);
            this.tabControl1.Controls.Add(this.mfaTab);
            this.tabControl1.Controls.Add(this.packDataTab);
            this.tabControl1.Controls.Add(this.objViewerTab);
            this.tabControl1.Controls.Add(this.soundViewTab);
            this.tabControl1.Controls.Add(this.cryptKeyTab);
            this.tabControl1.Controls.Add(this.pluginTab);
            this.tabControl1.Controls.Add(this.settingsTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.tabControl1.HotTrack = true;
            this.tabControl1.ImeMode = System.Windows.Forms.ImeMode.On;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(943, 507);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.TabIndex = 25;
            // 
            // mainTab
            // 
            this.mainTab.BackColor = System.Drawing.Color.Black;
            this.mainTab.Controls.Add(this.imagesButton);
            this.mainTab.Controls.Add(this.listBox1);
            this.mainTab.Controls.Add(this.label1);
            this.mainTab.Controls.Add(this.loadingLabel);
            this.mainTab.Controls.Add(this.button1);
            this.mainTab.Controls.Add(this.imageBar);
            this.mainTab.Controls.Add(this.treeView1);
            this.mainTab.Controls.Add(this.musicLabel);
            this.mainTab.Controls.Add(this.imageLabel);
            this.mainTab.Controls.Add(this.GameInfo);
            this.mainTab.Controls.Add(this.musicBar);
            this.mainTab.Controls.Add(this.soundBar);
            this.mainTab.Controls.Add(this.musicsButton);
            this.mainTab.Controls.Add(this.soundLabel);
            this.mainTab.Controls.Add(this.FolderBTN);
            this.mainTab.Controls.Add(this.soundsButton);
            this.mainTab.Location = new System.Drawing.Point(4, 24);
            this.mainTab.Name = "mainTab";
            this.mainTab.Padding = new System.Windows.Forms.Padding(3);
            this.mainTab.Size = new System.Drawing.Size(935, 479);
            this.mainTab.TabIndex = 0;
            this.mainTab.Text = "Main";
            // 
            // mfaTab
            // 
            this.mfaTab.BackColor = System.Drawing.Color.Black;
            this.mfaTab.Controls.Add(this.dumpMFAButton);
            this.mfaTab.Controls.Add(this.mfaLogBox);
            this.mfaTab.Location = new System.Drawing.Point(4, 24);
            this.mfaTab.Name = "mfaTab";
            this.mfaTab.Padding = new System.Windows.Forms.Padding(3);
            this.mfaTab.Size = new System.Drawing.Size(935, 479);
            this.mfaTab.TabIndex = 2;
            this.mfaTab.Text = "MFA Dump";
            // 
            // dumpMFAButton
            // 
            this.dumpMFAButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dumpMFAButton.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.dumpMFAButton.ForeColor = System.Drawing.SystemColors.Desktop;
            this.dumpMFAButton.Location = new System.Drawing.Point(411, 3);
            this.dumpMFAButton.Name = "dumpMFAButton";
            this.dumpMFAButton.Size = new System.Drawing.Size(103, 61);
            this.dumpMFAButton.TabIndex = 1;
            this.dumpMFAButton.Text = "Dump MFA";
            this.dumpMFAButton.UseVisualStyleBackColor = true;
            this.dumpMFAButton.Click += new System.EventHandler(this.dumpMFAButton_Click);
            // 
            // mfaLogBox
            // 
            this.mfaLogBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.mfaLogBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.mfaLogBox.Location = new System.Drawing.Point(3, 3);
            this.mfaLogBox.MaxLength = 999999999;
            this.mfaLogBox.Multiline = true;
            this.mfaLogBox.Name = "mfaLogBox";
            this.mfaLogBox.ReadOnly = true;
            this.mfaLogBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mfaLogBox.Size = new System.Drawing.Size(405, 473);
            this.mfaLogBox.TabIndex = 0;
            this.mfaLogBox.Text = "MFA Generation is currently unstable\r\nUSE AT YOUR OWN RISK";
            // 
            // packDataTab
            // 
            this.packDataTab.BackColor = System.Drawing.Color.Black;
            this.packDataTab.Controls.Add(this.infoLabel);
            this.packDataTab.Controls.Add(this.dumpAllPackButton);
            this.packDataTab.Controls.Add(this.dumpPackButton);
            this.packDataTab.Controls.Add(this.packDataListBox);
            this.packDataTab.Location = new System.Drawing.Point(4, 24);
            this.packDataTab.Name = "packDataTab";
            this.packDataTab.Padding = new System.Windows.Forms.Padding(3);
            this.packDataTab.Size = new System.Drawing.Size(935, 479);
            this.packDataTab.TabIndex = 4;
            this.packDataTab.Text = "Pack Data";
            // 
            // infoLabel
            // 
            this.infoLabel.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.infoLabel.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.infoLabel.Location = new System.Drawing.Point(204, 11);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(255, 91);
            this.infoLabel.TabIndex = 7;
            this.infoLabel.Text = "Name: PackFile1.mvx\r\nSize: 5 MB\r\n";
            // 
            // dumpAllPackButton
            // 
            this.dumpAllPackButton.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.dumpAllPackButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dumpAllPackButton.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.dumpAllPackButton.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.dumpAllPackButton.Location = new System.Drawing.Point(204, 148);
            this.dumpAllPackButton.Name = "dumpAllPackButton";
            this.dumpAllPackButton.Size = new System.Drawing.Size(143, 37);
            this.dumpAllPackButton.TabIndex = 6;
            this.dumpAllPackButton.Text = "Dump All";
            this.dumpAllPackButton.UseVisualStyleBackColor = false;
            this.dumpAllPackButton.Click += new System.EventHandler(this.dumpAllPackButton_Click);
            // 
            // dumpPackButton
            // 
            this.dumpPackButton.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.dumpPackButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dumpPackButton.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.dumpPackButton.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.dumpPackButton.Location = new System.Drawing.Point(204, 105);
            this.dumpPackButton.Name = "dumpPackButton";
            this.dumpPackButton.Size = new System.Drawing.Size(143, 37);
            this.dumpPackButton.TabIndex = 5;
            this.dumpPackButton.Text = "Dump";
            this.dumpPackButton.UseVisualStyleBackColor = false;
            this.dumpPackButton.Click += new System.EventHandler(this.dumpPackButton_Click);
            // 
            // packDataListBox
            // 
            this.packDataListBox.BackColor = System.Drawing.SystemColors.WindowText;
            this.packDataListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.packDataListBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.packDataListBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.packDataListBox.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.packDataListBox.FormattingEnabled = true;
            this.packDataListBox.ItemHeight = 15;
            this.packDataListBox.Items.AddRange(new object[] {"PackFile1", "PackFile2", "PackFile3", "PackFile4", "PackFile5", "PackFile6"});
            this.packDataListBox.Location = new System.Drawing.Point(3, 3);
            this.packDataListBox.Name = "packDataListBox";
            this.packDataListBox.Size = new System.Drawing.Size(198, 473);
            this.packDataListBox.TabIndex = 4;
            this.packDataListBox.SelectedIndexChanged += new System.EventHandler(this.packDataListBox_SelectedIndexChanged);
            // 
            // objViewerTab
            // 
            this.objViewerTab.BackColor = System.Drawing.Color.Black;
            this.objViewerTab.Controls.Add(this.dumpSelectedBtn);
            this.objViewerTab.Controls.Add(this.objViewerInfo);
            this.objViewerTab.Controls.Add(this.imageViewerPlayAnim);
            this.objViewerTab.Controls.Add(this.imageViewPictureBox);
            this.objViewerTab.Controls.Add(this.objTreeView);
            this.objViewerTab.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.objViewerTab.Location = new System.Drawing.Point(4, 24);
            this.objViewerTab.Name = "objViewerTab";
            this.objViewerTab.Padding = new System.Windows.Forms.Padding(3);
            this.objViewerTab.Size = new System.Drawing.Size(935, 479);
            this.objViewerTab.TabIndex = 1;
            this.objViewerTab.Text = "Objects";
            // 
            // dumpSelectedBtn
            // 
            this.dumpSelectedBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dumpSelectedBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dumpSelectedBtn.Location = new System.Drawing.Point(201, 426);
            this.dumpSelectedBtn.Name = "dumpSelectedBtn";
            this.dumpSelectedBtn.Size = new System.Drawing.Size(731, 25);
            this.dumpSelectedBtn.TabIndex = 4;
            this.dumpSelectedBtn.Text = "Dump Selected";
            this.dumpSelectedBtn.UseVisualStyleBackColor = true;
            // 
            // objViewerInfo
            // 
            this.objViewerInfo.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.objViewerInfo.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.objViewerInfo.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.objViewerInfo.Location = new System.Drawing.Point(623, 3);
            this.objViewerInfo.Name = "objViewerInfo";
            this.objViewerInfo.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.objViewerInfo.Size = new System.Drawing.Size(309, 63);
            this.objViewerInfo.TabIndex = 3;
            this.objViewerInfo.Text = "DEBUG";
            // 
            // imageViewerPlayAnim
            // 
            this.imageViewerPlayAnim.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.imageViewerPlayAnim.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.imageViewerPlayAnim.Location = new System.Drawing.Point(201, 451);
            this.imageViewerPlayAnim.Name = "imageViewerPlayAnim";
            this.imageViewerPlayAnim.Size = new System.Drawing.Size(731, 25);
            this.imageViewerPlayAnim.TabIndex = 2;
            this.imageViewerPlayAnim.Text = "Play Animation";
            this.imageViewerPlayAnim.UseVisualStyleBackColor = true;
            this.imageViewerPlayAnim.Click += new System.EventHandler(this.advancedPlayAnimation_Click);
            // 
            // imageViewPictureBox
            // 
            this.imageViewPictureBox.BackColor = System.Drawing.Color.FromArgb(((int) (((byte) (64)))), ((int) (((byte) (64)))), ((int) (((byte) (64)))));
            this.imageViewPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageViewPictureBox.Location = new System.Drawing.Point(201, 3);
            this.imageViewPictureBox.Name = "imageViewPictureBox";
            this.imageViewPictureBox.Size = new System.Drawing.Size(731, 473);
            this.imageViewPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.imageViewPictureBox.TabIndex = 1;
            this.imageViewPictureBox.TabStop = false;
            // 
            // objTreeView
            // 
            this.objTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.objTreeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.objTreeView.Location = new System.Drawing.Point(3, 3);
            this.objTreeView.Name = "objTreeView";
            this.objTreeView.Size = new System.Drawing.Size(198, 473);
            this.objTreeView.TabIndex = 1;
            this.objTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.advancedTreeView_AfterSelect);
            // 
            // soundViewTab
            // 
            this.soundViewTab.BackColor = System.Drawing.Color.Black;
            this.soundViewTab.Controls.Add(this.stopSoundBtn);
            this.soundViewTab.Controls.Add(this.soundList);
            this.soundViewTab.Controls.Add(this.playSoundBtn);
            this.soundViewTab.Location = new System.Drawing.Point(4, 24);
            this.soundViewTab.Name = "soundViewTab";
            this.soundViewTab.Padding = new System.Windows.Forms.Padding(3);
            this.soundViewTab.Size = new System.Drawing.Size(935, 479);
            this.soundViewTab.TabIndex = 6;
            this.soundViewTab.Text = "Sounds";
            // 
            // stopSoundBtn
            // 
            this.stopSoundBtn.Location = new System.Drawing.Point(328, 3);
            this.stopSoundBtn.Name = "stopSoundBtn";
            this.stopSoundBtn.Size = new System.Drawing.Size(113, 50);
            this.stopSoundBtn.TabIndex = 2;
            this.stopSoundBtn.Text = "Stop Sound";
            this.stopSoundBtn.UseVisualStyleBackColor = true;
            this.stopSoundBtn.Click += new System.EventHandler(this.stopSoundBtn_Click);
            // 
            // soundList
            // 
            this.soundList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.soundList.Dock = System.Windows.Forms.DockStyle.Left;
            this.soundList.Location = new System.Drawing.Point(3, 3);
            this.soundList.Name = "soundList";
            this.soundList.Size = new System.Drawing.Size(198, 473);
            this.soundList.TabIndex = 1;
            this.soundList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.soundList_AfterSelect);
            // 
            // playSoundBtn
            // 
            this.playSoundBtn.Location = new System.Drawing.Point(209, 3);
            this.playSoundBtn.Name = "playSoundBtn";
            this.playSoundBtn.Size = new System.Drawing.Size(113, 50);
            this.playSoundBtn.TabIndex = 0;
            this.playSoundBtn.Text = "Play Sound";
            this.playSoundBtn.UseVisualStyleBackColor = true;
            this.playSoundBtn.Click += new System.EventHandler(this.playSoundBtn_Click);
            // 
            // cryptKeyTab
            // 
            this.cryptKeyTab.BackColor = System.Drawing.Color.Black;
            this.cryptKeyTab.Controls.Add(this.minusCharButton);
            this.cryptKeyTab.Controls.Add(this.plusCharBtn);
            this.cryptKeyTab.Controls.Add(this.hexBox1);
            this.cryptKeyTab.Controls.Add(this.charBox);
            this.cryptKeyTab.Location = new System.Drawing.Point(4, 24);
            this.cryptKeyTab.Name = "cryptKeyTab";
            this.cryptKeyTab.Padding = new System.Windows.Forms.Padding(3);
            this.cryptKeyTab.Size = new System.Drawing.Size(935, 479);
            this.cryptKeyTab.TabIndex = 3;
            this.cryptKeyTab.Text = "CryptoKey";
            // 
            // minusCharButton
            // 
            this.minusCharButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.minusCharButton.Location = new System.Drawing.Point(3, 24);
            this.minusCharButton.Name = "minusCharButton";
            this.minusCharButton.Size = new System.Drawing.Size(20, 452);
            this.minusCharButton.TabIndex = 3;
            this.minusCharButton.Text = "-";
            this.minusCharButton.UseVisualStyleBackColor = true;
            this.minusCharButton.Click += new System.EventHandler(this.minusCharButton_Click);
            // 
            // plusCharBtn
            // 
            this.plusCharBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.plusCharBtn.Location = new System.Drawing.Point(912, 24);
            this.plusCharBtn.Name = "plusCharBtn";
            this.plusCharBtn.Size = new System.Drawing.Size(20, 452);
            this.plusCharBtn.TabIndex = 2;
            this.plusCharBtn.Text = "+";
            this.plusCharBtn.UseVisualStyleBackColor = true;
            this.plusCharBtn.Click += new System.EventHandler(this.plusCharBtn_Click);
            // 
            // hexBox1
            // 
            this.hexBox1.ColumnInfoVisible = true;
            this.hexBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.hexBox1.LineInfoVisible = true;
            this.hexBox1.Location = new System.Drawing.Point(3, 24);
            this.hexBox1.Name = "hexBox1";
            this.hexBox1.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int) (((byte) (100)))), ((int) (((byte) (60)))), ((int) (((byte) (188)))), ((int) (((byte) (255)))));
            this.hexBox1.Size = new System.Drawing.Size(929, 452);
            this.hexBox1.StringViewVisible = true;
            this.hexBox1.TabIndex = 1;
            // 
            // charBox
            // 
            this.charBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.charBox.Location = new System.Drawing.Point(3, 3);
            this.charBox.Name = "charBox";
            this.charBox.Size = new System.Drawing.Size(929, 21);
            this.charBox.TabIndex = 0;
            this.charBox.Text = "54";
            this.charBox.TextChanged += new System.EventHandler(this.charBox_TextChanged);
            // 
            // pluginTab
            // 
            this.pluginTab.BackColor = System.Drawing.Color.Black;
            this.pluginTab.Controls.Add(this.activatePluginBtn);
            this.pluginTab.Controls.Add(this.pluginsList);
            this.pluginTab.Location = new System.Drawing.Point(4, 24);
            this.pluginTab.Name = "pluginTab";
            this.pluginTab.Padding = new System.Windows.Forms.Padding(3);
            this.pluginTab.Size = new System.Drawing.Size(935, 479);
            this.pluginTab.TabIndex = 5;
            this.pluginTab.Text = "Plugins";
            // 
            // activatePluginBtn
            // 
            this.activatePluginBtn.AutoSize = true;
            this.activatePluginBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.activatePluginBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.activatePluginBtn.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
            this.activatePluginBtn.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.activatePluginBtn.Location = new System.Drawing.Point(207, 6);
            this.activatePluginBtn.Name = "activatePluginBtn";
            this.activatePluginBtn.Size = new System.Drawing.Size(101, 31);
            this.activatePluginBtn.TabIndex = 1;
            this.activatePluginBtn.Text = "Activate";
            this.activatePluginBtn.UseVisualStyleBackColor = true;
            this.activatePluginBtn.Click += new System.EventHandler(this.activatePluginBtn_Click);
            // 
            // pluginsList
            // 
            this.pluginsList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pluginsList.Dock = System.Windows.Forms.DockStyle.Left;
            this.pluginsList.FormattingEnabled = true;
            this.pluginsList.ItemHeight = 15;
            this.pluginsList.Location = new System.Drawing.Point(3, 3);
            this.pluginsList.Name = "pluginsList";
            this.pluginsList.Size = new System.Drawing.Size(198, 473);
            this.pluginsList.TabIndex = 0;
            // 
            // settingsTab
            // 
            this.settingsTab.BackColor = System.Drawing.Color.Black;
            this.settingsTab.Controls.Add(this.langComboBox);
            this.settingsTab.Controls.Add(this.langLabel);
            this.settingsTab.Controls.Add(this.colorLabel);
            this.settingsTab.Controls.Add(this.updateSettings);
            this.settingsTab.Controls.Add(this.colorBox);
            this.settingsTab.Location = new System.Drawing.Point(4, 24);
            this.settingsTab.Name = "settingsTab";
            this.settingsTab.Size = new System.Drawing.Size(935, 479);
            this.settingsTab.TabIndex = 7;
            this.settingsTab.Text = "Settings";
            // 
            // langComboBox
            // 
            this.langComboBox.FormattingEnabled = true;
            this.langComboBox.Items.AddRange(new object[] {"en-US", "ru-RU"});
            this.langComboBox.Location = new System.Drawing.Point(70, 31);
            this.langComboBox.Name = "langComboBox";
            this.langComboBox.Size = new System.Drawing.Size(109, 23);
            this.langComboBox.TabIndex = 4;
            this.langComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // langLabel
            // 
            this.langLabel.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.langLabel.Location = new System.Drawing.Point(8, 34);
            this.langLabel.Name = "langLabel";
            this.langLabel.Size = new System.Drawing.Size(56, 15);
            this.langLabel.TabIndex = 3;
            this.langLabel.Text = "Lang:";
            // 
            // colorLabel
            // 
            this.colorLabel.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.colorLabel.Location = new System.Drawing.Point(8, 8);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new System.Drawing.Size(56, 15);
            this.colorLabel.TabIndex = 2;
            this.colorLabel.Text = "Color:";
            // 
            // updateSettings
            // 
            this.updateSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.updateSettings.ForeColor = System.Drawing.Color.FromArgb(((int) (((byte) (255)))), ((int) (((byte) (128)))), ((int) (((byte) (0)))));
            this.updateSettings.Location = new System.Drawing.Point(8, 399);
            this.updateSettings.Name = "updateSettings";
            this.updateSettings.Size = new System.Drawing.Size(131, 72);
            this.updateSettings.TabIndex = 1;
            this.updateSettings.Text = "Update";
            this.updateSettings.UseVisualStyleBackColor = true;
            this.updateSettings.Click += new System.EventHandler(this.updateSettings_Click);
            // 
            // colorBox
            // 
            this.colorBox.Location = new System.Drawing.Point(70, 5);
            this.colorBox.Name = "colorBox";
            this.colorBox.Size = new System.Drawing.Size(109, 21);
            this.colorBox.TabIndex = 0;
            this.colorBox.TextChanged += new System.EventHandler(this.colorBox_TextChanged);
            // 
            // packDataDialog
            // 
            this.packDataDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.packDataDialog_FileOk);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(943, 507);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 15);
            this.Name = "MainForm";
            this.Text = "CTFAK";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ChunkCombo.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.mainTab.ResumeLayout(false);
            this.mainTab.PerformLayout();
            this.mfaTab.ResumeLayout(false);
            this.mfaTab.PerformLayout();
            this.packDataTab.ResumeLayout(false);
            this.objViewerTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.imageViewPictureBox)).EndInit();
            this.soundViewTab.ResumeLayout(false);
            this.cryptKeyTab.ResumeLayout(false);
            this.cryptKeyTab.PerformLayout();
            this.pluginTab.ResumeLayout(false);
            this.pluginTab.PerformLayout();
            this.settingsTab.ResumeLayout(false);
            this.settingsTab.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button dumpSelectedBtn;

        private System.Windows.Forms.ComboBox langComboBox;

        private System.Windows.Forms.Label langLabel;

        private System.Windows.Forms.TreeView objTreeView;

        private System.Windows.Forms.Label objViewerInfo;

        private System.Windows.Forms.TabPage objViewerTab;

        private System.Windows.Forms.TextBox colorBox;
        private System.Windows.Forms.Label colorLabel;
        private System.Windows.Forms.Button updateSettings;

        private System.Windows.Forms.TabPage settingsTab;

        private System.Windows.Forms.Button stopSoundBtn;

        private System.Windows.Forms.TreeView soundList;

        private System.Windows.Forms.Button playSoundBtn;

        private System.Windows.Forms.TabPage soundViewTab;

        private System.Windows.Forms.Button imageViewerPlayAnim;
        private System.Windows.Forms.PictureBox imageViewPictureBox;

        private System.Windows.Forms.Button activatePluginBtn;
        private System.Windows.Forms.ListBox pluginsList;

        private System.Windows.Forms.TabPage pluginTab;

        private System.Windows.Forms.Button dumpAllPackButton;
        private System.Windows.Forms.Button dumpPackButton;
        private System.Windows.Forms.SaveFileDialog packDataDialog;
        private System.Windows.Forms.ListBox packDataListBox;

        private System.Windows.Forms.Label infoLabel;

        private System.Windows.Forms.TabPage mainTab;
        private System.Windows.Forms.TabPage mfaTab;
        private System.Windows.Forms.TabPage packDataTab;

        private System.Windows.Forms.TextBox charBox;

        private System.Windows.Forms.Button minusCharButton;

        private System.Windows.Forms.Button plusCharBtn;

        private Be.Windows.Forms.HexBox hexBox1;

        private System.Windows.Forms.TabPage cryptKeyTab;

        private System.Windows.Forms.TextBox mfaLogBox;

        private System.Windows.Forms.Button dumpMFAButton;

        private System.Windows.Forms.TabControl tabControl1;

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip ChunkCombo;
        private System.Windows.Forms.Button FolderBTN;
        private System.Windows.Forms.Label GameInfo;
        private System.Windows.Forms.ProgressBar imageBar;
        private System.Windows.Forms.Label imageLabel;
        private System.Windows.Forms.Button imagesButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.ProgressBar musicBar;
        private System.Windows.Forms.Label musicLabel;
        private System.Windows.Forms.Button musicsButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem previewFrameBtn;
        private System.Windows.Forms.ToolStripMenuItem saveChunkBtn;
        private System.Windows.Forms.ProgressBar soundBar;
        private System.Windows.Forms.Label soundLabel;
        private System.Windows.Forms.Button soundsButton;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStripMenuItem viewHexBtn;

        #endregion
    }
}