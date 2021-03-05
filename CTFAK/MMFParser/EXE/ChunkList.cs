using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using CTFAK.MMFParser.MFA.Loaders;
using CTFAK.Properties.Locale;
using CTFAK.Utils;
using Events = CTFAK.MMFParser.EXE.Loaders.Events.Events;
using Frame = CTFAK.MMFParser.EXE.Loaders.Frame;
using Transition = CTFAK.MMFParser.EXE.Loaders.Transition;

namespace CTFAK.MMFParser.EXE
{
    public class ChunkList
    {
        public List<Chunk> Chunks = new List<Chunk>();
        public bool Verbose = false;

        public void Write(ByteWriter Writer)
        {
            foreach (Chunk chunk in Chunks)
            {
                chunk.Write(Writer);
            }
        }

        public void Read(ByteReader reader)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Chunks.Clear();

            while (true)
            {
                Chunk chunk = new Chunk(Chunks.Count, this);
                chunk.Verbose = Verbose;
                chunk.Read(reader);
                if (chunk.Id == 26214 && Settings.GameType != GameType.TwoFivePlus) chunk.Loader = LoadModern(chunk);
                else chunk.Loader = LoadModern(chunk);
                if(chunk.Loader!=null)chunk.Loader.Chunk = chunk;

                Chunks.Add(chunk);
                if (reader.Tell() >= reader.Size()) break; //In case there is no LAST chunk(may happen for fnac)
                if (chunk.Id == 32639) break; //LAST chunkID
                if (chunk.Id == 8750) BuildKey(); //Only build key when we have all the needed info
                if (chunk.Id == 8788) Settings.GameType = GameType.TwoFivePlus; //Can be only seen in 2.5+
                
            }
            stopwatch.Stop();
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

                Logger.Log("Reading "+(Constants.ChunkNames)Id);

                switch (Flag)
                {
                    case ChunkFlags.Encrypted:
                        ChunkData = Decryption.DecryptChunk(exeReader.ReadBytes(Size), Size);
                        break;
                    case ChunkFlags.CompressedAndEncrypted:
                        ChunkData = Decryption.DecodeMode3(exeReader.ReadBytes(Size), Size, Id, out DecompressedSize);
                        break;
                    case ChunkFlags.Compressed:
                        if (Settings.GameType == GameType.OnePointFive)
                        {
                            var start = exeReader.Tell();
                            ChunkData = Decompressor.DecompressOld(exeReader);
                            exeReader.Seek(start + Size);
                        }
                        else ChunkData = Decompressor.Decompress(exeReader, out DecompressedSize);

                        break;
                    case ChunkFlags.NotCompressed:
                        ChunkData = exeReader.ReadBytes(Size);
                        break;
                }
                if(ChunkData==null) throw new NullReferenceException("ChunkData is null after reading");
                //Save();
            }

