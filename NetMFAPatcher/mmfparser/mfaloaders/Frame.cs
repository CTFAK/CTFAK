using mmfparser;
using NetMFAPatcher.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser.mfaloaders
{
    class Frame : DataLoader
    {
        public string name = "ERROR";

        public Frame(ByteIO reader) : base(reader)
        {
        }


        public override void Print()
        {
            throw new NotImplementedException();
        }

        public override void Read()
        {
            var handle = reader.ReadInt32();
            name = reader.ReadAscii(reader.ReadInt32());
            var sizeX = reader.ReadInt32();
            var sizeY = reader.ReadInt32();
            var background = reader.ReadColor();
            var flags = reader.ReadInt32();
            var maxObjects = reader.ReadInt32();
            var password = reader.ReadAscii(reader.ReadInt32());
            reader.Skip(4);
            var lastViewedX = reader.ReadInt32();
            var lastViewedY = reader.ReadInt32();

            var paletteNum = reader.ReadInt32();
            List<Color> palette = new List<Color>();
            for (int i = 0; i < paletteNum; i++)
            {
                palette.Add(reader.ReadColor());
            }
            var stampHandle = reader.ReadInt32();
            var activeLayer = reader.ReadInt32();
            var layersCunt = reader.ReadInt32();
            var layers = new List<Layer>();
            for (int i = 0; i < layersCunt; i++)
            {
                var layer = new Layer(reader);
                layer.Read();
                layers.Add(layer);

            }
            //fadein

            //fadeout
            reader.Skip(2);
            var frameitems = new List<FrameItem>();
            var frameitemsCount = reader.ReadInt32();
           
            for (int i = 0; i < frameitemsCount; i++)
            {
                var frameitem = new FrameItem(reader);
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





