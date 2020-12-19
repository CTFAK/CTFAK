using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Be.Windows.Forms;
using DotNetCTFDumper.MMFParser;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.MMFParser.EXE.Loaders;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Banks;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Objects;
using DotNetCTFDumper.MMFParser.MFA.Loaders.mfachunks;
using DotNetCTFDumper.MMFParser.Translation;
using DotNetCTFDumper.Utils;
using Animation = DotNetCTFDumper.MMFParser.EXE.Loaders.Objects.Animation;
using AnimationDirection = DotNetCTFDumper.MMFParser.EXE.Loaders.Objects.AnimationDirection;

namespace DotNetCTFDumper.GUI
{
    public partial class MainForm : Form
    {
        public static bool IsDumpingImages;
        public static bool IsDumpingSounds;
        public static bool IsDumpingMusics;
        public static bool BreakImages;
        public static bool BreakSounds;
        public static bool BreakMusics;
        public static bool Loaded;
        public Thread LoaderThread;
        public Color ColorTheme = Color.FromArgb(223, 114, 38);

        public delegate void SaveHandler(int index, int all);

        public delegate void IncrementSortedProgressBar(int all);



        public MainForm()
        {
            //Buttons
            InitializeComponent();
            foreach (Control item in Controls)
            {
                item.ForeColor = ColorTheme;
                if (!(item is PictureBox) && !(item is TabPage)) item.BackColor = Color.Black;

                if (item is Button) item.BackColor = Color.FromArgb(30, 30, 30);

                if (item is Label)
                {
                    //item.BackColor = Color.Transparent;
                    item.Refresh();
                }
            }

            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                foreach (Control item in tabPage.Controls)
                {
                    item.ForeColor = ColorTheme;
                    if (!(item is PictureBox) && !(item is TabPage)&&!(item is Label)) item.BackColor = Color.Black;
                    if (item is Button) item.BackColor = Color.FromArgb(30, 30, 30);

                    if (item is Label)
                    {
                        //item.BackColor = Color.Transparent;
                        item.Refresh();
                    }

                }
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

            Pame2Mfa.OnMessage += (obj) =>
            {
                var date = DateTime.Now;
                string msg = (string) obj;
                mfaLogBox.AppendText(msg.Length > 0
                    ? $"[{date.Hour,2}:{date.Minute,2}:{date.Second,2}:{date.Millisecond,3}] {msg}\r\n"
                    : "\r\n");
            };
            tabControl1.Selecting += tabControl1_Selecting;
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!Loaded)
            {
                if (e.TabPage != mainTab)
                    e.Cancel = true;
            }

        }



        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (workSender, workE) => StartReading();
            worker.RunWorkerCompleted += (workSender, workE) => AfterLoad();
            worker.RunWorkerAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            loadingLabel.Visible = false;
            listBox1.Items.Clear();
        }


        private void StartReading()
        {
            var path = openFileDialog1.FileName;
            loadingLabel.Visible = true;
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
            dumpSortedBtn.Visible = false;



        }

        private void treeView1_AfterDblClick(object sender, EventArgs e)
        {
            ChunkCombo.Show(Cursor.Position);

        }

        private void treeView1_RightClick(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) != 0)
            {
                ChunkCombo.Show(Cursor.Position);
            }

        }

        private void ChunkCombo_ItemSelected(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Name)
            {
                case "saveChunkBtn":
                    var chunk = ((ChunkNode) treeView1.SelectedNode).chunk;
                    if (chunk != null)
                    {
                        chunk.Save();
                    }

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
                listBox1.Items.Add($"Id: {nodeChunk.Id}");
                listBox1.Items.Add($"Flag: {nodeChunk.Flag}");
                listBox1.Items.Add($"Size: {nodeChunk.Size.ToPrettySize()}");
                if (nodeChunk.DecompressedSize > -1)
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
            //GameData gameData = null;
            var exe = Exe.Instance;
            var gameData = exe.GameData;


            treeView1.Nodes.Clear();
            foreach (var item in gameData.GameChunks.Chunks)
            {

                string ActualName = item.Name;
                if (item.Loader is Frame frm) ActualName = ActualName + " " + frm.Name;
                ChunkNode newNode = Helper.GetChunkNode(item, ActualName);
                treeView1.Nodes.Add(newNode);
                if (item.Loader is Frame frame)
                {
                    foreach (var frmChunk in frame.Chunks.Chunks)
                    {
                        var frameNode = Helper.GetChunkNode(frmChunk);
                        newNode.Nodes.Add(frameNode);
                        if (frameNode.loader is ObjectInstances)
                        {
                            var objs = frame.Chunks.GetChunk<ObjectInstances>();
                            if (objs != null)
                            {
                                foreach (var frmitem in objs.Items)
                                {
                                    var objNode = new ChunkNode(frmitem.Name, frmitem);
                                    frameNode.Nodes.Add(objNode);
                                }
                            }
                        }
                    }
                }
                else if (item.Loader is FrameItems items)
                {
                    foreach (var key in items.ItemDict.Keys)
                    {
                        var frameItem = items.ItemDict[key];
                        var objNode = new ChunkNode($"{(Constants.ObjectType) frameItem.ObjectType} - {frameItem.Name}",
                            frameItem);
                        newNode.Nodes.Add(objNode);

                    }
                }
            }

            FolderBTN.Visible = true;
            imagesButton.Visible = true;
            soundsButton.Visible = true;
            musicsButton.Visible = true;
            dumpSortedBtn.Visible = true;
            GameInfo.Visible = true;
            loadingLabel.Visible = false;
            Loaded = true;
            InitKeyTab();
            InitPackDataTab();
            InitAdvancedDump();
            InitPlugins();
            var toLog = "";
            toLog += $"Title:{Exe.Instance.GameData.Name}\n";
            toLog += $"Copyright:{Exe.Instance.GameData.Copyright}\n";
            //toLog += $"Editor Filename: {Exe.Instance.GameData.EditorFilename}\n";
            toLog += $"Product Version: {Exe.Instance.GameData.ProductVersion}\n";
            toLog += $"Build: {Exe.Instance.GameData.Build}\n";
            toLog += $"Runtime Version: {Exe.Instance.GameData.RuntimeVersion}\n";
            toLog += $"Number Of Images: {Exe.Instance.GameData.Images?.NumberOfItems ?? 0}\n";
            toLog += $"Number Of Sounds: {Exe.Instance.GameData.Sounds?.NumOfItems ?? 0}\n";
            toLog += $"Number Of Musics: {Exe.Instance.GameData.Music?.NumOfItems ?? 0}\n";
            toLog += $"Unique FrameItems: {Exe.Instance.GameData.Frameitems?.NumberOfItems}\n";
            toLog += $"Frame Count: {Exe.Instance.GameData.Frames.Count}\n";
            toLog += $"Chunks Count: {Exe.Instance.GameData.GameChunks.Chunks.Count}\n";
            if (Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>() != null)
                Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>().OnImageSaved += UpdateImageBar;
            if (Exe.Instance.GameData.GameChunks.GetChunk<SoundBank>() != null)
                Exe.Instance.GameData.GameChunks.GetChunk<SoundBank>().OnSoundSaved += UpdateSoundBar;
            if (Exe.Instance.GameData.GameChunks.GetChunk<MusicBank>() != null)
                Exe.Instance.GameData.GameChunks.GetChunk<MusicBank>().OnMusicSaved += UpdateMusicBar;
            ImageDumper.SortedImageSaved += IncrementSortedBar;


            GameInfo.Text = toLog;
        }




        public void UpdateImageBar(int index, int all)
        {
            all -= 1;
            imageBar.Value = (int) (index / (float) all * 100);
            imageLabel.Text = $"{index}/{all}";
        }

        public void UpdateSoundBar(int index, int all)
        {
            all -= 1;
            soundBar.Value = (int) (index / (float) all * 100);
            soundLabel.Text = $"{index}/{all}";
        }

        public void UpdateMusicBar(int index, int all)
        {
            all -= 1;
            musicBar.Value = (int) (index / (float) all * 100);
            musicLabel.Text = $"{index}/{all}";
        }

        public void IncrementSortedBar(int all)
        {
            SortedProgressBar.Visible = true;
            SortedProgressBar.Maximum = all;
            SortedProgressBar.Value += 1;
            if (SortedProgressBar.Value >= SortedProgressBar.Maximum)
            {
                SortedProgressBar.Visible = false;
            }
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
                var node = ((ChunkNode) treeView1.SelectedNode);
                HexViewForm hexform = null;

                hexform = new HexViewForm(node.chunk.ChunkData, node.chunk.RawData, ColorTheme,
                    $"Hew View: {node.chunk.Name}");

                hexform.Show();
            }

        }



        private void dumpSortedBtn_Click(object sender, EventArgs e)
        {
            imageBar.Visible = true;
            imageLabel.Visible = true;
            var worker = new BackgroundWorker();
            worker.DoWork += (senderA, eA) =>
            {
                Settings.DumpImages = true;
                var bank = Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>();
                bank.SaveImages = false;
                bank.Read();

            };
            worker.RunWorkerCompleted += (senderA, eA) =>
            {
                imageBar.Visible = false;
                imageLabel.Visible = false;
                ImageDumper.DumpImages();
            };
            worker.RunWorkerAsync();
        }







        private void dumpMFAButton_Click(object sender, EventArgs e)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (workSender, workE) => MFAGenerator.BuildMFA();
            worker.RunWorkerCompleted += (workSender, workE) =>
            {
                Logger.Log("MFA Done", true, ConsoleColor.Yellow);
                var res = MessageBox.Show("Dump Extensions?", "Finished", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    foreach (var item in Exe.Instance.PackData.Items)
                    {
                        item.Dump();
                        Pame2Mfa.Message("Dumping " + item.PackFilename);
                    }

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
                var previewKey = Decryption.MakeKeyFromBytes(rawData, (byte) int.Parse((charBox.Text)));
                hexBox1.ByteProvider = new DynamicByteProvider(previewKey);

            }
            catch
            {
                // ignored
            }
        }

        public void InitPackDataTab()
        {
            packDataListBox.Items.Clear();
            foreach (var item in Exe.Instance.PackData.Items)
            {
                packDataListBox.Items.Add(item.PackFilename);
            }

            UpdatePackInfo(0);
        }

        private void UpdatePackInfo(int index)
        {
            var item = Exe.Instance.PackData.Items[index];
            infoLabel.Text = $"Name: {item.PackFilename}\nSize: {item.Data.Length.ToPrettySize()}";
        }

        private void dumpPackButton_Click(object sender, EventArgs e)
        {
            var item = Exe.Instance.PackData.Items[packDataListBox.SelectedIndex];

            packDataDialog.FileName = item.PackFilename;
            if (item.PackFilename.EndsWith(".mfx")) packDataDialog.Filter = "Clickteam Extension(*.mfx)|.mfx";
            else if (item.PackFilename.EndsWith(".dll")) packDataDialog.Filter = "Clickteam Module(*.dll)|.dll";


            packDataDialog.InitialDirectory = Path.GetFullPath(Settings.ExtensionPath);
            packDataDialog.ShowDialog();
        }

        private void dumpAllPackButton_Click(object sender, EventArgs e)
        {
            foreach (var item in Exe.Instance.PackData.Items)
            {
                item.Dump();
            }
        }

        private void packDataDialog_FileOk(object sender, CancelEventArgs e)
        {
            var item = Exe.Instance.PackData.Items[packDataListBox.SelectedIndex];
            item.Dump(packDataDialog.FileName);
        }

        private void packDataListBox_SelectedIndexChanged(object sender, EventArgs e) =>
            UpdatePackInfo(packDataListBox.SelectedIndex);

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

        private void charBox_TextChanged(object sender, EventArgs e) => InitKeyTab();




        public void InitAdvancedDump()
        {
            var bank = Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>();
            var items = bank.Images.ToList();
            var filtered = items.OrderBy(x => x.Value.Handle);
            foreach (Frame frame in Exe.Instance.GameData.Frames)
            {
                var frameNode = new ChunkNode(frame.Name, frame);
                advancedTreeView.Nodes.Add(frameNode);
                if (frame.Objects != null)
                {
                    foreach (ObjectInstance objInst in frame.Objects.Items)
                    {
                        var objInstNode = new ChunkNode(objInst.FrameItem.Name, objInst);
                        frameNode.Nodes.Add(objInstNode);
                        var loader = objInst.FrameItem.Properties.Loader;
                        if (loader is ObjectCommon common)
                        {
                            if (common.Animations != null)
                            {
                                foreach (var pair in common.Animations.AnimationDict)
                                {
                                    var animNode = new ChunkNode($"Animation {pair.Key}", pair.Value);
                                    objInstNode.Nodes.Add(animNode);
                                    foreach (var dir in pair.Value.DirectionDict)
                                    {
                                        for (int a = 0; a < dir.Value.Frames.Count; a++)
                                        {
                                            var animFrame = dir.Value.Frames[a];
                                            bank.Images.TryGetValue(animFrame, out var img);
                                            var animFrameNode = new ChunkNode(a.ToString(), img);
                                            animNode.Nodes.Add(animFrameNode);
                                        }
                                    }
                                }
                            }
                        }
                        else if (loader is Backdrop backdrop)
                        {
                            var backdropNode = new ChunkNode("Image", bank.Images[backdrop.Image]);
                            objInstNode.Nodes.Add(backdropNode);
                        }
                    }
                }
            }
        }

        private bool breakAnim;
        private bool isAnimRunning;
        private void advancedPlayAnimation_Click(object sender, EventArgs e)
        {
            if (((ChunkNode) advancedTreeView.SelectedNode).loader is Animation anim)
            {
                if (isAnimRunning)
                {
                    breakAnim = true;
                }
                else
                {
                    isAnimRunning = true;
                    var animThread = new Thread(PlayAnimation);
                    List<Bitmap> frames = new List<Bitmap>();
                    foreach (var dir in anim.DirectionDict)
                    {
                        foreach (var frame in dir.Value.Frames)
                        {
                            frames.Add(Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>().Images[frame].Bitmap);
                        }
                        animThread.Start(new Tuple<List<Bitmap>,AnimationDirection>(frames,dir.Value));
                    }
                }

                

                
            }

        }
        public void PlayAnimation(object o)
        {
            var (frames,anim) = (Tuple<List<Bitmap>,AnimationDirection>) o;
            var fps = (float)anim.MaxSpeed;
            float delay = 1f/fps;
            int i = 0;
            if (anim.Repeat > 0)
            {
                foreach (Bitmap frame in frames)
                {
                    advancedPictureBox.Image = frame;
                    advancedInfoLabel.Text = $"Current frame: {frames.IndexOf(frame)}\nAnimation Speed: {fps}";
                    Thread.Sleep((int) (delay*1500));
                }
                isAnimRunning = false;
                Thread.CurrentThread.Abort();
            }
            else
            {
                while (true)
                {
                    var frame = frames[i];
                    advancedPictureBox.Image = frame;
                    advancedInfoLabel.Text = $"Current frame: {i.ToString()}\nAnimation Speed: {fps}";
                    Thread.Sleep((int) (delay*1500));
                    i++;
                    if (i == frames.Count) i = 0;
                    if (breakAnim)
                    {
                        isAnimRunning = false;
                        breakAnim = false;
                        Thread.CurrentThread.Abort();
                        break;
                    
                    }


                }
            }
            
            //Limited
            

        }

        private void advancedTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            if (((ChunkNode) node).loader is ImageItem)
            {
                var img = ((ImageItem) ((ChunkNode) node).loader);
                advancedPictureBox.Image = img.Bitmap;
            }
            
        }

        

        public void InitPlugins()
        {
            PluginAPI.PluginAPI.InitializePlugins();
            foreach (var plugin in PluginAPI.PluginAPI.Plugins)
            {
                pluginsList.Items.Add(plugin.Name);

            }

        }

        private void activatePluginBtn_Click(object sender, EventArgs e)
        {
            PluginAPI.PluginAPI.ActivatePlugin(PluginAPI.PluginAPI.Plugins[pluginsList.SelectedIndex]);
        }

        
    }
}
    




