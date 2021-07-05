//----------------------------------------------------------------------------------
//
// CACT: une action
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Params;
using RuntimeXNA.Application;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Events;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions
{
	
	public abstract class CAct:CEvent
	{
		public const byte ACTFLAGS_REPEAT = (byte) (0x0001);
		
		public CAct()
		{
		}
		
		public static CAct create(CRunApp app)
		{
			int debut = app.file.getFilePointer();
			
			short size = app.file.readAShort(); // evtSize
			CAct act = null;
			int c = app.file.readAInt();

            switch (c)
			{
				
				case ((0 << 16) | 0xFFFF):  // 	ACT_SKIP
					act = new ACT_SKIP();
					break;
				
				case ((3 << 16) | 0xFFFF):  // 	ACT_SETVARG
					act = new ACT_SETVARG();
					break;
				
				case ((4 << 16) | 0xFFFF):  // 	ACT_SUBVARG
					act = new ACT_SUBVARG();
					break;
				
				case ((5 << 16) | 0xFFFF):  // 	ACT_ADDVARG
					act = new ACT_ADDVARG();
					break;
				
				case ((6 << 16) | 0xFFFF):  // 	ACT_GRPACTIVATE
					act = new ACT_GRPACTIVATE();
					break;
				
				case ((7 << 16) | 0xFFFF):  // 	ACT_GRPDEACTIVATE
					act = new ACT_GRPDEACTIVATE();
					break;
				
				case ((14 << 16) | 0xFFFF):  // 	ACT_STARTLOOP
					act = new ACT_STARTLOOP();
					break;
				
				case ((15 << 16) | 0xFFFF):  // 	ACT_STOPLOOP
					act = new ACT_STOPLOOP();
					break;
				
				case ((16 << 16) | 0xFFFF):  // 	ACT_SETLOOPINDEX
					act = new ACT_SETLOOPINDEX();
					break;
				
				case ((17 << 16) | 0xFFFF):  // 	ACT_RANDOMIZE
					act = new ACT_RANDOMIZE();
					break;
				
				case ((19 << 16) | 0xFFFF):  //  ACT_SETGLOBALSTRING
					act = new ACT_SETGLOBALSTRING();
					break;
				
				case ((23 << 16) | 0xFFFF):  //  ACT_OPENDEBUGGER
					act = new ACT_OPENDEBUGGER();
					break;
				
				case ((24 << 16) | 0xFFFF):  //  ACT_PAUSEDEBUGGER
					act = new ACT_PAUSEDEBUGGER();
					break;
				
				case ((0 << 16) | 0xFFFE):  // 	ACT_PLAYSAMPLE
					act = new ACT_PLAYSAMPLE();
					break;
				
				case ((1 << 16) | 0xFFFE):  // 	ACT_STOPSAMPLE
					act = new ACT_STOPSAMPLE();
					break;
				
				case ((4 << 16) | 0xFFFE):  // 	ACT_PLAYLOOPSAMPLE
					act = new ACT_PLAYLOOPSAMPLE();
					break;
				
				case ((6 << 16) | 0xFFFE):  // 	ACT_STOPSPESAMPLE
					act = new ACT_STOPSPESAMPLE();
					break;
				
				case ((7 << 16) | 0xFFFE):  // 	ACT_PAUSESAMPLE
					act = new ACT_PAUSESAMPLE();
					break;
				
				case ((8 << 16) | 0xFFFE):  // 	ACT_RESUMESAMPLE
					act = new ACT_RESUMESAMPLE();
					break;
				
				case ((11 << 16) | 0xFFFE):  // 	ACT_PLAYCHANNEL
					act = new ACT_PLAYCHANNEL();
					break;
				
				case ((12 << 16) | 0xFFFE):  // 	ACT_PLAYLOOPCHANNEL
					act = new ACT_PLAYLOOPCHANNEL();
					break;
				
				case ((13 << 16) | 0xFFFE):  // 	ACT_PAUSECHANNEL
					act = new ACT_PAUSECHANNEL();
					break;
				
				case ((14 << 16) | 0xFFFE):  // 	ACT_RESUMECHANNEL
					act = new ACT_RESUMECHANNEL();
					break;
				
				case ((15 << 16) | 0xFFFE):  // 	ACT_STOPCHANNEL
					act = new ACT_STOPCHANNEL();
					break;
				
				case ((17 << 16) | 0xFFFE):  // 	ACT_SETCHANNELVOL
					act = new ACT_SETCHANNELVOL();
					break;
				
				case ((18 << 16) | 0xFFFE):  // 	ACT_SETCHANNELPAN
					act = new ACT_SETCHANNELPAN();
					break;
				
				case ((20 << 16) | 0xFFFE):  // 	ACT_SETSAMPLEMAINVOL
					act = new ACT_SETSAMPLEMAINVOL();
					break;
				
				case ((21 << 16) | 0xFFFE):  // 	ACT_SETSAMPLEVOL
					act = new ACT_SETSAMPLEVOL();
					break;
				
				case ((22 << 16) | 0xFFFE):  // 	ACT_SETSAMPLEMALNPAN
					act = new ACT_SETSAMPLEMALNPAN();
					break;
				
				case ((23 << 16) | 0xFFFE):  // 	ACT_SETSAMPLEPAN
					act = new ACT_SETSAMPLEPAN();
					break;
				
				case ((24 << 16) | 0xFFFE):  //  ACT_PAUSEALLCHANNELS
					act = new ACT_PAUSEALLCHANNELS();
					break;
				
				case ((25 << 16) | 0xFFFE):  //  ACT_RESUMEALLCHANNELS
					act = new ACT_RESUMEALLCHANNELS();
					break;
				
				case ((30 << 16) | 0xFFFE):  //  ACT_LOCKCHANNEL
					act = new ACT_LOCKCHANNEL();
					break;
				
				case ((31 << 16) | 0xFFFE):  //  ACT_UNLOCKCHANNEL
					act = new ACT_UNLOCKCHANNEL();
					break;

                case ((32 << 16) | 0xFFFE):		//  ACT_SETCHANNELFREQ		
                    act = new ACT_SETCHANNELFREQ();
                    break;

                case ((33 << 16) | 0xFFFE):		//  ACT_SETSAMPLEFREQ		
                    act = new ACT_SETSAMPLEFREQ();
                    break;

				case ((0 << 16) | 0xFFFD):  // 	ACT_NEXTLEVEL
					act = new ACT_NEXTLEVEL();
					break;
				
				case ((1 << 16) | 0xFFFD):  // 	ACT_PREVLEVEL
					act = new ACT_PREVLEVEL();
					break;
				
				case ((2 << 16) | 0xFFFD):  // 	ACT_GOLEVEL
					act = new ACT_GOLEVEL();
					break;
				
				case ((3 << 16) | 0xFFFD):  // 	ACT_PAUSE
					act = new ACT_PAUSE();
					break;
				
				case ((4 << 16) | 0xFFFD):  // 	ACT_ENDGAME
					act = new ACT_ENDGAME();
					break;
				
				case ((5 << 16) | 0xFFFD):  // 	ACT_RESTARTGAME
					act = new ACT_RESTARTGAME();
					break;
				
				case ((6 << 16) | 0xFFFD):  // 	ACT_RESTARTLEVEL
					act = new ACT_RESTARTLEVEL();
					break;
				
				case ((7 << 16) | 0xFFFD):  // 	ACT_CDISPLAY
					act = new ACT_CDISPLAY();
					break;
				
				case ((8 << 16) | 0xFFFD):  // 	ACT_CDISPLAYX
					act = new ACT_CDISPLAYX();
					break;
				
				case ((9 << 16) | 0xFFFD):  // 	ACT_CDISPLAYY
					act = new ACT_CDISPLAYY();
					break;
				
				case ((14 << 16) | 0xFFFD):  //  ACT_FULLSCREENMODE
					act = new ACT_FULLSCREENMODE();
					break;
				
				case ((15 << 16) | 0xFFFD):  //  ACT_WINDOWEDMODE
					act = new ACT_WINDOWEDMODE();
					break;
				
				case ((16 << 16) | 0xFFFD):  //  ACT_SETFRAMERATE
					act = new ACT_SETFRAMERATE();
					break;
				
				case ((17 << 16) | 0xFFFD):  //  ACT_PAUSEKEY
					act = new ACT_PAUSEKEY();
					break;
				
				case ((18 << 16) | 0xFFFD):  //  ACT_PAUSEANYKEY
					act = new ACT_PAUSEANYKEY();
					break;
				
				case ((21 << 16) | 0xFFFD):  // 	ACT_SETVIRTUALWIDTH
					act = new ACT_SETVIRTUALWIDTH();
					break;
				
				case ((22 << 16) | 0xFFFD):  // 	ACT_SETVIRTUALHEIGHT
					act = new ACT_SETVIRTUALHEIGHT();
					break;
				
				case ((23 << 16) | 0xFFFD):  //  ACT_SETFRAMEBDKCOLOR
					act = new ACT_SETFRAMEBDKCOLOR();
					break;
				
				case ((24 << 16) | 0xFFFD):  //  ACT_DELCREATEDBKDAT
					act = new ACT_DELCREATEDBKDAT();
					break;
				
				case ((25 << 16) | 0xFFFD):  //  ACT_DELALLCREATEDBKD
					act = new ACT_DELALLCREATEDBKD();
					break;
				
				case ((26 << 16) | 0xFFFD):  //  ACT_SETFRAMEWIDTH
					act = new ACT_SETFRAMEWIDTH();
					break;
				
				case ((27 << 16) | 0xFFFD):  //  ACT_SETFRAMEHEIGHT
					act = new ACT_SETFRAMEHEIGHT();
					break;
				
				case ((32 << 16) | 0xFFFD):  //  ACT_SETFRAMEEFFECT
					act = new ACT_SKIP();
					break;
				
				case ((33 << 16) | 0xFFFD):  //  ACT_SETFRAMEEFFECTPARAM
					act = new ACT_SKIP();
					break;
				
				case ((34 << 16) | 0xFFFD):  //  ACT_SETFRAMEEFFECTPARAMTEXTURE
					act = new ACT_SKIP();
					break;
				
				case ((35 << 16) | 0xFFFD):  //  ACT_SETFRAMEALPHACOEF
					act = new ACT_SKIP();
					break;
				
				case ((36 << 16) | 0xFFFD):  //  ACT_SETFRAMERGBCOEF
					act = new ACT_SKIP();
					break;
				
				case ((0 << 16) | 0xFFFC):  // 	ACT_SETTIMER
					act = new ACT_SETTIMER();
					break;
				
				case ((0 << 16) | 0xFFFA):  // 	ACT_HIDECURSOR
					act = new ACT_HIDECURSOR();
					break;
				
				case ((1 << 16) | 0xFFFA):  // 	ACT_SHOWCURSOR
					act = new ACT_SHOWCURSOR();
					break;
				
				case ((0 << 16) | 0xFFF9):  // 	ACT_SETSCORE
					act = new ACT_SETSCORE();
					break;
				
				case ((1 << 16) | 0xFFF9):  // 	ACT_SETLIVES
					act = new ACT_SETLIVES();
					break;
				
				case ((2 << 16) | 0xFFF9):  // 	ACT_NOINPUT
					act = new ACT_NOINPUT();
					break;
				
				case ((3 << 16) | 0xFFF9):  // 	ACT_RESTINPUT
					act = new ACT_RESTINPUT();
					break;
				
				case ((4 << 16) | 0xFFF9):  // 	ACT_ADDSCORE
					act = new ACT_ADDSCORE();
					break;
				
				case ((5 << 16) | 0xFFF9):  // 	ACT_ADDLIVES
					act = new ACT_ADDLIVES();
					break;
				
				case ((6 << 16) | 0xFFF9):  // 	ACT_SUBSCORE
					act = new ACT_SUBSCORE();
					break;
				
				case ((7 << 16) | 0xFFF9):  // 	ACT_SUBLIVES
					act = new ACT_SUBLIVES();
					break;
				
				case ((8 << 16) | 0xFFF9):  // 	ACT_SETINPUT
					act = new ACT_SETINPUT();
					break;
				
				case ((9 << 16) | 0xFFF9):  // 	ACT_SETINPUTKEY
					act = new ACT_SETINPUTKEY();
					break;
				
				case ((10 << 16) | 0xFFF9):  // 	ACT_SETPLAYERNAME
					act = new ACT_SETPLAYERNAME();
					break;
				
				case ((0 << 16) | 0xFFFB):  // 	ACT_CREATE
					act = new ACT_CREATE();
					break;
				
				case (((80 + 0) << 16) | 3):  // 	ACT_STRDESTROY
					act = new ACT_STRDESTROY();
					break;
				
				case (((80 + 1) << 16) | 3):  // 	ACT_STRDISPLAY
					act = new ACT_STRDISPLAY();
					break;
				
				case (((80 + 2) << 16) | 3):  // 	ACT_STRDISPLAYDURING
					act = new ACT_STRDISPLAYDURING();
					break;
				
				case (((80 + 3) << 16) | 3):  // 	ACT_STRSETCOLOUR
					act = new ACT_STRSETCOLOUR();
					break;
				
				case (((80 + 4) << 16) | 3):  // 	ACT_STRSET
					act = new ACT_STRSET();
					break;
				
				case (((80 + 5) << 16) | 3):  // 	ACT_STRPREV
					act = new ACT_STRPREV();
					break;
				
				case (((80 + 6) << 16) | 3):  // 	ACT_STRNEXT
					act = new ACT_STRNEXT();
					break;
				
				case (((80 + 7) << 16) | 3):  //  ACT_STRDISPLAYSTRING
					act = new ACT_STRDISPLAYSTRING();
					break;
				
				case (((80 + 8) << 16) | 3):  // 	ACT_STRSETSTRING
					act = new ACT_STRSETSTRING();
					break;
				
				case (((80 + 0) << 16) | 2):  // ACT_SPRPASTE
					act = new ACT_SPRPASTE();
					break;
				
				case (((80 + 1) << 16) | 2):  // ACT_SPRFRONT
					act = new ACT_SPRFRONT();
					break;
				
				case (((80 + 2) << 16) | 2):  // ACT_SPRBACK
					act = new ACT_SPRBACK();
					break;
				
				case (((80 + 3) << 16) | 2):  // ACT_SPRADDBKD
					act = new ACT_SPRADDBKD();
					break;
				
				case (((80 + 4) << 16) | 2):  // ACT_SPRREPLACECOLOR
					act = new ACT_SPRREPLACECOLOR();
					break;
				
				case (((80 + 5) << 16) | 2):  // ACT_SPRSETSCALE
					act = new ACT_SPRSETSCALE();
					break;
				
				case (((80 + 6) << 16) | 2):  // ACT_SPRSETSCALEX
					act = new ACT_SPRSETSCALEX();
					break;
				
				case (((80 + 7) << 16) | 2):  // ACT_SPRSETSCALEY
					act = new ACT_SPRSETSCALEY();
					break;
				
				case (((80 + 8) << 16) | 2):  // ACT_SPRSETANGLE
					act = new ACT_SPRSETANGLE();
					break;
				
				case (((80 + 0) << 16) | 7):  // 	ACT_CSETVALUE
					act = new ACT_CSETVALUE();
					break;
				
				case (((80 + 1) << 16) | 7):  // 	ACT_CADDVALUE
					act = new ACT_CADDVALUE();
					break;
				
				case (((80 + 2) << 16) | 7):  // 	ACT_CSUBVALUE
					act = new ACT_CSUBVALUE();
					break;
				
				case (((80 + 3) << 16) | 7):  // 	ACT_CSETMIN
					act = new ACT_CSETMIN();
					break;
				
				case (((80 + 4) << 16) | 7):  // 	ACT_CSETMAX
					act = new ACT_CSETMAX();
					break;
				
				case (((80 + 5) << 16) | 7):  // 	ACT_CSETCOLOR1
					act = new ACT_CSETCOLOR1();
					break;
				
				case (((80 + 6) << 16) | 7):  // 	ACT_CSETCOLOR2
					act = new ACT_CSETCOLOR2();
					break;
				
				case (((80 + 0) << 16) | 4):  // 	ACT_QASK
					act = new ACT_QASK();
					break;
				case (((80 + 0) << 16) | 9):  // ACT_CCARESTARTAPP
					act = new ACT_CCARESTARTAPP();
					break;
				
				case (((80 + 1) << 16) | 9):  // ACT_CCARESTARTFRAME
					act = new ACT_CCARESTARTFRAME();
					break;
				
				case (((80 + 2) << 16) | 9):  // ACT_CCANEXTFRAME
					act = new ACT_CCANEXTFRAME();
					break;
				
				case (((80 + 3) << 16) | 9):  // ACT_CCAPREVIOUSFRAME
					act = new ACT_CCAPREVIOUSFRAME();
					break;
				
				case (((80 + 4) << 16) | 9):  // ACT_CCAENDAPP
					act = new ACT_CCAENDAPP();
					break;
				
				case (((80 + 6) << 16) | 9):  // ACT_CCAJUMPFRAME
					act = new ACT_CCAJUMPFRAME();
					break;
				
				case (((80 + 7) << 16) | 9):  // ACT_CCASETGLOBALVALUE
					act = new ACT_CCASETGLOBALVALUE();
					break;
				
				case (((80 + 8) << 16) | 9):  // ACT_CCASHOW
					act = new ACT_CCASHOW();
					break;
				
				case (((80 + 9) << 16) | 9):  // ACT_CCAHIDE
					act = new ACT_CCAHIDE();
					break;
				
				case (((80 + 10) << 16) | 9):  // ACT_CCASETGLOBALSTRING
					act = new ACT_CCASETGLOBALSTRING();
					break;
				
				case (((80 + 11) << 16) | 9):  // ACT_CCAPAUSEAPP
					act = new ACT_CCAPAUSEAPP();
					break;
				
				case (((80 + 12) << 16) | 9):  // ACT_CCARESUMEAPP
					act = new ACT_CCARESUMEAPP();
					break;
					// Actions pour les objets extensions 
				
				default: 
					{
						switch (c & unchecked((int) 0xFFFF0000))
						{
							
							case (1 << 16):  // 	ACT_EXTSETPOS
								act = new ACT_EXTSETPOS();
								break;
							
							case (2 << 16):  // 	ACT_EXTSETX
								act = new ACT_EXTSETX();
								break;
							
							case (3 << 16):  // 	ACT_EXTSETY
								act = new ACT_EXTSETY();
								break;
							
							case (4 << 16):  // 	ACT_EXTSTOP
								act = new ACT_EXTSTOP();
								break;
							
							case (5 << 16):  // 	ACT_EXTSTART
								act = new ACT_EXTSTART();
								break;
							
							case (6 << 16):  // 	ACT_EXTSPEED
								act = new ACT_EXTSPEED();
								break;
							
							case (7 << 16):  // 	ACT_EXTMAXSPEED
								act = new ACT_EXTMAXSPEED();
								break;
							
							case (8 << 16):  // 	ACT_EXTWRAP
								act = new ACT_EXTWRAP();
								break;
							
							case (9 << 16):  // 	ACT_EXTBOUNCE
								act = new ACT_EXTBOUNCE();
								break;
							
							case (10 << 16):  // 	ACT_EXTREVERSE
								act = new ACT_EXTREVERSE();
								break;
							
							case (11 << 16):  // 	ACT_EXTNEXTMOVE
								act = new ACT_EXTNEXTMOVE();
								break;
							
							case (12 << 16):  // 	ACT_EXTPREVMOVE
								act = new ACT_EXTPREVMOVE();
								break;
							
							case (13 << 16):  // 	ACT_EXTSELMOVE
								act = new ACT_EXTSELMOVE();
								break;
							
							case (14 << 16):  // 	ACT_EXTLOOKAT
								act = new ACT_EXTLOOKAT();
								break;
							
							case (15 << 16):  // 	ACT_EXTSTOPANIM
								act = new ACT_EXTSTOPANIM();
								break;
							
							case (16 << 16):  // 	ACT_EXTSTARTANIM
								act = new ACT_EXTSTARTANIM();
								break;
							
							case (17 << 16):  // 	ACT_EXTFORCEANIM
								act = new ACT_EXTFORCEANIM();
								break;
							
							case (18 << 16):  // 	ACT_EXTFORCEDIR
								act = new ACT_EXTFORCEDIR();
								break;
							
							case (19 << 16):  // 	ACT_EXTFORCESPEED
								act = new ACT_EXTFORCESPEED();
								break;
							
							case (20 << 16):  // 	ACT_EXTRESTANIM
								act = new ACT_EXTRESTANIM();
								break;
							
							case (21 << 16):  // 	ACT_EXTRESTDIR
								act = new ACT_EXTRESTDIR();
								break;
							
							case (22 << 16):  // 	ACT_EXTRESTSPEED
								act = new ACT_EXTRESTSPEED();
								break;
							
							case (23 << 16):  // 	ACT_EXTSETDIR
								act = new ACT_EXTSETDIR();
								break;
							
							case (24 << 16):  // 	ACT_EXTDESTROY
								act = new ACT_EXTDESTROY();
								break;
							
							case (25 << 16):  // 	ACT_EXTSHUFFLE
								act = new ACT_EXTSHUFFLE();
								break;
							
							case (26 << 16):  // 	ACT_EXTHIDE
								act = new ACT_EXTHIDE();
								break;
							
							case (27 << 16):  // 	ACT_EXTSHOW
								act = new ACT_EXTSHOW();
								break;
							
							case (28 << 16):  // 	ACT_EXTDISPLAYDURING
								act = new ACT_EXTDISPLAYDURING();
								break;
							
							case (29 << 16):  // 	ACT_EXTSHOOT
								act = new ACT_EXTSHOOT();
								break;
							
							case (30 << 16):  // 	ACT_EXTSHOOTTOWARD
								act = new ACT_EXTSHOOTTOWARD();
								break;
							
							case (31 << 16):  // 	ACT_EXTSETVAR
								act = new ACT_EXTSETVAR();
								break;
							
							case (32 << 16):  // 	ACT_EXTADDVAR
								act = new ACT_EXTADDVAR();
								break;
							
							case (33 << 16):  // 	ACT_EXTSUBVAR
								act = new ACT_EXTSUBVAR();
								break;
							
							case (34 << 16):  // 	ACT_EXTDISPATCHVAR
								act = new ACT_EXTDISPATCHVAR();
								break;
							
							case (35 << 16):  // 	ACT_EXTSETFLAG
								act = new ACT_EXTSETFLAG();
								break;
							
							case (36 << 16):  // 	ACT_EXTCLRFLAG
								act = new ACT_EXTCLRFLAG();
								break;
							
							case (37 << 16):  // 	ACT_EXTCHGFLAG
								act = new ACT_EXTCHGFLAG();
								break;
							
							case (38 << 16):  // 	ACT_EXTINKEFFECT
								act = new ACT_EXTINKEFFECT();
								break;
							
							case (39 << 16):  //  ACT_EXTSETSEMITRANSPARENCY
								act = new ACT_EXTSETSEMITRANSPARENCY();
								break;
							
							case (40 << 16):  //  ACT_EXTFORCEFRAME
								act = new ACT_EXTFORCEFRAME();
								break;
							
							case (41 << 16):  //  ACT_EXTRESTFRAME
								act = new ACT_EXTRESTFRAME();
								break;
							
							case (42 << 16):  //  ACT_EXTSETACCELERATION
								act = new ACT_EXTSETACCELERATION();
								break;
							
							case (43 << 16):  //  ACT_EXTSETDECELERATION
								act = new ACT_EXTSETDECELERATION();
								break;
							
							case (44 << 16):  //  ACT_EXTSETROTATINGSPEED
								act = new ACT_EXTSETROTATINGSPEED();
								break;
							
							case (45 << 16):  //  ACT_EXTSETDIRECTIONS
								act = new ACT_EXTSETDIRECTIONS();
								break;
							
							case (46 << 16):  //  ACT_EXTBRANCHNODE
								act = new ACT_EXTBRANCHNODE();
								break;
							
							case (47 << 16):  //  ACT_EXTSETGRAVITY
								act = new ACT_EXTSETGRAVITY();
								break;
							
							case (48 << 16):  //  ACT_EXTGOTONODE
								act = new ACT_EXTGOTONODE();
								break;
							
							case (49 << 16):  // 	ACT_EXTSETVARSTRING
								act = new ACT_EXTSETVARSTRING();
								break;
							
							case (50 << 16):  //  ACT_EXTSETFONTNAME
								act = new ACT_EXTSETFONTNAME();
								break;
							
							case (51 << 16):  //  ACT_EXTSETFONTSIZE
								act = new ACT_EXTSETFONTSIZE();
								break;
							
							case (52 << 16):  //  ACT_EXTSETBOLD
								act = new ACT_EXTSETBOLD();
								break;
							
							case (56 << 16):  // 	ACT_EXTSETTEXTCOLOR
								act = new ACT_EXTSETTEXTCOLOR();
								break;
							
							case (57 << 16):  //  ACT_EXTSPRFRONT
								act = new ACT_EXTSPRFRONT();
								break;
							
							case (58 << 16):  //  ACT_EXTSPRBACK
								act = new ACT_EXTSPRBACK();
								break;
							
							case (59 << 16):  // 	ACT_EXTMOVEBEFORE
								act = new ACT_EXTMOVEBEFORE();
								break;
							
							case (60 << 16):  // 	ACT_EXTMOVEAFTER
								act = new ACT_EXTMOVEAFTER();
								break;
							
							case (61 << 16):  //  ACT_EXTMOVETOLAYER
								act = new ACT_EXTMOVETOLAYER();
								break;
							
							case (62 << 16):  //  ACT_EXTADDTODEBUGGER
								act = new ACT_EXTADDTODEBUGGER();
								break;

                            case (63 << 16):  //  ACT_EXTSETEFFECT
                                act = new ACT_EXTSETEFFECT();
                                break;

                            case (64 << 16):  //  ACT_EXTSETEFFECTPARAM
                                act = new ACT_EXTSETEFFECTPARAM();
                                break;

                            case (65 << 16):  // ACT_EXTSETALPHACOEF
                                act=new ACT_EXTSETALPHACOEF();
                                break;

                            case (66 << 16):  // ACT_EXTSETALPHACOEF
                                act = new ACT_EXTSETRGBCOEF();
                                break;

                            case (67 << 16):  // ACT_EXTSETEFFECTPARAMTEXTURE
                                act = new ACT_EXTSETEFFECTPARAMTEXTURE();
                                break;

                            default: 
								act = new CActExtension(); // EXTENSIONS
								break;
							
						}
					}
					break;
				
			}
			
			if (act != null)
			{
				act.evtCode = c;
				act.evtOi = app.file.readAShort();
				act.evtOiList = app.file.readAShort();
				act.evtFlags = (byte) app.file.readByte();
				act.evtFlags2 = (byte) app.file.readByte();
				act.evtNParams = (byte) app.file.readByte();
				act.evtDefType = (byte) app.file.readByte();
				
				// Lis les parametres
				if (act.evtNParams > 0)
				{
					act.evtParams = new CParam[act.evtNParams];
					int n;
					for (n = 0; n < act.evtNParams; n++)
					{
						act.evtParams[n] = CParam.create(app);
					}
				}
			}
			else
			{
				System.Console.Out.WriteLine("*** Missing action!");
			}
			
			// Positionne a la fin de la condition
			app.file.seek((int)(debut + size));
			
			return act;
		}
		
		public abstract void  execute(CRun rhPtr);
	}
}