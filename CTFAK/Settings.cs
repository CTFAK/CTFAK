using System.IO;

namespace DotNetCTFDumper
{
    public static class Settings
    {
        public static bool DumpImages;
        public static bool DumpSounds;
        public static bool DumpMusic;
        public static bool SaveChunks;
        public static bool Verbose;
        public static bool Old;
        public static bool twofiveplus;
        
        public static string GamePath;
        public static string GameName => Path.GetFileNameWithoutExtension(GamePath);
        public static string DumpPath => $"DUMP\\{GameName}";
        public static string ImagePath=>$"{DumpPath}\\Images";
        public static string SoundPath=>$"{DumpPath}\\Sounds";
        public static string MusicPath=>$"{DumpPath}\\Musics";

        public static string ChunkPath=>$"{DumpPath}\\Chunks";
        public static string ExtensionPath=>$"{DumpPath}\\Extensions";
        
        
        public static string AppName;
        public static string Copyright;
        public static string ProjectPath;

        public static int Build;

        public static bool DoMFA;
        public static bool UseGUI;

        public static string DumperVersion = true ? "CTFAK 0.2.5 Alpha" : "CTFAK 0.2.1-a Debug";
    }
}