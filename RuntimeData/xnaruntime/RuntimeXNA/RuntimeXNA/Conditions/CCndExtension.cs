// ------------------------------------------------------------------------------
// 
// EXTENSION conditions
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Expressions;
using RuntimeXNA.Events;
using RuntimeXNA.Services;
using Microsoft.Xna.Framework.Input;
namespace RuntimeXNA.Conditions
{
	
	public class CCndExtension:CCnd
	{
		public override bool eva1(CRun rhPtr, CObject pHo)
		{
			if (pHo == null)
			{
				return eva2(rhPtr);
			}
			CExtension extPtr = (CExtension) pHo;
			pHo.hoFlags |= CObject.HOF_TRUEEVENT;
			int cond = -(int)((short)((evtCode >> 16) & 0xFFFF)) - CEventProgram.EVENTS_EXTBASE - 1; // Vire le type
			if (extPtr.condition(cond, this))
			{
				rhPtr.rhEvtProg.evt_AddCurrentObject(pHo);
				return true;
			}
			return false;
		}
		
		public override bool eva2(CRun rhPtr)
		{
			// Boucle d'exploration
			CObject pHo = rhPtr.rhEvtProg.evt_FirstObject(evtOiList);
			int cpt = rhPtr.rhEvtProg.evtNSelectedObjects;
            int cond = -(int)((short)((evtCode >> 16) & 0xFFFF)) - CEventProgram.EVENTS_EXTBASE - 1;
			
			while (pHo != null)
			{
				CExtension pExt = (CExtension) pHo;
				pHo.hoFlags &= ~ CObject.HOF_TRUEEVENT;
				if (pExt.condition(cond, this))
				{
					if ((evtFlags2 & EVFLAG2_NOT) != 0)
					{
						cpt--;
						rhPtr.rhEvtProg.evt_DeleteCurrentObject(); // On le vire!
					}
				}
				else
				{
					if ((evtFlags2 & EVFLAG2_NOT) == 0)
					{
						cpt--;
						rhPtr.rhEvtProg.evt_DeleteCurrentObject(); // On le vire!
					}
				}
				pHo = rhPtr.rhEvtProg.evt_NextObject();
			}
			// Vrai / Faux?
			if (cpt != 0)
			{
				return true;
			}
			return false;
		}
		
		// Recolte des parametres
		// ----------------------
		public virtual PARAM_OBJECT getParamObject(CRun rhPtr, int num)
		{
			return (PARAM_OBJECT) evtParams[num];
		}
		
		public virtual int getParamTime(CRun rhPtr, int num)
		{
			if (evtParams[num].code == 2)
			// PARAM_TIME
			{
				return ((PARAM_TIME) evtParams[num]).timer;
			}
			return rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]);
		}
		
		public virtual short getParamBorder(CRun rhPtr, int num)
		{
			return ((PARAM_SHORT) evtParams[num]).value;
		}
		
		public virtual short getParamAltValue(CRun rhPtr, int num)
		{
			return ((PARAM_SHORT) evtParams[num]).value;
		}
		
		public virtual short getParamDirection(CRun rhPtr, int num)
		{
			return ((PARAM_SHORT) evtParams[num]).value;
		}
		
		public virtual int getParamAnimation(CRun rhPtr, int num)
		{
			if (evtParams[num].code == 10)
			// PARAM_TIME
			{
				return ((PARAM_SHORT) evtParams[num]).value;
			}
			return rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]);
		}
		
		public virtual short getParamPlayer(CRun rhPtr, int num)
		{
			return ((PARAM_SHORT) evtParams[num]).value;
		}
		
		public virtual PARAM_EVERY getParamEvery(CRun rhPtr, int num)
		{
			return (PARAM_EVERY) evtParams[num];
		}
		
		public virtual Keys getParamKey(CRun rhPtr, int num)
		{
			return ((PARAM_KEY) evtParams[num]).key;
		}
		
		public virtual int getParamSpeed(CRun rhPtr, int num)
		{
			return rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]);
		}
		
		public virtual PARAM_POSITION getParamPosition(CRun rhPtr, int num)
		{
			return (PARAM_POSITION) evtParams[num];
		}
		
		public virtual short getParamJoyDirection(CRun rhPtr, int num)
		{
			return ((PARAM_SHORT) evtParams[num]).value;
		}
		
		public virtual int getParamExpression(CRun rhPtr, int num)
		{
			return rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]);
		}
		
		public virtual int getParamColour(CRun rhPtr, int num)
		{
			if (evtParams[num].code == 24)
			// PARAM_COLOUR
			{
				return ((PARAM_COLOUR) evtParams[num]).color;
			}
			return CServices.swapRGB(rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]));
		}
		
		public virtual short getParamFrame(CRun rhPtr, int num)
		{
			return ((PARAM_SHORT) evtParams[num]).value;
		}
		
		public virtual int getParamNewDirection(CRun rhPtr, int num)
		{
			if (evtParams[num].code == 29)
			// PARAM_NEWDIRECTION
			{
				return ((PARAM_SHORT) evtParams[num]).value;
			}
			return rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]);
		}
		
		public virtual short getParamClick(CRun rhPtr, int num)
		{
			return ((PARAM_SHORT) evtParams[num]).value;
		}
		
		public virtual PARAM_PROGRAM getParamProgram(CRun rhPtr, int num)
		{
			return (PARAM_PROGRAM) evtParams[num];
		}
		
		public virtual System.String getParamFilename(CRun rhPtr, int num)
		{
			if (evtParams[num].code == 40)
			// PARAM_FILENAME
			{
				return ((PARAM_STRING) evtParams[num]).pString;
			}
			return rhPtr.get_EventExpressionString((CParamExpression) evtParams[num]);
		}
		
		public virtual System.String getParamExpString(CRun rhPtr, int num)
		{
			return rhPtr.get_EventExpressionString((CParamExpression) evtParams[num]);
		}
		
		public virtual System.String getParamFilename2(CRun rhPtr, int num)
		{
			if (evtParams[num].code == 63)
			// PARAM_FILENAME2
			{
				return ((PARAM_STRING) evtParams[num]).pString;
			}
			return rhPtr.get_EventExpressionString((CParamExpression) evtParams[num]);
		}
		
		public virtual bool compareValues(CRun rhPtr, int num, CValue value_Renamed)
		{
			CValue value2 = rhPtr.get_EventExpressionAny((CParamExpression) evtParams[num]);
			short comp = ((CParamExpression) evtParams[num]).comparaison;
			return CRun.compareTo(value_Renamed, value2, comp);
		}
		
		public virtual bool compareTime(CRun rhPtr, int num, int t)
		{
			PARAM_CMPTIME p = (PARAM_CMPTIME) evtParams[num];
			CValue value2 = new CValue(p.timer);
			short comp = p.comparaison;
			CValue value_Renamed = new CValue(t);
			return CRun.compareTo(value_Renamed, value2, comp);
		}
	}
}