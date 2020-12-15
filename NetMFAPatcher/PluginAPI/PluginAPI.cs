using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DotNetCTFDumper.MMFParser.EXE.Loaders;
using DotNetCTFDumper.MMFParser.MFA;
using DotNetCTFDumper.MMFParser.MFA.Loaders;
using Frame = DotNetCTFDumper.MMFParser.MFA.Loaders.Frame;
using Layer = DotNetCTFDumper.MMFParser.MFA.Loaders.Layer;

namespace DotNetCTFDumper.PluginAPI
{
    public class PluginAPI
    {
        public static Frame GetEmptyFrame(List<Color> palette,int handle=0,int x=640,int y=480,string name="New Frame")
        {
            var frame = new Frame(null)
            {
                Handle = 0,
                Name = name,
                Password = "",
                SizeX = x,
                SizeY = y,
                Background = Color.Green,
                Flags = 260,
                Palette = palette,
                Layers = new List<Layer>(),
                Folders = new List<ItemFolder>(),
                Items = new List<FrameItem>(),
                Events = MFA.emptyEvents,
                Chunks = MFA.emptyFrameChunks
                
            };
            //frame.Instances = template.Frames[0].Instances;
            var testLayer = new Layer(null) {Name = "New Super Layer"};
            frame.Layers.Add(testLayer);
            

            
            
            return frame;

        }
        
    }
}