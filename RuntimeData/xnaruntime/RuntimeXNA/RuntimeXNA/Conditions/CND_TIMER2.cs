// ------------------------------------------------------------------------------
// 
// TIMER EQUALS
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Events;
using RuntimeXNA.Params;

namespace RuntimeXNA.Conditions
{
    class CND_TIMER2 : CCnd
    {
        public override bool eva1(CRun rhPtr, CObject hoPtr)
        {
            return eva2(rhPtr);
        }
        public override bool eva2(CRun rhPtr)
        {
            int time;
            if (this.evtParams[0].code == 22)
                time = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
            else
                time = ((PARAM_TIME)evtParams[0]).timer;

            PARAM_INT param2 = (PARAM_INT)evtParams[1];
            if (rhPtr.rhTimer >= time)
            {
                if (param2.value_Renamed == rhPtr.rhLoopCount)
                {
                    param2.value_Renamed = rhPtr.rhLoopCount + 1;
                    return false;
                }
                param2.value_Renamed = rhPtr.rhLoopCount + 1;
                return true;
            }
            return false;
        }
    }
}
