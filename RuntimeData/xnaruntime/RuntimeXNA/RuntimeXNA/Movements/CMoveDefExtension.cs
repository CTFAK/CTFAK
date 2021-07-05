//----------------------------------------------------------------------------------
//
// CMOVEDEFEXTENSION : données d'un movement extension
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;

namespace RuntimeXNA.Movements
{
    public class CMoveDefExtension : CMoveDef
    {
        public String moduleName;
        public int mvtID;
        public byte[] data;

        public override void load(CFile file, int length)
        {
            file.skipBytes(14);
            data = new byte[length - 14];
            file.read(data);
        }

        public void setModuleName(String name, int id)
        {
            moduleName = name;
            mvtID = id;
        }
    }
}
