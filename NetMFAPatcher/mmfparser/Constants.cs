using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMFAPatcher.mmfparser
{
    public static class Constants
    {
        public static readonly string GAME_HEADER = "PAME";
        public static readonly string UNICODE_GAME_HEADER = "PAMU";//"50 41 4D 55";
        public static bool isUnicode;
        public enum Products
        {
            MMF1,
            MMF15,
            MMF2,
            CNC1

        }
        public enum ChunkNames
        {
            Preview=4368,
            AppMiniHeader=8738,
            AppHeader=8739,
            AppName=8740,
            AppAuthor=8741,
            AppMenu=8742,
            ExtPath=8743,
            Extensions=8744,
            FrameItems=8745,
            GlobalEvents=8746,
            FrameHandles=8747,
            ExtData=8748,
            Additional_Extension=8749,
            AppEditorFilename=8750,
            AppTargetFilename=8751,
            AppDoc=8752,
            OtherExt=8753,
            GlobalValues=8754,
            GlobalStrings=8755,
            Extensions2=8756,
            AppIcon=8757,
            DemoVersion=8758,
            SecNum=8759,
            BinaryFiles=8760,
            AppMenuImages=8761,
            AboutText=8762,
            Copyright=8763,
            GlobalValuesNames=8764,
            GlobalStringNames=8765,
            MVTexts=8766,
            FrameItems2=8767,
            ExeOnly=8768,
            Protection=8770,
            Shaders=8771,
            AppHeader2=8773,
            Frame=13107,
            FrameHeader=13108,
            FrameName=13109,
            FramePassword=13110,
            FramePalette=13111,
            FrameItemInstances=13112,
            FrameFadeInFrame=13113,
            FrameFadeOutFrame=13114,
            FrameFadeIn=13115,
            FrameFadeOut=13116,
            FrameEvents=13117,
            FramePlayHeader=13118,
            AdditionalFrameItem=13119,
            AdditionalFrameItemInstance2=13120,
            FrameLayers=13121,
            FrameVirtualRect=13122,
            DemoFilePath=13123,
            RandomSeed=13124,
            FrameLayerEffects=13125,
            BlurayFrameOptions=13126,
            MVTimerBase=13127,
            MosaicImageTable=13128,
            FrameEffects=13129,
            FrameIphoneOptions=13130,
            ObjInfoHeader=17476,
            ObjInfoName=17477,
            ObjectsCommon=17478,
            ObjectUnknown=17479,
            ObjectEffects=17480,
            ImagesOffsets=21845,
            FontsOffsets=21846,
            SoundsOffsets=21847,
            MusicsOffsets=21848,
            Images=26214,
            Fonts=26215,
            Sounds=26216,
            Musics=26217,
            Last=32639








         

        }
    }
}
