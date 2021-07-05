//----------------------------------------------------------------------------------
//
// CKEYCONVERT : conversion des keycodes en keycodes java
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace RuntimeXNA.Application
{
    class CKeyConvert
    {
        public static int[] pcKeys=
        {
	        0x01, 
	        0x02, 
	        0x04, 
            0x1B, 
            0x0D, 
            0x10, 
            0x11,
            0x12,
            0x20,
            0x25,
            0x26,
            0x27,
            0x28,
            144, 
            0x6F,
            0x6A,
            0x6D,
            0x6B,
            0x6E,
            226, 
            221, 
            186, 
            219, 
            187, 
            0x08, 
	        0x2D, 
            0x24, 
            0x2E, 
            0x23, 
            0x21, 
            0x22, 
            0x09, 
	        188, 
	        190, 
	        191, 
	        223, 
            0x70, 
            0x71, 
            0x72, 
            0x73, 
            0x74, 
            0x75, 
            0x76, 
            0x77, 
            0x78, 
            0x79, 
            0x7A, 
            0x7B, 
            0x7C, 
            0x7D, 
            0x7E, 
            0x7F, 
            0x80, 
            0x81, 
            0x82, 
            0x83, 
            0x84, 
            0x85, 
            0x86, 
            0x87, 
            0x30, 
            0x31, 
            0x32, 
            0x33, 
            0x34, 
            0x35, 
            0x36, 
            0x37, 
            0x38, 
            0x39, 
            0x41, 
            0x42, 
            0x43, 
            0x44, 
            0x45, 
            0x46, 
            0x47, 
            0x48, 
            0x49, 
            0x4A, 
            0x4B, 
            0x4C, 
            0x4D, 
            0x4E, 
            0x4F, 
            0x50, 
            0x51, 
            0x52, 
            0x53, 
            0x54, 
            0x55, 
            0x56, 
            0x57, 
            0x58, 
            0x59, 
            0x5A, 
            0x60, 
            0x61, 
            0x62, 
            0x63, 
            0x64, 
            0x65, 
            0x66, 
            0x67, 
            0x68, 
            0x69, 
            -1
        };
        public static Keys[] xnaKeys =
        {
	        00,		// LButton
	        00,		// RButton
	        00,		// MButton
            Keys.Escape,
            Keys.Enter,		// Return
            Keys.LeftShift,		// Shift
            Keys.LeftControl,		// Control
            0,		// Alt
            Keys.Space,		// Space
            Keys.Left,	    // 37,		// Left
            Keys.Up,	    // 38,		// Up
            Keys.Right,    // 39,		// Right
            Keys.Down,	    // 40,		// Down
            Keys.NumLock,		// Numlock
            Keys.Divide,		// Divide
            Keys.Multiply,		// Multiply
            Keys.Subtract,		// Subtract
            Keys.Add,		// Add
            Keys.Decimal,		// Decimal
            0,		// Inferieur
            Keys.OemOpenBrackets,	    // Accent circonflexe
            Keys.OemCloseBrackets,		// Dollar
            Keys.OemCloseBrackets,		// Parenthese fermee
            Keys.OemPlus,		// Egal
            Keys.Back,		// Backspace
	        Keys.Insert,		// INSERT
            Keys.Home,		// HOME
            Keys.Delete,		// Delete
            Keys.End,		// End
            Keys.PageUp,		// Prev page
            Keys.PageDown,		// Next page
            Keys.Tab,		// Tab
	        Keys.OemComma,		// Virgule
	        Keys.OemSemicolon,	// Point virgule
	        0,		// Deux points
	        0,	// !
            Keys.F1,
            Keys.F2,
            Keys.F3,
            Keys.F4,
            Keys.F5,
            Keys.F6,
            Keys.F7,
            Keys.F8,
            Keys.F9,
            Keys.F10,
            Keys.F11,
            Keys.F12,
            Keys.F13,
            Keys.F14,
            Keys.F15,
            Keys.F16,
            Keys.F17,
            Keys.F18,
            Keys.F19,
            Keys.F20,
            Keys.F21,
            Keys.F22,
            Keys.F23,
            Keys.F24,
            Keys.D0,
            Keys.D1,
            Keys.D2,
            Keys.D3,
            Keys.D4,
            Keys.D5,
            Keys.D6,
            Keys.D7,
            Keys.D8,
            Keys.D9,
            Keys.A,		// A
            Keys.B,		// b
            Keys.C,		// c
            Keys.D,		// d
            Keys.E,		// e
            Keys.F,		// f
            Keys.G,		// g
            Keys.H,		// h
            Keys.I,		// i
            Keys.J,		// j
            Keys.K,		// k	
            Keys.L,		// l
            Keys.M,		// m
            Keys.N,		// n
            Keys.O,		// o
            Keys.P,		// p
            Keys.Q,		// q
            Keys.R,		// r
            Keys.S,		// s
            Keys.T,		// t
            Keys.U,		// u
            Keys.V,		// v
            Keys.W,		// w
            Keys.X,		// x
            Keys.Y,		// y
            Keys.Z,		// Z
            Keys.NumPad0,		// Numpad0
            Keys.NumPad1, 
            Keys.NumPad2, 
            Keys.NumPad3, 
            Keys.NumPad4, 
            Keys.NumPad5, 
            Keys.NumPad6, 
            Keys.NumPad7, 
            Keys.NumPad8, 
            Keys.NumPad9,         
        };

        public static string[] keyNames=
        {
	        "LButton",
	        "MButton",
	        "RButton",
            "Escape",
            "Return", 
            "Shift",
            "Control",
            "Alt",
            "Space",
            "Left",
            "Up",
            "Right",
            "Down",
            "Numlock",
            "Divide",
            "Multiply",
            "Subtract",
            "Add",
            "Decimal",
            "Key1",
            "Key2",
            "Key3",
            "Close bracket",
            "Equal",
            "Backspace",
	        "Insert",
            "Home",
            "Delete",
            "End",
            "Previous page",
            "Next page",
            "Tab",
	        "Comma",
	        "Semi colon",
	        "Colon",
	        "Exclamation",
	        "Unknown",
        };

        private const int NB_SPECIAL_KEYS=29;
        
        public static Keys getXnaKey(int pcKey)
        {
            int n;
            for (n=0; pcKeys[n]!=-1; n++)
            {
                if (pcKeys[n]==pcKey)
                {
                    return xnaKeys[n];
                }
            }
            return 0;
        }


        // Get key text
        public static String getKeyText ( Keys vkCode )
        {
	        string s="";
        	
	        // Rechercher la touche parmi les touches speciales
	        // ------------------------------------------------
	        if ( vkCode >= Keys.NumPad0 && vkCode <= Keys.NumPad9 )
	        {
	            s=String.Format("Numpad{0}", vkCode-Keys.NumPad0);
	        }
	        else if ( vkCode >= Keys.F1 && vkCode <= Keys.F24 )
	        {
	            s=String.Format("F{0}", vkCode-Keys.NumPad0);
	        }
	        else if ( vkCode >= Keys.D0 && vkCode <= Keys.D9)
	        {
	            s=String.Format("{0}", vkCode-Keys.NumPad0);
	        }
            else if (vkCode >= Keys.A && vkCode <= Keys.Z)
            {
                char[] buffer = new char[1];
                buffer[0] = (char)('A' + (char)vkCode);
                return new String(buffer, 0, 1);
            }
            else
            {
                int n;
                for (n = 0; n < NB_SPECIAL_KEYS; n++)
                {
                    if (xnaKeys[n] == vkCode)
                    {
                        s = keyNames[n];
                        break;
                    }
                }
            }
            return s;
        }
    
    }
}
