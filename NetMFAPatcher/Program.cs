using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.Utils;
using NetMFAPatcher.mmfparser;
using System;
using System.IO;
using System.Collections;
using NetMFAPatcher.mfa;
using NetMFAPatcher.utils;
using System.Runtime.InteropServices;

namespace NetMFAPatcher
{
    class Program
    {
        public static PackData pack_data;
        public static GameData game_data;
        //public static string path = @"H:\fnaf-world.exe";//test
        //public static string path = @"D:\SteamLibrary\steamapps\common\Five Nights at Freddy's Sister Location\SisterLocation.exe";
        public static string path = "";//TODO: Make Selectable

        public static string GameName;// = Path.GetFileNameWithoutExtension(path);
        public static string DumpPath;// = $"DUMP\\{GameName}";

        public static bool doMFA=false;
        public static bool DumpImages = false;
        public static bool DumpSounds = false;
        public static bool verbose;


        [STAThread]
        static void Main(string[] args)
        {
            string Path="";
            bool Verbose=false;
            bool DumpImages=true;
            bool DumpSounds=true;
            bool CreateFolders=true;


            if (args.Length == 0)
            {
                if (true)
                {


                    ByteIO mfaReader = new ByteIO(@"E:\DotNetCTF\Tests\mmftest.mfa", FileMode.Open);
                    var mfa = new MFA(mfaReader);
                    mfa.Read();
                    
                    Console.ReadKey();
                    Environment.Exit(0);
                    
                }
                else
                {
                    var testWriter = new ByteWriter(@"C:\testcock.bin",FileMode.OpenOrCreate);
                    testWriter.Write(0);
                    testWriter.BaseStream.Close();
                    var testReader = new ByteIO(@"C:\testcock.bin",FileMode.Open);




                }
                Console.ReadKey();
                Environment.Exit(0);
                Logger.Log("Finished!", true, ConsoleColor.Yellow);
                Logger.Log("Args are not provided, launch dumper with -h or -help for help");
                Logger.Log("Press any key to exit or press Z to launch with default args(debug only)");

                var key = Console.ReadKey();
                if(key.Key==ConsoleKey.Z)
                {
                    Console.WriteLine("");
                    ReadFile("H:\\SteamLibrary\\steamapps\\common\\Freddy Fazbear's Pizzeria Simulator\\Pizzeria Simulator.exe", Verbose, DumpImages, DumpSounds);
                    
                }
                if (key.Key == ConsoleKey.X)
                {
                    Console.WriteLine("");
                    ReadFile("E:\\Games\\sl\\SisterLocation.exe", Verbose, DumpImages, DumpSounds);
                }

                Environment.Exit(0);
            }
            if (args.Length > 0)
            {
                Path = args[0];
            }
            if (args.Length > 1)
            {
                Boolean.TryParse(args[1],out Verbose);
            }
            if (args.Length > 2)
            {
                 Boolean.TryParse(args[2],out DumpImages);
            }
            if(args.Length>3)
            {
                 Boolean.TryParse(args[3],out DumpSounds);
            }
            if(args[0]=="-h"||args[0]=="-help")
            {
                Logger.Log($"DotNetCTFDumper: 0.0.5",true,ConsoleColor.Green);
                Logger.Log($"Lauch Args:", true, ConsoleColor.Green);
                Logger.Log($"   Filename - path to your exe or mfa", true, ConsoleColor.Green);
                Logger.Log($"   Info - Dump debug info to console(default:true)", true, ConsoleColor.Green);
                Logger.Log($"   DumpImages - Dump images to 'DUMP\\[your game]\\ImageBank'(default:false)", true, ConsoleColor.Green);
                Logger.Log($"   DumpSounds - Dump sounds to 'DUMP\\[your game]\\SoundBank'(default:true)\n", true, ConsoleColor.Green);
                Logger.Log($"Exaple: DotNetCTFDumper.exe E:\\SisterLocation.exe true true false true", true, ConsoleColor.Green);
                Console.ReadKey();
                Environment.Exit(0);

            }


            ReadFile(Path,Verbose,DumpImages,DumpSounds);


        }
        public static void ReadFile(string path,bool verbose=false,bool dumpImages=false,bool dumpSounds=true)
        {
            GameName = Path.GetFileNameWithoutExtension(path);
            DumpPath = $"DUMP\\{GameName}";
            PrepareFolders();
            
            DumpImages = dumpImages;
            DumpSounds = dumpSounds;
            Program.verbose = verbose;
            if (File.Exists(path))
            {


                if (path.EndsWith(".exe"))
                {
                    doMFA = false;

                    ByteIO exeReader = new ByteIO(path, FileMode.Open);
                    

                    ParseExe(exeReader);
                    Logger.Log("Finished!", true, ConsoleColor.Yellow);
                    Console.ReadKey();
                }
                else if (path.EndsWith(".mfa"))
                {
                    Logger.Log("MFA reading is currently unstable");
                    Logger.Log("Are you sure?");
                    Console.ReadKey();

                    ByteIO mfaReader = new ByteIO(path, FileMode.Open);
                    var mfa = new MFA(mfaReader);
                    mfa.Read();
                    Console.ReadKey();
                }
                else
                {
                    Logger.Log($"File '{path}' is not a valid file", true, ConsoleColor.Red);
                }
            }
            else
            {
                Logger.Log($"File '{path}' does not exist",true,ConsoleColor.Red);
            }

            



        }
        public static void PrepareFolders()
        {
            Directory.CreateDirectory($"{DumpPath}\\CHUNKS\\OBJECTINFO");
            Directory.CreateDirectory($"{DumpPath}\\CHUNKS\\FRAMES");
            Directory.CreateDirectory($"{DumpPath}\\ImageBank");
            Directory.CreateDirectory($"{DumpPath}\\MusicBank");
            Directory.CreateDirectory($"{DumpPath}\\SoundBank");
            Directory.CreateDirectory($"{DumpPath}\\extensions");
        }

