//----------------------------------------------------------------------------------
//
// CEMBEDDEFILE : fichiers binaires
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;

namespace RuntimeXNA.Application
{
    public class CEmbeddedFile
    {
        private CRunApp app;
        public string path;
        private int length;
        private int offset;
        private CFile data;

        public CEmbeddedFile(CRunApp a)
        {
            app = a;
        }
        string cleanName(string name)
        {
            int pos = name.LastIndexOf('\\');
            if (pos < 0)
            {
                pos = name.LastIndexOf('/');
            }
            if (pos >= 0 && pos + 1 < name.Length)
            {
                name = name.Substring(pos + 1);
            }
            return name;
        }
        public void preLoad()
        {
            short l=app.file.readAShort();
            path=app.file.readAString(l);
            path=cleanName(path);

            length=app.file.readAInt();
            offset=app.file.getFilePointer();
            app.file.skipBytes(length);
        }
        public CFile open()
        {
            app.file.seek(offset);
            data=new CFile(app.file, length);
            return data;
        }
    }
}
