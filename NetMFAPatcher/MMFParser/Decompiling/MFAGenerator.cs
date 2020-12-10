using System.IO;
using DotNetCTFDumper.MMFParser.Data;
using DotNetCTFDumper.Utils;

namespace DotNetCTFDumper.MMFParser.Decompiling
{
    public static class MFAGenerator
    {
        public static readonly string TemplatePath = @"F:\CPP\DotNetCTFDumper\testNoFrames.mfa";

        public static void BuildMFA()
        {
            Settings.DoMFA = true;
            var mfaReader = new ByteReader(TemplatePath, FileMode.Open);
            var template = new MFA(mfaReader);

            template.Read(); //Loading template

            var gameMFA = template; //Pame2Mfa.Translate(template, Exe.LatestInst.GameData); //Translation

            var mfaWriter =
                new ByteWriter(
                    Settings.GameName.Length > 0 ? $"{Settings.DumpPath}\\{Exe.Instance.GameData.Name}.mfa" : "out.mfa",
                    FileMode.Create); //New writer for new MFA
            gameMFA.Write(mfaWriter); //Writing new MFA
        }

        public static void ReadTestMFA()
        {
            var mfaReader = new ByteReader(TemplatePath, FileMode.Open);
            var template = new MFA(mfaReader);
            Settings.DoMFA = true;
            template.Read();
            var mfaWriter = new ByteWriter("outTest.mfa", FileMode.Create);
            template.Write(mfaWriter);
        }
    }
}