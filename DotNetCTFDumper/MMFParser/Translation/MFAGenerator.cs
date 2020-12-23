using System;
using System.IO;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.Translation
{
    public static class MFAGenerator
    {
        // public static readonly string TemplatePath = @"C:\Users\MED45\Downloads\OneObjOneFrame.mfa";
        public static readonly string TemplatePath = @"C:\Users\ivani\Desktop\CTFResearch\OneObjOneFrame.mfa";

        public static MFA.MFA BuildMFA()
        {
            Settings.DoMFA = true;
            var mfaReader = new ByteReader(TemplatePath, FileMode.Open);
            var template = new MFA.MFA(mfaReader);
            Pame2Mfa.Message("Loading Template");
            template.Read(); //Loading template
            Pame2Mfa.Message("Translating...");
            Pame2Mfa.Translate(ref template, Exe.Instance.GameData); //Translation

            var mfaWriter =
                new ByteWriter(
                    Settings.GameName.Length > 0 ? $"{Settings.DumpPath}\\{Exe.Instance.GameData.Name}-decompiled.mfa" : "out.mfa",
                    FileMode.Create); //New writer for new MFA
            Pame2Mfa.Message("");
            Pame2Mfa.Message("Writing MFA");
            template.Write(mfaWriter); //Writing new MFA
            mfaWriter.Dispose();
            Pame2Mfa.Message("Writing is finished!");
            return template;

            Logger.Log("MFA Done", true, ConsoleColor.Yellow);
        }

        public static void ReadTestMFA()
        {
            var outputPath = Path.Combine(Path.GetDirectoryName(TemplatePath), "decompiled.mfa");
            var mfaReader = new ByteReader(TemplatePath, FileMode.Open);
            var template = new MFA.MFA(mfaReader);
            Settings.DoMFA = true;
            template.Read();

            //Add modifications


            var mfaWriter = new ByteWriter(outputPath, FileMode.Create);
            template.Write(mfaWriter);
        }
    }
}