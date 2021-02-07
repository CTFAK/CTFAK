using System.Collections.Generic;
using System.Drawing;
using CTFAK.MMFParser.MFA.Loaders;

namespace CTFAK_Runtime.RuntimeObjects
{
    public class FrameInfo
    {
        public Color BackgroundColor;
        public List<RuntimeObject> Items = new List<RuntimeObject>();
        public List<ObjectInstance> Instances = new List<ObjectInstance>();
        public static FrameInfo Create(Frame frame)
        {
            var frameInfo = new FrameInfo();
            frameInfo.BackgroundColor = Color.FromArgb(255,frame.Background.R,frame.Background.G,frame.Background.B);
            foreach (FrameItem item in frame.Items)
            {
                frameInfo.Items.Add(RuntimeObject.Create(item));
            }
            foreach (FrameInstance item in frame.Instances)
            {
                frameInfo.Instances.Add(ObjectInstance.Create(item));
            }
            return frameInfo;
        }
        
    }
}