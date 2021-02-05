using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Be.Windows.Forms;
using CTFAK.GUI.GUIComponents;
using CTFAK.MMFParser;
using CTFAK.MMFParser.Attributes;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using CTFAK.MMFParser.EXE.Loaders.Objects;
using CTFAK.MMFParser.Translation;
using CTFAK.Utils;


namespace CTFAK.GUI
{
    public partial class MainForm : Form
    {
        public delegate void IncrementSortedProgressBar(int all);

        public delegate void SaveHandler(int index, int all);

        public static bool IsDumpingImages;
        public static bool IsDumpingSounds;
        public static bool IsDumpingMusics;
        public static bool BreakImages;
        public static bool BreakSounds;
        public static bool BreakMusics;
        public static bool Loaded;
        public static Color ColorTheme = Color.FromArgb(223, 114, 38);

        private bool _breakAnim;
        private bool _isAnimRunning;


        private SoundPlayer _soundPlayer;
        public Label ObjectViewerLabel;
        public TreeNode LastSelected;


        public MainForm(Color color)
        {
            //Buttons
            InitializeComponent();
            Thread.CurrentThread.CurrentUICulture=new CultureInfo("en-US");
            
            
            ColorTheme = color;
            foreach (Control item in Controls)
            {
                item.ForeColor = ColorTheme;
                if (!(item is PictureBox) && !(item is TabPage)) item.BackColor = Color.Black;

                if (item is Button) item.BackColor = Color.FromArgb(30, 30, 30);

                if (item is Label)
                    //item.BackColor = Color.Transparent;
                    item.Refresh();
            }

            foreach (TabPage tabPage in tabControl1.TabPages)
            foreach (Control item in tabPage.Controls)
            {
                item.ForeColor = ColorTheme;
                if (!(item is PictureBox) && !(item is TabPage) && !(item is Label)) item.BackColor = Color.Black;
                if (item is Button) item.BackColor = Color.FromArgb(30, 30, 30);

                if (item is Label)
                    //item.BackColor = Color.Transparent;
                    item.Refresh();
            }

            foreach (var item in ChunkCombo.Items)
            {
                ((ToolStripItem) item).ForeColor = ColorTheme;
                ((ToolStripItem) item).BackColor = Color.Black;
            }

            hexBox1.ForeColor = ColorTheme;
            hexBox1.InfoForeColor = Color.FromArgb(ColorTheme.R / 2, ColorTheme.G / 2, ColorTheme.B / 2);
            hexBox1.SelectionForeColor = Color.FromArgb(ColorTheme.R, ColorTheme.G, ColorTheme.B);
            hexBox1.SelectionBackColor = Color.FromArgb(ColorTheme.R / 4, ColorTheme.G / 4, ColorTheme.B / 4);
            hexBox1.ShadowSelectionColor = Color.FromArgb(150, ColorTheme.R / 4, ColorTheme.G / 4, ColorTheme.B / 4);
            label1.Text = Settings.DumperVersion;
            Text = Settings.DumperVersion;
            //Locale
            button1.Text = Properties.GlobalStrings.selectFile;
            loadingLabel.Text = Properties.GlobalStrings.loading;
            imagesButton.Text = Properties.GlobalStrings.dumpImages;
            soundsButton.Text = Properties.GlobalStrings.dumpSounds;
            musicsButton.Text = Properties.GlobalStrings.dumpMusics;
            FolderBTN.Text=Properties.GlobalStrings.openFolder;

            
            Pame2Mfa.OnMessage += obj =>
            {
                var date = DateTime.Now;
                var msg = (string) obj;
                mfaLogBox.AppendText(msg.Length > 0
                    ? $"[{date.Hour,2}:{date.Minute,2}:{date.Second,2}:{date.Millisecond,3}] {msg}\r\n"
                    : "\r\n");
            };
            Closing += (a, e) =>
            {
                var dlg = MessageBox.Show("Are you sure you want to exit?", "Exiting", MessageBoxButtons.YesNo);
                if (dlg == DialogResult.Yes) Environment.Exit(0);
                else e.Cancel = true;
            };
            objViewerInfo.Parent = imageViewPictureBox;
            objViewerInfo.BackColor=Color.Transparent;
            objViewerInfo.Dock = DockStyle.Right;
            
            
               
            KeyPreview = true;
            tabControl1.Selecting += tabControl1_Selecting;
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!Loaded)
                if (e.TabPage == settingsTab || e.TabPage == mainTab)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
                   
