using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.MMFParser.EXE.Loaders.Events;
using CTFAK.MMFParser.EXE.Loaders.Events.Parameters;
using CTFAK.MMFParser.EXE.Loaders.Objects;
using CTFAK.MMFParser.MFA.Loaders;
using CTFAK.MMFParser.MFA.Loaders.mfachunks;
using CTFAK.Utils;
using Action = CTFAK.MMFParser.EXE.Loaders.Events.Action;
using Animation = CTFAK.MMFParser.MFA.Loaders.mfachunks.Animation;
using AnimationDirection = CTFAK.MMFParser.MFA.Loaders.mfachunks.AnimationDirection;
using Backdrop = CTFAK.MMFParser.MFA.Loaders.mfachunks.Backdrop;
using ChunkList = CTFAK.MMFParser.MFA.Loaders.ChunkList;
using Counter = CTFAK.MMFParser.MFA.Loaders.mfachunks.Counter;
using Events = CTFAK.MMFParser.MFA.Loaders.Events;
using Frame = CTFAK.MMFParser.EXE.Loaders.Frame;
using Layer = CTFAK.MMFParser.MFA.Loaders.Layer;
using Movement = CTFAK.MMFParser.MFA.Loaders.mfachunks.Movement;
using Paragraph = CTFAK.MMFParser.MFA.Loaders.mfachunks.Paragraph;
using Text = CTFAK.MMFParser.MFA.Loaders.mfachunks.Text;

namespace CTFAK.MMFParser.Translation
{
    public static class Pame2Mfa
    {
        public static Dictionary<int, FrameItem> FrameItems;
        public static event Program.DumperEvent OnMessage;

        

