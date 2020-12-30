using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders.Objects;
using CTFAK.MMFParser.MFA.Loaders;
using CTFAK.MMFParser.MFA.Loaders.mfachunks;
using CTFAK.Utils;
using Animation = CTFAK.MMFParser.MFA.Loaders.mfachunks.Animation;
using AnimationDirection = CTFAK.MMFParser.MFA.Loaders.mfachunks.AnimationDirection;
using ChunkList = CTFAK.MMFParser.MFA.Loaders.ChunkList;
using Frame = CTFAK.MMFParser.EXE.Loaders.Frame;
using Layer = CTFAK.MMFParser.MFA.Loaders.Layer;
using Movement = CTFAK.MMFParser.MFA.Loaders.mfachunks.Movement;

namespace CTFAK.MMFParser.Translation
{
    public static class Pame2Mfa
    {
        public static Dictionary<int, FrameItem> FrameItems;
        public static event Program.DumperEvent OnMessage;
        
        public static void Translate(ref MFA.MFA mfa, GameData game)
        {
            Message("Running Pame2MFA");
            Message("Original MFA Build: "+mfa.BuildVersion);
            Message("");
            //mfa.MfaBuild = 4;
            //mfa.Product = (int) game.ProductVersion;
            //mfa.BuildVersion = 283;
            mfa.Name = game.Name;
            mfa.LangId = 8192;
            mfa.Description = "";
            mfa.Path = game.EditorFilename;
            
            //mfa.Stamp = wtf;
            if (game.Fonts != null) mfa.Fonts = game.Fonts;
            
            //mfa.Sounds = game.Sounds;
            //foreach (var item in mfa.Sounds.Items)
            //{
            //    item.IsCompressed = false;
            //}
            mfa.Music = game.Music;
            mfa.Images.Items = game.Images.Images;
            foreach (var key in mfa.Images.Items.Keys)
            {
                mfa.Images.Items[key].Debug = true;
            }

            mfa.Author = game.Author ?? "";
            mfa.Copyright = game.Copyright ??"";
            mfa.Company = "";
            mfa.Version = "";
            //TODO:Binary Files
            var displaySettings = mfa.DisplayFlags;
            var graphicSettings = mfa.GraphicFlags;
            var flags = game.Header.Flags;
            var newFlags = game.Header.NewFlags;
            //TODO:Flags, no setter
            mfa.WindowX = game.Header.WindowWidth;
            mfa.WindowY = game.Header.WindowHeight;
            mfa.BorderColor = game.Header.BorderColor;
            mfa.HelpFile = "";
            mfa.VitalizePreview = new byte[]{0x0};
            mfa.InitialScore = game.Header.InitialScore;
            mfa.InitialLifes = game.Header.InitialLives;
            mfa.FrameRate = game.Header.FrameRate;
            mfa.BuildType = 0;
            mfa.BuildPath = game.TargetFilename;
            mfa.CommandLine = "";
            mfa.Aboutbox = game.AboutText?.Length > 0
                ? game?.AboutText
                : "";
            
            //Object Section
            FrameItems = new Dictionary<int,FrameItem>();
            for (int i = 0; i < game.Frameitems.ItemDict.Keys.Count; i++)
            {
                var key = game.Frameitems.ItemDict.Keys.ToArray()[i];
                var item = game.Frameitems.ItemDict[key];
                if (item.ObjectType != 2) continue;
                var newItem = new FrameItem(null);
                newItem.Name = item.Name;
                newItem.ObjectType = item.ObjectType;
                newItem.Handle = item.Handle;
                newItem.Transparent = item.Transparent ? 1:0;
                newItem.InkEffect = item.InkEffect;
                newItem.InkEffectParameter = item.InkEffectValue;
                newItem.AntiAliasing = item.Antialias ? 1 : 0;
                newItem.Flags = (int) item.Flags; //32 TODO:Fix this 
                newItem.IconHandle = 12;
                newItem.Chunks = new ChunkList(null);
                var itemLoader = (ObjectCommon)item.Properties.Loader;
                //Only actives
                //TODO:Other types of objects
                    
                var newLoader = new Active(null);
                newLoader.ObjectFlags = (int) itemLoader.Flags.flag;//820
                newLoader.NewObjectFlags = (int) itemLoader.NewFlags.flag;//8
                newLoader.BackgroundColor = Color.FromArgb(0x0, 0xff, 0xff, 0xff);
                //newLoader.Qualifiers;
                newLoader.Strings = ConvertStrings(itemLoader.Strings);
                newLoader.Values = ConvertValue(itemLoader.Values);
                newLoader.Movements=new MFA.Loaders.mfachunks.Movements(null);
                for (int j = 0; j < itemLoader.Movements.Items.Count; j++)
                {
                    var mov = itemLoader.Movements.Items[j];
                    var newMov = new Movement(null);
                    newMov.Name = $"Movement #{j}";
                    newMov.Extension = "";
                    newMov.Identifier = (uint) mov.Type;
                    newMov.Player = mov.Player;
                    newMov.MovingAtStart = mov.MovingAtStart;
                    newMov.DirectionAtStart = mov.DirectionAtStart;
                    newLoader.Movements.Items.Add(newMov);
                }
                newLoader.Behaviours=new Behaviours(null);
                //TODO: Transitions
                if (itemLoader.Animations != null)
                {
                    var animHeader = itemLoader.Animations;
                    for (int j = 0; j < animHeader.AnimationDict.Count; j++)
                    {

                        var newAnimation = new Animation(null);
                        var newDirections = new List<AnimationDirection>();
                        EXE.Loaders.Objects.Animation animation = null;
                        try
                        {
                            if (animHeader.AnimationDict.ContainsKey(j)) animation = animHeader.AnimationDict[j];
                            else break;

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            //throw;
                        }

                        if (animation != null)
                        {
                            for (int n = 0; n < 1; n++)
                            {
                                var direction = animation.DirectionDict.ToArray()[n].Value;
                                var newDirection = new AnimationDirection(null);
                                newDirection.MinSpeed = direction.MinSpeed;
                                newDirection.MaxSpeed = direction.MaxSpeed;
                                newDirection.Index = n;
                                newDirection.Repeat = direction.Repeat;
                                newDirection.BackTo = direction.BackTo;
                                newDirection.Frames = direction.Frames;
                                newDirections.Add(newDirection);
                            }

                            newAnimation.Directions = newDirections;

                        }

                        newLoader.Items.Add(newAnimation);
                    }
                }

                newItem.Loader = newLoader;
                FrameItems.Add(newItem.Handle,newItem);
                


            }


            mfa.Frames.Clear();
            foreach (Frame frame in game.Frames)
            {
                //if(frame.Name!="what day")continue;   
                var newFrame = new MFA.Loaders.Frame(null);
                //FrameInfo
                newFrame.Handle = game.Frames.IndexOf(frame);
                newFrame.Name = frame.Name;
                newFrame.SizeX = frame.Width;
                newFrame.SizeY = frame.Height;
                newFrame.Background = frame.Background;
                newFrame.FadeIn = frame.FadeIn!=null? ConvertTransition(frame.FadeIn):null;
                newFrame.FadeOut = frame.FadeOut!=null? ConvertTransition(frame.FadeOut):null;
                var mfaFlags = newFrame.Flags;
                var originalFlags = frame.Flags;
                
                mfaFlags["GrabDesktop"] = originalFlags["GrabDesktop"];
                mfaFlags["KeepDisplay"] = originalFlags["KeepDisplay"];
                mfaFlags["BackgroundCollisions"] = originalFlags["TotalCollisionMask"];
                mfaFlags["ResizeToScreen"] = originalFlags["ResizeAtStart"];
                mfaFlags["ForceLoadOnCall"] = originalFlags["ForceLoadOnCall"];
                mfaFlags["NoDisplaySurface"] = false;
                mfaFlags["TimerBasedMovements"] = originalFlags["TimedMovements"];
                newFrame.Flags = mfaFlags;
                newFrame.Flags.flag = 260;
                newFrame.MaxObjects = frame.Events?.MaxObjects ?? 1000;
                newFrame.Password = "";
                newFrame.LastViewedX = 320;
                newFrame.LastViewedY = 240;
                newFrame.Palette = frame.Palette.Items;
                newFrame.StampHandle = 13;
                newFrame.ActiveLayer = 0;
                //LayerInfo
                newFrame.Layers=new List<Layer>();
                foreach (EXE.Loaders.Layer layer in frame.Layers.Items)
                {
                    var newLayer = new Layer(null);
                    newLayer.Name = layer.Name;
                    var layerFlags = layer.Flags;
                    newLayer.Flags["HideAtStart"] = originalFlags["ToHide"];
                    newLayer.Flags["Visible"] = true;
                    newLayer.Flags["NoBackground"] = originalFlags["DoNotSaveBackground"];
                    newLayer.Flags["WrapHorizontally"] = originalFlags["WrapHorizontally"];
                    newLayer.Flags["WrapVertically"] = originalFlags["WrapVertically"];
                    newLayer.XCoefficient = layer.XCoeff;
                    newLayer.YCoefficient = layer.YCoeff;
                    newFrame.Layers.Add(newLayer);
                }
                Message("Translating frame: "+newFrame.Name);
                var newFrameItems = new List<FrameItem>();
                var newInstances = new List<FrameInstance>();
                if (frame.Objects != null)
                {

                    for (int i = 0; i < frame.Objects.Items.Count; i++)
                    {

                        var instance = frame.Objects.Items[i];
                        FrameItem frameItem;

                        if (FrameItems.ContainsKey(instance.ObjectInfo))
                        {
                            frameItem = FrameItems[instance.ObjectInfo];
                            

                                newFrameItems.Add(frameItem);
                                var newInstance = new FrameInstance((ByteReader) null);
                                newInstance.X = instance.X;
                                newInstance.Y = instance.Y;
                                newInstance.Handle = i;
                                newInstance.Flags = 1;
                                newInstance.ParentType = (uint) instance.ParentType;
                                newInstance.ItemHandle = (uint) (instance.ObjectInfo + 3);
                                newInstance.ParentHandle = (uint) instance.ParentHandle; //0xffffffff;
                                newInstance.Layer = (uint) instance.Layer;
                                newInstances.Add(newInstance);
                            
                        }
                    }
                }
                newFrame.Items = newFrameItems;
                newFrame.Instances = newInstances;
                newFrame.Folders=new List<ItemFolder>();
                foreach (FrameItem newFrameItem in newFrame.Items)
                {
                    var newFolder = new ItemFolder((ByteReader) null);
                    newFolder.Items=new List<uint>() {(uint) newFrameItem.Handle};
                    newFrame.Folders.Add(newFolder);
                }
                //EventInfo
                
                


                newFrame.Events = MFA.MFA.emptyEvents;
                newFrame.Events.Version = 1028;
                foreach (var item in newFrame.Items)
                {
                    var newObject = new EventObject((ByteReader) null);
                    
                    newObject.Handle = (uint) item.Handle;
                    newObject.Name = item.Name ??"";
                    newObject.TypeName = "";
                    newObject.ItemType = (ushort) item.ObjectType;
                    newObject.ObjectType = 2;
                    newObject.Flags = 0;
                    newObject.ItemHandle = (uint) item.Handle;
                    newObject.InstanceHandle = 0xFFFFFFFF;
                    // newFrame.Events.Objects.Add(newObject);
                    
                }
                newFrame.Chunks = new ChunkList(null);
                mfa.Frames.Add(newFrame);
                
            }
            
            
            
            
        }

