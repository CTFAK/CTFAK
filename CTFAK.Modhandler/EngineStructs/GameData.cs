using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CTFAK.Modhandler.EngineStructs
{
    public struct GameDataStruct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] header;
    }
    public class GameData
    {
        public GameData(IntPtr pointer)
        {
            this.pointer = pointer;
        }
        private GameDataStruct _struct=>Marshal.PtrToStructure<GameDataStruct>(pointer);
        private IntPtr pointer;
        public string Header => new string(_struct.header);
    }
}
