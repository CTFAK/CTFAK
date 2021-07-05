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
// CFILE : chargement des fichiers dans le bon sens
//
//----------------------------------------------------------------------------------
package Services;

import java.io.File;
import java.io.IOException;

import android.content.pm.PackageInfo;
import Runtime.Log;
import Runtime.MMFRuntime;

public class CFile
{
    public final static String charset = "iso-8859-1";
    public final static String charsetU16 = "UTF-16LE";
    public final static String charsetU8 = "utf-8";

    private native void allocNative (String filename, int deviceRAM);
    private native void freeNative ();

    public native void loadAll ();
    public native void mmap ();
    public native boolean rooted();

    public CFile (String filename)

    {
         allocNative (filename, CServices.getTotalMegaRAM());
    }

    @Override
    public void finalize ()
    {   if (ptr != 0)
        {   freeNative ();
            ptr = 0;
        }
    }

    public long ptr;

    public CFile (int resourceID, boolean loadAll)
    {
        //ResourceID for ccn in OBB is 0
        String filename = "mmf-res-" + resourceID;

        File checkfile = new File(MMFRuntime.inst.getFilesDir() + "/" + filename);
        
        // Check and delete old MMF-RES with non matching ID
        // 25/11/2014: 284
        deleteOldMMF(resourceID);
        
        if(!checkfile.exists()) { 
        	if(!MMFRuntime.inst.obbAvailable)
        		MMFRuntime.inst.inputStreamToFile (MMFRuntime.inst.getResources().openRawResource
                    (resourceID), filename);  
        	else
                MMFRuntime.inst.inputStreamToFile (
                		MMFRuntime.inst.getInputStreamFromAPK("res/raw/application.ccn"), filename);  
        }
        else {
            try {
            	// Here we check for a new version updated or installed in that case rewrite the CCN application resource
            	// 29/10/2014: 283.3 FVD
                // 25/09/2019  292.14 Yves
                PackageInfo packageInfo = MMFRuntime.inst.getPackageManager().getPackageInfo(MMFRuntime.inst.getPackageName(), 0);
                if((packageInfo.firstInstallTime > checkfile.lastModified()
                        || packageInfo.lastUpdateTime > checkfile.lastModified())) {
                    if(!MMFRuntime.inst.obbAvailable) {
                        MMFRuntime.inst.inputStreamToFile(MMFRuntime.inst.getResources().openRawResource
                                (resourceID), filename);
                        Log.Log ("Updating magical apk");
                    }
                    else {
                        MMFRuntime.inst.inputStreamToFile(
                                MMFRuntime.inst.getInputStreamFromAPK("res/raw/application.ccn"), filename);
                        Log.Log ("Updating magical exp");
                    }
                }
            } 		
            catch (Throwable t) {
    			Log.Log ("Error retrieving package info: " + t);
    		}

        }

        allocNative (MMFRuntime.inst.getFilesDir() + "/" + filename, CServices.getTotalMegaRAM());

        if (loadAll)
        {
            loadAll ();

			Log.Log ("About to delete file: " + filename);
            MMFRuntime.inst.deleteFile (filename);
        }
    }

    public native int readUnsignedByte ();
    public native byte readByte();
    public native short readAShort();
    public native int getFilePointer ();
    public native void seek (int pos);

    public void seek (long pos)
    {
        seek ((int) pos);
    }

    public native int skipBytes(int n);
    public native int read (byte dest[], int offset, int length);
    public native void close ();

    public native void setUnicode (boolean unicode);
    public native boolean unicode ();

    public class InputStream extends java.io.InputStream
    {
        protected CFile file;

        protected long streamBegin;
        protected long streamEnd;

        protected int mark;
        protected int markExpires;