            //_soundPlayer.Stop();
        }


        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (File.Exists(openFileDialog1.FileName))
            {
                objTreeView.Nodes.Clear();
                Loaded = false;
                loadingLabel.Visible = true;
                var worker = new BackgroundWorker();
                worker.DoWork += (workSender, workE) => { StartReading(); };
                worker.RunWorkerCompleted += (workSender, workE) => { AfterLoad(); };

                worker.RunWorkerAsync();
            }
            else throw new NotImplementedException("File not found");
        }



        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            loadingLabel.Visible = false;
            listBox1.Items.Clear();
            var console = new MainConsole();
            Location = new Point(0, 0);
            Size = new Size(Size.Width - 100, Screen.PrimaryScreen.Bounds.Height - 120);
            console.Show();
            console.Location = new Point(Location.X + Size.Width - 15, 0);
            console.Size = new Size(console.Size.Width, Size.Height);
            
            

        }


        private void StartReading()
        {
            var path = openFileDialog1.FileName;
            
            Program.ReadFile(path, Settings.Verbose, Settings.DumpImages, Settings.DumpSounds);

            imageBar.Value = 0;
            soundBar.Value = 0;
            GameInfo.Text = "";
            imageLabel.Text = "Using nonGUI mode";
            soundLabel.Text = "Using nonGUI mode";

            FolderBTN.Visible = false;
            imagesButton.Visible = false;
            soundsButton.Visible = false;
            musicsButton.Visible = false;
        }

        private void treeView1_AfterDblClick(object sender, EventArgs e)
        {
            ChunkCombo.Show(Cursor.Position);
        }

        private void treeView1_RightClick(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) != 0) ChunkCombo.Show(Cursor.Position);
        }

        private void ChunkCombo_ItemSelected(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Name)
            {
                case "saveChunkBtn":
                    var chunk = ((ChunkNode) treeView1.SelectedNode).chunk;
                    chunk?.Save();
                    break;
                case "viewHexBtn":
                    ShowHex();
                    break;
                case "previewFrameBtn":
                    var selected = ((ChunkNode) treeView1.SelectedNode).loader;
                    if (selected is Frame frm)
                    {
                        var viewer = new FrameViewer(frm, Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>());
                        viewer.Show();
                    }

                    break;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var nodeChunk = ((ChunkNode) treeView1.SelectedNode).chunk;
            var nodeLoader = ((ChunkNode) treeView1.SelectedNode).loader;

            listBox1.Items.Clear();
            if (nodeChunk != null)
            {
                listBox1.Items.Add($"Name: {nodeChunk.Name}");
                listBox1.Items.Add($"Id: {nodeChunk.Id} - 0x{nodeChunk.Id:X4}");
                listBox1.Items.Add($"Flag: {nodeChunk.Flag}");
                listBox1.Items.Add($"Size: {nodeChunk.Size.ToPrettySize()}");
                if (nodeChunk.DecompressedSize > 0)
                    listBox1.Items.Add($"Decompressed Size: {nodeChunk.DecompressedSize.ToPrettySize()}");
            }

            if (nodeLoader != null)
            {
                var extData = nodeLoader.GetReadableData();
                if (extData.Length > 0)
                {
                    listBox1.Items.Add("");
                    listBox1.Items.Add("Loader Data:");

                    foreach (var item in extData)
                    {
                        var splitFlags = item.Split(';');
                        foreach (var splitItem in splitFlags) listBox1.Items.Add(splitItem);
                    }
                }
            }

            GameInfo.BackColor = Color.Transparent;

            GameInfo.Refresh();
        }

        public void AfterLoad()
        {
            Logger.Log("Loading GUI");
            //GameData gameData = null;
            var exe = Exe.Instance;
            var gameData = exe?.GameData ?? Program.CleanData;


            treeView1.Nodes.Clear();
            foreach (var item in gameData.GameChunks.Chunks)
            {
                var ActualName = item.Name;
                // if (item.Loader is Frame frm) ActualName = ActualName + " " + frm.Name;
                var newNode = Helper.GetChunkNode(item);
                treeView1.Nodes.Add(newNode);
                
                /*if (item.Loader is Frame frame)
                    foreach (var frmChunk in frame.Chunks.Chunks)
                    {
                        var frameNode = Helper.GetChunkNode(frmChunk);
                        newNode.Nodes.Add(frameNode);
                        if (frameNode.loader is ObjectInstances)
                        {
                            var objs = frame.Chunks.GetChunk<ObjectInstances>();
                            if (objs != null)
                                foreach (var frmitem in objs.Items)
                                {
                                    var objNode = new ChunkNode(frmitem.Name, frmitem);
                                    frameNode.Nodes.Add(objNode);
                                }
                        }
                    }
                else if (item.Loader is FrameItems items)
                    foreach (var key in items.ItemDict.Keys)
                    {
                        var frameItem = items.ItemDict[key];
                        var objNode = new ChunkNode($"{(Constants.ObjectType) frameItem.ObjectType} - {frameItem.Name}",
                            frameItem);
                        foreach (ChunkList.Chunk chunk in frameItem.Chunks)
                        {
                            objNode.Nodes.Add(new ChunkNode(chunk.Name, chunk));
                        }
                        newNode.Nodes.Add(objNode);
                    }*/
            }

            FolderBTN.Visible = true;
            imagesButton.Visible = true;
            soundsButton.Visible = true;
            musicsButton.Visible = true;
            GameInfo.Visible = true;
            loadingLabel.Visible = false;
            var toLog = "";
            toLog += $"{Properties.Locale.ChunkNames.title}: {Program.CleanData.Name}\n";
            toLog += $"{Properties.Locale.ChunkNames.copyright}: {Program.CleanData.Copyright}\n";
            //toLog += $"Editor Filename: {Exe.Instance.GameData.EditorFilename}\n";
            toLog += $"Product Version: {Program.CleanData.ProductVersion}\n";
            toLog += $"Build: {Program.CleanData.Build}\n";
            toLog += $"Runtime Version: {Program.CleanData.RuntimeVersion}\n";
            toLog += $"{Properties.GlobalStrings.imageCount}: {Program.CleanData.Images?.Images.Count ?? 0}\n";
            toLog += $"{Properties.GlobalStrings.soundCount}: {Program.CleanData.Sounds?.NumOfItems ?? 0}\n";
            toLog += $"{Properties.GlobalStrings.musicCount}: {Program.CleanData.Music?.NumOfItems ?? 0}\n";
            toLog += $"{Properties.GlobalStrings.frameitemCount}: {Program.CleanData.Frameitems?.ItemDict.Count}\n";
            toLog += $"{Properties.GlobalStrings.frameCount}: {Program.CleanData.Frames.Count}\n";
            toLog += $"Chunks Count: {Program.CleanData.GameChunks.Chunks.Count}\n";
            GameInfo.Text = toLog;
            InitPackDataTab();
            InitMFA();
            InitImages();
            InitSounds();
            InitKeyTab();
            InitPlugins();
            
            if (Program.CleanData.GameChunks.GetChunk<ImageBank>() != null)
                Program.CleanData.GameChunks.GetChunk<ImageBank>().OnImageSaved += UpdateImageBar;
            if (Program.CleanData.GameChunks.GetChunk<SoundBank>() != null)
                Program.CleanData.GameChunks.GetChunk<SoundBank>().OnSoundSaved += UpdateSoundBar;
            if (Program.CleanData.GameChunks.GetChunk<MusicBank>() != null)
                Program.CleanData.GameChunks.GetChunk<MusicBank>().OnMusicSaved += UpdateMusicBar;


            
            Loaded = true;
        }


        private void UpdateImageBar(int index, int all)
        {
            all -= 1;
            imageBar.Value = (int) (index / (float) all * 100);
            imageLabel.Text = $@"{index}/{all}";
        }

        private void UpdateSoundBar(int index, int all)
        {
            all -= 1;
            soundBar.Value = (int) (index / (float) all * 100);
            soundLabel.Text = $@"{index}/{all}";
        }

        private void UpdateMusicBar(int index, int all)
        {
            all -= 1;
            musicBar.Value = (int) (index / (float) all * 100);
            musicLabel.Text = $@"{index}/{all}";
        }
        

        private void FolderBTN_Click(object sender, EventArgs e)
        {
            Process.Start($"{Settings.DumpPath}");
        }


        private void soundsButton_Click(object sender, EventArgs e)
        {
            if (Exe.Instance.GameData.GameChunks.GetChunk<SoundBank>() == null) return;
            if (!IsDumpingSounds)
            {
                SetSoundElements(true);
                IsDumpingSounds = true;
                Backend.DumpSounds(this, true, true);
            }
            else
            {
                BreakSounds = true;
                IsDumpingSounds = false;
                SetSoundElements(false);
            }
        }

        private void imagesButton_Click(object sender, EventArgs e)
        {
            if (Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>() == null) return;
            if (!IsDumpingImages)
            {
                SetImageElements(true);
                IsDumpingImages = true;
                Backend.DumpImages(this, true, true);
            }
            else
            {
                BreakImages = true;
                IsDumpingImages = false;
                SetImageElements(false);
            }
        }

        private void musicsButton_Click(object sender, EventArgs e)
        {
            if (Exe.Instance.GameData.GameChunks.GetChunk<MusicBank>() == null) return;
            if (!IsDumpingMusics)
            {
                SetMusicElements(true);
                IsDumpingMusics = true;
                Backend.DumpMusics(this, true, true);
            }
            else
            {
                BreakMusics = true;
                IsDumpingMusics = false;
                SetMusicElements(false);
            }
        }


        public void SetSoundElements(bool state)
        {
            soundBar.Visible = state;
            soundLabel.Visible = state;
            soundsButton.Text = state ? "Cancel" : "Dump Sounds";
            soundBar.Value = 0;
        }

        public void SetImageElements(bool state)
        {
            imageBar.Visible = state;
            imageLabel.Visible = state;
            imagesButton.Text = state ? "Cancel" : "Dump Images";
            imageBar.Value = 0;
        }

        public void SetMusicElements(bool state)
        {
            musicBar.Visible = state;
            musicLabel.Visible = state;
            musicsButton.Text = state ? "Cancel" : "Dump Musics";
            musicBar.Value = 0;
        }

        private void ShowHex()
        {
            if ((ChunkNode) treeView1.SelectedNode != null)
            {
                var node = (ChunkNode) treeView1.SelectedNode;
                HexViewForm hexform = null;

                hexform = new HexViewForm(node.chunk.ChunkData, node.chunk.RawData, ColorTheme,
                    $"Hew View: {node.chunk.Name}");

                hexform.Show();
            }
        }
        private void dumpMFAButton_Click(object sender, EventArgs e)
        {
            var msg = MessageBox.Show("By using CTFAK, you agree that you will only used the decompiled MFAs for personal use and educational purposes.\nYou also agree that unless you are the original creator or have been given permission, you will not recompile any games using this tool.","Warning",MessageBoxButtons.OKCancel);
            if(msg != DialogResult.OK) Environment.Exit(0);
            var worker = new BackgroundWorker();
            worker.DoWork += (workSender, workE) => MFAGenerator.BuildMFA();
            worker.RunWorkerCompleted += (workSender, workE) =>
            {
                Logger.Log("MFA Done", true, ConsoleColor.Yellow);
                var res = MessageBox.Show("Dump Extensions?", "Finished", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                    foreach (var item in Exe.Instance.PackData.Items)
                    {
                        item.Dump();
                        Pame2Mfa.Message("Dumping " + item.PackFilename);
                    }

                Process.Start($"{Settings.DumpPath}");
            };

            worker.RunWorkerAsync();
        }

        public void InitKeyTab()
        {
            var rawData = "";

            if (Settings.Build > 284)
            {
                rawData += Settings.AppName;
                rawData += Settings.Copyright;
                rawData += Settings.ProjectPath;
            }
            else
            {
                rawData += Settings.ProjectPath;
                rawData += Settings.AppName;
                rawData += Settings.Copyright;
            }

            try
            {
                var previewKey = Decryption.MakeKeyFromComb(rawData, (byte) int.Parse(charBox.Text));
                hexBox1.ByteProvider = new DynamicByteProvider(previewKey);
            }
            catch
            {
                // ignored
            }
        }

        public void InitPackDataTab()
        {
            if (Settings.GameType == GameType.Android) return;
            packDataListBox.Items.Clear();
            foreach (var item in Exe.Instance.PackData.Items) packDataListBox.Items.Add(item.PackFilename);

            UpdatePackInfo(0);
        }

        private void UpdatePackInfo(int index)
        {
            var item = Exe.Instance.PackData.Items[index];
            string text = String.Empty;
            text += $"Name: {item.PackFilename}\n";
            text += $"Size: {item.Data.Length.ToPrettySize()}\n";

            infoLabel.Text = text;
        }

        private void dumpPackButton_Click(object sender, EventArgs e)
        {
            var item = Exe.Instance.PackData.Items[packDataListBox.SelectedIndex];

            packDataDialog.FileName = item.PackFilename;
            if (item.PackFilename.EndsWith(".mfx")) packDataDialog.Filter = "Clickteam Extension(*.mfx)|.mfx";
            else if (item.PackFilename.EndsWith(".dll")) packDataDialog.Filter = "Clickteam Module(*.dll)|.dll";
            else packDataDialog.Filter = $"Unknown File (*{Path.GetExtension(item.PackFilename)})|{Path.GetExtension(item.PackFilename)}";


            packDataDialog.InitialDirectory = Path.GetFullPath(Settings.ExtensionPath);
            packDataDialog.ShowDialog();
        }

        private void dumpAllPackButton_Click(object sender, EventArgs e)
        {
            foreach (var item in Exe.Instance.PackData.Items) item.Dump();
        }

        private void packDataDialog_FileOk(object sender, CancelEventArgs e)
        {
            var item = Exe.Instance.PackData.Items[packDataListBox.SelectedIndex];
            item.Dump(packDataDialog.FileName);
        }

        private void packDataListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePackInfo(packDataListBox.SelectedIndex);
        }

        private void plusCharBtn_Click(object sender, EventArgs e)
        {
            charBox.Text = (byte.Parse(charBox.Text) + 1).ToString();
            InitKeyTab();
        }


        private void minusCharButton_Click(object sender, EventArgs e)
        {
            charBox.Text = (byte.Parse(charBox.Text) - 1).ToString();
            InitKeyTab();
        }

        private void charBox_TextChanged(object sender, EventArgs e)=> InitKeyTab();
        public void InitMFA()
        {
            string toLog = string.Empty;
            toLog += $"MFA Generator 1.0\n";
            toLog += $"Game Build: {Program.CleanData.ProductBuild}\n";
            toLog += $"Game Name: {Program.CleanData.Name}\n";
            toLog += $"MFA Name: {Path.GetFileNameWithoutExtension(Program.CleanData.EditorFilename)}.mfa\n";
            toLog += $"Frames to translate: {Program.CleanData.Frames.Count}\n";
            toLog += $"Objects to translate: {Program.CleanData.Frameitems.ItemDict.Count}\n";
            toLog += $"Images to write: {Program.CleanData.Images.Images.Count}\n";
            toLog += $"Sounds to write: {Program.CleanData.Sounds.Items.Count}\n";


            mfaDumpInfoLabel.Text = toLog;

        }
        public void InitImages()
        {
            if (Settings.GameType == GameType.TwoFivePlus||Settings.GameType == GameType.Android) return;
            var bank = Program.CleanData.GameChunks.GetChunk<ImageBank>();
            var items = bank.Images.ToList();
            var filtered = items.OrderBy(x => x.Value.Handle);
            foreach (var frame in Exe.Instance.GameData.Frames)
            {
                var frameNode = new ChunkNode(frame.Name, frame);
                objTreeView.Nodes.Add(frameNode);
                if (frame.Objects != null)
                    foreach (var objInst in frame.Objects)
                    {
                        var objInstNode = new ChunkNode(objInst.FrameItem.Name, objInst);
                        frameNode.Nodes.Add(objInstNode);
                        var loader = objInst.FrameItem.Properties.Loader;
                        if (loader is ObjectCommon common)
                        {
                            if (common.Parent.ObjectType == Constants.ObjectType.Active) //Active
                            {
                                if (common.Animations != null)
                                    foreach (var pair in common.Animations?.AnimationDict)
                                    {
                                        var animNode = new ChunkNode($"Animation {pair.Key}", pair.Value);
                                        if (pair.Value.Reader == null) continue;
                                        objInstNode.Nodes.Add(animNode);
                                        if (pair.Value?.DirectionDict != null)
                                        {
                                            foreach (var dir in pair.Value?.DirectionDict)
                                                if (pair.Value.DirectionDict.Count > 1)
                                                {
                                                    if (dir.Value.Reader == null) continue;
                                                    var dirNode = new ChunkNode(
                                                        $"Direction {pair.Value.DirectionDict.ToList().IndexOf(dir)}",
                                                        dir.Value);
                                                    animNode.Nodes.Add(dirNode);
                                                    for (var a = 0; a < dir.Value.Frames.Count; a++)
                                                    {
                                                        var animFrame = dir.Value.Frames[a];
                                                        bank.Images.TryGetValue(animFrame, out var img);
                                                        if (img != null)
                                                        {
                                                            var animFrameNode = new ChunkNode(a.ToString(), img);
                                                            dirNode.Nodes.Add(animFrameNode);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    for (var a = 0; a < dir.Value.Frames.Count; a++)
                                                    {
                                                        var animFrame = dir.Value.Frames[a];
                                                        bank.Images.TryGetValue(animFrame, out var img);
                                                        if (img != null)
                                                        {
                                                            var animFrameNode = new ChunkNode(a.ToString(), img);
                                                            animNode.Nodes.Add(animFrameNode);
                                                        }
                                                    }
                                                }
                                        }
                                    }
                            }
                            else if(common.Parent.ObjectType==Constants.ObjectType.Counter)//Counter
                            {
                                
                                var count = common.Counters?.Frames?.Count??0;

                                for (var a = 0; a < count; a++)
                                {
                                    var animFrame = common.Counters.Frames[a];
                                    bank.Images.TryGetValue(animFrame, out var img);
                                    if (img == null)
                                    {
                                        
                                        continue;
                                    }
                                    var animFrameNode = new ChunkNode(a.ToString(), img);
                                    objInstNode.Nodes.Add(animFrameNode);
                                }
                                

                            }
                        
                        }
                        else if (loader is Backdrop backdrop)
                        {
                            bank.Images.TryGetValue(backdrop.Image, out var img);
                            if (img != null)
                            {
                                var backdropNode = new ChunkNode("Image", img);
                                objInstNode.Nodes.Add(backdropNode);
                            }
                        }
                    }
            }
        }

        private void advancedPlayAnimation_Click(object sender, EventArgs e)
        {
            if (((ChunkNode) objTreeView.SelectedNode).loader is Animation anim)
            {
                if (_isAnimRunning)
                {
                    _breakAnim = true;
                }
                else
                {
                    _isAnimRunning = true;
                    var animThread = new Thread(PlayAnimation);
                    var frames = new List<Bitmap>();
                    foreach (var dir in anim.DirectionDict)
                    {
                        foreach (var frame in dir.Value.Frames)
                            frames.Add(Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>().Images[frame].Bitmap);
                        animThread.Start(new Tuple<List<Bitmap>, AnimationDirection>(frames, dir.Value));
                        break;
                    }
                }
            }
            else if (((ChunkNode) objTreeView.SelectedNode).loader is AnimationDirection dir)
            {
                if (_isAnimRunning)
                {
                    _breakAnim = true;
                }
                else
                {
                    _isAnimRunning = true;
                    var animThread = new Thread(PlayAnimation);
                    var frames = new List<Bitmap>();
                    foreach (var frame in dir.Frames)
                    {
                        frames.Add(Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>().Images[frame].Bitmap);

                        animThread.Start(new Tuple<List<Bitmap>, AnimationDirection>(frames, dir));
                        break;
                    }
                }
            }
        }

        public void PlayAnimation(object o)
        {
            var (frames, anim) = (Tuple<List<Bitmap>, AnimationDirection>) o;
            var fps = (float) anim.MaxSpeed;
            var delay = 1f / fps;
            var i = 0;
            if (anim.Repeat > 0 && anim.Frames.Count > 1)
            {
                foreach (var frame in frames)
                {
                    imageViewPictureBox.Image = frame;
                    objViewerInfo.Text = $"Current frame: {frames.IndexOf(frame)}\nAnimation Speed: {fps}";
                    Thread.Sleep((int) (delay * 1500));
                }

                _isAnimRunning = false;
                try
                {
                    Thread.CurrentThread.Abort();
                }
                catch
                {
                }
            }
            else
            {
                while (true)
                {
                    var frame = frames[i];
                    imageViewPictureBox.Image = frame;
                    objViewerInfo.Text = $"Current frame: {i.ToString()}\nAnimation Speed: {fps}";
                    Thread.Sleep((int) (delay * 1500));
                    i++;
                    if (i == frames.Count) i = 0;
                    if (_breakAnim)
                    {
                        _isAnimRunning = false;
                        _breakAnim = false;
                        if (Thread.CurrentThread.IsAlive) Thread.CurrentThread.Abort();
                        break;
                    }
                }
            }
        }

        private void advancedTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ObjectViewerLabel?.Dispose();
            LastSelected = objTreeView.SelectedNode;
            var node = e.Node;
            var loader = ((ChunkNode) node).loader;
            string text=String.Empty;
            imageViewPictureBox.Image = null;
            if (loader is ImageItem img)
            {
                text += $"Size: {img.Bitmap.Width}x{img.Bitmap.Height}\r\n";
                text += $"Action Point: {img.ActionX}x{img.ActionY}\r\n";
                text += $"Hotspot: {img.XHotspot}x{img.YHotspot}\r\n";
                imageViewPictureBox.Image = img.Bitmap;
            }
            else if (loader is Animation anim)
            {
                text += $"Current frame: 0";
                text+=$"\r\nAnimation Speed: {anim.DirectionDict.FirstOrDefault().Value.MaxSpeed}";

                imageViewPictureBox.Image = Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>()
                    .FromHandle(anim.DirectionDict.FirstOrDefault().Value.Frames[0]).Bitmap;

            }
            else if (loader is Frame frame)
            {
                text += $"Name: {frame.Name}\r\n";
                text += $"Size: {frame.Width}x{frame.Height}\r\n";
                text += $"Objects: {frame.Objects.Count}\r\n";
                text += $"Layers: {frame.Layers.Count}\r\n";
                text += $"Flags:\r\n";
                foreach (var part in frame.Flags.ToString().Split(';'))
                {
                    text += part+"\r\n";

                }


                
            }
            else if (loader is ObjectInstance instance)
            {
                
                text += $"Name: {instance.FrameItem.Name}\r\n";
                text += $"Type: {(Constants.ObjectType)instance.FrameItem.ObjectType}\r\n";
                text += $"Position: {instance?.X}x{instance?.Y}\r\n";
                text += $"Size: {instance.FrameItem.GetPreview()?.Bitmap.Width}x{instance.FrameItem.GetPreview()?.Bitmap.Width}\r\n";
                if (instance.FrameItem.Properties.IsCommon)
                {
                    var common = ((ObjectCommon) instance.FrameItem.Properties.Loader);
                    switch (instance.FrameItem.ObjectType)
                    {
                        case Constants.ObjectType.Active:
                            text += $"Animations: {common.Animations?.AnimationDict.Count}";
                            imageViewPictureBox.Image = Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>()
                                .FromHandle(common.Animations.AnimationDict.FirstOrDefault().Value.DirectionDict
                                    .FirstOrDefault().Value.Frames.FirstOrDefault()).Bitmap;
                            break;
                        case Constants.ObjectType.Text:
                            ObjectViewerLabel = new Label();
                            var content = string.Empty;
                            foreach (var par in common.Text.Items)
                            {
                                content += $"{par.Value}\r\n";
                                content += $"\r\n\r\n\r\n";
                            }
                            ObjectViewerLabel.Text = content;
                            ObjectViewerLabel.Parent = imageViewPictureBox;
                            ObjectViewerLabel.Dock = DockStyle.Fill;
                            ObjectViewerLabel.TextAlign = ContentAlignment.MiddleCenter;

                            imageViewPictureBox.Controls.Add(ObjectViewerLabel);
                            break;
                        case Constants.ObjectType.Counter:
                            var handle = common.Counters?.Frames.FirstOrDefault();
                            if (handle == null) imageViewPictureBox.Image = imageViewPictureBox.ErrorImage;
                            else
                            {
                                imageViewPictureBox.Image = Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>()
                                .FromHandle((int) handle).Bitmap;
                            }
                            
                            //text += $"Add 0's to the left: {common?.Counters?.AddNulls}";
                            //text += $"Fixed number of digits: {(common.Counters.UseDecimals ? common?.Counters?.FloatDigits : common?.Counters?.IntegerDigits)}";
                            break;

                        default:
                            text += "No additional info";
                            break;
                    }
                }
                else
                {
                    if (instance.FrameItem.ObjectType == Constants.ObjectType.Backdrop)
                        imageViewPictureBox.Image = Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>().FromHandle(((Backdrop)instance.FrameItem.Properties.Loader).Image).Bitmap;
                }
                
                

                
            }
            objViewerInfo.Text = text;
        }


        public void InitPlugins()
        {
            PluginAPI.PluginAPI.InitializePlugins();
            foreach (var plugin in PluginAPI.PluginAPI.Plugins) pluginsList.Items.Add(plugin.Name);
        }

        private void activatePluginBtn_Click(object sender, EventArgs e)
        {
            PluginAPI.PluginAPI.ActivatePlugin(PluginAPI.PluginAPI.Plugins[pluginsList.SelectedIndex]);
        }


        public void InitSounds()
        {
            if (Settings.GameType == GameType.Android) return;
            var bank = Program.CleanData.GameChunks.GetChunk<SoundBank>();
            if (bank == null) return;
            foreach (var soundItem in bank.Items) soundList.Nodes.Add(new ChunkNode(soundItem.Name, soundItem));
            _soundPlayer =
                new SoundPlayer(new MemoryStream(Program.CleanData.GameChunks.GetChunk<SoundBank>().Items[0].Data));
        }

        private void playSoundBtn_Click(object sender, EventArgs e)
        {
            _soundPlayer.Stream = new MemoryStream(Exe.Instance.GameData.GameChunks.GetChunk<SoundBank>()
                .Items[soundList.SelectedNode.Index].Data);
            _soundPlayer.Play();
        }

        private void soundList_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (tabControl1.SelectedTab == objViewerTab)
                if (e.Control)
                {
                    var node = (ChunkNode) objTreeView.SelectedNode;
                    var path =
                        $"{Settings.ImagePath}\\{objTreeView.SelectedNode.FullPath}";
                    if (node == null) return;
                    ImageDumper.SaveFromNode(node);
                }
        }

        private void stopSoundBtn_Click(object sender, EventArgs e)
        {
            _soundPlayer.Stop();
        }


        private void colorBox_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void updateSettings_Click(object sender, EventArgs e)
        {
            LoadableSettings.instance["mainColor"] = colorBox.Text;
            LoadableSettings.instance["lang"] = langComboBox.SelectedItem;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        

        private void dumpSelectedBtn_Click(object sender, EventArgs e)
        {
            Logger.Log("Dumping");
            var node = (ChunkNode) LastSelected;
            var path =
                $"{Settings.ImagePath}\\{objTreeView.SelectedNode.FullPath}";
            if (node == null) return;
            ImageDumper.SaveFromNode(node);
        }
    }
}