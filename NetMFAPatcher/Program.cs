using NetMFAPatcher.chunkloaders;
using NetMFAPatcher.Utils;
using NetMFAPatcher.mmfparser;
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
        public static string path = "";//TODO: Make Selectable

        public static string GameName;// = Path.GetFileNameWithoutExtension(path);
        public static string DumpPath;// = $"DUMP\\{GameName}";

        public static bool doMFA=false;
        public static bool DumpImages = false;
        public static bool DumpSounds = false;
        public static bool verbose;

        public static bool LogAll=false;
        public static bool UseGUI = false;

        [STAThread]
        static void Main(string[] args)
        {
            string Path="";
            bool Verbose=false;
            bool DumpImages=true;
            bool DumpSounds=true;
            
            if(args.Length==0)
            {
                UseGUI = true;
                var form = new MainForm();
                Application.Run(form);
                
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

            if(args.Length>0) ReadFile(Path, Verbose, DumpImages, DumpSounds);



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
                    EXE currentEXE = new EXE();
                    currentEXE.ParseExe(exeReader);
                    Logger.Log("Finished!", true, ConsoleColor.Yellow);
                    if(!UseGUI) Console.ReadKey();

                }
                else if (path.EndsWith(".mfa"))
                {
                    doMFA = true;
                    Logger.Log("MFA reading is currently unstable");
                    Logger.Log("Are you sure?");
                    Console.ReadKey();

                    ByteIO mfaReader = new ByteIO(path, FileMode.Open);
                    var mfa = new MFA(mfaReader);
                    mfa.Read();
                    Console.WriteLine("Writing");
                    var MFAWriter = new ByteWriter("out.mfa",FileMode.Create);
                    mfa.Write(MFAWriter);
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

        
    }
}