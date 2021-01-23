using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CTFAK.GUI.GUIComponents;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using Frame = CTFAK.MMFParser.EXE.Loaders.Frame;

namespace CTFAK.GUI
{
    public partial class FrameViewer : Form
    {
        private ImageBank images;
        private ObjectBox _lastSelectedObject;

        public FrameViewer()
        {
            InitializeComponent();
        }

        public FrameViewer(Frame frame, ImageBank imgs)
        {
            InitializeComponent();
            this.Width = frame.Width;
            this.Height = frame.Height;
            this.BackColor = Color.FromArgb(255,frame.Background.R,frame.Background.G,frame.Background.B);
            this.Text = "Frame Viewer: "+frame.Name;
            images = imgs;
            contextMenuStrip1.ItemClicked+= new ToolStripItemClickedEventHandler(MenuItemSelected);
            LoadObjects(frame);
        }

        public void CreateImage(ObjectInstance obj)
        {
            var pictureBox1 = new ObjectBox(obj);
            var img = obj.FrameItem.GetPreview();
            if (img != null)
            {
                pictureBox1.Anchor = AnchorStyles.None;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Location = new Point(obj.X-obj.FrameItem.GetPreview().XHotspot, obj.Y-obj.FrameItem.GetPreview().YHotspot);
                pictureBox1.ClientSize = new Size(img.Bitmap.Width, img.Bitmap.Height);
                pictureBox1.Image = img.Bitmap;
                pictureBox1.MouseClick += new MouseEventHandler(OnObjectSelected);
                Controls.Add(pictureBox1);
            }
        }

        private void OnObjectSelected(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) != 0)
            {


                _lastSelectedObject = (ObjectBox) sender;
                nameMenuItem.Text = "Name: " + _lastSelectedObject.Obj.Name;
                positionMenuItem.Text = $"Position: {_lastSelectedObject.Obj.X}x{_lastSelectedObject.Obj.Y}";
                contextMenuStrip1.Show(Cursor.Position);
            }
            else
            {
               
            }
        }

        private void LoadObjects(Frame frame)
        {
            var size = new Size(Exe.Instance.GameData.Header.WindowWidth,Exe.Instance.GameData.Header.WindowHeight);
            //scrollableControl1.Size = new Size(frame.Width,frame.Height);;
            ClientSize = size;
            var list = frame.Objects.OrderBy(x=>x.Handle);
            foreach (var obj in list)
            {
                //TODO:Layers
                CreateImage(obj);
            }
        }


        

        private void MenuItemSelected(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "deleteObjBtn")
            {
                Controls.Remove(_lastSelectedObject);
                _lastSelectedObject.Dispose();
                
            }
            
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
           
        }
    }

}
