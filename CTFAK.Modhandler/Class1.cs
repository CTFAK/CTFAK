using CTFAK.Modhandler.EngineStructs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTFAK.Modhandler
{
    public static class Loader
    {
        public static GameData MyGameData;
        public static void OnInjected(IntPtr game)
        {
            MyGameData = new GameData(game);
        }
        public static void OnUpdate()
        {
            Console.WriteLine(MyGameData.Header);
        }
        public static void OnFrameChanged(IntPtr frame)
        {
            Console.WriteLine("Frame changed");
        }
    }
}
