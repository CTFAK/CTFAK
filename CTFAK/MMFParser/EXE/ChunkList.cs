using System;
using System.Collections.Generic;
using System.IO;
using DotNetCTFDumper.MMFParser.EXE.Loaders;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Banks;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Events;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.EXE
{
    public class ChunkList
    {
        public List<Chunk> Chunks = new List<Chunk>();
        public bool Verbose = false;
        public List<Frame> Frames = new List<Frame>();

        public void Read(ByteReader reader)
        {
            Chunks.Clear();
            while (true)
            {
                Chunk chunk = new Chunk(Chunks.Count, this);
                chunk.Verbose = Verbose;
                chunk.Read(reader);
                chunk.Loader = LoadChunk(chunk);
                Chunks.Add(chunk);
                if (chunk.Id == 8750) chunk.BuildKey();
                if (reader.Tell() >= reader.Size()) break;
                if (chunk.Id == 32639) break; //LAST chunkID
            }
        }

        public class Chunk
        {
            ChunkList _chunkList;
            public string Name = "UNKNOWN";
            int _uid;
            public int Id = 0;
            
            public ChunkLoader Loader;
            public byte[] ChunkData;
            public byte[] RawData;
            public ChunkFlags Flag;
            public int Size;
            public int DecompressedSize = -1;
            public bool Verbose = false;

            public Chunk(int actualuid, ChunkList actualChunkList)
            {
                _uid = actualuid;
                _chunkList = actualChunkList;
            }

            public ByteReader GetReader()
            {
                return new ByteReader(ChunkData);
            }

            public void Read(ByteReader exeReader)
            {
                
                Id = exeReader.ReadInt16();
                Name = _chunkList.GetNameByID(Id);

                Flag = (ChunkFlags) exeReader.ReadInt16();
                Size = exeReader.ReadInt32();
                if((Id!=26214&&Id!=26216)) //To prevent RAM from commiting suicide
                {                
                    RawData = exeReader.ReadBytes(Size);
                    exeReader.BaseStream.Position -= Size;
                    //Saving raw data cuz why not 
                }

                switch (Flag)
                {
                    case ChunkFlags.Encrypted:                       
                        ChunkData = Decryption.DecodeChunk(exeReader.ReadBytes(Size),Size);
                        break;
                    case ChunkFlags.CompressedAndEncrypyed:
                        ChunkData = Decryption.DecodeMode3(exeReader.ReadBytes(Size), Size,Id,out DecompressedSize);
                        break;
                    case ChunkFlags.Compressed:
                        ChunkData = Decompressor.Decompress(exeReader,out DecompressedSize);
                        break;
                    case ChunkFlags.NotCompressed:
                        ChunkData = exeReader.ReadBytes(Size);
                        break;
                }
                Save();


            }

            public void Save()
            {
                if (ChunkData != null)
                {
                    string path = $"{Settings.ChunkPath}\\{Name}.chunk";
                    File.WriteAllBytes(path, ChunkData);
                }

            }
            
            public void Print(bool extented)
            {
                if(extented)
                {
                    Logger.Log($"Chunk: {Name} ({_uid})", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    ID: {Id} - 0x{Id.ToString("X")}", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    Flags: {Flag}", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    Loader: {(Loader != null ? Loader.GetType().Name : "Empty Loader")}", true,ConsoleColor.DarkCyan);
                    Logger.Log($"    Size: {Size} B", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    Decompressed Size: {DecompressedSize} B", true, ConsoleColor.DarkCyan);
                    Logger.Log("---------------------------------------------", true, ConsoleColor.DarkCyan);
                    

                }
                else
                {
                    Logger.Log($"Chunk: {Name} ({_uid})", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    ID: {Id} - 0x{Id.ToString("X")}", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    Decompressed Size: {DecompressedSize} B", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    Flags: {Flag}", true, ConsoleColor.DarkCyan);
                    Logger.Log("---------------------------------------------", true, ConsoleColor.DarkCyan);
                }
                
            }
            public void BuildKey()
            {
                
                
                
                Settings.AppName=_chunkList.GetChunk<AppName>()?.Value??"";
                Settings.Copyright = _chunkList.GetChunk<Copyright>()?.Value??"";
                Settings.ProjectPath = _chunkList.GetChunk<EditorFilename>()?.Value??"";
               

                if (Exe.Instance.GameData.ProductBuild > 284)Decryption.MakeKey(Settings.AppName,Settings.Copyright,Settings.ProjectPath);
                else Decryption.MakeKey(Settings.ProjectPath, Settings.AppName, Settings.Copyright);



            }
        }

        public enum ChunkFlags
        {
            NotCompressed = 0,
            Compressed = 1,
            Encrypted = 2,
            CompressedAndEncrypyed = 3
        }

        public ChunkLoader LoadChunk(Chunk chunk)
        {
            ChunkLoader loader = null;
            switch (chunk.Id)
            {
                case 8739:
                    loader = new AppHeader(chunk);
                    break;
                case 8740:
                    loader = new AppName(chunk);
                    break;
                case 8741:
                    loader = new AppAuthor(chunk);
                    break;
                case 8743:
                    loader = new ExtPath(chunk);
                    break;
                case 8750:
                    loader = new EditorFilename(chunk);
                    break;
                case 8751:
                    loader = new TargetFilename(chunk);
                    break;
                case 8752:
                    loader = new AppDoc(chunk);
                    break;
                case 8745:
                    loader = new FrameItems(chunk);
                    break;
                case 8757:
                    loader = new AppIcon(chunk);
                    break;
                case 8762:
                    loader = new AboutText(chunk);
                    break;
                case 8763:
                    loader = new Copyright(chunk);
                    break;
                case 13123:
                    loader = new DemoFilePath(chunk);
                    break;
                case 13109:
                    loader = new FrameName(chunk);
                    break;
                case 13107:
                    loader = new Frame(chunk);
                    Frames.Add((Frame)loader);
                    break;
                case 13108:
                    loader = new FrameHeader(chunk);
                    break;
                case 13111:
                    loader = new FramePalette(chunk);
                    break;
                case 13112:
                    loader = new ObjectInstances(chunk);
                    break;
                case 13115:
                    loader = new Transition(chunk);
                    break;
                case 13116:
                    loader = new Transition(chunk);
                    break;
                case 13121:
                    loader = new Layers(chunk);
                    break;
                case 26214:
                    loader = new ImageBank(chunk);
                    break;
                case 26216:
                    loader = new SoundBank(chunk);
                    break;
                case 26217:
                    loader = new MusicBank(chunk);
                    break;
                case 17477:
                    loader = new ObjectName(chunk);
                    break;
                case 17476:
                    loader = new ObjectHeader(chunk);
                    break;
                case 17478:
                    loader = new ObjectProperties(chunk);
                    return loader;
                case 8788:
                    //loader = new ObjectNames(chunk);
                    break;
                case 8754:
                    loader = new GlobalValues(chunk);
                    break;
                case 8755:
                    loader = new GlobalStrings(chunk);
                    break;
                case 13117:
                    // loader = new Events(chunk);//NOT WORKING
                    break;
                
                
            }

            loader?.Read();
            return loader;
        }


        public T GetChunk<T>() where T : ChunkLoader
        {
            foreach (Chunk chunk in Chunks)
            {
                if (chunk.Loader != null)
                {
                    if (chunk.Loader.GetType().Name == typeof(T).Name)
                    {
                        return (T) chunk.Loader;
                    }
                }
            }
            //Logger.Log($"ChunkLoader {typeof(T).Name} not found", true, ConsoleColor.Red);
            return null;  
        }
        public T PopChunk<T>() where T : ChunkLoader
        {
            for(int i=0;i<Chunks.Count;i++)
            {
                var chunk = Chunks[i];
                if (chunk.Loader != null)
                {
                    if (chunk.Loader.GetType().Name == typeof(T).Name)
                    {
                        Chunks.Remove(chunk);
                        return (T) chunk.Loader;
                    }

                    
                }
            }
            return null;  
        }

        public string GetNameByID(int id)
        {
            switch (id)
            {
                case 4386: return "PREVIEW";
                case 8738: return "Mini Header";
                case 8739: return "Header";
                case 8740: return "Title";
                case 8741: return "Author";
                case 8742: return "Menu";
                case 8743: return "Extra Path";
                case 8744: return "Extensions";
                case 8745: return "Object Bank";
                case 8746: return "Global Events";
                case 8747: return "Frame Handles";
                case 8748: return "Extra Data";
                case 8749: return "Additional Extensions";
                case 8750: return "Project Path";
                case 8751: return "Output Path";
                case 8752: return "App Doc";
                case 8753: return "Other Extensions";
                case 8754: return "Global Values";
                case 8755: return "Global Strings";
                case 8756: return "Extensions List";
                case 8757: return "Icon";
                case 8758: return "Demo Version";
                case 8759: return "Security Number";
                case 8760: return "Binary Files";
                case 8761: return "Menu Images";
                case 8762: return "About";
                case 8763: return "Copyright";
                case 8764: return "Global Value Names";
                case 8765: return "Global String Names";
                case 8766: return "Movement Extensions";
                case 8767: return "Object Bank 2";
                case 8768: return "EXE Only";
                case 8770: return "Protection";
                case 8771: return "Shaders";
                case 8773: return "Extended Header";
                case 13107:return "Frame";
                case 13108:return "Frame Header";
                case 13109:return "Frame Name";
                case 13110:return "Frame Password";
                case 13111:return "Frame Palette";
                case 13112:return "Frame Objects";
                case 13113:return "Frame Fade In Frame";
                case 13114:return "Frame Fade Out Frame";
                case 13115:return "Frame Fade In";
                case 13116:return "Frame Fade Out";
                case 13117:return "Frame Events";
                case 13118:return "Frame Play Header";
                case 13119:return "Additional Frame Item";
                case 13120:return "Additional Object Instance";
                case 13121:return "Frame Layers";
                case 13122:return "Frame Virtual Rect";
                case 13123:return "Demo File Path";
                case 13124:return "Random Seed";
                case 13125:return "Frame Layer Effects";
                case 13126:return "Bluray Options";
                case 13127:return "MVTimer Base";
                case 13128:return "Mosaic Image Table";
                case 13129:return "Frame Effects";
                case 13130:return "Frame Iphone Options";
                case 17476:return "Object Header";
                case 17477:return "Object Name";
                case 17478:return "Object Common";
                case 17479:return "Object Unknown";
                case 17480:return "Object Effects";
                case 21845:return "Image Offsets";
                case 21846:return "Font Offsets";
                case 21847:return "Sound Offsets";
                case 21848:return "Music Offsets";
                case 26214:return "Image Bank";
                case 26215:return "Font Bank";
                case 26216:return "Sound Bank";
                case 26217:return "Music Bank";
                case 32639:return "Last";
                default: return $"Unknown-{id}";
            }
        }
        
    }
}