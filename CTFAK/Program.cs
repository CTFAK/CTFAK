using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DotNetCTFDumper.GUI;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.MMFParser.Translation;
using DotNetCTFDumper.Utils;
using Joveler.Compression.ZLib;

namespace DotNetCTFDumper
{
    public class Program
    {
        public static MainForm MyForm;
        public delegate void DumperEvent(object obj);


        [STAThread]
        private static void Main(string[] args)
        {
            InitNativeLibrary();
            
            // MFAGenerator.ReadTestMFA();
            // Environment.Exit(0);

            var path = "";
            var verbose = false;
            var dumpImages = true;
            var dumpSounds = true;
            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                var error = new ErrorLogBox(eventArgs.Exception);
                Application.Run(error);
            };
            
            Settings.UseGUI = true;
            
            if (args.Length > 0)
            {
                MyForm = new MainForm(Color.FromName(args[0]));
            }
            else
            {
                MyForm = new MainForm(Color.FromArgb(223, 114, 38));
            }
            Application.Run(MyForm);
            

            


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
                Logger.Log("Example: DotNetCTFDumper.exe E:\\SisterLocation.exe true true false true", true,
                    ConsoleColor.Green);
                Console.ReadKey();
                Environment.Exit(0);
            }

            if (args.Length > 1) ReadFile(path, verbose, dumpImages, dumpSounds);
        }

        public static void ReadFile(string path, bool verbose = false, bool dumpImages = false, bool dumpSounds = true)
        {
            Settings.GamePath = path;
            Logger.Log("DecryptionLibExist: "+File.Exists("x64\\Decrypter-x64.dll"));
            PrepareFolders();

            Settings.DumpImages = dumpImages;
            Settings.DumpSounds = dumpSounds;
            Settings.Verbose = verbose;
            
            var exeReader = new ByteReader(path, FileMode.Open);
            var currentExe = new Exe();
            Exe.Instance = currentExe;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            currentExe.ParseExe(exeReader);
            stopWatch.Stop();
            Logger.Log("Finished in "+stopWatch.Elapsed.ToString("g"), true, ConsoleColor.Yellow);
            
        }

        public static void PrepareFolders()
        {
            Directory.CreateDirectory($"{Settings.ImagePath}");
            Directory.CreateDirectory($"{Settings.SoundPath}");
            Directory.CreateDirectory($"{Settings.MusicPath}");
            Directory.CreateDirectory($"{Settings.ChunkPath}");
            Directory.CreateDirectory($"{Settings.ExtensionPath}");
            Directory.CreateDirectory($"{PluginAPI.PluginAPI.PluginPath}");
        }
        public static void InitNativeLibrary()
        {
            string arch = null;
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86:
                    arch = "x86";
                    break;
                case Architecture.X64:
                    arch = "x64";
                    break;
                case Architecture.Arm:
                    arch = "armhf";
                    break;
                case Architecture.Arm64:
                    arch = "arm64";
                    break;
            }
            string libPath = Path.Combine(arch, "zlibwapi.dll");

            if (!File.Exists(libPath))
                throw new PlatformNotSupportedException($"Unable to find native library [{libPath}].");

            ZLibInit.GlobalInit(libPath);
        }
    }
}