using NetMFAPatcher.MMFParser.ChunkLoaders;
using NetMFAPatcher.MMFParser.ChunkLoaders.Events;
using NetMFAPatcher.utils;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using NetMFAPatcher.MMFParser.ChunkLoaders.banks;
using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using static NetMFAPatcher.mmfparser.Constants;

namespace NetMFAPatcher.MMFParser.Data
{
    public class ChunkList
    {
        public List<Chunk> Chunks = new List<Chunk>();
        public bool Verbose = false;
        public List<Frame> Frames = new List<Frame>();

        public void Read(ByteIO exeReader)
        {
            Chunks.Clear();
            while (true)
            {
                Chunk chunk = new Chunk(Chunks.Count, this);
                chunk.Verbose = Verbose;
                chunk.Read(exeReader);
                chunk.Loader = LoadChunk(chunk);

                if (chunk.Loader != null)
                {
                    if (chunk.Loader.Verbose)
                    {
                        //chunk.loader.Print(Program.LogAll);
                    }
                }
                if (chunk.Verbose)
                {
                    chunk.Print(Program.LogAll);
                    if(Program.LogAll) Console.ReadKey();




                }

                Chunks.Add(chunk);
                if (chunk.Id == 8750)
                {
                    chunk.BuildKey();
                }
                


                if (chunk.Id == 32639) break; //LAST chunkID
            }

            //Logger.Log(verbose ? $" Total Chunks Count: {chunks.Count}":"ChunkList Done", true, ConsoleColor.Blue);
        }

        public class Chunk
        {
            ChunkList _chunkList;
            public string Name = "UNKNOWN";
            int _uid;
            public int Id = 0;
            
            public ChunkLoader Loader;
            public byte[] ChunkData;
            public ChunkFlags Flag;
            public int Size = 0;
            public int DecompressedSize = 0;
            public bool Verbose = false;

            public Chunk(int actualuid, ChunkList actualChunkList)
            {
                _uid = actualuid;
                _chunkList = actualChunkList;
            }

            public ByteIO get_reader()
            {
                return new ByteIO(ChunkData);
            }

            public void Read(ByteIO exeReader)
            {
                Id = exeReader.ReadInt16();
                Name = ((ChunkNames) Id).ToString();

                Flag = (ChunkFlags) exeReader.ReadInt16();
                Size = exeReader.ReadInt32();

                switch (Flag)
                {
                    case ChunkFlags.Encrypted:                       
                        ChunkData = Decryption.DecodeChunk(exeReader.ReadBytes(Size),Size);
                        break;
                    case ChunkFlags.CompressedAndEncrypyed:
                        ChunkData = Decryption.DecodeMode3(exeReader.ReadBytes(Size), Size,Id);
                        break;
                    case ChunkFlags.Compressed:
                        ChunkData = Decompressor.Decompress(exeReader);
                        break;
                    case ChunkFlags.NotCompressed:
                        ChunkData = exeReader.ReadBytes(Size);
                        break;
                }

                if (ChunkData != null)
                {
                    DecompressedSize = ChunkData.Length;
                    // string path = $"{Program.DumpPath}\\CHUNKS\\{Name}.chunk";
                    // File.WriteAllBytes(path, ChunkData);
                }
                int tempId=0;
                int.TryParse(Name,out tempId);
                if(tempId==Id)
                {
                    //chunk_data.Log(true, "X2");
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
                string title = "";
                string copyright = "";
                string project = "";
                
                var titleChunk = _chunkList.get_chunk<AppName>();
                if (titleChunk != null) title = titleChunk.Value;

                var copyrightChunk = _chunkList.get_chunk<Copyright>();
                if (copyrightChunk != null) copyright = copyrightChunk.Value;

                var projectChunk = _chunkList.get_chunk<EditorFilename>();
                if (projectChunk != null) project = projectChunk.Value;

                if (Exe.LatestInst.GameData.ProductBuild >= 284)
                {
                    Decryption.MakeKey(title, copyright, project);
                }
                else
                {
                    Decryption.MakeKey(project, title, copyright);
                }

                Logger.Log("New Key!"); 



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
                case 13112:
                    loader = new ObjectInstances(chunk);
                    break;
                case 26214:
                    loader = new ImageBank(chunk);
                    break;
                case 26216:
                    loader = new SoundBank(chunk);
                    break;
                case 17477:
                    loader = new ObjectName(chunk);
                    break;
                case 17476:
                    loader = new ObjectHeader(chunk);
                    break;
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
                    //loader = new Events(chunk);//NOT WORKING
                    break;
            }

            if (loader != null)
            {
                //Logger.Log($"Reading {loader.GetType().Name}...",true,ConsoleColor.Yellow);
                loader.Read();

            }
            return loader;
        }


        public T get_chunk<T>() where T : ChunkLoader
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
        public T get_loader<T>(ChunkLoader loader) where T : ChunkLoader
        {
            
            return (T)loader;  
        }
    }
}