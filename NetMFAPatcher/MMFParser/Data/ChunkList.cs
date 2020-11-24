using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.mmfparser.chunkloaders;
using NetMFAPatcher.MMFParser.ChunkLoaders;
using NetMFAPatcher.MMFParser.ChunkLoaders.Events;
using NetMFAPatcher.utils;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using static NetMFAPatcher.mmfparser.Constants;

namespace NetMFAPatcher.MMFParser.Data
{
    public class ChunkList
    {
        public List<Chunk> chunks = new List<Chunk>();
        public bool verbose = false;
        public List<Frame> Frames = new List<Frame>();

        public void Read(ByteIO exeReader)
        {
            chunks.Clear();
            while (true)
            {
                Chunk chunk = new Chunk(chunks.Count, this);
                chunk.Read(exeReader);
                chunk.loader = LoadChunk(chunk);

                if (chunk.loader != null)
                {
                    if (chunk.loader.verbose)
                    {
                        //chunk.loader.Print(Program.LogAll);
                    }
                }
                if (chunk.verbose)
                {
                    chunk.Print(Program.LogAll);
                    if(Program.LogAll) Console.ReadKey();




                }

                chunks.Add(chunk);
                if (chunk.id == 8750)
                {
                    chunk.BuildKey();
                }
                if (chunk.id == 8755)
                {
                    Console.WriteLine("Fisting Found");
                    Console.ReadKey();
                }


                if (chunk.id == 32639) break; //LAST chunkID
            }

            //Logger.Log(verbose ? $" Total Chunks Count: {chunks.Count}":"ChunkList Done", true, ConsoleColor.Blue);
        }

        public class Chunk
        {
            ChunkList chunk_list;
            public string name = "UNKNOWN";
            int uid;
            public int id = 0;
            
            public ChunkLoader loader;
            public byte[] chunk_data;
            public ChunkFlags flag;
            public int size = 0;
            public int decompressed_size = 0;
            public bool verbose = false;

            public Chunk(int Actualuid, ChunkList actual_chunk_list)
            {
                uid = Actualuid;
                chunk_list = actual_chunk_list;
            }

            public ByteIO get_reader()
            {
                return new ByteIO(chunk_data);
            }

            public void Read(ByteIO exeReader)
            {
                id = exeReader.ReadInt16();
                name = ((ChunkNames) id).ToString();

                flag = (ChunkFlags) exeReader.ReadInt16();
                size = exeReader.ReadInt32();

                switch (flag)
                {
                    case ChunkFlags.Encrypted:
                        
                        chunk_data = Decryption.DecodeChunk(exeReader.ReadBytes(size),size,54);
                        break;
                    case ChunkFlags.CompressedAndEncrypyed:
                        chunk_data = Decryption.DecodeMode3(exeReader.ReadBytes(size), size,id, 54);
                        break;
                    case ChunkFlags.Compressed:
                        chunk_data = Decompressor.Decompress(exeReader);
                        break;
                    case ChunkFlags.NotCompressed:
                        chunk_data = exeReader.ReadBytes(size);
                        break;
                }

                if (chunk_data != null)
                {
                    decompressed_size = chunk_data.Length;
                    string path = $"{Program.DumpPath}\\CHUNKS\\{name}.chunk";
                    File.WriteAllBytes(path, chunk_data);
                }
                int tempId=0;
                int.TryParse(name,out tempId);
                if(tempId==id)
                {
                    chunk_data.Log(true, "X2");
                }

            }

            public void Print(bool extented)
            {
                if(extented)
                {
                    Logger.Log($"Chunk: {name} ({uid})", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    ID: {id} - 0x{id.ToString("X")}", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    Flags: {flag}", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    Loader: {(loader != null ? loader.GetType().Name : "Empty Loader")}", true,ConsoleColor.DarkCyan);
                    Logger.Log($"    Size: {size} B", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    Decompressed Size: {decompressed_size} B", true, ConsoleColor.DarkCyan);
                    Logger.Log("---------------------------------------------", true, ConsoleColor.DarkCyan);
                    

                }
                else
                {
                    Logger.Log($"Chunk: {name} ({uid})", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    ID: {id} - 0x{id.ToString("X")}", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    Decompressed Size: {decompressed_size} B", true, ConsoleColor.DarkCyan);
                    Logger.Log($"    Flags: {flag}", true, ConsoleColor.DarkCyan);
                    Logger.Log("---------------------------------------------", true, ConsoleColor.DarkCyan);
                }
                
            }
            public void BuildKey()
            {
                string title = "Five Nights at Candy's 3";
                string copyright = "";
                string project = @"C:\Users\Emil\Desktop\Five Nights at Candy's 3\Five Nights At Candy's 3.mfa";
                
                var titleChunk = chunk_list.get_chunk<AppName>();
                if (titleChunk != null) title = titleChunk.value;



                var copyrightChunk = chunk_list.get_chunk<Copyright>();
                if (copyrightChunk != null) copyright = copyrightChunk.value;
                var projectChunk = chunk_list.get_chunk<EditorFilename>();
                if (projectChunk != null) project = projectChunk.value;
                if (Program.game_data.product_build > 284)
                {
                    Decryption.MakeKey(title, copyright, project, 54);
                }
                else
                {
                    Decryption.MakeKey(project, title, copyright, 54);
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
            switch (chunk.id)
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
                case 8745://
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
                    loader = new ObjectNames(chunk);
                    break;
                case 8754:
                    loader = new GlobalValues(chunk);
                    break;
                case 8755:
                    loader = new GlobalStrings(chunk);
                    break;
                case 13117:
                    loader = new Events(chunk);
                    break;
            }

            if (loader != null)
            {
                loader.Read();
            }
            return loader;
        }


        public T get_chunk<T>() where T : ChunkLoader
        {
            foreach (Chunk chunk in chunks)
            {
                if (chunk.loader != null)
                {
                    if (chunk.loader.GetType().Name == typeof(T).Name)
                    {
                        return (T) chunk.loader;
                    }
                }
            }
            //Logger.Log($"ChunkLoader {typeof(T).Name} not found", true, ConsoleColor.Red);
            return null; //I hope this wont happen  
        }
        public T get_loader<T>(ChunkLoader loader) where T : ChunkLoader
        {
            
            return (T)loader;  
        }
    }
}