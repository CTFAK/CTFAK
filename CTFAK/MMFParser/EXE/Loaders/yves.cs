using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;
using CTFAK.Utils;
using static CTFAK.MMFParser.EXE.ChunkList;

namespace CTFAK.MMFParser.EXE.Loaders
{
    public class AppIcon : ChunkLoader
    {
        public AppIcon(ByteReader reader) : base(reader)
        {
        }

   

        public override void Read()
        {
            
        }

        public override void Write(ByteWriter Writer)
        {
            throw new NotImplementedException();
        }


        public override void Print(bool ext)
        {
        }

        public override string[] GetReadableData()
        {
            return Array.Empty<string>();
        }
    }
}