        public static MFA.Loaders.Transition ConvertTransition(EXE.Loaders.Transition gameTrans)
        {
            var mfaTrans = new MFA.Loaders.Transition((ByteReader) null);
            mfaTrans.Module = gameTrans.ModuleFile;
            mfaTrans.Name = gameTrans.Name.FirstCharToUpper();
            mfaTrans.Id = gameTrans.Module;
            mfaTrans.TransitionId = gameTrans.Name;
            mfaTrans.Flags = gameTrans.Flags;
            mfaTrans.Color = gameTrans.Color;
            mfaTrans.ParameterData = gameTrans.ParameterData;
            mfaTrans.Duration = gameTrans.Duration;
            return mfaTrans;

        }

        public static ValueList ConvertValue(AlterableValues values)
        {
            var alterables = new ValueList(null);
            if (values != null)
            {
                for (int i = 0; i < values.Items.Count; i++)
                {
                    var item = values.Items[i];
                    var newValue = new ValueItem(null);
                    newValue.Name = $"Alterable Value {i+1}";
                    newValue.Value = item;
                    alterables.Items.Add(newValue);
                }
            }

            return alterables;
        }
        public static ValueList ConvertStrings(AlterableStrings values)
        {
            var alterables = new ValueList(null);
            if (values != null)
            {
                for (int i = 0; i < values.Items.Count; i++)
                {
                    var item = values.Items[i];
                    var newValue = new ValueItem(null);
                    newValue.Name = $"Alterable String {i+1}";
                    newValue.Value = item;
                    alterables.Items.Add(newValue);
                }
            }

            return alterables;
        }

        public static void Message(string msg)
        {
            OnMessage?.Invoke(msg);
            Logger.Log(msg);
        }

        
        
    }

    
}