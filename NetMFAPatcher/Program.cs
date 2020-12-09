using System;
using System.IO;
using System.Windows.Forms;
using DotNetCTFDumper.GUI;
using DotNetCTFDumper.MMFParser.Data;
using DotNetCTFDumper.MMFParser.Decompiling;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper
{
    internal class Program
    {
        //public static string path = @"H:\fnaf-world.exe";//test
        //public static string path = @"D:\SteamLibrary\steamapps\common\Five Nights at Freddy's Sister Location\SisterLocation.exe";
        public static string Path = ""; //TODO: Make Selectable
        public static MainForm MyForm;


        [STAThread]
        private static void Main(string[] args)
        {
            var handle = Helper.GetConsoleWindow();
            Helper.ShowWindow(handle, Helper.SW_HIDE);

            //MFAGenerator.BuildMFA();
            //Environment.Exit(0);
            var path = "";
            var verbose = false;
            var dumpImages = false;
            var dumpSounds = true;

            if (args.Length == 0)
            {
                Settings.UseGUI = true;
                MyForm = new MainForm();
                Application.Run(MyForm);
            }


            if (args.Length > 0) path = args[0];

            if (args.Length > 1) bool.TryParse(args[1], out verbose);

            if (args.Length > 2) bool.TryParse(args[2], out dumpImages);

            if (args.Length > 3) bool.TryParse(args[3], out dumpSounds);

            if (args.Length > 0 && (args[0] == "-h" || args[0] == "-help"))
            {
                Logger.Log("DotNetCTFDumper: 0.0.5", true, ConsoleColor.Green);
                Logger.Log("Lauch Args:", true, ConsoleColor.Green);
                Logger.Log("   Filename - path to your exe or mfa", true, ConsoleColor.Green);
                Logger.Log("   Info - Dump debug info to console(default:true)", true, ConsoleColor.Green);
                Logger.Log("   DumpImages - Dump images to 'DUMP\\[your game]\\ImageBank'(default:false)", true,
                    ConsoleColor.Green);
                Logger.Log("   DumpSounds - Dump sounds to 'DUMP\\[your game]\\SoundBank'(default:true)\n", true,
                    ConsoleColor.Green);
                Logger.Log("Exaple: DotNetCTFDumper.exe E:\\SisterLocation.exe true true false true", true,
                    ConsoleColor.Green);
                Console.ReadKey();
                Environment.Exit(0);
            }

            if (args.Length > 0) ReadFile(path, verbose, dumpImages, dumpSounds);
        }

        public static void ReadFile(string path, bool verbose = false, bool dumpImages = false, bool dumpSounds = true)
        {
            Settings.GamePath = path;
            
            PrepareFolders();

            Settings.DumpImages = dumpImages;
            Settings.DumpSounds = dumpSounds;
            Settings.Verbose = verbose;
            var exeReader = new ByteReader(path, FileMode.OpenOrCreate);
            var currentExe = new Exe();
            Exe.Instance = currentExe;
            currentExe.ParseExe(exeReader);
            Logger.Log("Finished!", true, ConsoleColor.Yellow);
            return;
            if (File.Exists(path))
            {
                if (path.EndsWith(".exe"))
                {
                    Settings.DoMFA = false;
                   
                   
                }
                else if (path.EndsWith(".mfa"))
                {
                    Settings.DoMFA = true;
                    Logger.Log("Reading mfa");
                    Logger.Log("DEBUG ONLY");
                    MFAGenerator.ReadTestMFA();
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
            Directory.CreateDirectory($"{Settings.ImagePath}");
            Directory.CreateDirectory($"{Settings.SoundPath}");
            Directory.CreateDirectory($"{Settings.MusicPath}");
            Directory.CreateDirectory($"{Settings.ChunkPath}");

            Directory.CreateDirectory($"{Settings.ExtensionPath}");


        }
    }
}