using System.Collections.Generic;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.Utils;
using Frame = DotNetCTFDumper.MMFParser.EXE.Loaders.Frame;
using Layer = DotNetCTFDumper.MMFParser.MFA.Loaders.Layer;

namespace DotNetCTFDumper.MMFParser.Translation
{
    public static class Pame2Mfa
    {
        public static event Program.DumperEvent TranslatingFrame;
        
        public static void Translate(ref MFA.MFA mfa, GameData game)
        {
            
            //mfa.MfaBuild = 4;
            //mfa.Product = (int) game.ProductVersion;
            //mfa.BuildVersion = 283;
            mfa.Name = game.Name;
            mfa.LangId = 0;
            mfa.Description = $"Decompiled with {Settings.DumperVersion}";
            mfa.Path = game.EditorFilename;
            
            //mfa.Stamp = wtf;
            /*if (game.Fonts != null) mfa.Fonts = game.Fonts;
            
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
            */
            mfa.Author = game.Author!=null? game.Author:"Kostya";
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
            mfa.VitalizePreview = new byte[]{0x0};
            mfa.InitialScore = game.Header.InitialScore;
            mfa.InitialLifes = game.Header.InitialLives;
            mfa.FrameRate = game.Header.FrameRate;
            mfa.BuildType = 0;
            mfa.BuildPath = game.TargetFilename;
            mfa.CommandLine = "";
            mfa.Aboutbox = game.AboutText?.Length > 0
                ? game?.AboutText
                : "This game was decompiled with " + Settings.DumperVersion;
            mfa.Frames.Clear();
            foreach (Frame gameFrame in game.Frames)
            {
                var mfaFrame = ConvertFrame(gameFrame);
                mfaFrame.Handle = game.Frames.IndexOf(gameFrame);
                mfa.Frames.Add(mfaFrame);
               
            }
        }

        public static MFA.Loaders.Transition ConvertTransition(EXE.Loaders.Transition gameTrans)
        {
            var mfaTrans = new MFA.Loaders.Transition((ByteReader) null);
            mfaTrans.Module = gameTrans.ModuleFile;
            mfaTrans.Name = "Transition";
            mfaTrans.Id = gameTrans.Module;
            mfaTrans.TransitionId = gameTrans.Name;
            mfaTrans.Flags = gameTrans.Flags;
            mfaTrans.Color = gameTrans.Color;
            mfaTrans.ParameterData = gameTrans.ParameterData;
            mfaTrans.Duration = gameTrans.Duration;
            return mfaTrans;

        }

        public static MFA.Loaders.Frame ConvertFrame(EXE.Loaders.Frame gameFrame)
        {
            MFA.Loaders.Frame mfaFrame = new MFA.Loaders.Frame(null);
            TranslatingFrame.Invoke(gameFrame.Name);
            //mfaFrame.Handle = game.Frames.IndexOf(gameFrame);
            mfaFrame.Name = gameFrame.Name;
            mfaFrame.SizeX = gameFrame.Width;
            mfaFrame.SizeY = gameFrame.Height;
            mfaFrame.Background = gameFrame.Background;
            if (gameFrame.FadeIn != null)mfaFrame.FadeIn = ConvertTransition(gameFrame.FadeIn);
            if (gameFrame.FadeOut != null)mfaFrame.FadeOut = ConvertTransition(gameFrame.FadeOut);
                
                

            //TODO: Flags
            mfaFrame.MaxObjects = gameFrame.Events?.MaxObjects ?? 1337;
            mfaFrame.Password = gameFrame?.Password ?? "";
            mfaFrame.LastViewedX = 320;
            mfaFrame.LastViewedY = 240;
            mfaFrame.Palette = gameFrame.Palette.Items;
            mfaFrame.StampHandle = 12;
            mfaFrame.ActiveLayer = 0;
            mfaFrame.Layers = new List<Layer>();
            var layer = new Layer(null)
            {
                Name = "New Layer",
                    
                    
            };
            layer.Flags["Visible"] = true;
            mfaFrame.Layers.Add(layer);
                
                
            /*foreach (EXE.Loaders.Layer gameLayer in gameFrame.Layers.Items)
            {
                Layer mfaLayer = new Layer(null);
                mfaLayer.Name = gameLayer.Name;
                mfaLayer.Flags = (int) gameLayer.Flags;
                //TODO: Flags
                mfaLayer.XCoefficient = gameLayer.XCoeff;
                mfaLayer.YCoefficient = gameLayer.YCoeff;
                mfaFrame.Layers.Add(mfaLayer);
            }*/
            mfaFrame.Events = MFA.MFA.emptyEvents;
            mfaFrame.Chunks = MFA.MFA.emptyFrameChunks;
            return mfaFrame;
        }
    }

    
}