using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Be.Windows.Forms;
using NetMFAPatcher.MMFParser.ChunkLoaders;
using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.MMFParser.Decompiling;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.GUI
{
    public partial class MainForm : Form
    {
        public static bool IsDumpingImages;
        public static bool IsDumpingSounds;
        public static bool BreakImages;
        public static bool BreakSounds;
        public Thread LoaderThread;
        public Color ColorTheme = Color.FromArgb(223,114,38);
        public PackDataForm PackForm;

        public MainForm()
        {
            //Buttons
            InitializeComponent();
            cryptKeyBtn.ForeColor = ColorTheme;
            dumpSortedBtn.ForeColor = ColorTheme;
            showHexBtn.ForeColor = ColorTheme;
            FolderBTN.ForeColor = ColorTheme; 
            MFABtn.ForeColor = ColorTheme;
            ImagesButton.ForeColor = ColorTheme;
            SoundsButton.ForeColor = ColorTheme;
            packDataBtn.ForeColor = ColorTheme;
            //Menu
            saveChunkBtn.ForeColor = ColorTheme;
            saveChunkBtn.BackColor=Color.Black;                       
            viewHexBtn.ForeColor = ColorTheme; 
            viewHexBtn.BackColor=Color.Black;
            //Labels
            label1.ForeColor = ColorTheme;
            label1.Text = Settings.DumperVersion;
            button1.ForeColor = ColorTheme;
            GameInfo.ForeColor = ColorTheme;
            loadingLabel.ForeColor = ColorTheme;
            ImagesLabel.ForeColor = ColorTheme;
            SoundsLabel.ForeColor=ColorTheme;
            //Other
            treeView1.ForeColor = ColorTheme;
            listBox1.ForeColor = ColorTheme;
            ImagesBar.ForeColor = ColorTheme;
            SoundBar.ForeColor = ColorTheme;
            

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (senderA, eA) => { StartReading(); };
            worker.RunWorkerCompleted += (senderA, eA) => { AfterLoad(); };
            worker.RunWorkerAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            loadingLabel.Visible = false;
        }

        private void StartReading()
        {
            var path = openFileDialog1.FileName;
            ImagesBar.Value = 0;
            SoundBar.Value = 0;
            GameInfo.Text = "";
            loadingLabel.Visible = true;
            ImagesLabel.Text = "0/0";
            SoundsLabel.Text = "0/0";
            ImageBox.Enabled = false;
            SoundBox.Enabled = false;
            ChunkBox.Enabled = false;
            MFABtn.Visible = false;
            FolderBTN.Visible = false;
            ImagesButton.Visible = false;
            SoundsButton.Visible = false;
            cryptKeyBtn.Visible = false;
            showHexBtn.Visible = false;
            dumpSortedBtn.Visible = false;
            packDataBtn.Visible = false;


            Program.ReadFile(path, Settings.Verbose, Settings.DumpImages, Settings.DumpSounds);
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
                
            }
            
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var nodeChunk = ((ChunkNode) treeView1.SelectedNode).chunk;
            var nodeLoader = ((ChunkNode) treeView1.SelectedNode).loader;
            Console.WriteLine("NodeChunk:"+nodeChunk!=null);
            Console.WriteLine("NodeLoader:"+nodeLoader!=null);
            
            ChunkList.Chunk chunk = null;
            listBox1.Items.Clear();
            if (nodeChunk != null) chunk = nodeChunk;
            //if (nodeLoader.Chunk != null) chunk = nodeLoader.Chunk;
            if (chunk != null)
            {
                chunk = nodeChunk;
                listBox1.Items.Add($"Name: {chunk.Name}");
                listBox1.Items.Add($"Id: {chunk.Id}");
                listBox1.Items.Add($"Flag: {chunk.Flag}");
                listBox1.Items.Add($"Size: {chunk.Size.ToPrettySize()}");
                if (chunk.DecompressedSize>-1)listBox1.Items.Add($"Decompressed Size: {chunk.DecompressedSize.ToPrettySize()}");
                
                    
                
                
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
        }

        public void AfterLoad()
        {
            GameData gameData = null;

            gameData = Exe.LatestInst.GameData;


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
                        var frameNode = Helper.GetChunkNode(frmChunk);//new ChunkNode(frmChunk.Name, frmChunk);
                        newNode.Nodes.Add(frameNode);
                        if (frameNode.loader is ObjectInstances)
                        {
                            var objs = frame.Chunks.get_chunk<ObjectInstances>();
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

            ImageBox.Enabled = true;
            SoundBox.Enabled = true;
            ChunkBox.Enabled = true;
            MFABtn.Visible = true;
            FolderBTN.Visible = true;
            ImagesButton.Visible = true;
            SoundsButton.Visible = true;
            cryptKeyBtn.Visible = true;
            //showHexBtn.Visible = true;
            dumpSortedBtn.Visible = true;
            packDataBtn.Visible = true;
            GameInfo.Visible = true;
            loadingLabel.Visible = false;
            var toLog = "";
            toLog += $"Title:{Exe.LatestInst.GameData.Name}\n";
            toLog += $"Copyright:{Exe.LatestInst.GameData.Copyright}\n";
            toLog += $"Editor Filename: {Exe.LatestInst.GameData.EditorFilename}\n";
            toLog += $"Product Version: {Exe.LatestInst.GameData.ProductVersion}\n";
            toLog += $"Build: {Exe.LatestInst.GameData.Build}\n";
            toLog += $"Runtime Version: {Exe.LatestInst.GameData.RuntimeVersion}\n";
            toLog += $"Number Of Images: {Exe.LatestInst.GameData.Images.NumberOfItems}\n";
            toLog += $"Number Of Sounds: {Exe.LatestInst.GameData.Sounds.NumOfItems}\n";
            toLog += $"Unique FrameItems: {Exe.LatestInst.GameData.Frameitems.NumberOfItems}\n";
            toLog += $"Frame Count: {Exe.LatestInst.GameData.Frames.Count}\n";
            toLog += $"Chunks Count: {Exe.LatestInst.GameData.GameChunks.Chunks.Count}\n";
            



            //toLog += $"Runtime Subversion: {Exe.LatestInst.GameData.RuntimeSubversion}\n";
                        
            GameInfo.Text = toLog;
        }


        private void ImageBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.DumpImages = ImageBox.Checked;
            ImagesBar.Visible = ImageBox.Checked;
            ImagesLabel.Visible = ImageBox.Checked;
        }

        private void SoundBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.DumpSounds = SoundBox.Checked;
            SoundBar.Visible = SoundBox.Checked;
            SoundsLabel.Visible = SoundBox.Checked;
        }

        private void ChunkBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SaveChunks = ChunkBox.Checked;
        }

        public void UpdateImageBar(int index, int all)
        {
            all -= 1;
            ImagesBar.Value = (int) (index / (float) all * 100);
            ImagesLabel.Text = $"{index}/{all}";
        }

        public void UpdateSoundBar(int index, int all)
        {
            all -= 1;
            SoundBar.Value = (int) (index / (float) all * 100);
            SoundsLabel.Text = $"{index}/{all}";
        }

        private void FolderBTN_Click(object sender, EventArgs e)
        {
            Process.Start($"{Settings.DumpPath}");
        }

        private void MFABtn_Click(object sender, EventArgs e)
        {
            MFAGenerator.BuildMFA();
        }

        private void SoundsButton_Click(object sender, EventArgs e)
        {
            if (!IsDumpingSounds)
            {
                SoundBar.Visible = true;
                SoundsLabel.Visible = true;
                SoundsButton.Text = "Cancel";
                IsDumpingSounds = true;
                var worker = new BackgroundWorker();
                worker.DoWork += (senderA, eA) =>
                {
                    var cachedImgState = Settings.DumpSounds;
                    Settings.DumpSounds = true;
                    Exe.LatestInst.GameData.GameChunks.get_chunk<SoundBank>().Read();
                    Settings.DumpSounds = cachedImgState;
                };
                worker.RunWorkerCompleted += (senderA, eA) =>
                {
                    SoundBar.Visible = false;
                    SoundsLabel.Visible = false;
                    Logger.Log("Sounds done");
                    SoundsButton.Text = "Dump Sounds";
                };
                worker.RunWorkerAsync();
            }
            else
            {
                BreakSounds = true;
                SoundBar.Visible = false;
                SoundsLabel.Visible = false;
                SoundsButton.Text = "Dump Sounds";
                IsDumpingSounds = false;
            }
        }

        private void ImagesButton_Click(object sender, EventArgs e)
        {
            if (!IsDumpingImages)
            {
                ImagesBar.Visible = true;
                ImagesLabel.Visible = true;
                ImagesButton.Text = "Cancel";
                IsDumpingImages = true;
                //ImagesLabel.BackColor=Color.Transparent;
                //ImagesLabel.ForeColor=Color.Red;

                ;
                var worker = new BackgroundWorker();
                worker.DoWork += (senderA, eA) =>
                {
                    var cachedImgState = Settings.DumpImages;
                    Settings.DumpImages = true;
                    Exe.LatestInst.GameData.GameChunks.get_chunk<ImageBank>().Read();
                    Settings.DumpImages = cachedImgState;
                };
                worker.RunWorkerCompleted += (senderA, eA) =>
                {
                    ImagesBar.Visible = false;
                    ImagesLabel.Visible = false;
                    ImagesButton.Text = "Dump\nImages";
                    Logger.Log("Images done");
                };
                worker.RunWorkerAsync();
            }
            else
            {
                BreakImages = true;
                ImagesBar.Visible = false;
                ImagesLabel.Visible = false;
                ImagesButton.Text = "Dump\nImages";
                IsDumpingImages = false;
            }
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

        private void ShowHex_Click(object sender, EventArgs e)
        {
            ShowHex();
            
        }

        private void ShowHex()
        {
        if ((ChunkNode) treeView1.SelectedNode != null)
                    {
                        var node = ((ChunkNode) treeView1.SelectedNode);
                        HexViewForm hexform = null;
                        
                            hexform = new HexViewForm(node.chunk.ChunkData,node.chunk.RawData,ColorTheme,$"Hew View: {node.chunk.Name}"); 
                        
                        hexform.Show();
                    }
            
        }

        private void loadingLabel_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void dumpSortedBtn_Click(object sender, EventArgs e)
        {
            ImageDumper.DumpImages();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //throw new System.NotImplementedException();
        }

       

        private void ChunkCombo_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void packDataBtn_Click(object sender, EventArgs e)
        {
            if(PackForm==null)PackForm = new PackDataForm(Exe.LatestInst.PackData,ColorTheme);
            PackForm.Show();
        }
    }
}