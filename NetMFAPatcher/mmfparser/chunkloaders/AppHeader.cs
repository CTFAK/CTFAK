using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using NetMFAPatcher.Utils;
using static NetMFAPatcher.MMFParser.Data.ChunkList;

namespace NetMFAPatcher.MMFParser.ChunkLoaders
{
    public class AppHeader : ChunkLoader
    {
        public int Size;
        public int WindowWidth;
        public int WindowHeight;
        public int InitialScore;
        public int InitialLives;
        public int NumberOfFrames;
        public BitDict Flags = new BitDict(new string[]
        {
            "BorderMax",
            "NoHeading",
            "Panic",
            "SpeedIndependent",
            "Stretch",
            "MusicOn",
            "SoundOn",
            "MenuHidden",
            "MenuBar",
            "Maximize",
            "MultiSamples",
            "FullscreenAtStart",
            "FullscreenSwitch",
            "Protected",
            "Copyright",
            "OneFile"
        });
        public BitDict NewFlags = new BitDict(new string[]
        {
            "SamplesOverFrames",
            "RelocFiles",
            "RunFrame",
            "SamplesWhenNotFocused",
            "NoMinimizeBox",
            "NoMaximizeBox",
            "NoThiccFrame",
            "DoNotCenterFrame",
            "ScreensaverAutostop",
            "DisableClose",
            "HiddenAtStart",
            "XPVisualThemes",
            "VSync",
            "RunWhenMinimized",
            "MDI",
            "RunWhileResizing"
        });

        public Color BorderColor;
        public int FrameRate;


        public override void Read()
        {
            Reader = new ByteIO(Chunk.ChunkData);
            Size = Reader.ReadInt32();
            Flags.flag=(uint) Reader.ReadInt16();//I finally got my balls back

            var newFlags = Reader.ReadInt16(); //read flags or no balls
            var graphicsMode = Reader.ReadInt16(); //i am serious
            var otherflags = Reader.ReadInt16(); //last chance to get balls back
            WindowWidth = Reader.ReadInt16();
            WindowHeight = Reader.ReadInt16();
            InitialScore = (int) (Reader.ReadUInt32() ^ 0xffffffff);
            InitialLives = (int) (Reader.ReadUInt32() ^ 0xffffffff);
            var controls = new Controls(Reader);
            controls.Read();
            // controls.Print(false);

            BorderColor = Reader.ReadColor();
            NumberOfFrames = Reader.ReadInt32();
            FrameRate = Reader.ReadInt32();
            var windowsMenuIndex = Reader.ReadSByte();
        }

        public override void Print(bool ext)
        {
            Logger.Log($"ScreenRes: {WindowWidth}x{WindowHeight}", true, ConsoleColor.DarkMagenta);
            Logger.Log($"Score: {InitialScore}, Lives: {InitialLives}", true, ConsoleColor.DarkMagenta);
            Logger.Log($"Frame count: {NumberOfFrames}", true, ConsoleColor.DarkMagenta);
            Logger.Log("");
        }

        public override string[] GetReadableData()
        {
            return new string[]
                        {
                            $"Screen Resolution: {WindowWidth}x{WindowHeight}",
                            $"Initial Score: {InitialScore}",
                            $"Initial Lives: {InitialLives}",
                            $"Flags:;{Flags.ToString()}"
                        };
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
        public List<PlayerControl> Items;

        public Controls(ByteIO reader) : base(reader)
        {
            this.Reader = reader;
        }

        

        public override void Read()
        {
            Items = new List<PlayerControl>();
            for (int i = 0; i < 4; i++)
            {
                var item = new PlayerControl(Reader);
                Items.Add(item);
                item.Read();
            }
        }

        public override void Print(bool ext)
        {
            Logger.Log("Controls: ",true,ConsoleColor.Yellow);
            foreach (var item in Items)
            {
                item.Print();
            }
        }

        public override string[] GetReadableData()
        {
            throw new NotImplementedException();
        }
    }

    public class PlayerControl
    {
        int _controlType = 0;
        ByteIO _reader;
        Keys _keys;

        public PlayerControl(ByteIO reader)
        {
            this._reader = reader;
        }

        public void Read()
        {
            _keys = new Keys(_reader);
            _controlType = _reader.ReadInt16();
            _keys.Read();
        }

        public void Print()
        {
            Logger.Log("    PlayerControl:", true, ConsoleColor.Yellow);
            Logger.Log($"       ControlType: {_controlType}", true, ConsoleColor.Yellow);
            _keys.Print();
        }
    }

    public class Keys
    {
        int _up;
        int _down;
        int _left;
        int _right;
        int _button1;
        int _button2;
        int _button3;
        int _button4;
        ByteIO _reader;

        public Keys(ByteIO reader)
        {
            this._reader = reader;
        }


        public void Read()
        {
            _up = _reader.ReadInt16();
            _down = _reader.ReadInt16();
            _left = _reader.ReadInt16();
            _right = _reader.ReadInt16();
            _button1 = _reader.ReadInt16();
            _button2 = _reader.ReadInt16();
            _button3 = _reader.ReadInt16();
            _button4 = _reader.ReadInt16();
        }

        public void Print()
        {
            Logger.Log($"           Up: {_up}", true, ConsoleColor.Yellow);
            Logger.Log($"           Down: {_down}", true, ConsoleColor.Yellow);
            Logger.Log($"           Left: {_left}", true, ConsoleColor.Yellow);
            Logger.Log($"           Right: {_right}", true, ConsoleColor.Yellow);
            Logger.Log($"           Button1: {_button1}", true, ConsoleColor.Yellow);
            Logger.Log($"           Button2: {_button2}", true, ConsoleColor.Yellow);
            Logger.Log($"           Button3: {_button3}", true, ConsoleColor.Yellow);
            Logger.Log($"           Button4: {_button4}", true, ConsoleColor.Yellow);
        }
    }
}