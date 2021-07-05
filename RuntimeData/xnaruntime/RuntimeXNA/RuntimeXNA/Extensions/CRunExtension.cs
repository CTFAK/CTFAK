//----------------------------------------------------------------------------------
//
// CRUNEXTENSION: Classe abstraite run extension
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Expressions;
using RuntimeXNA.Sprites;
using RuntimeXNA.Conditions;
using RuntimeXNA.Actions;

namespace RuntimeXNA.Extensions
{
    public class CRunExtension
    {
        public const int REFLAG_DISPLAY = 1;
        public const int REFLAG_ONESHOT = 2;
        public CExtension ho;
        public CRun rh;

        public virtual void init(CExtension hoPtr)
        {
            ho = hoPtr;
            rh = hoPtr.hoAdRunHeader;
        }

        public virtual int getNumberOfConditions()
        {
            return 0;
        }

        public virtual bool createRunObject(CFile file, CCreateObjectInfo cob, int version)
        {
            return false;
        }

        public virtual int handleRunObject()
        {
            return REFLAG_ONESHOT;
        }

        public virtual void displayRunObject(SpriteBatchEffect batch)
        {
        }

        public virtual void destroyRunObject(bool bFast)
        {
        }

        public virtual void pauseRunObject()
        {

        }

        public virtual void continueRunObject()
        {

        }

        public virtual void getZoneInfos()
        {

        }

        public virtual bool condition(int num, CCndExtension cnd)
        {
            return false;
        }

        public virtual void action(int num, CActExtension act)
        {

        }

        public virtual CValue expression(int num)
        {
            return null;
        }

        public virtual CMask getRunObjectCollisionMask(int flags)
        {
            return null;
        }

        public virtual Texture2D getRunObjectSurface()
        {
            return null;
        }

        public virtual CFontInfo getRunObjectFont()
        {
            return null;
        }

        public virtual void setRunObjectFont(CFontInfo fi, CRect rc)
        {

        }

        public virtual int getRunObjectTextColor()
        {
            return 0;
        }

        public virtual void setRunObjectTextColor(int rgb)
        {
        }

    }
}
