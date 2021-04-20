using System;
using CTFAK;
using CTFAK.MMFParser;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.Utils;
using CTFAK_Runtime_Tools.IO;
using CTFAK_Runtime_Tools.RuntimeParsers.RuntimeChunks;

namespace CTFAK_Runtime_Tools.RuntimeParsers
{
    public class RuntimeGameData:GameData
    {
        public override void Read(ByteReader exeReader)
        {
            string magic = exeReader.ReadAscii(4); //Reading header

            if (magic == Constants.UnicodeGameHeader) Settings.Unicode = true; //PAMU
            else if (magic == Constants.GameHeader) Settings.Unicode = false; //PAME
            else Logger.Log("Couldn't found any known headers", true, ConsoleColor.Red); //Header not found

            RuntimeVersion = (short) exeReader.ReadUInt16();
            RuntimeSubversion = (short) exeReader.ReadUInt16();
            ProductVersion = (Constants.Products) exeReader.ReadInt32();
            ProductBuild = exeReader.ReadInt32();

            Settings.Build = ProductBuild;

            Header = new AppHeader(exeReader);
            Header.Read();

            exeReader.ReadAtOffset(exeReader.ReadInt32(), (reader =>
            {
                Name = new AppName(reader);
                Name.Read();

            }));

        }
    }
}