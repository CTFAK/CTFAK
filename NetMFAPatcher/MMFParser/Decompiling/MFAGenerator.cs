using System.IO;
using NetMFAPatcher.MMFParser.Data;
using NetMFAPatcher.Utils;

namespace NetMFAPatcher.MMFParser.Decompiling
{
    public static class MFAGenerator
    {
        public static readonly string TemplatePath = @"C:\Users\ivani\Desktop\CTFResearch\TestWriting.mfa";

        public static void BuildMFA()
        {
            Settings.DoMFA = true;
            var mfaReader = new ByteIO(TemplatePath, FileMode.Open);
            var template = new MFA(mfaReader);

            template.Read(); //Loading template

            var gameMFA = Pame2Mfa.Translate(template, Exe.LatestInst.GameData); //Translation

            var mfaWriter =
                new ByteWriter(
                    Settings.GameName.Length > 0 ? $"{Settings.DumpPath}\\{Exe.LatestInst.GameData.Name}.mfa" : "out.mfa",
                    FileMode.Create); //New writer for new MFA
            gameMFA.Write(mfaWriter); //Writing new MFA
        }

        public static void ReadTestMFA()
        {
            var mfaReader = new ByteIO(TemplatePath, FileMode.Open);
            var template = new MFA(mfaReader);
            template.Read();
            var mfaWriter = new ByteWriter("outTest.mfa", FileMode.Create);
            template.Write(mfaWriter);
        }
    }
}