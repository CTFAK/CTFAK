using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMFAPatcher.utils;

namespace NetMFAPatcher.mmfparser.mfaloaders
{
    class Frame : DataLoader
    {
        public string Name = "ERROR";
        public int SizeX;
        public int SizeY;
        public Color Background;
        public int MaxObjects;

        public Frame(ByteIO reader) : base(reader)
        {
        }


        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var handle = Reader.ReadInt32();
            Name = Helper.AutoReadUnicode(Reader);
            Console.WriteLine(Name);
            SizeX = Reader.ReadInt32();
            SizeY = Reader.ReadInt32();
            var background = Reader.ReadColor();
            var flags = Reader.ReadInt32();
            MaxObjects = Reader.ReadInt32();
            var password = Helper.AutoReadUnicode(Reader);
            Reader.Skip(4);
            var lastViewedX = Reader.ReadInt32();
            var lastViewedY = Reader.ReadInt32();

            var paletteNum = Reader.ReadInt32();
            List<Color> palette = new List<Color>();
            for (int i = 0; i < paletteNum; i++)
            {
                palette.Add(Reader.ReadColor());
            }
            var stampHandle = Reader.ReadInt32();
            var activeLayer = Reader.ReadInt32();
            var layersCunt = Reader.ReadInt32();
            var layers = new List<Layer>();
            for (int i = 0; i < layersCunt; i++)
            {
                var layer = new Layer(Reader);
                layer.Read();
                layers.Add(layer);

            }
            //fadein

            //fadeout
            Reader.Skip(2);
            var frameitems = new List<FrameItem>();
            var frameitemsCount = Reader.ReadInt32();
           
            for (int i = 0; i < frameitemsCount; i++)
            {
                var frameitem = new FrameItem(Reader);
                frameitem.Read();
                frameitems.Add(frameitem);
                //break;

            }



            //ПРОЧИТАЙ ЭТО
            //вжух и весь код для фрейма готов
            //блин не сработало
            //я задолбался, завтра доделаю
            //короче я из будущего, тут надо с циклами аккуратно работать, надо создавать переменную для размера
            //тип var frameCount = reader.ReadInt32();
            //for(int i=0;i<frameCount;i++), иначе смещения уплывут и будет жопа жопная


            


        }
    }
    
    


}