            public void Write(ByteWriter Writer)
            {
                Loader = null;
                Writer.WriteInt16((short) Id);
                Writer.WriteInt16((short) Flag);
                // if (!(Loader is Frame)) Loader = null;
                if ((Loader is Events)) Loader = null;
                if ((Loader is ImageBank)) Loader = null;

                if ((Loader is FrameHeader)) Loader = null;
                if ((Loader is VirtualRect)) Loader = null;
                if ((Loader is FrameName)) Loader = null;
                if ((Loader is FramePalette)) Loader = null;
                if ((Loader is Layers)) Loader = null;
                // if ((Loader is AppMenu)) Loader = null;
                // if ((Loader is MovementTimerBase)) Loader = null;

                // if ((Loader is FrameHandles)) Loader = null;
                // if ((Loader is Extensions)) Loader = null;
                // if ((Loader is StringChunk)) Loader = null;
                // if ((Loader is Frame)) Loader = null;

                var dataWriter = new ByteWriter(new MemoryStream());
                if (Loader == null)
                {
                    // Logger.Log($"Loader for {Name} is null");
                    dataWriter.WriteBytes(ChunkData);
                }
                else
                {
                    Logger.Log("Writing " + Name);
                    Loader.Write(dataWriter);
                }

                ByteWriter compressedData = new ByteWriter(new MemoryStream());

                switch (Flag)
                {
                    case ChunkFlags.NotCompressed:
                        compressedData = dataWriter;

                        break;
                    case ChunkFlags.Compressed:
                        var compressedBuffer = Decompressor.compress_block(dataWriter.GetBuffer());
                        compressedData.WriteInt32((int) dataWriter.Size());
                        compressedData.WriteInt32(compressedBuffer.Length);
                        compressedData.WriteBytes(compressedBuffer);
                        break;
                    case ChunkFlags.Encrypted:
                        var encryptedData = Decryption.EncryptChunk(dataWriter.GetBuffer(), (int) dataWriter.Size());
                        compressedData.WriteBytes(encryptedData);
                        break;
                    case ChunkFlags.CompressedAndEncrypted:
                        var newData = Decryption.EncryptAndCompressMode3(dataWriter.GetBuffer(), Id);
                        compressedData.WriteBytes(newData);
                        break;
                }

                Writer.WriteInt32((int) compressedData.Size());
                Writer.WriteWriter(compressedData);
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
                if (extented)
                {
                    Logger.Log($"Chunk: {Name} ({_uid})", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    ID: {Id} - 0x{Id.ToString("X")}", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    Flags: {Flag}", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    Loader: {(Loader != null ? Loader.GetType().Name : "Empty Loader")}", true,
                        ConsoleColor.DarkCyan);
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
        }

        public void BuildKey()
        {
            Settings.AppName = GetChunk<AppName>()?.Value ?? "";
            Settings.Copyright = GetChunk<Copyright>()?.Value ?? "";
            Settings.ProjectPath = GetChunk<EditorFilename>()?.Value ?? "";

            Logger.Log($"Using {(Settings.Build > 284 ? ("New") : ("Old"))} Key");
            if (Settings.Build > 284) Decryption.MakeKey(Settings.AppName, Settings.Copyright, Settings.ProjectPath);
            else Decryption.MakeKey(Settings.ProjectPath, Settings.AppName, Settings.Copyright);
        }

        public enum ChunkFlags
        {
            NotCompressed = 0,
            Compressed = 1,
            Encrypted = 2,
            CompressedAndEncrypted = 3
        }

        public T CreateChunk<T>(ChunkFlags flag = ChunkFlags.NotCompressed) where T : ChunkLoader, new()
        {
            var newChunk = new Chunk(0, this);
            newChunk.Flag = flag;
            newChunk.Id = GetIdForChunk<T>();
            var newLoader = new T();
            newLoader.Chunk = newChunk;
            Chunks.Add(newChunk);
            return (T) newChunk.Loader;
        }


        public static int GetIdForChunk<T>() where T : ChunkLoader
        {
            switch (typeof(T).Name)
            {
                case "AppHeader": return 8739;
                case "AppName": return 8740;
                case "AppAuthor": return 8741;
                case "AppMenu": return 8742;
                case "ExtPath": return 8739;
                case "FrameItems": return 8745;
                case "FrameHandles": return 8747;


                default: return -1;
            }
        }

        public ChunkLoader LoadModern(Chunk chunk)
        {
            var reader = chunk.GetReader();
            ChunkLoader loader = null;
            switch (chunk.Id)
            {
                case 8739:
                    loader = new AppHeader(reader);
                    break;
                case 8740:
                    loader = new AppName(reader);
                    break;
                case 8741:
                    loader = new AppAuthor(reader);
                    break;
                case 8742:
                    loader = new AppMenu(reader);
                    break;
                case 8743:
                    loader = new ExtPath(reader);
                    break;
                case 8747:
                    loader = new FrameHandles(reader);
                    break;
                case 8750:
                    loader = new EditorFilename(reader);
                    break;
                case 8751:
                    loader = new TargetFilename(reader);
                    break;
                case 8752:
                    loader = new AppDoc(reader);
                    break;
                case 8771:
                    loader = new Shaders(reader);
                    break;
                case 8756:
                    loader = new Extensions(reader);
                    break;
                case 8745:
                    loader = new FrameItems(reader);
                    break;
                case 8787:
                    loader = new ObjectHeaders(reader);
                    break;
                case 8790:
                    loader = new ObjectPropertyList(reader);
                    break;
                case 8762:
                    loader = new AboutText(reader);
                    break;
                case 8763:
                    loader = new Copyright(reader);
                    break;
                case 13123:
                    loader = new DemoFilePath(reader);
                    break;
                case 13109:
                    loader = new FrameName(reader);
                    break;
                case 13107:
                    loader = new Frame(reader);
                    break;
                case 13108:
                    loader = new FrameHeader(reader);
                    break;
                case 13111:
                    loader = new FramePalette(reader);
                    break;
                case 13112:
                    loader = new ObjectInstances(reader);
                    break;
                case 13115:
                    loader = new Transition(reader);
                    break;
                case 13116:
                    loader = new Transition(reader);
                    break;
                case 13122:
                    loader = new VirtualRect(reader);
                    break;
                case 13121:
                    loader = new Layers(reader);
                    break;
                case 26214:
                    loader = new ImageBank(reader);
                    break;
                case 26216:
                    if (Settings.GameType == GameType.Android) break;
                    loader = new SoundBank(reader);
                    break;
                case 26217:
                    if (Settings.GameType == GameType.Android) break;
                    loader = new MusicBank(reader);
                    break;
                case 26215:
                    loader = new FontBank(reader);
                    break;
                case 17477:
                    loader = new ObjectName(reader);
                    break;
                case 17476:
                    loader = new ObjectHeader(reader);
                    break;
                case 8748:
                    loader = new ExtData(reader);
                    break;
                case 17478:
                    loader = new ObjectProperties(reader);
                    return loader;
                case 8788:
                    loader = new ObjectNames(reader);
                    break;
                case 8754:
                    loader = new GlobalValues(reader);
                    break;
                case 8755:
                    loader = new GlobalStrings(reader);
                    break;
                case 13117:
                    if (Settings.GameType == GameType.Android) break;
                    loader = new Events(reader);
                    break;
                case 13127:
                    loader = new MovementTimerBase(reader);
                    break;
            }

            if (Settings.GameType == GameType.OnePointFive)
            {
                if (loader == null)
                {
                    Logger.Log("NULL loader for "+chunk.Name);
                }
                else
                {
                    Logger.Log("Loading old "+loader.GetType().Name);
                }
            }
            loader?.Read();
            // chunk.ChunkData = null; //TODO:Do something smarter
            // chunk.RawData = null;
            return loader;
        }
        


        public T GetChunk<T>() where T : ChunkLoader
        {
            foreach (Chunk chunk in Chunks)
            {
                if (chunk.Loader != null)
                {
                    if (chunk.Loader.GetType() == typeof(T))
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
            for (int i = 0; i < Chunks.Count; i++)
            {
                var chunk = Chunks[i];
                if (chunk.Loader != null)
                {
                    if (chunk.Loader.GetType() == typeof(T))
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
                case 8738: return Properties.Locale.ChunkNames.miniHeader; //"Mini Header";
                case 8739: return Properties.Locale.ChunkNames.appHeader; //"Header";
                case 8740: return Properties.Locale.ChunkNames.title; //"Title";
                case 8741: return Properties.Locale.ChunkNames.author; //"Author";
                case 8742: return Properties.Locale.ChunkNames.menu; //"Menu";
                case 8743: return Properties.Locale.ChunkNames.extraPath; //"Extra Path";
                case 8744: return Properties.Locale.ChunkNames.extensions; //"Extensions";
                case 8745: return Properties.Locale.ChunkNames.objectBank; //"Object Bank";
                case 8746: return Properties.Locale.ChunkNames.globalEvents; //"Global Events";
                case 8747: return Properties.Locale.ChunkNames.frameHandles; //"Frame Handles";
                case 8748: return Properties.Locale.ChunkNames.extraData; //"Extra Data";
                case 8749: return Properties.Locale.ChunkNames.additionalExts; //"Additional Extensions";
                case 8750: return Properties.Locale.ChunkNames.projectPath; //"Project Path";
                case 8751: return Properties.Locale.ChunkNames.outputPath; //"Output Path";
                case 8752: return Properties.Locale.ChunkNames.appDoc; //"App Doc";
                case 8753: return Properties.Locale.ChunkNames.otherExts; //"Other Extensions";
                case 8754: return Properties.Locale.ChunkNames.globalValues; //"Global Values";
                case 8755: return Properties.Locale.ChunkNames.globalStrings; //"Global Strings";
                case 8756: return Properties.Locale.ChunkNames.extList; //"Extensions List";
                case 8757: return Properties.Locale.ChunkNames.icon; //"Icon";
                case 8758: return Properties.Locale.ChunkNames.demoVersion; //"Demo Version";
                case 8759: return Properties.Locale.ChunkNames.secNum; //"Security Number";
                case 8760: return Properties.Locale.ChunkNames.binaryFiles; //"Binary Files";
                case 8761: return Properties.Locale.ChunkNames.menuImages; //"Menu Images";
                case 8762: return Properties.Locale.ChunkNames.about; //"About";
                case 8763: return Properties.Locale.ChunkNames.copyright; //"Copyright";
                case 8764: return Properties.Locale.ChunkNames.globalValueNames; //"Global Value Names";
                case 8765: return Properties.Locale.ChunkNames.globalStringNames; //"Global String Names";
                case 8766: return "Movement Extensions";
                case 8767: return Properties.Locale.ChunkNames.objectBank2; //"Object Bank 2";
                case 8768: return Properties.Locale.ChunkNames.exeOnly; //"EXE Only";
                case 8770: return Properties.Locale.ChunkNames.protection; //"Protection";
                case 8771: return Properties.Locale.ChunkNames.shaders; //"Shaders";
                case 8773: return Properties.Locale.ChunkNames.extHeader; //"Extended Header";
                case 8774: return "Spacer"; //"Spacer";
                case 8787: return "Object Headers";
                case 8788: return "Object Names";
                case 13107: return Properties.Locale.ChunkNames.frame; //"Frame";
                case 13108: return Properties.Locale.ChunkNames.frameHeader; //"Frame Header";
                case 13109: return Properties.Locale.ChunkNames.frameName; //"Frame Name";
                case 13110: return Properties.Locale.ChunkNames.framePassword; //"Frame Password";
                case 13111: return Properties.Locale.ChunkNames.framePalette; //"Frame Palette";
                case 13112: return Properties.Locale.ChunkNames.frameObjects; //"Frame Objects";
                case 13113: return Properties.Locale.ChunkNames.frameFadeInFrame; //"Frame Fade In Frame";
                case 13114: return Properties.Locale.ChunkNames.frameFadeOutFrame; //"Frame Fade Out Frame";
                case 13115: return Properties.Locale.ChunkNames.FrameFadeIn; //"Frame Fade In";
                case 13116: return Properties.Locale.ChunkNames.frameFadeOut; //"Frame Fade Out";
                case 13117: return Properties.Locale.ChunkNames.frameEvents; //"Frame Events";
                case 13118: return Properties.Locale.ChunkNames.framePlayHeader; //"Frame Play Header";
                case 13119: return Properties.Locale.ChunkNames.additionalFrameItem; //"Additional Frame Item";
                case 13120:
                    return Properties.Locale.ChunkNames.additionalObjectInstance; //"Additional Object Instance";
                case 13121: return Properties.Locale.ChunkNames.frameLayers; //"Frame Layers";
                case 13122: return Properties.Locale.ChunkNames.frameVirtualRect; //"Frame Virtual Rect";
                case 13123: return Properties.Locale.ChunkNames.demoFilePath; //"Demo File Path";
                case 13124: return Properties.Locale.ChunkNames.randomSeed; //"Random Seed";
                case 13125: return Properties.Locale.ChunkNames.frameLayerEffects; //"Frame Layer Effects";
                case 13126: return Properties.Locale.ChunkNames.blurayOptions; //"Bluray Options";
                case 13127: return Properties.Locale.ChunkNames.mvTimerBase; //"MVTimer Base";
                case 13128: return Properties.Locale.ChunkNames.mosaicImageTable; //"Mosaic Image Table";
                case 13129: return Properties.Locale.ChunkNames.frameEffects; //"Frame Effects";
                case 13130: return Properties.Locale.ChunkNames.frameIphoneOptions; //"Frame Iphone Options";
                case 17476: return Properties.Locale.ChunkNames.objectHeader; //"Object Header";
                case 17477: return Properties.Locale.ChunkNames.objectName; //"Object Name";
                case 17478: return Properties.Locale.ChunkNames.objectCommon; //"Object Common";
                case 17479: return Properties.Locale.ChunkNames.objectUnknown; //"Object Unknown";
                case 17480: return Properties.Locale.ChunkNames.objectEffects; //"Object Effects";
                case 21845: return Properties.Locale.ChunkNames.imagesOffsets; //"Image Offsets";
                case 21846: return Properties.Locale.ChunkNames.fontOffsets; //"Font Offsets";
                case 21847: return Properties.Locale.ChunkNames.soundOffsets; //"Sound Offsets";
                case 21848: return Properties.Locale.ChunkNames.musicOffsets; //"Music Offsets";
                case 26214: return Properties.Locale.ChunkNames.imageBank; //"Image Bank";
                case 26215: return Properties.Locale.ChunkNames.fontBank; //"Font Bank";
                case 26216: return Properties.Locale.ChunkNames.soundBank; //"Sound Bank";
                case 26217: return Properties.Locale.ChunkNames.musicBank; //"Music Bank";
                case 32639: return Properties.Locale.ChunkNames.last; //"Last";
                default: return $"Unknown-{id}";
            }
        }
    }
}