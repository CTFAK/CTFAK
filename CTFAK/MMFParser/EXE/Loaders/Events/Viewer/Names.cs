using System.Collections.Generic;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Viewer
{
     public static class Names
     {
          public static Dictionary<int, Dictionary<int, string>> ExpressionNames =
               new Dictionary<int, Dictionary<int, string>>()
               {
                    {
                         -7, new Dictionary<int, string>()
                         {

                              {0, "PlayerScore"},
                              {1, "PlayerLives"},
                              {2, "PlayerInputDevice"},
                              {3, "PlayerKeyName"},
                              {4, "PlayerName"}
                         }
                    },
                    {
                         -6, new Dictionary<int, string>()
                         {
                              {0, "XMouse"},
                              {1, "YMouse"},
                              {2, "MouseWheelValue"}
                         }
                    },
                    {-5, new Dictionary<int, string>() {{0, "TotalObjectCount"}}},
                    {
                         -4, new Dictionary<int, string>()
                         {
                              {0, "TimerValue"},
                              {1, "TimerHundreds"},
                              {2, "TimerSeconds"},
                              {3, "TimerHours"},
                              {4, "TimerMinutes"},
                              {5, "TimerEventIndex"}
                         }
                    },
                    {
                         -3, new Dictionary<int, string>()
                         {
                              {0, "CurrentFrameOld"},
                              {1, "PlayerCount"},
                              {2, "XLeftFrame"},
                              {3, "XRightFrame"},
                              {4, "YTopFrame"},
                              {5, "YBottomFrame"},
                              {6, "FrameWidth"},
                              {7, "FrameHeight"},
                              {8, "CurrentFrame"},
                              {9, "GetCollisionMask"},
                              {10, "FrameRate"},
                              {11, "GetVirtualWidth"},
                              {12, "GetVirtualHeight"},
                              {13, "FrameBackgroundColor"},
                              {14, "DisplayMode"},
                              {15, "PixelShaderVersion"},
                              {16, "FrameAlphaCoefficient"},
                              {17, "FrameRGBCoefficient"},
                              {18, "FrameEffectParameter"}
                         }
                    },
                    {
                         -2, new Dictionary<int, string>()
                         {
                              {0, "GetMainVolume"},
                              {1, "GetSampleVolume"},
                              {2, "GetChannelVolume"},
                              {3, "GetMainPan"},
                              {4, "GetSamplePan"},
                              {5, "GetChannelPan"},
                              {6, "GetSamplePosition"},
                              {7, "GetChannelPosition"},
                              {8, "GetSampleDuration"},
                              {9, "GetChannelDuration"},
                              {10, "GetSampleFrequency"},
                              {11, "GetChannelFrequency"}
                         }
                    },
                    {
                         -1, new Dictionary<int, string>()
                         {
                              {-3, ","},
                              {-2, ")"},
                              {-1, "("},
                              {0, "Long"},
                              {1, "Random"},
                              {2, "GlobalValueExpression"},
                              {3, "String"},
                              {4, "ToString"},
                              {5, "ToNumber"},
                              {6, "ApplicationDrive"},
                              {7, "ApplicationDirectory"},
                              {8, "ApplicationPath"},
                              {9, "ApplicationFilename"},
                              {10, "Sin"},
                              {11, "Cos"},
                              {12, "Tan"},
                              {13, "SquareRoot"},
                              {14, "Log"},
                              {15, "Ln"},
                              {16, "Hex"},
                              {17, "Bin"},
                              {18, "Exp"},
                              {19, "LeftString"},
                              {20, "RightString"},
                              {21, "MidString"},
                              {22, "StringLength"},
                              {23, "Double"},
                              {24, "GlobalValue"},
                              {28, "ToInt"},
                              {29, "Abs"},
                              {30, "Ceil"},
                              {31, "Floor"},
                              {32, "Acos"},
                              {33, "Asin"},
                              {34, "Atan"},
                              {35, "Not"},
                              {36, "DroppedFileCount"},
                              {37, "DroppedFilename"},
                              {38, "GetCommandLine"},
                              {39, "GetCommandItem"},
                              {40, "Min"},
                              {41, "Max"},
                              {42, "GetRGB"},
                              {43, "GetRed"},
                              {44, "GetGreen"},
                              {45, "GetBlue"},
                              {46, "LoopIndex"},
                              {47, "NewLine"},
                              {48, "Round"},
                              {49, "GlobalStringExpression"},
                              {50, "GlobalString"},
                              {51, "LowerString"},
                              {52, "UpperString"},
                              {53, "Find"},
                              {54, "ReverseFind"},
                              {55, "GetClipboard"},
                              {56, "TemporaryPath"},
                              {57, "TemporaryBinaryFilePath"},
                              {58, "FloatToString"},
                              {59, "Atan2"},
                              {60, "Zero"},
                              {61, "Empty"},
                              {62, "DistanceBetween"},
                              {63, "AngleBetween"},
                              {64, "ClampValue"},
                              {65, "RandomRange"}
                         }
                    },
                    {
                         0, new Dictionary<int, string>()
                         {
                              {0, ";"},
                              {2, "+"},
                              {4, "-"},
                              {6, "*"},
                              {8, "/"},
                              {10, "%"},
                              {12, "**"},
                              {14, "&"},
                              {16, "|"},
                              {18, "^"}
                         }
                    },
                    {
                         2, new Dictionary<int, string>()
                         {
                              {16, "AlterableValue"},
                              {27, "GetAlphaCoefficient"},
                              {80, "GetColorAt"},
                              {81, "GetXScale"},
                              {82, "GetYScale"},
                              {83, "GetAngle"}
                         }
                    },
                    {
                         3, new Dictionary<int, string>()
                         {
                              {80, "CurrentParagraphIndex"},
                              {81, "CurrentText"},
                              {82, "GetParagraph"},
                              {83, "TextAsNumber"},
                              {84, "ParagraphCount"}
                         }
                    },
                    {
                         7, new Dictionary<int, string>()
                         {
                              {80, "CounterValue"},
                              {81, "CounterMinimumValue"},
                              {82, "CounterMaximumValue"},
                              {83, "CounterColor1"},
                              {84, "CounterColor2"}
                         }
                    },
                    {
                         8, new Dictionary<int, string>()
                         {
                              {80, "RTFXPOS"},
                              {81, "RTFYPOS"},
                              {82, "RTFSXPAGE"},
                              {83, "RTFSYPAGE"},
                              {84, "RTFZOOM"},
                              {85, "RTFWORDMOUSE"},
                              {86, "RTFWORDXY"},
                              {87, "RTFWORD"},
                              {88, "RTFXWORD"},
                              {89, "RTFYWORD"},
                              {90, "RTFSXWORD"},
                              {91, "RTFSYWORD"},
                              {92, "RTFLINEMOUSE"},
                              {93, "RTFLINEXY"},
                              {94, "RTFXLINE"},
                              {95, "RTFYLINE"},
                              {96, "RTFSXLINE"},
                              {97, "RTFSYLINE"},
                              {98, "RTFPARAMOUSE"},
                              {99, "RTFPARAXY"},
                              {100, "RTFXPARA"},
                              {101, "RTFYPARA"},
                              {102, "RTFSXPARA"},
                              {103, "RTFSYPARA"},
                              {104, "RTFXWORDTEXT"},
                              {105, "RTFYWORDTEXT"},
                              {106, "RTFXLINETEXT"},
                              {107, "RTFYLINETEXT"},
                              {108, "RTFXPARATEXT"},
                              {109, "RTFYPARATEXT"},
                              {110, "RTFMEMSIZE"},
                              {111, "RTFGETFOCUSWORD"},
                              {112, "RTFGETHYPERLINK"}
                         }
                    },
                    {
                         9, new Dictionary<int, string>()
                         {
                              {80, "SubApplicationFrameNumber"},
                              {81, "SubApplicationGlobalValue"},
                              {82, "SubApplicationGlobalString"}
                         }
                    },
                    {
                         32, new Dictionary<int, string>()
                         {
                              {82, "GetValue"},
                              {83, "GroupItemValue"}
                         }
                    },
                    {
                         35, new Dictionary<int, string>()
                         {
                              {22, "GetTextColor"},
                              {80, "GetText"}
                         }
                    }
               };






     public static Dictionary<int, Dictionary<int, string>> ConditionNames =
               new Dictionary<int, Dictionary<int, string>>()
               {
                    {
                         -7, new Dictionary<int, string>()
                         {
                              {-6, "PlayerKeyDown"},
                              {-5, "PlayerDied"},
                              {-4, "PlayerKeyPressed"},
                              {-3, "NumberOfLives"},
                              {-2, "CompareScore"},
                              {-1, "PLAYERPLAYING"}
                         }
                    },

                    {
                         -6, new Dictionary<int, string>()
                         {
                              {-12, "MouseWheelDown"},
                              {-11, "MouseWheelUp"},
                              {-10, "MouseVisible"},
                              {-9, "AnyKeyPressed"},
                              {-8, "WhileMousePressed"},
                              {-7, "ObjectClicked"},
                              {-6, "MouseClickedInZone"},
                              {-5, "MouseClicked"},
                              {-4, "MouseOnObject"},
                              {-3, "MouseInZone"},
                              {-2, "KeyDown"},
                              {-1, "KeyPressed"}
                         }
                    },
                    {
                         -5, new Dictionary<int, string>()
                         {
                              {-23, "PickObjectsInLine"},
                              {-22, "PickFlagOff"},
                              {-21, "PickFlagOn"},
                              {-20, "PickAlterableValue"},
                              {-19, "PickFromFixed"},
                              {-18, "PickObjectsInZone"},
                              {-17, "PickRandomObject"},
                              {-16, "PickRandomObjectInZone"},
                              {-15, "CompareObjectCount"},
                              {-14, "AllObjectsInZone"},
                              {-13, "NoAllObjectsInZone"},
                              {-12, "PickFlagOff"},
                              {-11, "PickFlagOn"},
                              {-8, "PickAlterableValue"},
                              {-7, "PickFromFixed"},
                              {-6, "PickObjectsInZone"},
                              {-5, "PickRandomObject"},
                              {-4, "PickRandomObjectInZoneOld"},
                              {-3, "CompareObjectCount"},
                              {-2, "AllObjectsInZone"},
                              {-1, "NoAllObjectsInZone"}
                         }
                    },
                    {
                         -4, new Dictionary<int, string>()
                         {
                              {-8, "Every"},
                              {-7, "TimerEquals"},
                              {-6, "OnTimerEvent"},
                              {-5, "CompareAwayTime"},
                              {-4, "Every"},
                              {-3, "TimerEquals"},
                              {-2, "TimerLess"},
                              {-1, "TimerGreater"}
                         }
                    },
                    {
                         -3, new Dictionary<int, string>()
                         {
                              {-10, "FrameSaved"},
                              {-9, "FrameLoaded"},
                              {-8, "ApplicationResumed"},
                              {-7, "VsyncEnabled"},
                              {-6, "IsLadder"},
                              {-5, "IsObstacle"},
                              {-4, "EndOfApplication"},
                              {-3, "LEVEL"},
                              {-2, "EndOfFrame"},
                              {-1, "StartOfFrame"}
                         }
                    },
                    {
                         -2, new Dictionary<int, string>()
                         {
                              {-9, "ChannelPaused"},
                              {-8, "ChannelNotPlaying"},
                              {-7, "MusicPaused"},
                              {-6, "SamplePaused"},
                              {-5, "MusicFinished"},
                              {-4, "NoMusicPlaying"},
                              {-3, "NoSamplesPlaying"},
                              {-2, "SpecificMusicNotPlaying"},
                              {-1, "SampleNotPlaying"}
                         }
                    },
                    {
                         -1, new Dictionary<int, string>()
                         {
                              {-40, "RunningAs"},
                              {-39, "CompareGlobalValueDoubleGreater"},
                              {-38, "CompareGlobalValueDoubleGreaterEqual"},
                              {-37, "CompareGlobalValueDoubleLess"},
                              {-36, "CompareGlobalValueDoubleLessEqual"},
                              {-35, "CompareGlobalValueDoubleNotEqual"},
                              {-34, "CompareGlobalValueDoubleEqual"},
                              {-33, "CompareGlobalValueIntGreater"},
                              {-32, "CompareGlobalValueIntGreaterEqual"},
                              {-31, "CompareGlobalValueIntLess"},
                              {-30, "CompareGlobalValueIntLessEqual"},
                              {-29, "CompareGlobalValueIntNotEqual"},
                              {-28, "CompareGlobalValueIntEqual"},
                              {-27, "ElseIf"},
                              {-26, "Chance"},
                              {-25, "OrLogical"},
                              {-24, "OrFiltered"},
                              {-23, "OnGroupActivation"},
                              {-22, "ClipboardDataAvailable"},
                              {-21, "CloseSelected"},
                              {-20, "CompareGlobalString"},
                              {-19, "MenuVisible"},
                              {-18, "MenuEnabled"},
                              {-17, "MenuChecked"},
                              {-16, "OnLoop"},
                              {-15, "FilesDropped"},
                              {-14, "MenuSelected"},
                              {-13, "RECORDKEY"},
                              {-12, "GroupActivated"},
                              {-11, "GroupEnd"},
                              {-10, "NewGroup"},
                              {-9, "Remark"},
                              {-8, "CompareGlobalValue"},
                              {-7, "NotAlways"},
                              {-6, "Once"},
                              {-5, "Repeat"},
                              {-4, "RestrictFor"},
                              {-3, "Compare"},
                              {-2, "Never"},
                              {-1, "Always"}
                         }
                    },
                    {
                         2, new Dictionary<int, string>()
                         {
                              {-81, "ObjectClicked"},
                              {-25, "FlagOn"},
                              {-4, "Overlapping"},
                              {-24, "FlagOff"},
                              {-14, "UNK3"},
                              {-13, "UNK2"},
                              {-12, "UNK"},
                              {-42, "AlterableValue"},
                              {-2, "AnimationOver"}
                         }
                    },
                    {
                         4, new Dictionary<int, string>()
                         {
                              {-83, "AnswerMatches"},
                              {-82, "AnswerFalse"},
                              {-81, "AnswerTrue"}
                         }
                    },
                    {
                         7, new Dictionary<int, string>()
                         {
                              {-81, "CompareCounter"}
                         }
                    },
                    {
                         9, new Dictionary<int, string>()
                         {
                              {-84, "SubApplicationPaused"},
                              {-83, "SubApplicationVisible"},
                              {-82, "SubApplicationFinished"},
                              {-81, "SubApplicationFrameChanged"}
                         }
                    },
                    {
                         35, new Dictionary<int, string>()
                         {
                              {-85, "IsFocused"},
                              {22, "GetTextColor"},
                              {80, "GetText"}

                         }
                    }
               };
     }
}
