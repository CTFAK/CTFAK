//----------------------------------------------------------------------------------
//
// CRVAL : Alterable values et strings
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.RunLoop;
using RuntimeXNA.OI;
using RuntimeXNA.Objects;
using RuntimeXNA.Expressions;

namespace RuntimeXNA.Values
{
    public class CRVal
    {
        public const int VALUES_NUMBEROF_ALTERABLE = 26;
        public const int STRINGS_NUMBEROF_ALTERABLE = 10;
        public int rvValueFlags;
        public CValue[] rvValues;
        public String[] rvStrings;

        public void init(CObject ho, CObjectCommon ocPtr, CCreateObjectInfo cob)
        {
            // Creation des tableaux
            rvValueFlags = 0;
            rvValues = new CValue[VALUES_NUMBEROF_ALTERABLE];
            rvStrings = new string[STRINGS_NUMBEROF_ALTERABLE];
            int n;
            for (n = 0; n < VALUES_NUMBEROF_ALTERABLE; n++)
            {
                rvValues[n] = null;
            }
            for (n = 0; n < STRINGS_NUMBEROF_ALTERABLE; n++)
            {
                rvStrings[n] = null;
            }

            // Initialisation des valeurs
            if (ocPtr.ocValues != null)
            {
                CValue value;
                for (n = 0; n < ocPtr.ocValues.nValues; n++)
                {
                    value = getValue(n);
                    value.forceInt(ocPtr.ocValues.values[n]);
                }
            }
            if (ocPtr.ocStrings != null)
            {
                for (n = 0; n < ocPtr.ocStrings.nStrings; n++)
                {
                    rvStrings[n] = ocPtr.ocStrings.strings[n];
                }
            }
        }

        public void kill(bool bFast)
        {
            int n;
            for (n = 0; n < VALUES_NUMBEROF_ALTERABLE; n++)
            {
                rvValues[n] = null;
            }
            for (n = 0; n < STRINGS_NUMBEROF_ALTERABLE; n++)
            {
                rvStrings[n] = null;
            }
        }

        public CValue getValue(int n)
        {
            if (rvValues[n] == null)
            {
                rvValues[n] = new CValue();
            }
            return rvValues[n];
        }

        public String getString(int n)
        {
            if (rvStrings[n] == null)
            {
                rvStrings[n] = "";
            }
            return rvStrings[n];
        }

        public void setString(int n, String s)
        {
            rvStrings[n] = s;
        }

    }
}
