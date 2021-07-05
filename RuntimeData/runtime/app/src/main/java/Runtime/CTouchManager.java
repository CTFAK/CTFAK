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
package Runtime;

import java.util.HashSet;
import java.util.Iterator;
import java.util.Set;

import android.view.MotionEvent;

public abstract class CTouchManager
{
	public int lastTouch;

	public abstract void process(MotionEvent event);

	protected Set <ITouchAware> touchAwareSet = new HashSet <ITouchAware> ();

	protected Set <ITouchAware> pendingAdd = new HashSet <ITouchAware> ();
	protected Set <ITouchAware> pendingRemove  = new HashSet <ITouchAware> ();

	protected Set <Integer> activeTouches = new HashSet <Integer> ();

	private void procPending ()
	{
		if (pendingAdd.size () > 0)
		{
			//for(Iterator<ITouchAware> it = pendingAdd.iterator(); it.hasNext(); )
			//	touchAwareSet.add (it.next());
			for(ITouchAware it : pendingAdd )
				if(it != null)
					touchAwareSet.add (it);

			pendingAdd.clear();
		}

		if (pendingRemove.size () > 0)
		{
			//for(Iterator<ITouchAware> it = pendingRemove.iterator(); it.hasNext(); )
			//	touchAwareSet.remove(it.next());
			for(ITouchAware it : pendingRemove)
				if(it != null)
					touchAwareSet.remove(it);

			pendingRemove.clear();
		}
	}

	protected void newTouch(int id, float x, float y)
	{
		if(activeTouches.contains(id))
			return;

		activeTouches.add(id);

		lastTouch = id;

		x -= MMFRuntime.inst.viewportX;
		y -= MMFRuntime.inst.viewportY;

		x /= MMFRuntime.inst.scaleX;
		y /= MMFRuntime.inst.scaleY;

		procPending ();

		//for(Iterator<ITouchAware> it = touchAwareSet.iterator(); it.hasNext(); )
		//	it.next().newTouch(id, x, y);
		for(ITouchAware i: touchAwareSet) {
			if (i != null)
				i.newTouch(id, x, y);
		}
	}

	protected void touchMoved(int id, float x, float y)
	{
		lastTouch = id;

		x -= MMFRuntime.inst.viewportX;
		y -= MMFRuntime.inst.viewportY;

		x /= MMFRuntime.inst.scaleX;
		y /= MMFRuntime.inst.scaleY;

		procPending ();

		//for(Iterator<ITouchAware> it = touchAwareSet.iterator(); it.hasNext(); )
		//	it.next().touchMoved(id, x, y);
		for(ITouchAware i: touchAwareSet) {
			if (i != null)
				i.touchMoved(id, x, y);
		}
	}

	protected void endTouch(int id)
	{
		if (lastTouch == id)
			lastTouch = -1;

		if(!activeTouches.contains(id) && id != -1)
			return;

		activeTouches.remove(id);

		if(id == -1)
			activeTouches.clear();     	

		procPending ();

		//for(Iterator<ITouchAware> it = touchAwareSet.iterator(); it.hasNext(); )
		//	it.next().endTouch(id, flag);
		for(ITouchAware i: touchAwareSet) {
			if (i != null)
				i.endTouch(id);
		}
	}

	public void addTouchAware(ITouchAware touchAware)
	{
		Log.Log("Adding touch aware: " + touchAware.getClass().getName());

		pendingRemove.remove (touchAware);
		pendingAdd.add (touchAware);
	}

	public void removeTouchAware(ITouchAware touchAware)
	{
		Log.Log("Removing touch aware: " + touchAware.getClass().getName());
		pendingAdd.remove (touchAware);
		pendingRemove.add (touchAware);
	}
	

}
