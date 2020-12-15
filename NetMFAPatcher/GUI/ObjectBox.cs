using System.Windows.Forms;
using DotNetCTFDumper.MMFParser.EXE.Loaders;

namespace DotNetCTFDumper.GUI
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