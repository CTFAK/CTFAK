// -----------------------------------------------------------------------------
//
// CACTEXTENSION : actions extension
//
// -----------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Expressions;
using RuntimeXNA.Events;
using RuntimeXNA.Services;
using Microsoft.Xna.Framework.Input;
namespace RuntimeXNA.Actions
{
	
	public class CActExtension:CAct
	{
		public override void  execute(CRun rhPtr)
		{
			CObject pHo = rhPtr.rhEvtProg.get_ActionObjects(this);
			if (pHo == null)
			{
				return ;
			}
			
			int act = (int)((evtCode>>16)&0xFFFF) - CEventProgram.EVENTS_EXTBASE; // Vire le type
			CExtension pExt = (CExtension) pHo;
			pExt.action(act, this);
		}
		
		// Recolte des parametres
		// ----------------------
		public virtual CObject getParamObject(CRun rhPtr, int num)
		{
			return rhPtr.rhEvtProg.get_ParamActionObjects(((PARAM_OBJECT) evtParams[num]).oiList, this);
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
		
		public virtual short getParamShort(CRun rhPtr, int num)
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
		
		public virtual PARAM_CREATE getParamCreate(CRun rhPtr, int num)
		{
			return (PARAM_CREATE) evtParams[num];
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
		
		public virtual CPositionInfo getParamPosition(CRun rhPtr, int num)
		{
			CPosition position = (CPosition) evtParams[num];
			CPositionInfo pInfo = new CPositionInfo();
			position.read_Position(rhPtr, 0, pInfo);
			return pInfo;
		}
		
		public virtual short getParamJoyDirection(CRun rhPtr, int num)
		{
			return ((PARAM_SHORT) evtParams[num]).value;
		}
		
		public virtual PARAM_SHOOT getParamShoot(CRun rhPtr, int num)
		{
			return (PARAM_SHOOT) evtParams[num];
		}
		
		public virtual PARAM_ZONE getParamZone(CRun rhPtr, int num)
		{
			return (PARAM_ZONE) evtParams[num];
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
			int rgb = rhPtr.get_EventExpressionInt((CParamExpression) evtParams[num]);
			return CServices.swapRGB(rgb);
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
		
		public virtual double getParamExpDouble(CRun rhPtr, int num)
		{
			CValue value = rhPtr.get_EventExpressionAny((CParamExpression) evtParams[num]);
            return value.getDouble();
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
		
		public virtual CFile getParamExtension(CRun rhPtr, int num)
		{
/*
            PARAM_EXTENSION p = (PARAM_EXTENSION) evtParams[num];
			if (p.data != null)
			{
				return new CBinaryFile(p.data, rhPtr.rhApp.bUnicode);
			}
*/			return null;
		}
	}
}