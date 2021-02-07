using System.Drawing;
using System.Windows.Forms;
using CTFAK.MMFParser.MFA.Loaders;

namespace CTFAK_Runtime.RuntimeObjects
{
    public class ObjectInstance
    {
        public uint ObjectHandle;
        public int X;
        public int Y;

        public static ObjectInstance Create(FrameInstance insatnce)
        {
            var newInst = new ObjectInstance();
            newInst.ObjectHandle = insatnce.ItemHandle;
            newInst.X = insatnce.X;
            newInst.Y = insatnce.Y;
            return newInst;
        }

        public void Spawn(FrameInfo frame,Form window)
        {
            foreach (RuntimeObject frameItem in frame.Items)
            {
                if (frameItem.ID == this.ObjectHandle)
                {
                    frameItem.Spawn(window);
                    frameItem.Location=new Point(X,Y);
                }
            }
        }
    }
}