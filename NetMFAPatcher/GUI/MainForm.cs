using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.MMFParser.EXE.Loaders;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Banks;
using DotNetCTFDumper.Utils;

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
        public Thread LoaderThread;
        public Color ColorTheme = Color.FromArgb(223,114,38);
        public PackDataForm PackForm;
        
        public delegate void SaveHandler(int index, int all);
        public MainForm()
        {
            //Buttons
            InitializeComponent();
            foreach (Control item in Controls)
            {
                item.ForeColor = ColorTheme;
                item.BackColor=Color.Black;
                if(item is Button) item.BackColor=Color.FromArgb(30,30,30);

                if (item is Label)
                {
                    item.BackColor = Color.Transparent;
                    item.Refresh();
                }

            }
            
            foreach (var item in ChunkCombo.Items)
            {
                ((ToolStripItem)item).ForeColor = ColorTheme;
                ((ToolStripItem)item).BackColor=Color.Black;
            }
            
            label1.Text = Settings.DumperVersion;

            
            
            

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted +=   worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            StartReading();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AfterLoad();
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
            Program.ReadFile(path, Settings.Verbose, Settings.DumpImages, Settings.DumpSounds);
            imageBar.Value = 0;
            soundBar.Value = 0;
            GameInfo.Text = "";
            loadingLabel.Visible = true;
            imageLabel.Text = "Using nonGUI mode";
            soundLabel.Text = "Using nonGUI mode";

            MFABtn.Visible = false;
            FolderBTN.Visible = false;
            imagesButton.Visible = false;
            soundsButton.Visible = false;
            musicsButton.Visible = false;
            cryptKeyBtn.Visible = false;
            showHexBtn.Visible = false;
            dumpSortedBtn.Visible = false;
            packDataBtn.Visible = false;
            

            
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
                    if ( chunk!= null)
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
                        var viewer = new FrameViewer(frm,Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>());
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
                if (nodeChunk.DecompressedSize>-1)listBox1.Items.Add($"Decompressed Size: {nodeChunk.DecompressedSize.ToPrettySize()}");
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
            GameInfo.BackColor=Color.Transparent;
            
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
                if (item.Loader is Frame frm) ActualName = ActualName + " "+frm.Name;
                ChunkNode newNode = Helper.GetChunkNode(item,ActualName);
                //if (item.Loader != null) newNode = new ChunkNode(ActualName, item.Loader);
                //else newNode = new ChunkNode(ActualName, item);
                
                    
                
                
                treeView1.Nodes.Add(newNode);
                if (item.Loader is Frame frame)
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

            
            MFABtn.Visible = true;
            FolderBTN.Visible = true;
            imagesButton.Visible = true;
            soundsButton.Visible = true;
            musicsButton.Visible = true;
            cryptKeyBtn.Visible = true;
            //showHexBtn.Visible = true;
            dumpSortedBtn.Visible = true;
            packDataBtn.Visible = true;
            GameInfo.Visible = true;
            loadingLabel.Visible = false;
            var toLog = "";
            toLog += $"Title:{Exe.Instance.GameData.Name}\n";
            toLog += $"Copyright:{Exe.Instance.GameData.Copyright}\n";
            toLog += $"Editor Filename: {Exe.Instance.GameData.EditorFilename}\n";
            toLog += $"Product Version: {Exe.Instance.GameData.ProductVersion}\n";
            toLog += $"Build: {Exe.Instance.GameData.Build}\n";
            toLog += $"Runtime Version: {Exe.Instance.GameData.RuntimeVersion}\n";
            toLog += $"Number Of Images: {Exe.Instance.GameData.Images?.NumberOfItems}\n";
            toLog += $"Number Of Sounds: {Exe.Instance.GameData.Sounds?.NumOfItems}\n";
            toLog += $"Unique FrameItems: {Exe.Instance.GameData.Frameitems?.NumberOfItems}\n";
            toLog += $"Frame Count: {Exe.Instance.GameData.Frames.Count}\n";
            toLog += $"Chunks Count: {Exe.Instance.GameData.GameChunks.Chunks.Count}\n";
            if (Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>() != null)
                Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>().OnImageSaved += UpdateImageBar;
            if (Exe.Instance.GameData.GameChunks.GetChunk<SoundBank>() != null)
                Exe.Instance.GameData.GameChunks.GetChunk<SoundBank>().OnSoundSaved += UpdateSoundBar;
            if (Exe.Instance.GameData.GameChunks.GetChunk<MusicBank>() != null)
                Exe.Instance.GameData.GameChunks.GetChunk<MusicBank>().OnMusicSaved += UpdateMusicBar;
            
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


        private void FolderBTN_Click(object sender, EventArgs e)
        {
            Process.Start($"{Settings.DumpPath}");
        }

        private void MFABtn_Click(object sender, EventArgs e)
        {
            var mfaForm = new MFABuilderForm(ColorTheme);
            mfaForm.Show();
            
        }

        private void soundsButton_Click(object sender, EventArgs e)
        {
            if (!IsDumpingSounds)
            {
                SetSoundElements(true);
                IsDumpingSounds = true;
                Backend.DumpSounds(this,true,true);
                
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
            if (!IsDumpingImages)
            {
                SetImageElements(true);
                IsDumpingImages = true;
                Backend.DumpImages(this,true,true);
                
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
            
            if (!IsDumpingMusics)
            {
                SetMusicElements(true);
                IsDumpingMusics = true;
                Backend.DumpMusics(this,true,true);
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
            soundsButton.Text = state ? "Cancel":"Dump Sounds";
            soundBar.Value = 0;
        }
        public void SetImageElements(bool state)
        {
            imageBar.Visible = state;
            imageLabel.Visible = state;
            imagesButton.Text = state ? "Cancel":"Dump Images";
            imageBar.Value = 0;
        }
        public void SetMusicElements(bool state)
        {
            musicBar.Visible = state;
            musicLabel.Visible = state;
            musicsButton.Text = state ? "Cancel":"Dump Musics";
            musicBar.Value = 0;
        }

        

        


        private void cryptKeyBtn_Click(object sender, EventArgs e)
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

            var KeyForm = new CryptoKeyForm(rawData,ColorTheme);
            KeyForm.Show();
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
                bank.SaveImages=false;
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

        

        private void tabPage1_Click(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void packDataBtn_Click(object sender, EventArgs e)
        {
            if(PackForm==null)PackForm = new PackDataForm(Exe.Instance.PackData,ColorTheme);
            PackForm.Show();
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            var myPic = new PictureBox();
            var img = Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>().Images[65].Bitmap;
            myPic.SizeMode = PictureBoxSizeMode.StretchImage;
            
            myPic.Image = img;
            myPic.Width = img.Width / 2;
            myPic.Height = img.Height / 2;
            Controls.Add(myPic);

        }
    }
}