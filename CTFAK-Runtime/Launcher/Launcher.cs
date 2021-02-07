using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CTFAK.MMFParser.MFA.Loaders;
using CTFAK.Utils;
using CTFAK_Runtime.RuntimeObjects;

namespace CTFAK_Runtime.Launcher
{
    public static class Launcher
    {
        private static bool terminated;
        public static Form Window;
        private static bool _key;

        public static int Launch()
        {
            Window = CreateWindow(Program.AppInfo);
            LoadFrame(Program.AppInfo.Frames[0]);
            var gameThread = new Thread(a=>Application.Run(Window));
            gameThread.Start();
            
            
            Window.Closing += (a,b) => Environment.Exit(0);
            Window.KeyPress+=(a,b)=>Window.Controls[0].Location=new Point(Window.Controls[0].Location.X+1,Window.Controls[0].Location.Y);
            Window.KeyDown += (sender, args) => { _key = true; };
            Window.KeyUp+= (sender, args) => { _key = false; };
            while (true)
            {
                if (_key)
                {
                    if(NativeUtils.IsKeyPushedDown(Keys.Right))Window.Controls[0].Location=new Point(Window.Controls[0].Location.X+1,Window.Controls[0].Location.Y);
                }
                
                Thread.Sleep(1);
            }

            
            return 0;
        }


        public static Form CreateWindow(RuntimeGameInfo game)
        {
            var form = new Form();
            form.Size=new Size(game.ScreenWidth,game.ScreenHeight);
            form.Text = game.AppName;
            form.BackColor = Color.White;
            form.Icon = Icon.FromHandle(game.AppIcon.GetHicon());
            return form;
        }

        public static int LoadFrame(FrameInfo frame)
        {
            for (int i=0;i<Window.Controls.Count;i++)
            {
                var control = Window.Controls[i];
                if(control is RuntimeObject) Window.Controls.Remove(control);
            }
            Window.BackColor = frame.BackgroundColor;
            foreach (ObjectInstance item in frame.Instances)
            {
                item.Spawn(frame,Window);

            }

            return 0;

        }






    }
}