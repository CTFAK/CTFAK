using System;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.PluginAPI;

namespace ExamplePlugin
{
    public class ExamplePlugin:IPlugin
    {
        public object Activate(object input)
        {
            var data = (GameData) input;
             PluginAPI.Message(data.TargetFilename);
             return null;
        }
        
    }
}