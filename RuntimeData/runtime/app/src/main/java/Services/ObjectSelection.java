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
package Services;

import RunLoop.*;
import Application.*;
import Events.*;
import Objects.CObject;

import java.util.Iterator;
import java.util.List;

/**
 * ...
 * @author Anders Riggelsen
 */
public class ObjectSelection
{
	private CRunApp rhPtr;
	private CRun run;
	private CEventProgram eventProgram;

	private CObject[] ObjectList;
	private CObjInfo[] OiList;
	private CQualToOiList[] QualToOiList;

	public ObjectSelection(CRunApp runApp)
	{
		rhPtr = runApp;
		run = rhPtr.run;
		eventProgram = rhPtr.frame.evtProg;
		ObjectList = run.rhObjectList;				//get a pointer to the mmf object list
		OiList = run.rhOiList;						//get a pointer to the mmf object info list
		QualToOiList = eventProgram.qualToOiList;	//get a pointer to the mmf qualifier to Oi list
	}

	//Selects *all* objects of the given object-type
	public void selectAll(int OiList)
	{
		CObjInfo pObjectInfo =  this.OiList[OiList]; //GetOILFromOI(Oi);
		if(pObjectInfo == null)
			return;
		pObjectInfo.oilNumOfSelected = pObjectInfo.oilNObjects;
		pObjectInfo.oilListSelected = pObjectInfo.oilObject;
		pObjectInfo.oilEventCount = eventProgram.rh2EventCount;

		int i = pObjectInfo.oilObject;
		while((i & 0x80000000)== 0)
		{
			CObject pObject = run.rhObjectList[i];
			if(pObject == null)
				continue;
			pObject.hoNextSelected = pObject.hoNumNext;
			i = pObject.hoNumNext;
		}
		return;
	}

	//Resets all objects of the given object-type
	public void selectNone(int OiList)
	{
		CObjInfo pObjectInfo = this.OiList[OiList]; //GetOILFromOI(Oi);
		if(pObjectInfo == null)
			return;
		pObjectInfo.oilNumOfSelected = 0;
		pObjectInfo.oilListSelected = -1;
		pObjectInfo.oilEventCount = eventProgram.rh2EventCount;
	}

	//Resets the SOL and inserts only one given object
	public void selectOneObject(CObject object)
	{
		CObjInfo pObjectInfo = object.hoOiList;
		if(pObjectInfo == null)
			return;
		pObjectInfo.oilNumOfSelected = 1;
		pObjectInfo.oilEventCount = eventProgram.rh2EventCount;
		pObjectInfo.oilListSelected = object.hoNumber;
		CObject cObj = ObjectList[object.hoNumber];
		if(cObj != null)
			cObj.hoNextSelected = -1;
	}

	//Resets the SOL and inserts the given list of objects
	public void selectObjects(int OiList, List<CObject> objects)
	{
		CObjInfo pObjectInfo = this.OiList[OiList]; //GetOILFromOI(Oi);

		if(pObjectInfo == null)
			return;

		pObjectInfo.oilNumOfSelected = objects.size();
		pObjectInfo.oilEventCount = eventProgram.rh2EventCount;

		Iterator<CObject> i = objects.iterator();

		if (!i.hasNext())
			return;

		int prevNumber = i.next().hoNumber;
		pObjectInfo.oilListSelected = (short) prevNumber;

		while(i.hasNext())
		{
			int currentNumber = i.next().hoNumber;
			if(ObjectList[prevNumber] != null)
			{
				ObjectList[prevNumber].hoNextSelected = (short) currentNumber;
				prevNumber = currentNumber;
			}
		}
		if(ObjectList[prevNumber] != null)
			ObjectList[prevNumber].hoNextSelected = -1;
	}

	public static abstract class Filter
	{
		public abstract boolean filter (Object rdPtr, CObject pObject);
	}

	//Run a custom filter on the SOL (via function callback)
	public boolean filterObjects(Object rdPtr, int OiList, boolean negate, Filter filter)
	{
		if ((OiList & 0x8000) != 0){
			return ((filterQualifierObjects(rdPtr, OiList & 0x7FFF, negate, filter) ? 1 : 0) ^ (negate ? 1 : 0)) != 0;
		}
		return ((filterNonQualifierObjects(rdPtr, OiList & 0x7FFF, negate, filter) ? 1 : 0) ^ (negate ? 1 : 0)) != 0;
	}

