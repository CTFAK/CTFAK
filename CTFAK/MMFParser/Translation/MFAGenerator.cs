using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using CTFAK.MMFParser.MFA.Loaders;
using CTFAK.Utils;
using ChunkList = CTFAK.MMFParser.MFA.Loaders.ChunkList;
using Frame = CTFAK.MMFParser.MFA.Loaders.Frame;
using Layer = CTFAK.MMFParser.MFA.Loaders.Layer;

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
            var cacheBuild = Settings.Build;
            var cacheType = Settings.GameType;
            Settings.Build = 288;
            // Settings.GameType = GameType.Normal;
            var template = new MFA.MFA(mfaReader);
            Pame2Mfa.Message("Loading Template");
            template.Read(); //Loading template
            Pame2Mfa.Message("Translating...");
            Settings.Build = cacheBuild;
            Settings.GameType = cacheType;
            Pame2Mfa.Translate(ref template, Program.CleanData); //Translation

            var dumpPath = Program.CleanData.EditorFilename?.Value?.Length > 0 ? $"{Settings.DumpPath}\\{Path.GetFileNameWithoutExtension(Program.CleanData.EditorFilename.Value)}.mfa" : $"{Settings.DumpPath}\\out.mfa";
            Logger.Log("Writing mfa to " + dumpPath);
                var mfaWriter =
                new ByteWriter(dumpPath,FileMode.Create); //New writer for new MFA
            Pame2Mfa.Message("");
            Pame2Mfa.Message("Writing MFA");
            template.Write(mfaWriter); //Writing new MFA
            mfaWriter.Dispose();
            Pame2Mfa.Message("Writing is finished!");
            return template;

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
        }public static void WriteTestMFA()
        {
            var outputPath = Path.Combine(Path.GetDirectoryName(TemplatePath), "patchNew.mfa");
            var mfaReader = new ByteReader(TemplatePath, FileMode.Open);
            var template = new MFA.MFA(mfaReader);
            Settings.DoMFA = true;
            template.Read();
            var refer = template.Frames.FirstOrDefault();
            // template.Frames.Clear();
            
            for (int i = 0; i < 25; i++)
            {
                template.Frames.Add(PluginAPI.PluginAPI.EmptyFrame);
            }  
            


            var mfaWriter = new ByteWriter(outputPath, FileMode.Create);
            template.Write(mfaWriter);
        }
    }
}