using NetMFAPatcher.Utils;
using System;
using System.IO;
using System.Collections;
using NetMFAPatcher.mfa;
using NetMFAPatcher.utils;
using System.Runtime.InteropServices;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.GUI;
using System.Windows.Forms;

namespace NetMFAPatcher
{
    class Program
    {
        //public static string path = @"H:\fnaf-world.exe";//test
        //public static string path = @"D:\SteamLibrary\steamapps\common\Five Nights at Freddy's Sister Location\SisterLocation.exe";
        public static string Path = ""; //TODO: Make Selectable

        public static string GameName; // = Path.GetFileNameWithoutExtension(path);
        public static string DumpPath; // = $"DUMP\\{GameName}";

        public static bool DoMfa = false;
        public static bool DumpImages = false;
        public static bool DumpSounds = false;
        public static bool Verbose;

        public static bool LogAll = false;
        public static bool UseGui = false;

        [STAThread]
        static void Main(string[] args)
        {
            string path = "";
            bool verbose = false;
            bool dumpImages = true;
            bool dumpSounds = true;

            if (args.Length == 0)
            {
                UseGui = true;
                var form = new MainForm();
                Application.Run(form);
            }


            if (args.Length > 0)
            {
                path = args[0];
            }

            if (args.Length > 1)
            {
                Boolean.TryParse(args[1], out verbose);
            }

            if (args.Length > 2)
            {
                Boolean.TryParse(args[2], out dumpImages);
            }

            if (args.Length > 3)
            {
                Boolean.TryParse(args[3], out dumpSounds);
            }

            if (args.Length > 0 && (args[0] == "-h" || args[0] == "-help"))
            {
                Logger.Log($"DotNetCTFDumper: 0.0.5", true, ConsoleColor.Green);
                Logger.Log($"Lauch Args:", true, ConsoleColor.Green);
                Logger.Log($"   Filename - path to your exe or mfa", true, ConsoleColor.Green);
                Logger.Log($"   Info - Dump debug info to console(default:true)", true, ConsoleColor.Green);
                Logger.Log($"   DumpImages - Dump images to 'DUMP\\[your game]\\ImageBank'(default:false)", true,
                    ConsoleColor.Green);
                Logger.Log($"   DumpSounds - Dump sounds to 'DUMP\\[your game]\\SoundBank'(default:true)\n", true,
                    ConsoleColor.Green);
                Logger.Log($"Exaple: DotNetCTFDumper.exe E:\\SisterLocation.exe true true false true", true,
                    ConsoleColor.Green);
                Console.ReadKey();
                Environment.Exit(0);
            }

            if (args.Length > 0) ReadFile(path, verbose, dumpImages, dumpSounds);
        }

        public static void ReadFile(string path, bool verbose = false, bool dumpImages = false, bool dumpSounds = true)
        {
            GameName = System.IO.Path.GetFileNameWithoutExtension(path);
            DumpPath = $"DUMP\\{GameName}";
            PrepareFolders();

            DumpImages = dumpImages;
            DumpSounds = dumpSounds;
            Program.Verbose = verbose;
            if (File.Exists(path))
            {
                if (path.EndsWith(".exe"))
                {
                    DoMfa = false;
                    ByteIO exeReader = new ByteIO(path, FileMode.Open);
                    Exe currentExe = new Exe();
                    currentExe.ParseExe(exeReader);
                    Logger.Log("Finished!", true, ConsoleColor.Yellow);
                    if (!UseGui) Console.ReadKey();
                }
                else if (path.EndsWith(".mfa"))
                {
                    DoMfa = true;
                    Logger.Log("MFA reading is currently unstable");
                    Logger.Log("Are you sure?");
                    Console.ReadKey();

                    ByteIO mfaReader = new ByteIO(path, FileMode.Open);
                    var mfa = new Mfa(mfaReader);
                    mfa.Read();
                    Console.WriteLine("Writing");
                    var mfaWriter = new ByteWriter("out.mfa", FileMode.Create);
                    mfa.Write(mfaWriter);
                    Console.ReadKey();
                }
                else
                {
                    Logger.Log($"File '{path}' is not a valid file", true, ConsoleColor.Red);
                }
            }
            else
            {
                Logger.Log($"File '{path}' does not exist", true, ConsoleColor.Red);
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
    }
}