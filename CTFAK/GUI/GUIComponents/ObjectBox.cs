using System.Windows.Forms;
using CTFAK.MMFParser.EXE.Loaders;

namespace CTFAK.GUI.GUIComponents
{
    public class ObjectBox:PictureBox
    {
        public ObjectInstance Obj;

        public ObjectBox(ObjectInstance obj)
        {
            this.Obj = obj;
        }
    }
}