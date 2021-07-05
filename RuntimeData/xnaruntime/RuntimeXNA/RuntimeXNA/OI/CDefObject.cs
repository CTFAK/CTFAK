//----------------------------------------------------------------------------------
//
// CDEFOBJECT : Classe abstraite de definition d'un objet'
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RuntimeXNA.Services;
using RuntimeXNA.Banks;

namespace RuntimeXNA.OI
{
    public class CDefObject
    {
        virtual public void load(CFile file) { }
        virtual public void enumElements(IEnum enumImages, IEnum enumFonts) { }
    }
}
