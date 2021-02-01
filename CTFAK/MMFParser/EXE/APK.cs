using System.IO;
using CTFAK.Utils;
using SevenZipNET;

namespace CTFAK.MMFParser.EXE
{
    public class APK
    {
        public GameData GameData;

        public void ParseAPK(string path)
        {
            Settings.GameType = GameType.Android;
            Settings.Unicode = true;
            
            Logger.Log("Unpacking APK");
            
            var extractor = new SevenZipExtractor(path);
            extractor.ProgressUpdated += a => Logger.Log($"Unpacking: {a}%");
            
            Directory.CreateDirectory($"{Settings.DumpPath}\\Unpacked");
            extractor.ExtractAll($"{Settings.DumpPath}\\Unpacked",true,true);
            
            var applicationReader = new ByteReader($"{Settings.DumpPath}\\Unpacked\\res\\raw\\application.ccn",FileMode.Open);
            GameData = new GameData();
            Program.CleanData = GameData;
            GameData.Read(applicationReader);
            
        }
    }
}