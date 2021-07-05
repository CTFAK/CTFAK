//----------------------------------------------------------------------------------
//
// CExp: un token d'expression
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Application;
using RuntimeXNA.Services;
using RuntimeXNA.RunLoop;
using RuntimeXNA.OI;

namespace RuntimeXNA.Expressions
{
    public abstract class CExp
    {
        public int code;

        public CExp()
        {
        }

        public static CExp create(CFile file)
        {
            int debut = file.getFilePointer();
            CExp exp = null;
            int c = file.readAInt();

            switch (c)
            {

                case 0x00000000:  // EXPL_END
                    exp = new EXP_END();
                    break;

                case 0x00020000:  // EXPL_PLUS:
                    exp = new EXP_PLUS();
                    break;

                case 0x00040000:  // EXPL_MOINS:
                    exp = new EXP_MINUS();
                    break;

                case 0x00060000:  // EXPL_MULT:
                    exp = new EXP_MULT();
                    break;

                case 0x00080000:  // EXPL_DIV:
                    exp = new EXP_DIV();
                    break;

                case 0x000A0000:  // EXPL_MOD
                    exp = new EXP_MOD();
                    break;

                case 0x000C0000:  // EXPL_POW
                    exp = new EXP_POW();
                    break;

                case 0x000E0000:  // EXPL_AND
                    exp = new EXP_AND();
                    break;

                case 0x00100000:  // EXPL_OR
                    exp = new EXP_OR();
                    break;

                case 0x00120000:  // EXPL_XOR
                    exp = new EXP_XOR();
                    break;

                case ((0 << 16) | 0xFFFF):  // EXP_LONG
                    exp = new EXP_LONG();
                    break;

                case ((1 << 16) | 0xFFFF):  // EXP_RANDOM
                    exp = new EXP_RANDOM();
                    break;

                case ((2 << 16) | 0xFFFF):  // EXP_VARGLO
                    exp = new EXP_VARGLO();
                    break;

                case ((3 << 16) | 0xFFFF):  // EXP_STRING
                    exp = new EXP_STRING();
                    break;

                case ((4 << 16) | 0xFFFF):  // EXP_STR
                    exp = new EXP_STR();
                    break;

                case ((5 << 16) | 0xFFFF):  // EXP_VAL
                    exp = new EXP_VAL();
                    break;

                case ((6 << 16) | 0xFFFF):  // EXP_DRIVE
                    exp = new EXP_PATH();
                    break;

                case ((7 << 16) | 0xFFFF):  // EXP_DIR
                    exp = new EXP_PATH();
                    break;

                case ((8 << 16) | 0xFFFF):  // EXP_PATH
                    exp = new EXP_PATH();
                    break;

                case ((10 << 16) | 0xFFFF):  // EXP_SIN
                    exp = new EXP_SIN();
                    break;

                case ((11 << 16) | 0xFFFF):  // EXP_COS
                    exp = new EXP_COS();
                    break;

                case ((12 << 16) | 0xFFFF):  // EXP_TAN
                    exp = new EXP_TAN();
                    break;

                case ((13 << 16) | 0xFFFF):  // EXP_SQR
                    exp = new EXP_SQR();
                    break;

                case ((14 << 16) | 0xFFFF):  // EXP_LOG
                    exp = new EXP_LOG();
                    break;

                case ((15 << 16) | 0xFFFF):  // EXP_LN
                    exp = new EXP_LN();
                    break;

                case ((16 << 16) | 0xFFFF):  // EXP_HEX
                    exp = new EXP_HEX();
                    break;

                case ((17 << 16) | 0xFFFF):  // EXP_BIN
                    exp = new EXP_BIN();
                    break;

                case ((18 << 16) | 0xFFFF):  // EXP_EXP
                    exp = new EXP_EXP();
                    break;

                case ((19 << 16) | 0xFFFF):  // EXP_LEFT
                    exp = new EXP_LEFT();
                    break;

                case ((20 << 16) | 0xFFFF):  // EXP_RIGHT
                    exp = new EXP_RIGHT();
                    break;

                case ((21 << 16) | 0xFFFF):  // EXP_MID
                    exp = new EXP_MID();
                    break;

                case ((22 << 16) | 0xFFFF):  // EXP_LEN
                    exp = new EXP_LEN();
                    break;

                case ((23 << 16) | 0xFFFF):  // EXP_DOUBLE
                    exp = new EXP_DOUBLE();
                    break;

                case ((24 << 16) | 0xFFFF):  // EXP_VARGLONAMED
                    exp = new EXP_VARGLONAMED();
                    break;
                //          case ((25<<16)|0xFFFF):		// EXP_ENTERSTRINGHERE	
                //                  exp=new EXP_ENTERSTRINGHERE	();
                //                  break;
                //          case ((26<<16)|0xFFFF):		// EXP_ENTERVALUEHERE	
                //                  exp=new EXP_ENTERVALUEHERE	();
                //                  break;
                //          case ((27<<16)|0xFFFF):		// EXP_FLOAT			
                //                  exp=new EXP_FLOAT			();
                //                  break;

                case ((28 << 16) | 0xFFFF):  // EXP_INT
                    exp = new EXP_INT();
                    break;

                case ((29 << 16) | 0xFFFF):  // EXP_ABS
                    exp = new EXP_ABS();
                    break;

                case ((30 << 16) | 0xFFFF):  // EXP_CEIL
                    exp = new EXP_CEIL();
                    break;

                case ((31 << 16) | 0xFFFF):  // EXP_FLOOR
                    exp = new EXP_FLOOR();
                    break;

                case ((32 << 16) | 0xFFFF):  // EXP_ACOS
                    exp = new EXP_ACOS();
                    break;

                case ((33 << 16) | 0xFFFF):  // EXP_ASIN
                    exp = new EXP_ASIN();
                    break;

                case ((34 << 16) | 0xFFFF):  // EXP_ATAN
                    exp = new EXP_ATAN();
                    break;

                case ((35 << 16) | 0xFFFF):  // EXP_NOT
                    exp = new EXP_NOT();
                    break;

                case ((40 << 16) | 0xFFFF):  // EXP_MIN
                    exp = new EXP_MIN();
                    break;

                case ((41 << 16) | 0xFFFF):  // EXP_MAX
                    exp = new EXP_MAX();
                    break;

                case ((42 << 16) | 0xFFFF):  // EXP_GETRGB
                    exp = new EXP_GETRGB();
                    break;

                case ((43 << 16) | 0xFFFF):  // EXP_GETRED
                    exp = new EXP_GETRED();
                    break;

                case ((44 << 16) | 0xFFFF):  // EXP_GETGREEN
                    exp = new EXP_GETGREEN();
                    break;

                case ((45 << 16) | 0xFFFF):  // EXP_GETBLUE
                    exp = new EXP_GETBLUE();
                    break;

                case ((46 << 16) | 0xFFFF):  // EXP_LOOPINDEX
                    exp = new EXP_LOOPINDEX();
                    break;

                case ((47 << 16) | 0xFFFF):  // EXP_NEWLINE
                    exp = new EXP_NEWLINE();
                    break;

                case ((48 << 16) | 0xFFFF):  // EXP_ROUND
                    exp = new EXP_ROUND();
                    break;

                case ((49 << 16) | 0xFFFF):  // EXP_STRINGGLO
                    exp = new EXP_STRINGGLO();
                    break;

                case ((50 << 16) | 0xFFFF):  // EXP_STRINGGLONAMED
                    exp = new EXP_STRINGGLONAMED();
                    break;

                case ((51 << 16) | 0xFFFF):  // EXP_LOWER
                    exp = new EXP_LOWER();
                    break;

                case ((52 << 16) | 0xFFFF):  // EXP_UPPER
                    exp = new EXP_UPPER();
                    break;

                case ((53 << 16) | 0xFFFF):  // EXP_FIND
                    exp = new EXP_FIND();
                    break;

                case ((54 << 16) | 0xFFFF):  // EXP_REVERSEFIND
                    exp = new EXP_REVERSEFIND();
                    break;

                case ((58 << 16) | 0xFFFF):
                    exp = new EXP_FLOATTOSTRING();
                    break;

                case ((59 << 16) | 0xFFFF):
                    exp = new EXP_ATAN2();
                    break;

                case ((60 << 16) | 0xFFFF):
                    exp = new EXP_ZERO();
                    break;

                case ((61 << 16) | 0xFFFF):
                    exp = new EXP_EMPTY();
                    break;

                case ((67 << 16) | 0xFFFF):
                    exp = new EXP_RUNTIMENAME();
                    break;

                case ((-1 << 16) | 0xFFFF):  // EXP_PARENTH1
                    exp = new EXP_PARENTH1();
                    break;

                case ((-2 << 16) | 0xFFFF):  // EXP_PARENTH2
                    exp = new EXP_PARENTH2();
                    break;

                case ((-3 << 16) | 0xFFFF):  // EXP_VIRGULE
                    exp = new EXP_VIRGULE();
                    break;

                case ((0 << 16) | 0xFFFE):  // EXP_GETSAMPLEMAINVOL
                    exp = new EXP_GETSAMPLEMAINVOL();
                    break;

                case ((1 << 16) | 0xFFFE):  // EXP_GETSAMPLEVOL
                    exp = new EXP_GETSAMPLEVOL();
                    break;

                case ((2 << 16) | 0xFFFE):  // EXP_GETCHANNELVOL
                    exp = new EXP_GETCHANNELVOL();
                    break;

                case ((3 << 16) | 0xFFFE):  // EXP_GETSAMPLEMAINPAN
                    exp = new EXP_GETSAMPLEMAINPAN();
                    break;

                case ((4 << 16) | 0xFFFE):  // EXP_GETSAMPLEPAN
                    exp = new EXP_GETSAMPLEPAN();
                    break;

                case ((5 << 16) | 0xFFFE):  // EXP_GETCHANNELPAN
                    exp = new EXP_GETCHANNELPAN();
                    break;

                case ((8 << 16) | 0xFFFE):  // EXP_GETSAMPLEDUR
                    exp = new EXP_GETSAMPLEDUR();
                    break;

                case ((9 << 16) | 0xFFFE):  // EXP_GETCHANNELDUR
                    exp = new EXP_GETCHANNELDUR();
                    break;

                case ((10 << 16) | 0xFFFE):		// EXP_GETSAMPLEFREQ		
                    exp = new EXP_GETSAMPLEFREQ();
                    break;

                case ((11 << 16) | 0xFFFE):		// EXP_GETCHANNELFREQ		
                    exp = new EXP_GETCHANNELFREQ();
                    break;

                case ((0 << 16) | 0xFFFD):  // EXP_GAMLEVEL
                    exp = new EXP_GAMLEVEL();
                    break;

                case ((1 << 16) | 0xFFFD):  // EXP_GAMNPLAYER
                    exp = new EXP_GAMNPLAYER();
                    break;

                case ((2 << 16) | 0xFFFD):  // EXP_PLAYXLEFT
                    exp = new EXP_PLAYXLEFT();
                    break;

                case ((3 << 16) | 0xFFFD):  // EXP_PLAYXRIGHT
                    exp = new EXP_PLAYXRIGHT();
                    break;

                case ((4 << 16) | 0xFFFD):  // EXP_PLAYYTOP
                    exp = new EXP_PLAYYTOP();
                    break;

                case ((5 << 16) | 0xFFFD):  // EXP_PLAYYBOTTOM
                    exp = new EXP_PLAYYBOTTOM();
                    break;

                case ((6 << 16) | 0xFFFD):  // EXP_PLAYWIDTH
                    exp = new EXP_PLAYWIDTH();
                    break;

                case ((7 << 16) | 0xFFFD):  // EXP_PLAYHEIGHT
                    exp = new EXP_PLAYHEIGHT();
                    break;

                case ((8 << 16) | 0xFFFD):  // EXP_GAMLEVELNEW
                    exp = new EXP_GAMLEVELNEW();
                    break;

                case ((9 << 16) | 0xFFFD):  // EXP_GETCOLLISIONMASK
                    exp = new EXP_GETCOLLISIONMASK();
                    break;

                case ((10 << 16) | 0xFFFD):  // EXP_FRAMERATE
                    exp = new EXP_FRAMERATE();
                    break;

                case ((11 << 16) | 0xFFFD):  // EXP_GETVIRTUALWIDTH
                    exp = new EXP_GETVIRTUALWIDTH();
                    break;

                case ((12 << 16) | 0xFFFD):  // EXP_GETVIRTUALHEIGHT
                    exp = new EXP_GETVIRTUALHEIGHT();
                    break;

                case ((13 << 16) | 0xFFFD):  // EXP_GETFRAMEBKDCOLOR
                    exp = new EXP_GETFRAMEBKDCOLOR();
                    break;

                case ((14 << 16) | 0xFFFD):  // EXP_GRAPHICMODE
                    exp = new EXP_ZERO();
                    break;

                case ((15 << 16) | 0xFFFD):  // EXP_PIXELSHADERVERSION
                    exp = new EXP_ZERO();
                    break;

                case ((16 << 16) | 0xFFFD):  // EXP_FRAMEALPHACOEF
                    exp = new EXP_ZERO();
                    break;

                case ((17 << 16) | 0xFFFD):  // EXP_FRAMERGBCOEF
                    exp = new EXP_FRAMERGBCOEF();
                    break;

                case ((18 << 16) | 0xFFFD):  // EXP_FRAMEEFFECTPARAM
                    exp = new EXP_ZERO();
                    break;

                case ((0 << 16) | 0xFFFC):  // EXP_TIMVALUE
                    exp = new EXP_TIMVALUE();
                    break;

                case ((1 << 16) | 0xFFFC):  // EXP_TIMCENT
                    exp = new EXP_TIMCENT();
                    break;

                case ((2 << 16) | 0xFFFC):  // EXP_TIMSECONDS
                    exp = new EXP_TIMSECONDS();
                    break;

                case ((3 << 16) | 0xFFFC):  // EXP_TIMHOURS
                    exp = new EXP_TIMHOURS();
                    break;

                case ((4 << 16) | 0xFFFC):  // EXP_TIMMINITS
                    exp = new EXP_TIMMINITS();
                    break;

                case ((0 << 16) | 0xFFFA):  // EXP_XMOUSE
                    exp = new EXP_XMOUSE();
                    break;

                case ((1 << 16) | 0xFFFA):  // EXP_YMOUSE
                    exp = new EXP_YMOUSE();
                    break;

                case ((2 << 16) | 0xFFFA):  // EXP_YMOUSE
                    exp = new EXP_MOUSEWHEELDELTA();
                    break;

                case ((0 << 16) | 0xFFF9):  // EXP_PLASCORE
                    exp = new EXP_PLASCORE();
                    break;

                case ((1 << 16) | 0xFFF9):  // EXP_PLALIVES
                    exp = new EXP_PLALIVES();
                    break;

                case ((2 << 16) | 0xFFF9):  // EXP_GETINPUT
                    exp = new EXP_GETINPUT();
                    break;

                case ((3 << 16) | 0xFFF9):  // EXP_GETINPUTKEY
                    exp = new EXP_GETINPUTKEY();
                    break;

                case ((4 << 16) | 0xFFF9):  // EXP_GETPLAYERNAME
                    exp = new EXP_GETPLAYERNAME();
                    break;

                case ((0 << 16) | 0xFFFB):  // EXP_CRENUMBERALL
                    exp = new EXP_CRENUMBERALL();
                    break;

                case (((80 + 0) << 16) | 3):  // EXP_STRNUMBER
                    exp = new EXP_STRNUMBER();
                    break;

                case (((80 + 1) << 16) | 3):  // EXP_STRGETCURRENT
                    exp = new EXP_STRGETCURRENT();
                    break;

                case (((80 + 2) << 16) | 3):  // EXP_STRGETNUMBER
                    exp = new EXP_STRGETNUMBER();
                    break;

                case (((80 + 3) << 16) | 3):  // EXP_STRGETNUMERIC
                    exp = new EXP_STRGETNUMERIC();
                    break;

                case (((80 + 4) << 16) | 3):  // EXP_STRGETNPARA
                    exp = new EXP_STRGETNPARA();
                    break;

                case ((80 + 0) << 16 | 2):  // EXP_GETRGBAT
                    exp = new EXP_GETRGBAT();
                    break;

                case ((80 + 1) << 16 | 2):  // EXP_GETSCALEX
                    exp = new EXP_GETSCALEX();
                    break;

                case ((80 + 2) << 16 | 2):  // EXP_GETSCALEY
                    exp = new EXP_GETSCALEY();
                    break;

                case ((80 + 3) << 16 | 2):  // EXP_GETANGLE
                    exp = new EXP_GETANGLE();
                    break;

                case (((80 + 0) << 16) | 7):  // EXP_CVALUE
                    exp = new EXP_CVALUE();
                    break;

                case (((80 + 1) << 16) | 7):  // EXP_CGETMIN
                    exp = new EXP_CGETMIN();
                    break;

                case (((80 + 2) << 16) | 7):  // EXP_CGETMAX
                    exp = new EXP_CGETMAX();
                    break;

                case (((80 + 3) << 16) | 7):  // EXP_CGETCOLOR1
                    exp = new EXP_CGETCOLOR1();
                    break;

                case (((80 + 4) << 16) | 7):  // EXP_CGETCOLOR2
                    exp = new EXP_CGETCOLOR2();
                    break;

                case (((80 + 0) << 16) | 9):  // EXP_CCAGETFRAMENUMBER
                    exp = new EXP_CCAGETFRAMENUMBER();
                    break;

                case (((80 + 1) << 16) | 9):  // EXP_CCAGETGLOBALVALUE
                    exp = new EXP_CCAGETGLOBALVALUE();
                    break;

                case (((80 + 2) << 16) | 9):  // EXP_CCAGETGLOBALSTRING
                    exp = new EXP_CCAGETGLOBALSTRING();
                    break;

                default:
                    switch (c & unchecked((int)0xFFFF0000))
                    {

                        case (1 << 16):  // EXP_EXTYSPR
                            exp = new EXP_EXTYSPR();
                            break;

                        case (2 << 16):  // EXP_EXTISPR
                            exp = new EXP_EXTISPR();
                            break;

                        case (3 << 16):  // EXP_EXTSPEED
                            exp = new EXP_EXTSPEED();
                            break;

                        case (4 << 16):  // EXP_EXTACC
                            exp = new EXP_EXTACC();
                            break;

                        case (5 << 16):  // EXP_EXTDEC
                            exp = new EXP_EXTDEC();
                            break;

                        case (6 << 16):  // EXP_EXTDIR
                            exp = new EXP_EXTDIR();
                            break;

                        case (7 << 16):  // EXP_EXTXLEFT
                            exp = new EXP_EXTXLEFT();
                            break;

                        case (8 << 16):  // EXP_EXTXRIGHT
                            exp = new EXP_EXTXRIGHT();
                            break;

                        case (9 << 16):  // EXP_EXTYTOP
                            exp = new EXP_EXTYTOP();
                            break;

                        case (10 << 16):  // EXP_EXTYBOTTOM
                            exp = new EXP_EXTYBOTTOM();
                            break;

                        case (11 << 16):  // EXP_EXTXSPR
                            exp = new EXP_EXTXSPR();
                            break;

                        case (12 << 16):  // EXP_EXTIDENTIFIER
                            exp = new EXP_EXTIDENTIFIER();
                            break;

                        case (13 << 16):  // EXP_EXTFLAG
                            exp = new EXP_EXTFLAG();
                            break;

                        case (14 << 16):  // EXP_EXTNANI
                            exp = new EXP_EXTNANI();
                            break;

                        case (15 << 16):  // EXP_EXTNOBJECTS
                            exp = new EXP_EXTNOBJECTS();
                            break;

                        case (16 << 16):  // EXP_EXTVAR
                            exp = new EXP_EXTVAR();
                            break;

                        case (17 << 16):  // EXP_EXTGETSEMITRANSPARENCY
                            exp = new EXP_EXTGETSEMITRANSPARENCY();
                            break;

                        case (18 << 16):  // EXP_EXTNMOVE
                            exp = new EXP_EXTNMOVE();
                            break;

                        case (19 << 16):  // EXP_EXTVARSTRING
                            exp = new EXP_EXTVARSTRING();
                            break;

                        case (20 << 16):  // EXP_EXTGETFONTNAME
                            exp = new EXP_EXTGETFONTNAME();
                            break;

                        case (21 << 16):  // EXP_EXTGETFONTSIZE
                            exp = new EXP_EXTGETFONTSIZE();
                            break;

                        case (22 << 16):  // EXP_EXTGETFONTCOLOR
                            exp = new EXP_EXTGETFONTCOLOR();
                            break;

                        case (23 << 16):  // EXP_EXTGETLAYER
                            exp = new EXP_EXTGETLAYER();
                            break;

                        case (24 << 16):  // EXP_EXTGETGRAVITY
                            exp = new EXP_EXTGETGRAVITY();
                            break;

                        case (25 << 16):  // EXP_EXTXAP
                            exp = new EXP_EXTXAP();
                            break;

                        case (26 << 16):  // EXP_EXTYAP
                            exp = new EXP_EXTYAP();
                            break;

                        case (27 << 16):  // EXP_EXTALPHACOEF
                            exp = new EXP_EXTALPHACOEF();
                            break;

                        case (28 << 16):  // EXP_EXTRGBCOEF
                            exp = new EXP_EXTRGBCOEF();
                            break;

                        case (29 << 16):  // EXP_EXTEFFECTPARAM
                            exp = new EXP_ZERO();
                            break;

                        case (30 << 16):  // EXP_VARBYINDEX
                            exp = new EXP_EXTVARBYINDEX();
                            break;

                        case (31 << 16):  // EXP_VARBYINDEX
                            exp = new EXP_EXTVARSTRINGBYINDEX();
                            break;

                        default:
                            exp = new CExpExtension(); // EXTENSION
                            break;

                    }
                    break;

            }
            if (exp != null)
            {
                exp.code = c;

                if (c != 0x00000000)
                {
                    short size = file.readAShort();

                    short type;
                    switch (c)
                    {

                        case ((3 << 16) | 0xFFFF):  // EXP_STRING
                            ((EXP_STRING)exp).pString = file.readAString();
                            break;

                        case ((0 << 16) | 0xFFFF):  // EXP_LONG
                            ((EXP_LONG)exp).value = file.readAInt();
                            break;

                        case ((23 << 16) | 0xFFFF):  // EXP_DOUBLE
                            ((EXP_DOUBLE)exp).value = file.readADouble();
                            break;

                        case ((24 << 16) | 0xFFFF):  // EXP_VARGLONAMED
                            file.skipBytes(4); // Oi et OIlist qui servent a rien
                            ((EXP_VARGLONAMED)exp).number = file.readAShort();
                            break;

                        case ((50 << 16) | 0xFFFF):  // EXP_STRINGGLONAMED
                            file.skipBytes(4); // Oi et OIlist qui servent a rien
                            ((EXP_STRINGGLONAMED)exp).number = file.readAShort();
                            break;

                        default:
                            type = (short)c;
                            if (type >= 2 || type == COI.OBJ_PLAYER)
                            {
                                CExpOi expOi = (CExpOi)exp;
                                expOi.oi = file.readAShort();
                                expOi.oiList = file.readAShort();
                                switch (c & unchecked((int)0xFFFF0000))
                                {

                                    case (16 << 16):  // EXP_EXTVAR
                                        ((EXP_EXTVAR)exp).number = file.readAShort();
                                        break;

                                    case (19 << 16):  // EXP_EXTVARSTRING
                                        ((EXP_EXTVARSTRING)exp).number = file.readAShort();
                                        break;

                                    default:
                                        break;

                                }
                            }
                            break;

                    }
                    file.seek((int)debut + size);
                }
            }
            else
            {
                System.Console.Out.WriteLine("*** Missing expression!");
            }
            return exp;
        }

        public abstract void evaluate(CRun rhPtr);
    }
}
