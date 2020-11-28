using NetMFAPatcher.MMFParser.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetMFAPatcher.GUI
{
    public partial class MainForm : Form
    {
        public Thread LoaderThread;
        public MainForm()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (senderA, eA) =>
            {
                StartReading();
            };
            worker.RunWorkerCompleted += (senderA, eA) =>
            {
                AfterLoad();
            };
            worker.RunWorkerAsync();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();


        }
        void StartReading()
        {
            var path = openFileDialog1.FileName;
            Program.ReadFile(path, false, false, false);


        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var currentFrame = Exe.LatestInst.GameData.Frames[treeView1.SelectedNode.Index];
            listBox1.Items.Clear();
            listBox1.Items.Add($"Size: {currentFrame.Width}x{currentFrame.Height}");
            listBox1.Items.Add($"Number of objects: {currentFrame.CountOfObjs}");
            
        }
        public void AfterLoad()
        {

            var gameData = Exe.LatestInst.GameData;
            foreach (var item in gameData.Frames)
            {
                treeView1.Nodes.Add(item.Name);    
            }
            string toLog = "";
            toLog += $"Title:{Exe.LatestInst.GameData.Name}\n";
            toLog += $"Copyright:{Exe.LatestInst.GameData.Copyright}\n";
            toLog += $"Editor Filename: {Exe.LatestInst.GameData.EditorFilename}\n";
            //toLog += $"Build Filename: {EXE.LatestInst.game_data.TargetFilename}\n";
            toLog += $"Product Build: {Exe.LatestInst.GameData.ProductBuild}\n";
            toLog += $"Build: {Exe.LatestInst.GameData.Build}\n";
            toLog += $"Runtime Version: {Exe.LatestInst.GameData.RuntimeVersion}\n";



            GameInfo.Text = toLog;

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
