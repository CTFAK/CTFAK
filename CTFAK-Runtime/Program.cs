using System.IO;
using CTFAK.MMFParser.MFA;
using CTFAK.Utils;
using CTFAK_Runtime.Launcher;
using CTFAK_Runtime.Parser;

namespace CTFAK_Runtime
{
    internal class Program
    {
        public static RuntimeGameInfo AppInfo;

        public static void Main(string[] args)
        {
            CTFAK.Program.InitNativeLibrary();
            var mfa = new MFA(new ByteReader(@"E:\DotNetCTF\CTFAK\bin\x64\Debug\template.mfa",FileMode.Open));
            AppInfo = MFA2OBJ.ParseMFA(mfa);
            var exitcode = Launcher.Launcher.Launch();
            Logger.Log($"Runtime application exited with code {exitcode}");
        }
    }
}