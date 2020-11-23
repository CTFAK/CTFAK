using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.mmfparser;
using NetMFAPatcher.mmfparser.mfaloaders;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.chunkloaders
{
    class AppHeader : ChunkLoader
    {
        int size;
        int windowWidth;
        int windowHeight;
        int initialScore;
        int initialLives;
        public int numberOfFrames;
        

        public override void Read()
        {
            reader = new ByteIO(chunk.chunk_data);
            size = reader.ReadInt32();
            var flags = reader.ReadInt16(); //raw,need convert
            var new_flags = reader.ReadInt16(); //read flags or no balls
            var graphics_mode = reader.ReadInt16(); //i am serious
            var otherflags = reader.ReadInt16(); //last chance to get balls back
            windowWidth = reader.ReadInt16();
            windowHeight = reader.ReadInt16();
            initialScore = (int) (reader.ReadUInt32() ^ 0xffffffff);
            initialLives = (int) (reader.ReadUInt32() ^ 0xffffffff);
            var controls = new Controls(reader);
            controls.Read();
            controls.Print(false);

            var borderColor = reader.ReadBytes(4);
            numberOfFrames = reader.ReadInt32();
            var frameRate = reader.ReadInt32();
            var windowsMenuIndex = reader.ReadSByte();
        }

        public override void Print(bool ext)
        {
            Logger.Log($"ScreenRes: {windowWidth}x{windowHeight}", true, ConsoleColor.DarkMagenta);
            Logger.Log($"Score: {initialScore}, Lives: {initialLives}", true, ConsoleColor.DarkMagenta);
            Logger.Log($"Frame count: {numberOfFrames}", true, ConsoleColor.DarkMagenta);
            Logger.Log("");
        }


        public AppHeader(ByteIO reader) : base(reader)
        {
        }

        public AppHeader(Chunk chunk) : base(chunk)
        {
        }
    }


    public class Controls:ChunkLoader
    {
        public List<PlayerControl> items;
        ByteIO reader;

        public Controls(ByteIO reader) : base(reader)
        {
            this.reader = reader;
        }

        

        public override void Read()
        {
            items = new List<PlayerControl>();
            for (int i = 0; i < 4; i++)
            {
                var item = new PlayerControl(reader);
                items.Add(item);
                item.Read();
            }
        }

        public override void Print(bool ext)
        {
            Logger.Log("Controls: ",true,ConsoleColor.Yellow);
            foreach (var item in items)
            {
                item.Print();
            }
        }
    }

    public class PlayerControl
    {
        int controlType = 0;
        ByteIO reader;
        Keys keys;

        public PlayerControl(ByteIO reader)
        {
            this.reader = reader;
        }

        public void Read()
        {
            keys = new Keys(reader);
            controlType = reader.ReadInt16();
            keys.Read();
        }

        public void Print()
        {
            Logger.Log("    PlayerControl:", true, ConsoleColor.Yellow);
            Logger.Log($"       ControlType: {controlType}", true, ConsoleColor.Yellow);
            keys.Print();
        }
    }

    public class Keys
    {
        int up;
        int down;
        int left;
        int right;
        int button1;
        int button2;
        int button3;
        int button4;
        ByteIO reader;

        public Keys(ByteIO reader)
        {
            this.reader = reader;
        }


        public void Read()
        {
            up = reader.ReadInt16();
            down = reader.ReadInt16();
            left = reader.ReadInt16();
            right = reader.ReadInt16();
            button1 = reader.ReadInt16();
            button2 = reader.ReadInt16();
            button3 = reader.ReadInt16();
            button4 = reader.ReadInt16();
        }

        public void Print()
        {
            Logger.Log($"           Up: {up}", true, ConsoleColor.Yellow);
            Logger.Log($"           Down: {down}", true, ConsoleColor.Yellow);
            Logger.Log($"           Left: {left}", true, ConsoleColor.Yellow);
            Logger.Log($"           Right: {right}", true, ConsoleColor.Yellow);
            Logger.Log($"           Button1: {button1}", true, ConsoleColor.Yellow);
            Logger.Log($"           Button2: {button2}", true, ConsoleColor.Yellow);
            Logger.Log($"           Button3: {button3}", true, ConsoleColor.Yellow);
            Logger.Log($"           Button4: {button4}", true, ConsoleColor.Yellow);
        }
    }
}