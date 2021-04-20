using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using CTFAK.Utils;
using CTFAK_Runtime_Tools.IO;
using CTFAK_Runtime_Tools.RuntimeParsers;
using CTFAK_Runtime_Tools.RuntimeParsers.RuntimeChunks;

namespace CTFAK_Runtime_Tools
{
    public class RuntimeCTFGame
    {
        public Process GameProcess;
        public RuntimeStream RuntimeStream;
        public ByteReader RuntimeReader;
        public ByteWriter RuntimeWriter;
        public IntPtr PamuOffset;
        public RuntimeGameData GameData;
        public delegate void CTFReadingEvent(object data);

        public event CTFReadingEvent ProcessAttached;
        public event CTFReadingEvent DataReadingFinished;
        public RuntimeCTFGame(Process proc)
        {
            GameProcess = proc;
        }

        public void Read()
        {
            Logger.Log("CONNECTING TO PROCESS: "+GameProcess.ProcessName);
            RuntimeStream = new RuntimeStream(GameProcess);
            RuntimeReader = new ByteReader(RuntimeStream);
            RuntimeWriter = new ByteWriter(RuntimeStream);
            ProcessAttached?.Invoke(null);
            
            PamuOffset = FindPAMUAddr();
            
            RuntimeReader.Seek(PamuOffset.ToInt64()); 
            GameData = new RuntimeGameData();
            GameData.Read(RuntimeReader);
            DataReadingFinished?.Invoke(GameData);
            GetLoadedSounds();
        }
        
        public IntPtr FindPAMUAddr()
        {
            var start = RuntimeReader.Tell();
            RuntimeReader.Seek(GetBaseAddr().ToInt32()+Pointers.PamuHeaderOff);
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

        public RFrame GetCurrentFrame()
        {
            var start = RuntimeReader.Tell();
            RuntimeReader.Seek(PamuOffset.ToInt32()+Pointers.CURRENT_FRAME_OFFSET);
            RuntimeReader.Seek(RuntimeReader.ReadInt32());
            var newFrame = new RFrame(RuntimeReader);
            newFrame.Read();
            RuntimeReader.Seek(start);
            return newFrame;
        }

        public List<RObjectInfo> GetObjectList()
        {
            var start = RuntimeReader.Tell();
            List<RObjectInfo> objects = new List<RObjectInfo>();
            RuntimeReader.Seek(PamuOffset.ToInt32()+Pointers.OBJ_LIST_OFFSET);
            
            var count = RuntimeReader.ReadInt32();
            var handlesCount = RuntimeReader.ReadInt32();
            var handlesPtr = RuntimeReader.ReadInt32();
            RuntimeReader.Seek(RuntimeReader.ReadInt32());
            for(int i=0;i<count;i++)
            {
                if (RuntimeReader.PeekInt32()<0) break;//make sure not to fuckup
                var newPtr = RuntimeReader.ReadInt32();
                var newStart = RuntimeReader.Tell();
                
                RuntimeReader.Seek(newPtr);
                var newObject = new RObjectInfo(RuntimeReader);
                newObject.Read();
                RuntimeReader.Seek(newStart);
                objects.Add(newObject);
            }
            return objects;
        }

        public List<SoundItem> GetLoadedSounds()
        {
            List<SoundItem> items = new List<SoundItem>();
            var start = RuntimeReader.Tell();
            var newaddy = Memory.FindFinalAddress(RuntimeReader, PamuOffset, new[] {Pointers.SND_LIST_OFFSET, 0x14});
            RuntimeReader.Seek(newaddy.ToInt32());
            
            var count = RuntimeReader.ReadInt32();
            Console.WriteLine(count);
            var unk1 = RuntimeReader.ReadInt32();
            var unk2 = RuntimeReader.ReadInt32();
            var unk3 = RuntimeReader.ReadInt32();
            Directory.CreateDirectory("sounds");
            for (int i = 0; i < count; i++)
            {
                var newAddy = RuntimeReader.ReadInt32();
                var newStart = RuntimeReader.Tell();
                if (newAddy != 0)
                {
                    RuntimeReader.Seek(newAddy);
                    var newSound = new RSoundItem(RuntimeReader);
                    newSound.Read();
                    RuntimeReader.Seek(newStart);
                    File.WriteAllBytes($"sounds\\{Helper.CleanInput(newSound.Name)}.wav",newSound.Data);
                    items.Add(newSound);
                }
            }
            RuntimeReader.Seek(start);
            return items;
        }
    }
    
}