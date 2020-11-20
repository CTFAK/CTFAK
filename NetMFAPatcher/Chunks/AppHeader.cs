using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.Chunks
{
    class AppHeader:ChunkLoader
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
            Logger.Log(reader.Tell().ToString());
            size = reader.ReadInt32();
            var flags = reader.ReadInt16();//raw,need convert
            var new_flags = reader.ReadInt16();//read flags or no balls
            var graphics_mode = reader.ReadInt16();//i am serious
            var otherflags = reader.ReadInt16();//last chance to get balls back
            windowWidth = reader.ReadInt16();
            windowHeight = reader.ReadInt16();
            initialScore = (int)(reader.ReadUInt32() ^ 0xffffffff);
            initialLives = (int)(reader.ReadUInt32() ^ 0xffffffff);
            var controls = new Controls(reader);
            controls.Read();
            //controls.Print();
            
            var borderColor = reader.ReadBytes(4);
            borderColor.Log(true,"X2");
            numberOfFrames = reader.ReadInt32();
            var frameRate = reader.ReadInt32();
            var windowsMenuIndex = reader.ReadSByte();



            



        }
        public override void Print()
        {
            Logger.Log($"ScreenRes: {windowWidth}x{windowHeight}");
            Logger.Log($"Score: {initialScore}, Lives: {initialLives}");
            Logger.Log($"Frame count: {numberOfFrames}");
            Logger.Log("");

        }


    }




    public class Controls
    {
        public List<PlayerControl> items;
        ByteIO reader;
        public Controls(ByteIO reader) { this.reader = reader; }
        
        public void Read()
        {
            items = new List<PlayerControl>();
            for (int i = 0; i <4; i++)
            {
                var item = new PlayerControl(reader);
                items.Add(item);
                item.Read();
                

            }

        }
        public void Print()
        {
            Logger.Log("Controls: ");
            foreach(var item in items)
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

        public PlayerControl(ByteIO reader) { this.reader = reader; }
        public void Read()
        {

            keys = new Keys(reader);
            controlType = reader.ReadInt16();
            keys.Read();

        }
        public void Print()
        {
            Logger.Log("PlayerControl:");
            Logger.Log($"ControlType: {controlType}");
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
        public Keys(ByteIO reader) { this.reader = reader; }


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
            Logger.Log($"Up: {up}");
            Logger.Log($"Down: {down}");
            Logger.Log($"Left: {left}");
            Logger.Log($"Right: {right}");
            Logger.Log($"Button1: {button1}");
            Logger.Log($"Button2: {button2}");
            Logger.Log($"Button3: {button3}");
            Logger.Log($"Button4: {button4}");


        }

    }
        

}