        @Override
        public int available ()
        {
            try
            {
                return (int) (streamEnd - file.getFilePointer ());
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        @Override
        public void close () throws IOException
        {
        }

        @Override
        public boolean markSupported ()
        {
            return true;
        }

        @Override
        public void mark (int readlimit)
        {
            this.mark = file.getFilePointer ();
            this.markExpires = mark + readlimit;
        }

        @Override
        public void reset () throws IOException
        {
            if (mark == -1 || file.getFilePointer() > markExpires)
                throw new IOException ();

            file.seek (mark);
        }

        @Override
        public long skip (long n)
        {
            return file.skipBytes ((int) n);
        }

        @Override
        public int read () throws IOException
        {
            if (file.getFilePointer () >= streamEnd)
                return -1;

            return file.readUnsignedByte ();
        }

        @Override
        public int read (byte [] b) throws IOException
        {
            return read (b, 0, b.length);
        }

        @Override
        public int read (byte [] b, int offset, int length) throws IOException
        {
            long fp = file.getFilePointer ();

            if ((fp + length) > streamEnd)
                length = (int) (streamEnd - fp);

            if (length <= 0)
                return -1;

            return file.read (b, offset, length);
        }

    }

    public InputStream getInputStream (int size)
    {
        InputStream inputStream = new InputStream ();

        try
        {
            inputStream.streamBegin = getFilePointer ();
            inputStream.streamEnd = inputStream.streamBegin + size;

            inputStream.file = this;
            inputStream.mark = -1;
            inputStream.markExpires = Integer.MAX_VALUE;
        }
        catch (Exception e)
        {
            return null;
        }

        return inputStream;
    }

    public char readAChar()
    {
        return (char) readAShort ();
    }

    public void readAChar(char[] b)
    {
        int b1, b2;
        int n;
        for (n=0; n<b.length; n++)
        {
            b1 = readUnsignedByte();
            b2 = readUnsignedByte();
            b[n]=(char) (b2 * 256 + b1);
        }
    }

    public int readAInt()
    {
        int b1, b2, b3, b4;
        b1 = readUnsignedByte();
        b2 = readUnsignedByte();
        b3 = readUnsignedByte();
        b4 = readUnsignedByte();
        return b4 * 0x01000000 + b3 * 0x00010000 + b2 * 0x00000100 + b1;
    }

    public int readAColor()
    {
        int b1, b2, b3;
        b1 = readUnsignedByte();
        b2 = readUnsignedByte();
        b3 = readUnsignedByte();
        readUnsignedByte();
        return b1 * 0x00010000 + b2 * 0x00000100 + b3;
    }

    public float readAFloat()
    {
        int b1, b2, b3, b4;
        b1 = readUnsignedByte();
        b2 = readUnsignedByte();
        b3 = readUnsignedByte();
        b4 = readUnsignedByte();
        int total = b4 * 0x01000000 + b3 * 0x00010000 + b2 * 0x00000100 + b1;
        return total / (float) 65536.0;
    }

    public double readADouble()
    {
        int b1, b2, b3, b4, b5, b6, b7, b8;
        b1 = readUnsignedByte();
        b2 = readUnsignedByte();
        b3 = readUnsignedByte();
        b4 = readUnsignedByte();
        b5 = readUnsignedByte();
        b6 = readUnsignedByte();
        b7 = readUnsignedByte();
        b8 = readUnsignedByte();
        long total1 = ((long) b4) * 0x01000000 + ((long) b3) * 0x00010000 + ((long) b2) * 0x00000100 + b1;
        long total2 = ((long) b8) * 0x01000000 + ((long) b7) * 0x00010000 + ((long) b6) * 0x00000100 + b5;
        long total = (total2 << 32) | total1;
        double temp = total / 65536.0;
        return temp / 65536.0;
    }

    public String readAString(int size)
    {
        try
        {
            if (!unicode ())
            {
                byte b[] = new byte[size];
                read(b);
                int n;
                for (n=0; n<size; n++)
                {
                    if (b[n]==0)
                    {
                        break;
                    }
                }
                int m;
                byte bb[]=new byte[n];
                for (m=0; m<n; m++)
                {
                    bb[m]=b[m];
                }

                return new String(bb, charset);
            }
            else
            {
                char b[]=new char[size];
                readAChar(b);
                int n;
                for (n=0; n<size; n++)
                {
                    if (b[n]==0)
                    {
                        break;
                    }
                }
                int m;
                char bb[]=new char[n];
                for (m=0; m<n; m++)
                {
                    bb[m]=b[m];
                }
                return new String(bb);
            }
        }
        catch (Throwable e)
        {
            return "";
        }
    }

    public String readAString()
    {
        try
        {
            String ret = "";
            int debut = getFilePointer();
            if (!unicode ())
            {
                int b;
                do
                {
                    b = readUnsignedByte();
                } while (b != 0);
                long end = getFilePointer();
                seek(debut);
                if (end > debut + 1)
                {
                    byte c[] = new byte[(int) (end - debut - 1)];
                    read(c);

                    ret = new String(c, charset);
                }
                skipBytes(1);
            }
            else
            {
                char b;
                do
                {
                    b = readAChar();
                } while (b != 0);
                long end = getFilePointer();
                seek(debut);
                if (end > debut + 2)
                {
                    int l=(int)(end - debut - 1)/2;
                    char c[] = new char[l];
                    readAChar(c);
                    ret = new String(c);
                }
                skipBytes(2);
            }
            return ret;
        }
        catch (Throwable e)
        {
            return "";
        }
    }

    public int read(byte dest[])
    {
        return read (dest, 0, dest.length);
    }

    public int read(byte dest[], int length)
    {
        return read (dest, 0, length);
    }

    // Scan all data/files directory looking for "mmf-res-" files
    // delete all them that not match the resourceID
    public void deleteOldMMF(int resourceID)
    {
    	String pathToScan = MMFRuntime.inst.getFilesDir().getAbsolutePath();
    	String mmf_file ;  // fileThatYouWantToFilter
    	File folderToScan = new File(pathToScan); 

    	File[] listOfFiles = folderToScan.listFiles();

    	for (int i = 0; i < listOfFiles.length; i++) {
    		if (listOfFiles[i].isFile()) {
    			mmf_file = listOfFiles[i].getName();
    			if (mmf_file.startsWith("mmf-res-") && !mmf_file.endsWith(String.valueOf(resourceID))) {
    				MMFRuntime.inst.deleteFile (mmf_file);
    			}
    		}
    	}
    }
}
