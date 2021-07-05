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
// CWINDOWMANAGER : Gestion des zones de refresh / transfert ecran
//
//----------------------------------------------------------------------------------
package Application;

import java.io.InputStream;

import Runtime.MMFRuntime;

public class CEmbeddedFile
{
    CRunApp app;

    String path;
    int length;
    long offset;

    public String extractedTo;
    
    public CEmbeddedFile(CRunApp a)
    {
        app=a;
    }

    public void preLoad()
    {
        short l=app.file.readAShort();
        path=app.file.readAString(l);
        // path=app.relocatePath(path);
        
        length=app.file.readAInt();
        offset=app.file.getFilePointer();
        app.file.skipBytes(length);
    }
    
    public String getPath() 
    {
    	return path;
    }

    public int getLength()
    {
    	return length; 
    }
    
    public InputStream getInputStream()
    {
        app.file.seek(offset);
        return app.file.getInputStream (length);
    }

    public void extract ()
    {
        String filename = "mmf-" + offset + ".tmp";

        MMFRuntime.inst.inputStreamToFile (getInputStream (),
                    filename);

        extractedTo = MMFRuntime.inst.getFilesDir() + "/" + filename;
    }

    public void deleteExtracted ()
    {
        MMFRuntime.inst.deleteFile
                (extractedTo.substring (extractedTo.lastIndexOf ("/") + 1));
    }
}
