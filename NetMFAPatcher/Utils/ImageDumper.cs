using System.Drawing;
using System.IO;
using NetMFAPatcher.MMFParser.ChunkLoaders.Banks;
using NetMFAPatcher.MMFParser.ChunkLoaders.Objects;
using NetMFAPatcher.MMFParser.Data;

namespace NetMFAPatcher.Utils
{
    public class ImageDumper
    {
        public static void DumpImages()
        {
            Dump();
            
        }

        public static void Dump()
        {
            var rootFolder = $"{Settings.DumpPath}\\ImageBank\\Sorted";
            var Bank = Exe.LatestInst.GameData.GameChunks.get_chunk<ImageBank>();
            foreach (var frame in Exe.LatestInst.GameData.Frames)
            {
                if (frame.Objects != null)
                {
                    var currentFramePath = rootFolder + "\\" + frame.Name;
                    Directory.CreateDirectory(currentFramePath);
                    foreach (var item in frame.Objects.Items)
                    {
                        var currentObjPath = currentFramePath + "\\" + item.Handle;
                        Directory.CreateDirectory(currentObjPath);
                        var anims = (item.FrameItem.Properties).Loader.Animations.AnimationDict;
                        foreach (var key in anims.Keys)
                        {
                            var anim = anims[key];
                            var directions = anim.DirectionDict;
                            foreach (var key1 in directions.Keys)
                            {
                                var dir = directions[0];
                                foreach (var AnimFrame in dir.Frames)
                                {
                                    ImageItem img = null;
                                    Bank.Images.TryGetValue(AnimFrame, out img);
                                    img.Save(currentObjPath+"\\"+AnimFrame+".png");
                                }
                            }
                        }


                    }
                }

            }
        }
    }
}