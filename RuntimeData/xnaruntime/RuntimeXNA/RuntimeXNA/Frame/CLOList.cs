//----------------------------------------------------------------------------------
//
// CLOLIST : liste de levelobjects
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;
using RuntimeXNA.Application;
using RuntimeXNA.OI;

namespace RuntimeXNA.Frame
{
    public class CLOList
    {
        public CLO[] list;
        public short[] handleToIndex;
        public int nIndex;
        int loFranIndex;

        public void load(CRunApp app)
        {
            nIndex = app.file.readAInt();
            list = new CLO[nIndex];
            int n;
            short maxHandles = 0;
            for (n = 0; n < nIndex; n++)
            {
                list[n] = new CLO();
                list[n].load(app.file);
                if (list[n].loHandle + 1 > maxHandles)
                {
                    maxHandles = (short) (list[n].loHandle + 1);
                }
                COI pOI = app.OIList.getOIFromHandle(list[n].loOiHandle);
                list[n].loType = pOI.oiType;
            }
            handleToIndex = new short[maxHandles];
            for (n = 0; n < nIndex; n++)
            {
                handleToIndex[list[n].loHandle] = (short) n;
            }
        }

        public CLO getLOFromIndex(short index)
        {
            return list[index];
        }

        public CLO getLOFromHandle(short handle)
        {
            if (handle<handleToIndex.Length)
            {
                return list[handleToIndex[handle]];
            }
            return null;
        }

        // Get next LevObj
        public CLO next_LevObj()
        {
            CLO plo;

            if (loFranIndex < nIndex)
            {
                do
                {
                    plo = list[loFranIndex++];
                    if (plo.loType >= COI.OBJ_SPR)
                    {
                        return plo;
                    }
                } while (loFranIndex < nIndex);
            }
            return null;
        }

        // Get first levObj address
        public CLO first_LevObj()
        {
            loFranIndex = 0;
            return next_LevObj();
        }
    }
}
