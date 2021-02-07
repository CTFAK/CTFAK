using System.Drawing;
using System.Windows.Forms;

namespace CTFAK_Runtime.RuntimeObjects
{
    public class Backdrop:RuntimeObject
    {
        public int ImageHandle;

        public static Backdrop Create(CTFAK.MMFParser.MFA.Loaders.mfachunks.Backdrop orig)
        {
            var newBackdrop = new Backdrop();
            newBackdrop.ImageHandle = orig.Handle;
            return newBackdrop;
        }

        public override void Spawn(Form Window)
        {
            var img = Program.AppInfo.Images[ImageHandle];
            this.Size=new Size(img.Bitmap.Width,img.Bitmap.Height);
            this.Image = img.Bitmap;
            Window.Controls.Add(this);
            this.Location=new Point(640/2,480/2);
        }
    }
}