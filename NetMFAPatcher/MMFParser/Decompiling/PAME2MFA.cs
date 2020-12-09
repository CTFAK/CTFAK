
using DotNetCTFDumper.MMFParser.Data;

namespace DotNetCTFDumper.MMFParser.Decompiling
{
    public static class Pame2Mfa
    {
        public static MFA Translate(MFA template, GameData game)
        {
            MFA mfa = template;
            mfa.MfaBuild = 4;
            mfa.Product = (int) game.ProductVersion;
            mfa.BuildVersion = 283;
            mfa.Name = game.Name;
            mfa.Description = $"Decompiled with {Settings.DumperVersion}";
            mfa.Path = game.EditorFilename;
            //mfa.Stamp = wtf;
            mfa.Fonts = game.Fonts;
            mfa.Sounds = game.Sounds;
            foreach (var item in mfa.Sounds.Items)
            {
                item.IsCompressed = false;
            }
            mfa.Music = game.Music;
            mfa.Images.Items = game.Images.Images;
            foreach (var key in mfa.Images.Items.Keys)
            {
                mfa.Images.Items[key].Debug = true;
            }

            mfa.Author = game.Author;
            mfa.Copyright = game.Copyright;
            mfa.Company = "CTFAN Team";
            mfa.Version = "";
            //TODO:Binary Files
            var displaySettings = mfa.DisplayFlags;
            var graphicSettings = mfa.GraphicFlags;
            var flags = game.Header.Flags;
            var newFlags = game.Header.NewFlags;
            //TODO:Flags, no setter
            mfa.WindowX = game.Header.WindowWidth;
            mfa.WindowY = game.Header.WindowHeight;
            mfa.BorderColor = game.Header.BorderColor;
            mfa.HelpFile = "";
            mfa.VitalizePreview = 0;
            mfa.InitialScore = game.Header.InitialScore;
            mfa.InitialLifes = game.Header.InitialLives;
            mfa.FrameRate = game.Header.FrameRate;
            mfa.BuildType = 0;
            mfa.BuildPath = game.TargetFilename;
            mfa.CommandLine = "";
            mfa.Aboutbox = game.AboutText.Length > 0
                ? game.AboutText
                : "This game was decompiled with " + Settings.DumperVersion;
            
            
            
            
            
            
            
            


            return mfa;
        }
    }
}