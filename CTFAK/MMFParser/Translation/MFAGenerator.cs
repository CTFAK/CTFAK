using System.IO;
using System.Net;
using System.Windows.Forms;
using CTFAK.MMFParser.EXE;
using CTFAK.Utils;

namespace CTFAK.MMFParser.Translation
{
    public static class MFAGenerator
    {
        
        public const string TemplatePath = @"template.mfa";
        public const string TemplateLink = @"https://github.com/CTFAK/CTFAK/raw/master/Dependencies/template.mfa";


        public static MFA.MFA BuildMFA()
        {
            ByteReader mfaReader;
            if (!File.Exists(TemplatePath))
            {
                var dlg = MessageBox.Show("Template MFA not found\nUse github version?", "Error",MessageBoxButtons.YesNo);
                if (dlg == DialogResult.No)
                {
                    Logger.Log("MFA Generation Error");
                    return null;
                }
                
                else if (dlg == DialogResult.Yes)
                {
                    using (var wc = new WebClient())
                    {
                        Logger.Log("Donwloading MFA from "+TemplateLink);
                        mfaReader=new ByteReader(wc.DownloadData(TemplateLink));
                    }
                }
                else return null;
            } 
            else mfaReader = new ByteReader(TemplatePath, FileMode.Open);
            Settings.DoMFA = true;
            
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

        }

        public static void ReadTestMFA()
        {
            var outputPath = Path.Combine(Path.GetDirectoryName(@"C:\Users\ivani\Desktop\CTFResearch\284final.mfa"), "decompiled.mfa");
            var mfaReader = new ByteReader(@"C:\Users\ivani\Desktop\CTFResearch\284final.mfa", FileMode.Open);
            var template = new MFA.MFA(mfaReader);
            Settings.DoMFA = true;
            template.Read();

            //Add modifications


            var mfaWriter = new ByteWriter(outputPath, FileMode.Create);
            template.Write(mfaWriter);
        }
    }
}