        public static void Translate(ref MFA.MFA mfa, GameData game)
        {
            Message("Running Pame2MFA");
            Message("Original MFA Build: " + mfa.BuildVersion);
            Message("");
            mfa.Name = game.Name;
            mfa.LangId = 0;//8192;
            mfa.Description = "";
            mfa.Path = game.EditorFilename;

            //mfa.Stamp = wtf;
            //if (game.Fonts != null) mfa.Fonts = game.Fonts;

            // mfa.Sounds = game.Sounds;
            // foreach (var item in mfa.Sounds.Items)
            // {
            // item.IsCompressed = false;
            // }
            // mfa.Music = game.Music;
            mfa.Images.Items = game.Images.Images;
            foreach (var key in mfa.Images.Items.Keys)
            {
                mfa.Images.Items[key].Debug = true;
            }

            mfa.Author = game.Author ?? "";
            mfa.Copyright = game.Copyright ?? "";
            mfa.Company = "";
            mfa.Version = "";
            //TODO:Binary Files
            var displaySettings = mfa.DisplayFlags;
            var graphicSettings = mfa.GraphicFlags;
            var flags = game.Header.Flags;
            var newFlags = game.Header.NewFlags;
            mfa.Extensions.Clear();
            displaySettings["MDI"] = newFlags["MDI"];
            displaySettings["MenuBar"] = flags["MenuBar"];
            displaySettings["MenuOnBoot"] = !flags["MenuHidden"];
            graphicSettings["MultiSamples"] = flags["MultiSamples"];
            mfa.DisplayFlags = displaySettings;
            mfa.WindowX = game.Header.WindowWidth;
            mfa.WindowY = game.Header.WindowHeight;
            mfa.BorderColor = game.Header.BorderColor;
            mfa.HelpFile = "";
            mfa.InitialScore = game.Header.InitialScore;
            mfa.InitialLifes = game.Header.InitialLives;
            mfa.FrameRate = game.Header.FrameRate;
            mfa.BuildType = 0;
            mfa.BuildPath = game.TargetFilename;
            mfa.CommandLine = "";
            mfa.Aboutbox = game.AboutText?.Length > 0
                ? game?.AboutText
                : "";
            //TODO: Controls

            //Object Section
            FrameItems = new Dictionary<int, FrameItem>();
            for (int i = 0; i < game.Frameitems.ItemDict.Keys.Count; i++)
            {
                var key = game.Frameitems.ItemDict.Keys.ToArray()[i];
                var item = game.Frameitems.ItemDict[key];
                var newItem = TranslateObject(item);
                if(newItem.Loader==null) throw new NotImplementedException("Unsupported Object: "+newItem.ObjectType);              
                FrameItems.Add(newItem.Handle, newItem);
            }


            // var reference = mfa.Frames.FirstOrDefault();
            mfa.Frames.Clear();
            
            foreach (Frame frame in game.Frames)
            {
                if(frame.Palette==null|| frame.Events==null|| frame.Objects==null) continue;

                var newFrame = new MFA.Loaders.Frame(null);
                newFrame.Handle = game.Frames.IndexOf(frame)+1;
                newFrame.Name = frame.Name;
                newFrame.SizeX = frame.Width;
                newFrame.SizeY = frame.Height;
                newFrame.Background = frame.Background;
                newFrame.FadeIn = null;
                newFrame.FadeOut = null;
                // newFrame.UnkString = "             ";
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
                newFrame.MaxObjects = frame.Events?.MaxObjects ?? 10000;
                newFrame.Password = "";
                newFrame.LastViewedX = 320;
                newFrame.LastViewedY = 240;
                newFrame.Palette = frame.Palette.Items;
                newFrame.StampHandle = 13;
                newFrame.ActiveLayer = 0;
                //LayerInfo
                var count = frame.Layers.Items.Count;
                Logger.Log($"{newFrame.Name} - {count}");
                for (int i=0;i<count;i++)
                {
                    var layer = frame.Layers.Items[i];
                    var newLayer = new MFA.Loaders.Layer(null);
                    newLayer.Name = layer.Name;
                    newLayer.Flags["HideAtStart"] = layer.Flags["ToHide"];
                    newLayer.Flags["Visible"] = true;
                    newLayer.Flags["NoBackground"] = layer.Flags["DoNotSaveBackground"];
                    newLayer.Flags["WrapHorizontally"] = layer.Flags["WrapHorizontally"];
                    newLayer.Flags["WrapVertically"] = layer.Flags["WrapVertically"];
                    // newLayer.Flags.flag = 0;
                    newLayer.XCoefficient = layer.XCoeff;
                    newLayer.YCoefficient = layer.YCoeff;
                    newFrame.Layers.Add(newLayer);
                    break;
                    // 

                }

                Message("Translating frame: " + newFrame.Name);
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
                            if(!newFrameItems.Contains(frameItem)) newFrameItems.Add(frameItem);
                            var newInstance = new FrameInstance((ByteReader) null);
                            newInstance.X = instance.X;
                            newInstance.Y = instance.Y;
                            newInstance.Handle = instance.Handle;
                            newInstance.Flags = instance.FrameItem.Flags.flag;
                            newInstance.ParentType = (uint) instance.ParentType;
                            newInstance.ItemHandle = (uint) (instance.ObjectInfo);
                            newInstance.ParentHandle = (uint) instance.ParentHandle;
                            newInstance.Layer = 0;//(uint) (instance.Layer);
                            newInstances.Add(newInstance);
                            // Logger.Log($"{instance.FrameItem.Name} - {i}");

                        }
                    }
                }



                newFrame.Items = newFrameItems;
                newFrame.Instances = newInstances;
                newFrame.Folders = new List<ItemFolder>();
                foreach (FrameItem newFrameItem in newFrame.Items)
                {
                    var newFolder = new ItemFolder((ByteReader) null);
                    newFolder.isRetard = true;
                    newFolder.Items = new List<uint>() {(uint) newFrameItem.Handle};
                    newFrame.Folders.Add(newFolder);
                }


