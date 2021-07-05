/* Copyright (c) 1996-2013 Clickteam
 *
 * This source code is part of the Android exporter for Clickteam Multimedia Fusion 2.
 * 
 * Permission is hereby granted to any person obtaining a legal copy 
 * of Clickteam Multimedia Fusion 2 to use or modify this source code for 
 * debugging, optimizing, or customizing applications created with 
 * Clickteam Multimedia Fusion 2.  Any other use of this source code is prohibited.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 */
//----------------------------------------------------------------------------------
//
// CTRANS : interface avec un effet de transition
//
//----------------------------------------------------------------------------------
package Transitions;

import Application.CRunApp;
import android.graphics.Bitmap;

public class CTransitionManager
{
    CRunApp app;

    public CTransitionManager(CRunApp a)
    {
        app = a;
    }

    public CTrans createTransition(CTransitionData pData, Bitmap display, Bitmap surfaceStart, Bitmap surfaceEnd)
    {
        String dllName = "CTransition" + pData.dllName;
        try
        {
            Class<?> c;
            c = Class.forName(dllName);
            CTransitions transitions = (CTransitions) c.newInstance();
            CTrans trans = transitions.getTrans(pData);
            app.file.seek(pData.dataOffset);
            trans.init(pData, app.file, display, surfaceStart, surfaceEnd);
            return trans;
        }
        catch (ClassNotFoundException e)
        {
        }
        catch (InstantiationException e)
        {
        }
        catch (IllegalAccessException e)
        {
        }
        return null;
    }
}
