using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetCTFDumper.MMFParser.EXE;
using DotNetCTFDumper.MMFParser.EXE.Loaders;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Banks;
using DotNetCTFDumper.MMFParser.EXE.Loaders.Objects;

namespace DotNetCTFDumper.Utils
{
    public static class ImageDumper
    {
        public static void DumpImages()
        {
            Dump();
            
        }

        public static void Dump()
        {
            var rootFolder = $"{Settings.DumpPath}\\ImageBank\\Sorted";
            var Bank = Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>();
            foreach (var frame in Exe.Instance.GameData.Frames)
            {
                if (frame.Objects != null)
                {
                    var currentFramePath = rootFolder + "\\" + Helper.CleanInput(frame.Name);

                    foreach (var item in frame.Objects.Items)
                    {
                        
                        var currentObjPath = currentFramePath + "\\" + Helper.CleanInput(item.Name);
                        Directory.CreateDirectory(currentObjPath);
                        var frames = item.FrameItem.GetFrames();
                        foreach (var key in frames.Keys)
                        {
                            
                             frames.TryGetValue(key, out var name);
                             Bank.Images.TryGetValue(key, out var actualFrame);
                             try
                             {
                                 var path =
                                     $"{Settings.ImagePath}\\Sorted\\{frame.Name}\\{Helper.CleanInput(item.Name)}\\{name}";
                                 Directory.CreateDirectory(Path.GetDirectoryName(path));
                                 Logger.Log("Saving Image: "+path);
                                 actualFrame.Save(path);
                                 
                             }
                             catch (Exception e)
                             {
                                 Logger.Log("Error while dumping images: "+e.Message,true,ConsoleColor.Red);
                                 
                             }
                             

                        }
                    }
                }
            }
            Logger.Log("Sorted Images Done",true,ConsoleColor.Yellow);
        }
        



        public static Dictionary<int,string> GetFrames(this ObjectInfo obj)
        {
            Dictionary<int, string> frames = new Dictionary<int, string>();
            
            
            if (obj.Properties.Loader is ObjectCommon common)
            {
                if (obj.ObjectType == 2)
                {
                    foreach (var animKey in common.Animations.AnimationDict.Keys)
                    {
                        var anim = common.Animations.AnimationDict[animKey];
                        foreach (var dirKey in anim.DirectionDict.Keys)
                        {
                            var dir = anim.DirectionDict[dirKey];
                            foreach (var frame in dir.Frames)
                            {
                                if (!frames.ContainsKey(frame))
                                {
                                    var animIndex = common.Animations.AnimationDict.Keys.ToList().IndexOf(animKey);
                                    var dirIndex = anim.DirectionDict.Keys.ToList().IndexOf(dirKey);
                                    var frameIndex = dir.Frames.IndexOf(frame);
                                    string finalPath = "";
                                    var animAll = dir.Frames.Count == 1;
                                    if(!animAll)
                                    {
                                        if (common.Animations.AnimationDict.Keys.Count > 1)
                                        {
                                            finalPath += $"Animation{animIndex}\\";
                                        }
                                    }

                                    if (anim.DirectionDict.Keys.Count > 1)
                                    {
                                        finalPath += $"Direction{dirIndex}\\";
                                    }

                                    
                                    finalPath += $"{(animAll ? ("Animation"+animIndex.ToString()):(frameIndex.ToString()))}.png";
                                    
                                    frames.Add(frame, finalPath);
                                }
                            }

                        }
                    }
                }
                else if (obj.ObjectType == 7)
                {
                    var counters = common.Counters;
                    if (counters == null) return frames;
                    foreach (var item in counters.Frames)
                    {
                        frames.Add(item,item.ToString());
                    }
                }
            }
            else if (obj.Properties.Loader is Backdrop backdrop)
            {
                frames.Add(backdrop.Image,"0.png");
            }
            

            return frames;

        }
    }
}