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
        public Thread loaderThread;
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
            var currentFrame = EXE.LatestInst.game_data.Frames[treeView1.SelectedNode.Index];
            listBox1.Items.Clear();
            listBox1.Items.Add($"Size: {currentFrame.width}x{currentFrame.height}");
            listBox1.Items.Add($"Number of objects: {currentFrame.CountOfObjs}");
            
        }
        public void AfterLoad()
        {

            var gameData = EXE.LatestInst.game_data;
            foreach (var item in gameData.Frames)
            {
                treeView1.Nodes.Add(item.name);    
            }
            string toLog = "";
            toLog += $"Title:{EXE.LatestInst.game_data.Name}\n";
            toLog += $"Copyright:{EXE.LatestInst.game_data.Copyright}\n";
            toLog += $"Editor Filename: {EXE.LatestInst.game_data.EditorFilename}\n";
            //toLog += $"Build Filename: {EXE.LatestInst.game_data.TargetFilename}\n";
            toLog += $"Product Build: {EXE.LatestInst.game_data.product_build}\n";
            toLog += $"Build: {EXE.LatestInst.game_data.build}\n";
            toLog += $"Runtime Version: {EXE.LatestInst.game_data.runtime_version}\n";



            GameInfo.Text = toLog;

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
