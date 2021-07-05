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

import android.view.MotionEvent;

public class CTouchManagerAPI5 extends CTouchManager
{
	int nSizeCount=0;
	public CTouchManagerAPI5()
	{
	}

	@Override
	public void process(MotionEvent event)
	{
		int pointerIndex = event.getActionIndex();
		int pointer = event.getPointerId(pointerIndex);
		int actionCode = event.getActionMasked();

		switch(actionCode)
		{
			case MotionEvent.ACTION_DOWN:
			case MotionEvent.ACTION_POINTER_DOWN:
					this.newTouch(pointer, event.getX(pointerIndex), event.getY(pointerIndex));
				break;
	
			case MotionEvent.ACTION_MOVE:
				
				for (int size = event.getPointerCount(), i = 0; i < size; i++)
				{
					this.touchMoved(event.getPointerId(i), event.getX(i), event.getY(i));
				}
	
				break;

			case MotionEvent.ACTION_UP:
			case MotionEvent.ACTION_POINTER_UP:
				this.endTouch(pointer);
				break;
			case MotionEvent.ACTION_CANCEL:
				this.endTouch(-1);
				break;
		}
	}
}
