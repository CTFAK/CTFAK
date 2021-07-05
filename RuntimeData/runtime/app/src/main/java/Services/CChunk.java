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
// --------------------------------------------------------------------------------
//
// CCHUNK : lecture de troncons de donn�es de l'application
//
// --------------------------------------------------------------------------------
package Services;

/** Chunk class.
 * Reads a MMF chunk from application.ccj.
 */
public class CChunk 
{
    public short chID;
    public short chFlags;
    public int chSize;

    // Declaration des types de chunks
//    public static int CHUNK_PREVIEW=0x1122;

//    public static short CHUNK_APPMINIHEADER=0x2222;
//    public static short CHUNK_APPHEADER=0x2223;
//    public static short CHUNK_APPNAME=0x2224;
//    public static short CHUNK_APPAUTHOR=0x2225;
//    public static short CHUNK_APPMENU=0x2226;
//    public static short CHUNK_EXTPATH=0x2227;
//    public static short CHUNK_EXTENSIONS=0x2228;
//    public static short CHUNK_FRAMEITEMS=0x2229;
//    public static short CHUNK_GLOBALEVENTS=0x222A;
//    public static short CHUNK_FRAMEHANDLES=0x222B;
//    public static short CHUNK_EXTDATA=0x222C;
//    public static short CHUNK_ADDITIONAL_EXTENSION=0x222D;
//    public static short CHUNK_APPEDITORFILENAME=0x222E;
//    public static short CHUNK_APPTARGETFILENAME=0x222F;
//    public static short CHUNK_APPDOC=0x2230;
//    public static short CHUNK_OTHEREXTS=0x2231;
//    public static short CHUNK_GLOBALVALUES=0x2232;
//    public static short CHUNK_GLOBALSTRINGS=0x2233;
//    public static short CHUNK_EXTENSIONS2=0x2234;
//    public static short CHUNK_APPICON_16x16x8=0x2235;
//    public static short CHUNK_DEMOVERSION=0x2236;
//    public static short CHUNK_SECNUM=0x2237;
//    public static short CHUNK_BINARYFILES=0x2238;
//    public static short CHUNK_APPMENUIMAGES=0x2239;
//    public static short CHUNK_ABOUTTEXT=0x223A;
//    public static short CHUNK_COPYRIGHT=0x223B;
//    public static short CHUNK_GLOBALVALUENAMES=0x223C;
//    public static short CHUNK_GLOBALSTRINGNAMES=0x223D;
//    public static short CHUNK_MVTEXTS=0x223E;
//    public static short CHUNK_FRAMEITEMS_2=0x223F;
//    public static short CHUNK_EXEONLY=0x2240;
//    public static short CHUNK_APPHEADER_EX=0x2241;
//    public static short CHUNK_APP_RESERVED=0x2242;
//    public static short CHUNK_EFFECTS=0x2243;
//    public static short CHUNK_BLURAYAPPOPTIONS=0x2244;
//    public static short CHUNK_APPHDR2=0x2245;

//    public static short CHUNK_FRAME=0x3333;
//    public static short CHUNK_FRAMEHEADER=0x3334;
//    public static short CHUNK_FRAMENAME=0x3335;
//    public static short CHUNK_FRAMEPASSWORD=0x3336;
//    public static short CHUNK_FRAMEPALETTE=0x3337;
//    public static short CHUNK_FRAMEITEMINSTANCES=0x3338;
//    public static short CHUNK_FRAMEFADEINFRAME=0x3339;
//    public static short CHUNK_FRAMEFADEOUTFRAME=0x333A;
//    public static short CHUNK_FRAMEFADEIN=0x333B;
//    public static short CHUNK_FRAMEFADEOUT=0x333C;
//    public static short CHUNK_FRAMEEVENTS=0x333D;
//    public static short CHUNK_FRAMEPLAYHEADER=0x333E;
//    public static short CHUNK_ADDITIONAL_FRAMEITEM=0x333F;
//    public static short CHUNK_ADDITIONAL_FRAMEITEMINSTANCE=0x3340;
//    public static short CHUNK_FRAMELAYERS=0x3341;
//    public static short CHUNK_FRAMEVIRTUALRECT=0x3342;
//    public static short CHUNK_DEMOFILEPATH=0x3343;
//    public static short CHUNK_RANDOMSEED=0x3344;
//    public statis short CHUNK_MVTTIMERBASE=0x3347

//    public static short CHUNK_OBJINFOHEADER=0x4444;
//    public static short CHUNK_OBJINFONAME=0x4445;
//    public static short CHUNK_OBJECTSCOMMON=0x4446;

//    public static short CHUNK_IMAGESOFFSETS=0x5555;
//    public static short CHUNK_FONTSOFFSETS=0x5556;
//    public static short CHUNK_SOUNDSOFFSETS=0x5557;
//    public static short CHUNK_MUSICSOFFSETS=0x5558;

//    public static short CHUNK_IMAGES=0x6666;
//    public static short CHUNK_FONTS=0x6667;
//    public static short CHUNK_SOUNDS=0x6668;
//    public static short CHUNK_MUSICS=0x6669;

    public static short CHUNK_LAST=0x7F7F;

    public CChunk() 
    {
		chID=0;
		chFlags=0;
		chSize=0;
    }
    
    // Lis le header d'un chunk
    public short readHeader(CFile file) 
    {
		chID=file.readAShort();
		chFlags=file.readAShort();
		chSize=file.readAInt();
    	return chID;
    }
    
    public void skipChunk(CFile file) 
    {
    	file.skipBytes(chSize);
    }    
}
