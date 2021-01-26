using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace CTFAK
{
    public static class Settings
    {
        public static bool DumpImages;
        public static bool DumpSounds;
        public static bool DumpMusic;
        public static bool SaveChunks;
        public static bool Verbose;
        public static GameType GameType;
        
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
        public static bool Unicode;

        public static bool DoMFA;
        public static bool UseGUI;
        
        

        public static string DumperVersion = true ? "CTFAK 0.5 BETA" : "CTFAK 0.2.1-a Debug";
    }

    public class LoadableSettings
    {
        private Dictionary<string, object> _data = new Dictionary<string, object>();
        public bool autoSave = true;
        public string path;
        public static LoadableSettings instance;

        public static LoadableSettings FromFile(string path)
        {
            if (!File.Exists(path))  File.Create(path);
            Thread.Sleep(1500);
            var settings = new LoadableSettings();
            settings.path = path;
            var rawData = File.ReadAllLines(path);
            if (rawData.Length > 0)
            {
                foreach (string rawLine in rawData)
                {
                    var split = rawLine.Split('=');
                    settings._data.Add(split[0],split[1]);
                }  
            }
            instance = settings;

            return settings;
        }

        public void Save(string path)
        {
            List<string> obj = new List<string>();
            foreach (var pair in _data.ToArray())
            {
                obj.Add($"{pair.Key}={pair.Value}");
            }
            File.WriteAllLines(path,obj);
            
        }

        public object this[string key]
        {
            get
            {
                _data.TryGetValue(key, out var dataPart);
                return dataPart;
            }
            set
            {
                if (_data.ContainsKey(key)) _data[key] = value;
                else _data.Add(key,value);
                Save(path);
            }
        }

        public T ToActual<T>(object value)
        {
            return (T) ToActualByType(typeof(T),value);
        }
        public object ToActualByType(Type type, object value)
        {
            if (type == typeof(Color))
            {
                var colorSplit = value.ToString().Split(',');
                return Color.FromArgb(int.Parse(colorSplit[3]),int.Parse(colorSplit[0]),int.Parse(colorSplit[1]),int.Parse(colorSplit[2]));
            }
            else
            {
                return "not supported";
            }
        }

        public object FromActualToSave<T>(object value)
        {
            return FromActualByType(typeof(T), value);
        }

        public object FromActualByType(Type type, object value)
        {
            if (type == typeof(Color))
            {
                var clr = (Color)value;
                return $"{clr.R},{clr.G},{clr.B},{clr.A}";
            }
            else
            {
                return "not supported";
            }


        }



    }

    public enum GameType
    {
        Normal,
        TwoFivePlus,
        OnePointFive,
        MMFTwo,
        Android
    }
}