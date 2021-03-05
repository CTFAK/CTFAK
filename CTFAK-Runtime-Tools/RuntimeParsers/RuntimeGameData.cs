using System;
using CTFAK;
using CTFAK.MMFParser;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.Utils;

namespace CTFAK_Runtime_Tools.RuntimeParsers
{
    public class RuntimeGameData:GameData
    {
        public override void Read(ByteReader exeReader)
        {
            string magic = exeReader.ReadAscii(4); //Reading header
            Logger.Log("MAGIC HEADER: "+magic);
            //Checking for header
            if (magic == Constants.UnicodeGameHeader) Settings.Unicode = true;//PAMU
            else if (magic == Constants.GameHeader) Settings.Unicode = false;//PAME
            else Logger.Log("Couldn't found any known headers", true, ConsoleColor.Red);//Header not found
            RuntimeVersion = (short) exeReader.ReadUInt16(); 
            RuntimeSubversion = (short) exeReader.ReadUInt16(); 
            ProductVersion = (Constants.Products)exeReader.ReadInt32();
            ProductBuild = exeReader.ReadInt32();
            Settings.Build=ProductBuild;//Easy Access
            Logger.Log("GAME BUILD: "+Settings.Build);
            Logger.Log("PRODUCT: "+ProductVersion);
            Header=new AppHeader(exeReader);
            Header.Read();
           
        }
    }
}