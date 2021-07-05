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
// CParamExpression, classe de base d'une expression
//
//----------------------------------------------------------------------------------
package Params;

import Application.CRunApp;
import Expressions.CExp;
import Services.CFile;

public abstract class CParamExpression extends CParam
{
    public CExp tokens[] = null;
    public short comparaison;

    public void load(CRunApp app, CFile file)
    {
        long debut = file.getFilePointer();

        // Compte le nombre de tokens
        int count = 0;
        short size;
        int code;
        while (true)
        {
            count++;
            code = file.readAInt();
            if (code == 0)
            {
                break;
            }
            size = file.readAShort();
            if (size > 6)
            {
                file.skipBytes(size - 6);
            }
        }
        ;

        // Charge les tokens
        file.seek(debut);
        tokens = new CExp[count];
        int n;
        for (n = 0; n < count; n++)
        {
            tokens[n] = CExp.create(app, file);
        }
    }

    @Override
	public abstract void load(CRunApp app);
}