	//Filter qualifier objects
	public boolean filterQualifierObjects(Object rdPtr, int OiList, boolean negate, Filter filter)
	{
		CQualToOiList CurrentQualToOi = QualToOiList[OiList];

		boolean hasSelected = false;
		int i = 0;
		int length = CurrentQualToOi.qoiList.length;
		while( i < length)
		{
			int CurrentOi = CurrentQualToOi.qoiList[i+1];

			hasSelected = (((hasSelected ? 1 : 0) |
					(filterNonQualifierObjects(rdPtr, CurrentOi, negate, filter) ? 1 : 0))) != 0;

			i+=2;
		}
		return hasSelected;
	}

	//Filter normal objects
	public boolean filterNonQualifierObjects(Object rdPtr, int OiList, boolean negate, Filter filter)
	{
		CObjInfo pObjectInfo = this.OiList[OiList]; //GetOILFromOI(Oi);
		if(pObjectInfo == null)
			return false;
		boolean hasSelected = false;
		if (pObjectInfo.oilEventCount != eventProgram.rh2EventCount){
			selectAll(OiList);	//The SOL is invalid, must reset.
		}

		//If SOL is empty
		if(pObjectInfo.oilNumOfSelected <= 0){
			return false;
		}

		int firstSelected = -1;
		int count = 0;
		int current = pObjectInfo.oilListSelected;
		CObject previous = null;

		while((current & 0x80000000)==0)
		{
			CObject pObject = ObjectList[current];
			if(pObject == null)
				continue;
			boolean filterResult = filter.filter(rdPtr, pObject);
			boolean useObject = ((filterResult ? 1 : 0) ^ (negate ? 1 : 0)) != 0;
			hasSelected = ((hasSelected ? 1 : 0) | (useObject ? 1 : 0)) != 0;

			if(useObject)
			{
				if(firstSelected == -1){
					firstSelected = current;
				}

				if(previous != null){
					previous.hoNextSelected = (short) current;
				}

				previous = pObject;
				count++;
			}
			current = pObject.hoNextSelected;
		}
		if(previous != null){
			previous.hoNextSelected = -1;
		}

		pObjectInfo.oilListSelected = (short) firstSelected;
		pObjectInfo.oilNumOfSelected = count;

		return hasSelected;
	}


	//Return the number of selected objects for the given object-type
	public int getNumberOfSelected(int OiList)
	{
		if((OiList & 0x8000) != 0)
		{
			OiList &= 0x7FFF;	//Mask out the qualifier part
			int numberSelected = 0;

			CQualToOiList CurrentQualToOi = QualToOiList[OiList];

			int i = 0;
			int length = CurrentQualToOi.qoiList.length;
			while( i < length)
			{
				CObjInfo CurrentOi = GetOILFromOI(CurrentQualToOi.qoiList[i+1]);
				if(CurrentOi == null)
					return 0;
				numberSelected += CurrentOi.oilNumOfSelected;
				i+=2;
			}
			return numberSelected;
		}
		else
		{
			CObjInfo pObjectInfo = this.OiList[OiList]; //GetOILFromOI(Oi);
			if(pObjectInfo == null)
				return 0;
			return pObjectInfo.oilNumOfSelected;
		}
	}

	public boolean objectIsOfType(CObject obj, int OiList)
	{
		if((OiList & 0x8000) != 0)
		{
			OiList &= 0x7FFF;	//Mask out the qualifier part

			CQualToOiList CurrentQualToOi = QualToOiList[OiList];

			int i = 0;
			while( i < CurrentQualToOi.qoiList.length)
			{
				CObjInfo CurrentOi = run.rhOiList[CurrentQualToOi.qoiList[i+1]]; //GetOILFromOI(Oi);
				if (CurrentOi == null)
					return false;
				if(CurrentOi.oilOi == obj.hoOi)
					return true;
				i+=2;
			}
			return false;
		}
		return (obj.hoOi == run.rhOiList[OiList].oilOi);
	}

	//Returns the object-info structure from a given object-type
	CObjInfo GetOILFromOI(int Oi)
	{
		for(int i=0; i < run.rhMaxOI; ++i)
		{
			CObjInfo oil = OiList[i];
			if(oil.oilOi == Oi)
				return oil;
		}
		return null;
	}
}
