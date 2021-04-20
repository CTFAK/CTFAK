using System;
using System.Diagnostics;
using CTFAK.Utils;
using CTFAK_Runtime_Tools.IO;
using CTFAK_Runtime_Tools.RuntimeParsers;

namespace CTFAK_Runtime_Tools
{
    public class RuntimeCTFGame
    {
        public Process GameProcess;
        public RuntimeStream RuntimeStream;
        public ByteReader RuntimeReader;
        public ByteWriter RuntimeWriter;
        public IntPtr PamuOffset;

        public RuntimeCTFGame(Process proc)
        {
            GameProcess = proc;
            RuntimeStream = new RuntimeStream(proc);
            RuntimeReader = new ByteReader(RuntimeStream);
            RuntimeWriter = new ByteWriter(RuntimeStream);
            PamuOffset = FindPAMUAddr();
            Console.WriteLine("PAMU OFFSET: "+PamuOffset.ToString("X8"));
            RuntimeReader.Seek(PamuOffset.ToInt64()); 
            var newData = new RuntimeGameData();
            newData.Read(RuntimeReader);
            var newPos = Memory.FindFinalAddress(RuntimeReader,new IntPtr(Pointers.CURRENT_FRAME_PTR),new []{0x10,0x0} );
            RuntimeReader.Seek(newPos.ToInt32());
            Console.WriteLine(RuntimeReader.ReadWideString());
        }
        public IntPtr FindPAMUAddr()
        {
            // var procBase = ;
            // var address = IntPtr.Add(procBase, magicOffset);
            // reader.Seek(address.ToInt32());
            // return reader.ReadInt32();
            var start = RuntimeReader.Tell();
            RuntimeReader.Seek(GetBaseAddr().ToInt32()+Pointers.PAMU_HEADER_PTR);
            var pointer = RuntimeReader.ReadInt32();
            RuntimeReader.Seek(start);
            return new IntPtr(pointer);
        }

        public IntPtr GetBaseAddr(string name = "default")
        {
            if (name == "default") return GameProcess.Modules[0].BaseAddress;
            else foreach (ProcessModule processModule in GameProcess.Modules)
            {
                if (processModule.ModuleName == name) return processModule.BaseAddress;
            }
            return IntPtr.Zero;
        }
    }
}