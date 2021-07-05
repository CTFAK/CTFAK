//----------------------------------------------------------------------------------
//
// CRECT : classe rectangle similaire a celle de windows
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuntimeXNA.Services
{
    public class CRect
    {
        public int left = 0;
        public int top = 0;
        public int right = 0;
        public int bottom = 0;

        public void load(CFile file)
        {
	        left=file.readAInt();
	        top=file.readAInt();
	        right=file.readAInt();
	        bottom=file.readAInt();
        }
        public void copyRect(CRect srce)
        {
	        left=srce.left;
	        right=srce.right;
	        top=srce.top;
	        bottom=srce.bottom;
        }
        public bool ptInRect(int x, int y)
        {
	        if (x>=left && x<right && y>=top && y<bottom)
	            return true;
	        return false;
        }
        public bool intersectRect(CRect rc)
        {
	        if ((left>=rc.left && left<rc.right) || (right>=rc.left && right<rc.right) || (rc.left>=left && rc.left<right) || (rc.right>=left && rc.right<right))
	        {
	            if ((top>=rc.top && top<rc.bottom) || (bottom>=rc.top && bottom<rc.bottom) || (rc.top>=top && rc.top<bottom) || (rc.bottom>=top && rc.bottom<bottom))
	            {
    		        return true;
	            }
	        }
	        return false;
        }
        public void offsetRect(int xOffset, int yOffset)
        {
            left += xOffset;
            top += yOffset;
            right += xOffset;
            bottom += yOffset;
        }
    }
}
