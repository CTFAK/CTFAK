using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using DotNetCTFDumper.MMFParser.EXE.Loaders;
using DotNetCTFDumper.MMFParser.MFA;
using DotNetCTFDumper.MMFParser.MFA.Loaders;
using Frame = DotNetCTFDumper.MMFParser.MFA.Loaders.Frame;
using Layer = DotNetCTFDumper.MMFParser.MFA.Loaders.Layer;

namespace DotNetCTFDumper.PluginAPI
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
                    var plugin = new Plugin(type.Name,"Kostya",pluginClass);
                    Plugins.Add(plugin);
                }
            }
        }
    }

    public class Plugin
    {
        public string Name;

        public Plugin(string name, string author, IPlugin pluginClass)
        {
            Name = name;
            Author = author;
            this.pluginClass = pluginClass;
        }

        public string Author;
        public IPlugin pluginClass;

    }
}

            
        
        
    
