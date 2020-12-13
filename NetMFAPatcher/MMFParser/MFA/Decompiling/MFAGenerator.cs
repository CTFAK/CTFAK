using System;
using System.IO;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.Decompiling
{
    public static class MFAGenerator
    {
        //public static readonly string TemplatePath = @"C:\Users\MED45\Downloads\testNoFrames.mfa";
        public static readonly string TemplatePath = @"C:\Users\ivani\Desktop\CTFResearch\testNoFrames.mfa";

        public static void BuildMFA()
        {
            Settings.DoMFA = true;
            var mfaReader = new ByteReader(TemplatePath, FileMode.Open);
            var template = new MFA.MFA(mfaReader);

            template.Read(); //Loading template

            Pame2Mfa.Translate(ref template, Exe.Instance.GameData); //Translation

            var mfaWriter =
                new ByteWriter(
                    Settings.GameName.Length > 0 ? $"{Settings.DumpPath}\\{Exe.Instance.GameData.Name}.mfa" : "out.mfa",
                    FileMode.Create); //New writer for new MFA
            template.Write(mfaWriter); //Writing new MFA
            mfaWriter.Dispose();
            Logger.Log("MFA Done",true,ConsoleColor.Yellow);
        }

        public static void ReadTestMFA()
        {
            var mfaReader = new ByteReader(TemplatePath, FileMode.Open);
            var template = new MFA.MFA(mfaReader);
            Settings.DoMFA = true;
            template.Read();
            
            //Add modifications
            

            var mfaWriter = new ByteWriter("outTest.mfa", FileMode.Create);
            template.Write(mfaWriter);
        }
    }
}