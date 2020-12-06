using System.IO;
using System.Runtime.Remoting.Messaging;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher
{
    public static class Settings
    {
        public static bool DumpImages;
        public static bool DumpSounds;
        public static bool SaveChunks;
        public static bool Verbose;
        
        public static string GamePath;
        public static string GameName => Path.GetFileNameWithoutExtension(GamePath);
        public static string DumpPath => $"DUMP\\{GameName}";
        public static string ImagePath=>$"{DumpPath}\\ImageBank";
        public static string SoundPath=>$"{DumpPath}\\SoundBank";
        public static string ChunkPath=>$"{DumpPath}\\Chunks";
        public static string ExtensionPath=>$"{DumpPath}\\Extensions";
        
        
        public static string AppName;
        public static string Copyright;
        public static string ProjectPath;

        public static int Build;

        public static bool DoMFA;
        public static bool UseGUI;

        public static string DumperVersion = "CTFAN 0.1.1 Debug";

        public static byte[] EncryptionKey=>Decryption.DecryptionKey;
        



    }
}