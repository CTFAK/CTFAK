using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CTFAK.MMFParser.EXE;

namespace CTFAK.PluginAPI
{
    public static class PluginAPI
    {
        public static string PluginPath = System.IO.Path.Combine(
            Directory.GetCurrentDirectory(),
            "Plugins");

        public static List<Plugin> Plugins = new List<Plugin>();

        public static void InitializePlugins()
        {
            Plugins.Clear();
            DirectoryInfo pluginDirectory = new DirectoryInfo(PluginPath);
            if (!pluginDirectory.Exists)
                pluginDirectory.Create();


            var pluginFiles = Directory.GetFiles(PluginPath, "*.dll");
            foreach (var file in pluginFiles)
            {
                Assembly asm = Assembly.LoadFrom(file);
                var types = asm.GetTypes().Where(t =>
                    t.GetInterfaces().Where(i => i.FullName == typeof(IPlugin).FullName).Any());
                foreach (var type in types)
                {
                    var pluginClass = asm.CreateInstance(type.FullName) as IPlugin;
                    string name = "ERROR";
                    foreach (var attribute in asm.GetCustomAttributes(typeof(CTFDumperPluginAttribute)))
                    {
                        name = ((CTFDumperPluginAttribute) attribute).Name;
                    }
                    
                    var plugin = new Plugin(name,"Kostya",asm,pluginClass);
                    
                    

                    Plugins.Add(plugin);
                }
            }
        }

        public static object ActivatePlugin(Plugin plugin)
        {
            foreach (var attribute in plugin.Asm.GetCustomAttributes(typeof(CTFDumperPluginAttribute)))
            {
                
                if (((CTFDumperPluginAttribute) attribute).Type == PluginIOType.GameData)
                {
                    return plugin.pluginClass.Activate(Exe.Instance.GameData);
                }
                else throw new NotImplementedException("Not Supported");
                
            }
            throw new NotImplementedException("Critical error ");
        }

       

    }

    public class Plugin
    {
        public string Name;
        public Assembly Asm;
        public string Author;
        public IPlugin pluginClass;
        public Plugin(string name, string author,Assembly asm, IPlugin pluginClass)
        {
            Name = name;
            Author = author;
            Asm=asm;
            this.pluginClass = pluginClass;
        }

        

    }

    public enum PluginIOType
    {
        GameData,
        PackData,
        MFA,
        Chunk,
        ChunkLoader,
        MFALoader
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class CTFDumperPluginAttribute : Attribute
    {
        public string Name { get; }
        public PluginIOType Type { get; }

        public CTFDumperPluginAttribute(string name, PluginIOType type)
        {
            Name = name;
            Type = type;
        }
    }
}

            
        
        
    
