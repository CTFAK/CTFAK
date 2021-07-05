//----------------------------------------------------------------------------------
//
// CPARAM: un parametre
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Params
{

    public abstract class CParam
    {
        public short code;

        public const short PARAM_EXPRESSION = 22;

        public CParam()
        {
        }
        public static CParam create(CRunApp app)
        {
            long debut = app.file.getFilePointer();

            CParam param = null;
            short size = app.file.readAShort();
            short c = app.file.readAShort();
            switch (c)
            {
                case 1:  // PARAM_OBJECT			
                    param = new PARAM_OBJECT();
                    break;

                case 2:  // PARAM_TIME				
                    param = new PARAM_TIME();
                    break;

                case 3:  // PARAM_BORDER			
                    param = new PARAM_SHORT();
                    break;

                case 4:  // PARAM_DIRECTION			
                    param = new PARAM_SHORT();
                    break;

                case 5:  // PARAM_INTEGER			
                    param = new PARAM_INT();
                    break;

                case 6:  // PARAM_SAMPLE			
                    param = new PARAM_SAMPLE();
                    break;

                case 7:  // PARAM_MUSIC				
                    param = new PARAM_SAMPLE();
                    break;

                case 9:  // PARAM_CREATE			
                    param = new PARAM_CREATE();
                    break;

                case 10:  // PARAM_ANIMATION			
                    param = new PARAM_SHORT();
                    break;

                case 11:  // PARAM_NOP				
                    param = new PARAM_SHORT();
                    break;

                case 12:  // PARAM_PLAYER			
                    param = new PARAM_SHORT();
                    break;

                case 13:  // PARAM_EVERY				
                    param = new PARAM_EVERY();
                    break;

                case 14:  // PARAM_KEY				
                    param = new PARAM_KEY();
                    break;

                case 15:  // PARAM_SPEED				
                    param = new PARAM_EXPRESSION();
                    break;

                case 16:  // PARAM_POSITION	  		
                    param = new PARAM_POSITION();
                    break;

                case 17:  // PARAM_JOYDIRECTION 		
                    param = new PARAM_SHORT();
                    break;

                case 18:  // PARAM_SHOOT				
                    param = new PARAM_SHOOT();
                    break;

                case 19:  // PARAM_ZONE				
                    param = new PARAM_ZONE();
                    break;

                case 21:  // PARAM_SYSCREATE		   	
                    param = new PARAM_CREATE();
                    break;

                case 22:  // PARAM_EXPRESSION	   	
                    param = new PARAM_EXPRESSION();
                    break;

                case 23:  // PARAM_COMPARAISON	   	
                    param = new PARAM_EXPRESSION();
                    break;

                case 24:  // PARAM_COLOUR		   	
                    param = new PARAM_COLOUR();
                    break;

                case 25:  // PARAM_BUFFER4		   	
                    param = new PARAM_INT();
                    break;

                case 26:  // PARAM_FRAME			   	
                    param = new PARAM_SHORT();
                    break;

                case 27:  // PARAM_SAMLOOP		   	
                    param = new PARAM_EXPRESSION();
                    break;

                case 28:  // PARAM_MUSLOOP		   	
                    param = new PARAM_EXPRESSION();
                    break;

                case 29:  // PARAM_NEWDIRECTION	   	
                    param = new PARAM_INT();
                    break;

                case 31:  // PARAM_TEXTNUMBER	 	
                    param = new PARAM_SHORT();
                    break;

                case 32:  // PARAM_CLICK			 	
                    param = new PARAM_SHORT();
                    break;

                case 33:  // PARAM_PROGRAM			
                    param = new PARAM_PROGRAM();
                    break;

                case 34:  // OLDPARAM_VARGLO			
                    param = new PARAM_INT();
                    break;

                case 35:  // PARAM_CNDSAMPLE			
                    param = new PARAM_SAMPLE();
                    break;

                case 36:  // PARAM_CNDMUSIC			
                    param = new PARAM_SAMPLE();
                    break;

                case 37:  // PARAM_REMARK			
                    param = new PARAM_SHORT();
                    break;

                case 38:  // PARAM_GROUP				
                    param = new PARAM_GROUP();
                    break;

                case 39:  // PARAM_GROUPOINTER		
                    param = new PARAM_GROUPOINTER();
                    break;

                case 40:  // PARAM_FILENAME			
                    param = new PARAM_STRING();
                    break;

                case 41:  // PARAM_STRING			
                    param = new PARAM_STRING();
                    break;

                case 42:  // PARAM_CMPTIME			
                    param = new PARAM_CMPTIME();
                    break;

                case 43:  // PARAM_PASTE				
                    param = new PARAM_SHORT();
                    break;

                case 44:  // PARAM_VMKEY				
                    param = new PARAM_KEY();
                    break;

                case 45:  // PARAM_EXPSTRING		   	
                    param = new PARAM_EXPRESSION();
                    break;

                case 46:  // PARAM_CMPSTRING		   	
                    param = new PARAM_EXPRESSION();
                    break;

                case 47:  // PARAM_INKEFFECT		   	
                    param = new PARAM_2SHORTS();
                    break;

                case 48:  // PARAM_MENU		   		
                    param = new PARAM_INT();
                    break;

                case 49:  // PARAM_VARGLOBAL		   	
                    param = new PARAM_SHORT();
                    break;

                case 50:  // PARAM_ALTVALUE		   	
                    param = new PARAM_SHORT();
                    break;

                case 51:  // PARAM_FLAG			   	
                    param = new PARAM_2SHORTS();
                    break;

                case 52:  // PARAM_VARGLOBAL_EXP		
                    param = new PARAM_EXPRESSION();
                    break;

                case 53:  // PARAM_ALTVALUE_EXP		
                    param = new PARAM_EXPRESSION();
                    break;

                case 54:  // PARAM_FLAG_EXP		   	
                    param = new PARAM_EXPRESSION();
                    break;

                case 55:  // PARAM_EXTENSION			
                    param = new PARAM_EXTENSION();
                    break;

                case 56:  // PARAM_8DIRECTIONS	   	
                    param = new PARAM_INT();
                    break;

                case 57:  // PARAM_MVT			
                    param = new PARAM_SHORT();
                    break;

                case 58:  // PARAM_STRINGGLOBAL		
                    param = new PARAM_SHORT();
                    break;

                case 59:  // PARAM_STRINGGLOBAL_EXP	
                    param = new PARAM_EXPRESSION();
                    break;

                case 60:  // PARAM_PROGRAM2			
                    param = new PARAM_SHORT();
                    break;

                case 61:  // PARAM_ALTSTRING		   	
                    param = new PARAM_SHORT();
                    break;

                case 62:  // PARAM_ALTSTRING_EXP		
                    param = new PARAM_EXPRESSION();
                    break;

                case 63:  // PARAM_FILENAME2			
                    param = new PARAM_STRING();
                    break;

                case 64:  // PARAM_EFFECT
                    param = new PARAM_EFFECT();
                    break;

                case 67:  // PARAM_RUNTIME
                    param = new PARAM_SHORT();
                    break;
            }

            param.code = c;
            param.load(app);
            app.file.seek((int)(debut + size));
            return param;
        }
        public abstract void load(CRunApp app);
    }
}