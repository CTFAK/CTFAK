//----------------------------------------------------------------------------------
//
// CEXTLOADER: Chargement des extensions
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Extensions
{
    public class CExtLoader
    {
        public const int KPX_BASE = 32;
        CRunApp app;
        CExtLoad[] extensions = null;
        short[] numOfConditions;

        public CExtLoader(CRunApp a)
        {
            app = a;
        }

        public void loadList(CFile file) 
        {
            int extCount = file.readAShort();
            int extMaxHandle = file.readAShort();

            extensions = new CExtLoad[extMaxHandle];
            numOfConditions = new short[extMaxHandle];
            int n;
            for (n = 0; n < extMaxHandle; n++)
            {
                extensions[n] = null;
            }

            for (n = 0; n < extCount; n++)
            {
                CExtLoad e = new CExtLoad();
                e.loadInfo(file);

                CRunExtension ext = e.loadRunObject();
                if (ext != null)
                {
                    extensions[e.handle] = e;
                    numOfConditions[e.handle] = (short)ext.getNumberOfConditions();
                }
            }
        }

        public CRunExtension loadRunObject(int type)
        {
            type -= KPX_BASE;
            CRunExtension ext =null;
            if (type < extensions.Length && extensions[type] != null)
            {
                ext = extensions[type].loadRunObject();
            }
            return ext;
        }

        public int getNumberOfConditions(int type)
        {
            type -= KPX_BASE;
            if (type < extensions.Length)
            {
                return numOfConditions[type];
            }
            return 0;
        }
    }
}
