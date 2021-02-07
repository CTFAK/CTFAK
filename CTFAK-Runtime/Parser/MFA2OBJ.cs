using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using CTFAK;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using CTFAK.MMFParser.MFA;
using CTFAK.Utils;
using CTFAK_Runtime.Launcher;
using CTFAK_Runtime.RuntimeObjects;

namespace CTFAK_Runtime.Parser
{
    public static class MFA2OBJ
    {
        public static RuntimeGameInfo ParseMFA(MFA mfa)
        {
            Settings.DoMFA = true;
            Settings.Build = 288;
            ImageBank.Load = true;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            mfa.Read();
            stopwatch.Stop();
            Logger.Log($"MFA Reading finished in {stopwatch.Elapsed.ToString("g")}");
            var newInfo = new RuntimeGameInfo();
            newInfo.ScreenWidth = mfa.WindowX;
            newInfo.ScreenHeight = mfa.WindowY;
            newInfo.AppName = mfa.Name;
            newInfo.AppIcon = mfa.Icons.Items[0].Bitmap;
            newInfo.Frames.Add(FrameInfo.Create(mfa.Frames[0]));
            newInfo.Images = mfa.Images.Items;
            
            
            
            
            
            return newInfo;
        }
        
    }
}