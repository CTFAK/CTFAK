using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CTFAK.MMFParser.EXE;
using CTFAK.MMFParser.EXE.Loaders;
using CTFAK.MMFParser.EXE.Loaders.Banks;
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
using Extension = CTFAK.MMFParser.EXE.Loaders.Extension;
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

            //if (game.Fonts != null) mfa.Fonts = game.Fonts;
            mfa.Sounds.Items.Clear();
            if (game.Sounds != null)
            {
                foreach (var item in game.Sounds.Items)
                {
                    mfa.Sounds.Items.Add(item);
                }
            }
            
            
            // mfa.Music = game.Music;
            mfa.Images.Items = game.Images?.Images ?? new Dictionary<int, ImageItem>();
            foreach (var key in mfa.Images.Items.Keys)
            {
                mfa.Images.Items[key].Debug = true;
            }

            // game.Images.Images.Clear();

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
            
            displaySettings["MaximizedOnBoot"] = flags["Maximize"];
            displaySettings["ResizeDisplay"] = flags["MDI"];
            displaySettings["FullscreenAtStart"] = flags["FullscreenAtStart"];
            displaySettings["AllowFullscreen"] = flags["FullscreenSwitch"];
            // displaySettings["Heading"] = !flags["NoHeading"];
            // displaySettings["HeadingWhenMaximized"] = true;
            displaySettings["MenuBar"] = flags["MenuBar"];
            displaySettings["MenuOnBoot"] = !flags["MenuHidden"];
            displaySettings["NoMinimize"] = newFlags["NoMinimizeBox"];
            displaySettings["NoMaximize"] = newFlags["NoMaximizeBox"];
            displaySettings["NoThickFrame"] = newFlags["NoThickFrame"];
            // displaySettings["NoCenter"] = flags["MDI"];
            displaySettings["DisableClose"] = newFlags["DisableClose"];
            displaySettings["HiddenAtStart"] = newFlags["HiddenAtStart"];
            displaySettings["MDI"] = newFlags["MDI"];
    
            
            mfa.GraphicFlags = graphicSettings;
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
            mfa.FrameRate = 60;
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
            
            Dictionary<int,int> indexHandles = new Dictionary<int, int>();
            foreach (var pair in Program.CleanData.GameChunks.GetChunk<FrameHandles>().Items)
            {
                var key = pair.Key;
                var handle = pair.Value;
                if (!indexHandles.ContainsKey(handle)) indexHandles.Add(handle, key);
                else indexHandles[handle] = key;
            }
            
            
            for (int a=0;a<game.Frames.Count;a++)
            {
                var frame = game.Frames[a];
                // if(frame.Palette==null|| frame.Events==null|| frame.Objects==null) continue;
                
                var newFrame = new MFA.Loaders.Frame(null);
                newFrame.Chunks = new ChunkList(null);//MFA.MFA.emptyFrameChunks;
                newFrame.Handle = a;
                indexHandles.TryGetValue(a,out newFrame.Handle);
                Message($"Translating frame: {frame.Name} - {newFrame.Handle}" );
                newFrame.Name = frame.Name;
                newFrame.SizeX = frame.Width;
                newFrame.SizeY = frame.Height;

                newFrame.Background = frame.Background;
                newFrame.FadeIn = frame.FadeIn!=null ? ConvertTransition(frame.FadeIn):null;
                newFrame.FadeOut = frame.FadeOut!=null ? ConvertTransition(frame.FadeOut):null;
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
                if (frame.Palette == null) continue;
                newFrame.Palette = frame.Palette;
                newFrame.StampHandle = 13;
                newFrame.ActiveLayer = 0;
                //LayerInfo
                if(frame.Layers==null) continue;
                var count = frame.Layers.Count;
                for (int i=0;i<count;i++)
                {
                    var layer = frame.Layers[i];
                    var newLayer = new Layer(null);
                    newLayer.Name = layer.Name;
                    newLayer.Flags["HideAtStart"] = layer.Flags["ToHide"];
                    newLayer.Flags["Visible"] = true;
                    newLayer.Flags["NoBackground"] = layer.Flags["DoNotSaveBackground"];
                    newLayer.Flags["WrapHorizontally"] = layer.Flags["WrapHorizontally"];
                    newLayer.XCoefficient = layer.XCoeff;
                    newLayer.YCoefficient = layer.YCoeff;
        
                    newFrame.Layers.Add(newLayer);
      

                }

                
                var newFrameItems = new List<FrameItem>();
                var newInstances = new List<FrameInstance>();
                if (frame.Objects != null)
                {

                    for (int i = 0; i < frame.Objects.Count; i++)
                    {
                        var instance = frame.Objects[i];
                        FrameItem frameItem;

                        if (FrameItems.ContainsKey(instance.ObjectInfo))
                        {
                            frameItem = FrameItems[instance.ObjectInfo];
                            if(!newFrameItems.Contains(frameItem)) newFrameItems.Add(frameItem);
                            var newInstance = new FrameInstance((ByteReader) null);
                            newInstance.X = instance.X;
                            newInstance.Y = instance.Y;
                            newInstance.Handle = instance.Handle;
                            newInstance.Flags = (uint) instance.FrameItem.Flags;
                            newInstance.ParentType = (uint) instance.ParentType;
                            newInstance.ItemHandle = (uint) (instance.ObjectInfo);
                            newInstance.ParentHandle = (uint) instance.ParentHandle;
                            newInstance.Layer = (uint) (instance.Layer);
                            newInstances.Add(newInstance);
                        }
                        else
                        {
                            throw new NullReferenceException("Object");
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
          
                {
                    newFrame.Events = new Events((ByteReader) null);
                    newFrame.Events.Items = new List<EventGroup>();
                    newFrame.Events.Objects = new List<EventObject>();
                    newFrame.Events._ifMFA = true;
                    newFrame.Events.Version = 1028;
                    // if (false) 
                    if(frame.Events != null)
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

                        newFrame.Events.Items = frame.Events.Items;
                        /*for (int i=0;i<frame.Events.Items.Count;i++)
                        {
                            var item = frame.Events.Items[i];
                            newFrame.Events.Items.Add(item);
                            for (int j = 0; j < item.Conditions.Count; j++)
                            {
                                var cond = item.Conditions[j];
                                // if (newFrame.Items.ContainsItem(cond.ObjectInfo)) item.Conditions.Remove(cond);
                            }
                            for (int j = 0; j < item.Actions.Count; j++)
                            {
                                var act = item.Actions[j];
                                // if (newFrame.Items.ContainsItem(act.ObjectInfo)) item.Actions.Remove(act);
                            }
                            
                        }*/
                    }
                }
                
                mfa.Frames.Add(newFrame);
                


            }
        }

        public static MFA.Loaders.Transition ConvertTransition(EXE.Loaders.Transition gameTrans)
        {
            var mfaTrans = new MFA.Loaders.Transition((ByteReader) null)
            {
                Module = "cctrans.dll",//gameTrans.ModuleFile,
                Name = "Transition",
                Id = gameTrans.Module,
                TransitionId = gameTrans.Name,
                Flags = gameTrans.Flags,
                Color = gameTrans.Color,
                ParameterData = gameTrans.ParameterData,
                Duration = gameTrans.Duration
            };
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
            newItem.Chunks = new ChunkList(null);
            newItem.Name = item.Name;
            newItem.ObjectType = (int)item.ObjectType;
            newItem.Handle = item.Handle;
            newItem.Transparent = 1;
            newItem.InkEffect = item.InkEffect;
            newItem.InkEffectParameter = item.InkEffectValue;
            newItem.AntiAliasing = item.Antialias? 1 : 0;
            newItem.Flags = item.Flags;
            if (item.InkEffectValue == 0&&Settings.Build<=284)
            {
                // newItem.Chunks.GetOrCreateChunk<Opacity>().Blend = 255;
            }
            else
            {
                // newItem.Chunks.GetOrCreateChunk<Opacity>().Blend = item.InkEffectValue;
            }
            // newItem.Chunks.GetOrCreateChunk<Opacity>().RGBCoeff = Color.White;
            
            newItem.IconHandle = 12;
            
            

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
                backdrop.Image = backdropLoader.Shape.Image;
                newItem.Loader = backdrop;

            }
            else if (item.ObjectType == Constants.ObjectType.Backdrop)
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
                Logger.Log(("Translating Object: " + itemLoader.Parent.Name),false,ConsoleColor.Blue,false);
                //CommonSection
                var newObject = new ObjectLoader(null);
                newObject.ObjectFlags = (int) (itemLoader.Flags.flag);
                newObject.NewObjectFlags = (int) (itemLoader.NewFlags.flag);

                newObject.BackgroundColor = itemLoader.BackColor;
                
                newObject.Qualifiers = itemLoader._qualifiers;
                
                newObject.Strings = ConvertStrings(itemLoader.Strings);
                newObject.Values = ConvertValue(itemLoader.Values);
                newObject.Movements = new MFA.Loaders.mfachunks.Movements(null);
                for (int j = 0; j < itemLoader.Movements?.Items?.Count; j++)
                {
                    var mov = itemLoader.Movements.Items[j];
                    var newMov = new Movement(null);
                    newMov.Name = $"Movement #{j}";
                    newMov.Extension = "";
                    newMov.Type =  mov.Type;
                    newMov.Identifier = (uint) mov.Type;
                    newMov.Loader = mov.Loader;
                    newMov.Player = mov.Player;
                    newMov.MovingAtStart = mov.MovingAtStart;
                    newMov.DirectionAtStart = mov.DirectionAtStart;
                    newObject.Movements.Items.Add(newMov);
                }

                newObject.Behaviours = new Behaviours(null);

                if (item.ObjectType == Constants.ObjectType.Active)
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
                        active.Qualifiers = newObject.Qualifiers;
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
                            if (animHeader.AnimationDict.ContainsKey(origAnim.Key))
                            {
                                animation = animHeader?.AnimationDict[origAnim.Key];
                            }
                            else break;

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

                if ((int)item.ObjectType >= 32)
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
                        newExt.Qualifiers = newObject.Qualifiers;

                    }
                    var exts = Program.CleanData.GameChunks.GetChunk<Extensions>();
                    Extension ext = null;
                    foreach (var testExt in exts.Items)
                    {
                        if (testExt.Handle == (int)item.ObjectType - 32) ext = testExt;
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
                else if (item.ObjectType == Constants.ObjectType.Text)
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
                        newText.Qualifiers = newObject.Qualifiers;

                    }
                    if (text == null)
                    {
                        newText.Width = 10;
                        newText.Height = 10;
                        newText.Font = 0;
                        newText.Color=Color.Black;
                        newText.Flags = 0;
                        newText.Items=new List<Paragraph>();
                    }
                    else
                    {
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
                    }
                    

                    newItem.Loader = newText;
                }
                else if (item.ObjectType == Constants.ObjectType.Lives|| item.ObjectType==Constants.ObjectType.Score)
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
                        lives.Qualifiers = newObject.Qualifiers;

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
                else if (item.ObjectType == Constants.ObjectType.Counter)
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
                        newCount.Qualifiers = newObject.Qualifiers;

                    }
                    if (itemLoader.Counter == null)
                    {
                        newCount.Value = 0;
                        newCount.Minimum = 0;
                        newCount.Maximum = 0;

                    }
                    else
                    {
                        newCount.Value = itemLoader.Counter.Initial;
                        newCount.Maximum = itemLoader.Counter.Maximum;
                        newCount.Minimum = itemLoader.Counter.Minimum;
                    }
                    
                    newCount.Images = new List<int>() {0};
                    var shape = counter?.Shape;
                    // if(counter==null) throw new NullReferenceException(nameof(counter));
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
                    
                    if (shape == null)
                    {
                        newCount.Color1=Color.Black;
                        newCount.Color2=Color.Black;
                        newCount.VerticalGradient = 0;
                        newCount.Flags = 0;
                    }
                    else
                    {
                        newCount.Color1 = shape.Color1;
                        newCount.Color2 = shape.Color2;
                        newCount.VerticalGradient = (uint) shape.GradFlags;
                        newCount.Flags = (uint) shape.FillType;
                        
                    }

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
