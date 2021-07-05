// ------------------------------------------------------------------------------
// 
// EVERY with expressions
// 
// ------------------------------------------------------------------------------
using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;

namespace RuntimeXNA.Conditions
{
    class CND_EVERY2 : CCnd
    {
        public override bool eva1(CRun rhPtr, CObject hoPtr)
        {
            return eva2(rhPtr);
        }
        public override bool eva2(CRun rhPtr)
        {
            PARAM_INT param2 = (PARAM_INT)evtParams[1];

            int time;
            if (param2.value2 == 0)
            {
                if (this.evtParams[0].code == 22)
                    time = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
                else
                    time = ((PARAM_TIME)evtParams[0]).timer;
                param2.value_Renamed = time;
                param2.value2 = -1;
            }
            else
            {
                param2.value_Renamed -= rhPtr.rhTimerDelta;
                if (param2.value_Renamed <= 0)
                {
                    if (this.evtParams[0].code == 22)
                        time = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
                    else
                        time = ((PARAM_TIME)evtParams[0]).timer;
                    param2.value_Renamed += time;
                    return true;
                }
            }
            return false;
        }
    }
}
