using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CTFAK.GUI;
using CTFAK.GUI.GUIComponents;
using CTFAK.MMFParser;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.MMFParser.EXE.Loaders.Banks;
using CTFAK.MMFParser.EXE.Loaders.Objects;

namespace CTFAK.Utils
{
    public static class ImageDumper
    {
        public static void SaveFromNode(ChunkNode node)
        {
            var timer = new Stopwatch();
            timer.Start();
            var bank = Program.CleanData.Images;
            var fullPath = $"{Settings.ImagePath}\\Sorted\\{node.FullPath}";
            if (fullPath == null) return;
            
            if (!(node.loader is ImageItem)) Directory.CreateDirectory(fullPath);
            else Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            
            if(node.loader is ImageItem img) img.Save($"{fullPath}.png");
            else if (node.loader is AnimationDirection dir)
            {
                SaveDirection(dir,bank,fullPath);
            }
            
            else if (node.loader is Animation anim)
            {
                SaveAnimation(anim,bank,fullPath);
            }
            
            else if (node.loader is ObjectInstance instance)
            {
                SaveInstance(instance,bank,fullPath);
            }
            else if(node.loader is Backdrop) Console.WriteLine("Dumping Backdrop");
            else if(node.loader is Frame frame)
            {
                SaveFrame(frame,bank,fullPath);
            }
            
            else Console.WriteLine("Unknown: "+node.loader.GetType().Name);
            timer.Stop();
            Logger.Log("Done in "+timer.Elapsed.ToString("g"));
            
        }

        public static void SaveFrame(Frame frame, ImageBank bank, string fullPath)
        {
            foreach (var inst in frame.Objects)
            {
                var path = $"{fullPath}\\{Helper.CleanInput(inst.FrameItem.Name)}";
                Logger.Log("Saving Object to "+path);
                SaveInstance(inst,bank,path);
            }
            
        }

        public static void SaveDirection(AnimationDirection dir, ImageBank bank,string fullPath)
        {
            if (dir.Reader == null) return;
            for (int i = 0; i < dir.Frames.Count; i++)
            {
                var frame = dir.Frames[i];
                bank.Images[frame].Save($"{fullPath}\\{i}.png");
            }
        }
        public static void SaveInstance(ObjectInstance inst, ImageBank bank,string fullPath)
        {
            if (inst.FrameItem.Properties.IsCommon)
            {
                var common = ((ObjectCommon)inst.FrameItem.Properties.Loader);
                Directory.CreateDirectory(fullPath);
                switch (common.Parent.ObjectType)
                {
                    case Constants.ObjectType.Active:
                        foreach (var pair in common.Animations.AnimationDict.ToArray())
                        {
                            SaveAnimation(pair.Value,bank,fullPath+"\\Animation "+pair.Key); 
                        }
                        break;
                    case Constants.ObjectType.Counter:
                        if (common?.Counters?.Frames.Count == 0) return;
                        if (common?.Counters?.Frames == null) return;
                        foreach (int frame in common?.Counters?.Frames)
                        {
                            var img = bank.FromHandle(frame);
                            img.Save(fullPath+$"\\{frame}.png");
                        }
                        break;
                        
                }
                
            }
            
        }
        public static void SaveAnimation(Animation anim, ImageBank bank, string fullPath)
        {
            if (anim.DirectionDict.ToArray().Length > 1)
            {
                foreach (var dirpair in anim.DirectionDict.ToList())
                {
                    Directory.CreateDirectory($"{fullPath}\\Direction {anim.DirectionDict.ToList().IndexOf(dirpair)}");
                    for (int i = 0; i < anim.DirectionDict[0].Frames.Count; i++)
                    {
                        if (dirpair.Value != null && dirpair.Value.Frames.Count > 0) continue;
                        var frame = dirpair.Value.Frames[i];
                        bank.Images[frame].Save($"{fullPath}\\Direction {anim.DirectionDict.ToList().IndexOf(dirpair)}\\{i}.png");
                    } 
                }
            }
            else
            {
                foreach (var dir in anim.DirectionDict.Values)
                {
                    for (int i = 0; i < dir.Frames.Count; i++)
                    {
                        Directory.CreateDirectory(fullPath);
                        var frame = dir.Frames[i];
                        bank.Images.TryGetValue(frame, out var img);
                        img?.Save($"{fullPath}\\{i}.png");
                    }

                    break;
                }
                
                
            }
            
        }
        
        public static void DumpImages()
        {
            using (var worker = new BackgroundWorker())
            {
                
                worker.DoWork += (senderA, eA) => { Dump(); };
                worker.RunWorkerCompleted += (senderA, eA) =>
                {
                    
                };
                worker.RunWorkerAsync();
            }
            
        }

        public static MainForm.IncrementSortedProgressBar SortedImageSaved;

        public static void Dump()
        {
            var rootFolder = $"{Settings.ImagePath}\\Sorted";
            var Bank = Exe.Instance.GameData.GameChunks.GetChunk<ImageBank>();
            var NumberOfImgFrames = CalculateFrameCount();
            foreach (var frame in Exe.Instance.GameData.Frames)
            {
                if (frame.Objects != null)
                {
                    var currentFramePath = rootFolder + "\\" + Helper.CleanInput(frame.Name);

                    foreach (var item in frame.Objects)
                    {
                        
                        var currentObjPath = currentFramePath + "\\" + Helper.CleanInput(item.Name);
                        //Directory.CreateDirectory(currentObjPath);
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
                                 //Logger.Log("Saving Image: "+path);
                                 actualFrame.Save(path);
                                 
                                 SortedImageSaved.Invoke(NumberOfImgFrames);
                                 
                                 
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

        public static int CalculateFrameCount()
        {
            int count = 0;
            foreach (var frame in Exe.Instance.GameData.Frames)
            {
                foreach (ObjectInstance objectInstance in frame.Objects)
                {
                    count += objectInstance.FrameItem.GetFrames().Count;
                }
            }
            return count;
        }
        



        public static Dictionary<int,string> GetFrames(this ObjectInfo obj)
        {
            Dictionary<int, string> frames = new Dictionary<int, string>();
            
            
            if (obj.Properties.Loader is ObjectCommon common)
            {
                if (obj.ObjectType == Constants.ObjectType.Active)
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
                else if (obj.ObjectType == Constants.ObjectType.Counter)
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