                if (frame.Events != null)
                {
                    newFrame.Events = new Events((ByteReader) null); //MFA.MFA.emptyEvents;
                    newFrame.Events.Items = new List<EventGroup>();
                    newFrame.Events.Objects = new List<EventObject>();
                    newFrame.Events._cache = MFA.MFA.emptyEvents._cache;
                    newFrame.Events._ifMFA = true;
                    newFrame.Events.Version = 1028;
                    // if (frame.Name == "jopajopaher")
                    {
                        foreach (var item in newFrame.Items)
                        {
                            var newObject = new EventObject((ByteReader) null);

                            newObject.Handle = (uint) item.Handle;
                            newObject.Name = item.Name ?? "";
                            newObject.TypeName = "";
                            newObject.ItemType = (ushort) item.ObjectType;
                            newObject.ObjectType = 1;
                            newObject.Flags = 0;
                            newObject.ItemHandle = (uint) item.Handle;
                            newObject.InstanceHandle = 0xFFFFFFFF;
                            newFrame.Events.Objects.Add(newObject);
                        }

                        foreach (EventGroup item in frame.Events.Items)
                        {
                            /*foreach (Action itemAction in item.Actions)
                            {
                                for (int a=0;a<itemAction.Items.Count;a++)
                                {
                                    if (itemAction.Items[a].Loader is ExpressionParameter exp)
                                    {
                                        // itemAction.Items.Remove(itemAction.Items[a]);
                                        
                                    }
                                    else if (itemAction.Items[a].Loader is Sample)
                                    {
                                        itemAction.Items.Remove(itemAction.Items[a]);
                                    }
                                }
                            }
                            foreach (Condition itemAction in item.Conditions)
                            {
                                for (int a=0;a<itemAction.Items.Count;a++)
                                {
                                    if (itemAction.Items[a].Loader is ExpressionParameter exp)
                                    {
                                        // itemAction.Items.Remove(itemAction.Items[a]);
                                        
                                    }
                                    else if (itemAction.Items[a].Loader is Sample)
                                    {
                                        itemAction.Items.Remove(itemAction.Items[a]);
                                    }
                                }
                            }*/
                            newFrame.Events.Items.Add(item);
                        }

                    }
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
            else
            {
                return alterables;
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
            else
            {
                return alterables;
            }

            return alterables;
        }
        public static FrameItem TranslateObject(ObjectInfo item)
        {
            var newItem = new FrameItem(null);
            newItem.Name = item.Name;
            newItem.ObjectType = item.ObjectType;
            newItem.Handle = item.Handle;
            newItem.Transparent = item.Transparent ? 1 : 0;
            newItem.InkEffect = item.InkEffect;
            newItem.InkEffectParameter = item.InkEffectValue;
            newItem.AntiAliasing = item.Antialias ? 1 : 0;
            newItem.Flags = (int) item.Flags.flag; //32 TODO:Fix this 
            newItem.IconHandle = 12;
            newItem.Chunks = new ChunkList(null);

            if (item.ObjectType == 0)
            {
                var backdropLoader = (EXE.Loaders.Objects.Quickbackdrop) item.Properties.Loader;
                var backdrop = new QuickBackdrop((ByteReader) null);
                backdrop.ObstacleType = (uint) backdropLoader.ObstacleType;
                backdrop.CollisionType = (uint) backdropLoader.CollisionType;
                backdrop.Width = backdropLoader.Width;
                backdrop.Height = backdropLoader.Height;
                backdrop.Shape = backdropLoader.Shape.ShapeType;
                backdrop.BorderSize = backdropLoader.Shape.BorderSize;
                backdrop.FillType = backdropLoader.Shape.FillType;
                backdrop.Color1 = backdropLoader.Shape.Color1;
                backdrop.Color2 = backdropLoader.Shape.Color2;
                backdrop.Flags = backdropLoader.Shape.GradFlags;
                backdrop.Image = backdropLoader.Image;
                newItem.Loader = backdrop;

            }
            else if (item.ObjectType == 1)
            {
                var backdropLoader = (EXE.Loaders.Objects.Backdrop) item.Properties.Loader;
                var backdrop = new Backdrop((ByteReader) null);
                backdrop.ObstacleType = (uint) backdropLoader.ObstacleType;
                backdrop.CollisionType = (uint) backdropLoader.CollisionType;
                backdrop.Handle = backdropLoader.Image;
                newItem.Loader = backdrop;
            }
            else
            {
                var itemLoader = (ObjectCommon) item?.Properties?.Loader;
                if (itemLoader == null) throw new NotImplementedException("Null loader");
                Logger.Log("Translating Object: " + itemLoader.Parent.Name);
                //CommonSection
                var newObject = new ObjectLoader(null);
                newObject.ObjectFlags = (int) (itemLoader?.Flags?.flag ?? 820);
                newObject.NewObjectFlags = (int) (itemLoader?.NewFlags?.flag ?? 8);
                newObject.BackgroundColor = Color.Black; //itemLoader.BackColor;
                //newLoader.Qualifiers;
                newObject.Strings = ConvertStrings(itemLoader.Strings);
                newObject.Values = ConvertValue(itemLoader.Values);
                newObject.Movements = new MFA.Loaders.mfachunks.Movements(null);
                for (int j = 0; j < itemLoader.Movements.Items.Count; j++)
                {
                    var mov = itemLoader.Movements.Items[j];
                    var newMov = new Movement(null);
                    newMov.Name = $"Movement #{j}";
                    newMov.Extension = "";
                    newMov.Identifier = 0; //(uint) mov.Type;
                    if (newMov.Identifier != 0) throw new Exception("Unknown Movement: " + newMov.Identifier);
                    newMov.Player = mov.Player;
                    newMov.MovingAtStart = mov.MovingAtStart;
                    newMov.DirectionAtStart = mov.DirectionAtStart;
                    newObject.Movements.Items.Add(newMov);
                }

                newObject.Behaviours = new Behaviours(null);

                if (item.ObjectType == 2)
                {
                    var active = new Active(null);
                    //Shit Section
                    {
                        active.ObjectFlags = newObject.ObjectFlags;
                        active.NewObjectFlags = newObject.NewObjectFlags;
                        active.BackgroundColor = newObject.BackgroundColor;
                        active.Strings = newObject.Strings;
                        active.Values = newObject.Values;
                        active.Movements = newObject.Movements;
                        active.Behaviours = newObject.Behaviours;
                    }


                    //TODO: Transitions
                    if (itemLoader.Animations != null)
                    {
                        var animHeader = itemLoader.Animations;
                        for (int j = 0; j < animHeader.AnimationDict.Count; j++)
                        {
                            var origAnim = animHeader.AnimationDict.ToArray()[j];
                            var newAnimation = new Animation(null);
                            newAnimation.Name = $"User Defined {j}";
                            var newDirections = new List<AnimationDirection>();
                            EXE.Loaders.Objects.Animation animation = null;
                            try
                            {
                                if (animHeader.AnimationDict.ContainsKey(origAnim.Key))
                                {
                                    animation = animHeader.AnimationDict[origAnim.Key];
                                }
                                else break;

                            }
                            catch
                            {
                            }

                            if (animation != null)
                            {
                                if (animation.DirectionDict != null)
                                {
                                    for (int n = 0; n < animation.DirectionDict.Count; n++)
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
                                }
                                else
                                {

                                }

                                newAnimation.Directions = newDirections;
                            }

                            active.Items.Add(j, newAnimation);
                        }
                    }

                    newItem.Loader = active;
                }

                if (item.ObjectType >= 32)
                {
                    var newExt = new ExtensionObject(null);
                    {
                        newExt.ObjectFlags = newObject.ObjectFlags;
                        newExt.NewObjectFlags = newObject.NewObjectFlags;
                        newExt.BackgroundColor = newObject.BackgroundColor;
                        newExt.Strings = newObject.Strings;
                        newExt.Values = newObject.Values;
                        newExt.Movements = newObject.Movements;
                        newExt.Behaviours = newObject.Behaviours;
                    }
                    var exts = Exe.Instance.GameData.GameChunks.GetChunk<Extensions>();
                    Extension ext = null;
                    foreach (var testExt in exts.Items)
                    {
                        if (testExt.Handle == item.ObjectType - 32) ext = testExt;
                    }

                    newExt.ExtensionType = -1;
                    newExt.ExtensionName = "";
                    newExt.Filename = $"{ext.Name}.mfx";
                    newExt.Magic = (uint) ext.MagicNumber;
                    newExt.SubType = ext.SubType;
                    newExt.ExtensionVersion = itemLoader.ExtensionVersion;
                    newExt.ExtensionId = itemLoader.ExtensionId;
                    newExt.ExtensionPrivate = itemLoader.ExtensionPrivate;
                    newExt.ExtensionData = itemLoader.ExtensionData;
                    newItem.Loader = newExt;
                    var tuple = new Tuple<int, string, string, int, string>(ext.Handle, ext.Name, "",
                        ext.MagicNumber, ext.SubType);
                    // mfa.Extensions.Add(tuple);

                }
                else if (item.ObjectType == 3)
                {
                    var text = itemLoader.Text;
                    var newText = new Text(null);
                    //Shit Section
                    {
                        newText.ObjectFlags = newObject.ObjectFlags;
                        newText.NewObjectFlags = newObject.NewObjectFlags;
                        newText.BackgroundColor = newObject.BackgroundColor;
                        newText.Strings = newObject.Strings;
                        newText.Values = newObject.Values;
                        newText.Movements = newObject.Movements;
                        newText.Behaviours = newObject.Behaviours;
                    }
                    newText.Width = (uint) text.Width;
                    newText.Height = (uint) text.Height;
                    var paragraph = text.Items[0];
                    newText.Font = paragraph.FontHandle;
                    newText.Color = paragraph.Color;
                    newText.Flags = 0;
                    newText.Items = new List<Paragraph>();
                    foreach (EXE.Loaders.Objects.Paragraph exePar in text.Items)
                    {
                        var newPar = new Paragraph((ByteReader) null);
                        newPar.Value = exePar.Value;
                        newPar.Flags = exePar.Flags.flag;
                        newText.Items.Add(newPar);
                    }

                    newItem.Loader = newText;
                }
                else if (item.ObjectType == 6)
                {
                    var counter = itemLoader.Counters;
                    var lives = new Lives(null);
                    {
                        lives.ObjectFlags = newObject.ObjectFlags;
                        lives.NewObjectFlags = newObject.NewObjectFlags;
                        lives.BackgroundColor = newObject.BackgroundColor;
                        lives.Strings = newObject.Strings;
                        lives.Values = newObject.Values;
                        lives.Movements = newObject.Movements;
                        lives.Behaviours = newObject.Behaviours;
                    }
                    lives.Player = counter?.Player ?? 0;
                    lives.Images = counter?.Frames ?? new List<int>() {0};
                    lives.DisplayType = counter?.DisplayType ?? 0;
                    lives.Flags = counter?.Flags ?? 0;
                    lives.Font = counter?.Font ?? 0;
                    lives.Width = (int) (counter?.Width ?? 0);
                    lives.Height = (int) (counter?.Height ?? 0);
                    newItem.Loader = lives;

                }
                else if (item.ObjectType == 7)
                {
                    var counter = itemLoader.Counters;
                    var newCount = new Counter(null);
                    {
                        newCount.ObjectFlags = newObject.ObjectFlags;
                        newCount.NewObjectFlags = newObject.NewObjectFlags;
                        newCount.BackgroundColor = newObject.BackgroundColor;
                        newCount.Strings = newObject.Strings;
                        newCount.Values = newObject.Values;
                        newCount.Movements = newObject.Movements;
                        newCount.Behaviours = newObject.Behaviours;
                    }
                    newCount.Value = itemLoader.Counter.Initial;
                    newCount.Maximum = itemLoader.Counter.Maximum;
                    newCount.Minimum = itemLoader.Counter.Minimum;
                    newCount.Images = new List<int>() {0};
                    if (counter == null)
                    {
                        newCount.DisplayType = 0;
                        newCount.CountType = 0;
                        newCount.Width = 0;
                        newCount.Height = 0;
                        newCount.Font = 0;
                    }
                    else
                    {
                        newCount.DisplayType = counter.DisplayType;
                        newCount.CountType = counter.Inverse ? 1 : 0;
                        newCount.Width = (int) counter.Width;
                        newCount.Height = (int) counter.Height;
                        newCount.Images = counter.Frames;
                        newCount.Font = counter.Font;
                    }

                    newCount.Color1 = Color.White;
                    newCount.Color2 = Color.White;
                    newCount.Flags = 0;
                    newCount.VerticalGradient = 0;

                    newItem.Loader = newCount;
                }

            }

            return newItem;
        }

        public static void Message(string msg)
        {
            OnMessage?.Invoke(msg);
            Logger.Log(msg);
        }

        
        
    }

    
}