        public static void ParseExe(ByteIO exeReader)
        {
            Logger.Log($"Executable: {GameName}\n",true,ConsoleColor.DarkRed);
            var es = exeReader.ReadAscii(2);
            Logger.Log("EXE Header: " + es, true, ConsoleColor.Yellow);
            if (es != "MZ")
            {
                Console.WriteLine("Invalid executable signature");
                Environment.Exit(0);
            }

            exeReader.Seek(60,SeekOrigin.Begin);

            UInt16 hdr_offset = exeReader.ReadUInt16();

            exeReader.Seek(hdr_offset, SeekOrigin.Begin);
            string peHdr = exeReader.ReadAscii(2);
            Logger.Log("PE Header: " + peHdr, true, ConsoleColor.Yellow);
            exeReader.Skip(4);

            UInt16 num_of_sections = exeReader.ReadUInt16();

            exeReader.Skip(16);
            var optional_header = 28 + 68;
            var data_dir = 16 * 8;
            exeReader.Skip(optional_header + data_dir);

            uint possition = 0;
            for (int i = 0; i < num_of_sections; i++)
            {
                var entry = exeReader.Tell();

                var section_name = exeReader.ReadAscii();

                if (section_name == ".extra")
                {
                    exeReader.Seek(entry + 20);
                    possition = exeReader.ReadUInt32();
                    break;
                }

                if (i >= num_of_sections - 1)
                {
                    exeReader.Seek(entry + 16);
                    uint size = exeReader.ReadUInt32();
                    uint address = exeReader.ReadUInt32();
                    possition = address + size;
                    break;
                }

                exeReader.Seek(entry + 40);
            }

            exeReader.Seek((int) possition);
            UInt16 first_short = exeReader.PeekUInt16();
            Logger.Log("First Short: " + first_short.ToString("X2"), true, ConsoleColor.Yellow);

            if (first_short == 0x7777)
            {
                Logger.Log("Found PackData header!\nReading PackData header.", true, ConsoleColor.Blue);
                pack_data = new PackData();
                pack_data.Read(exeReader);
                game_data = new GameData();
                game_data.Read(exeReader);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            else
            {
                Logger.Log("Failed to find PackData header!\n", true, ConsoleColor.Red);
            }
        }
    }
}