using System.Windows.Forms;
using DotNetCTFDumper.MMFParser.ChunkLoaders;

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