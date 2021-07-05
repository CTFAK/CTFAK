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
// CRUNFRAME : Classe Frame
//
//----------------------------------------------------------------------------------
package Frame;

import java.util.ArrayList;

import RunLoop.CBkd2;
import Services.CArrayList;
import Services.CFile;
import Services.CRect;

/**
 * Layer object. Defines a layer on the frame.
 */
public class CLayer {
	public static final int FLOPT_XCOEF = 0x0001;
	public static final int FLOPT_YCOEF = 0x0002;
	public static final int FLOPT_NOSAVEBKD = 0x0004;
	// public static final int FLOPT_WRAP_OBSOLETE=0x0008;
	public static final int FLOPT_VISIBLE = 0x0010;
	public static final int FLOPT_WRAP_HORZ = 0x0020;
	public static final int FLOPT_WRAP_VERT = 0x0040;
	public static final int FLOPT_REDRAW = 0x000010000;
	public static final int FLOPT_TOHIDE = 0x000020000;
	public static final int FLOPT_TOSHOW = 0x000040000;

	public String pName; // / Name

	// Offset
	public int x; // / Current offset
	public int y;
	public int dx; // / Offset to apply to the next refresh
	public int dy;

	public ArrayList<CBkd2> pBkd2 = null;

	// Ladders
	public ArrayList<CRect> pLadders = null;

	// Z-order max index for dynamic objects
	public int nZOrderMax;

	// Permanent data (EditFrameLayer)
	public int dwOptions; // / Options
	public float xCoef;
	public float yCoef;
	public int nBkdLOs; // / Number of backdrop objects
	public int nFirstLOIndex; // / Index of first backdrop object in LO table

	// Backup for restart
	public int backUp_dwOptions;
	public float backUp_xCoef;
	public float backUp_yCoef;
	public int backUp_nBkdLOs;
	public int backUp_nFirstLOIndex;

    // JAMES:
    public float angle = 0;
    public float scale = 1;
    public float scaleX = 1;
    public float scaleY = 1;
    public int xSpot;
    public int ySpot;
    public int xDest;
    public int yDest;

	// Collision optimization
	public CArrayList<CArrayList<Integer>> m_loZones;
	
	public CLayer() {
	}

	/**
	 * Loads the data from application file. Data include scrolling
	 * coefficients, and index of the background LO objects.
	 */
	public void load(CFile file) {
		dwOptions = file.readAInt();
		xCoef = file.readAFloat();
		yCoef = file.readAFloat();
		nBkdLOs = file.readAInt();
		nFirstLOIndex = file.readAInt();
		pName = file.readAString();

		backUp_dwOptions = dwOptions;
		backUp_xCoef = xCoef;
		backUp_yCoef = yCoef;
		backUp_nBkdLOs = nBkdLOs;
		backUp_nFirstLOIndex = nFirstLOIndex;
	}
}
