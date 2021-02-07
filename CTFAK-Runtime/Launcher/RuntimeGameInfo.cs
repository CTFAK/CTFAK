using System.Collections.Generic;
using System.Drawing;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using CTFAK_Runtime.RuntimeObjects;

namespace CTFAK_Runtime.Launcher
{
    public class RuntimeGameInfo
    {
        public int ScreenWidth;
        public int ScreenHeight;
        public string AppName;
        public Bitmap AppIcon;
        public List<FrameInfo> Frames = new List<FrameInfo>();
        public Dictionary<int, ImageItem> Images = new Dictionary<int, ImageItem>();
    }
}