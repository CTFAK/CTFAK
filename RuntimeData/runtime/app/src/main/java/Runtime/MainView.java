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

import Application.CRunApp;
import OpenGL.GLRenderer;
import android.content.Context;
import android.view.View;
import android.view.ViewGroup;

public class MainView extends ViewGroup
{
    public static int currentWidth;
    public static int currentHeight;

    public MainView(Context context)
    {
        super(context);
    }

    @Override
	protected void onMeasure(int widthMeasureSpec, int heightMeasureSpec)
    {
        int maxHeight = 0;
        int maxWidth = 0;

        measureChildren(widthMeasureSpec, heightMeasureSpec);

        int count = getChildCount();

        for (int i = 0; i < count; i++)
        {
            View child = getChildAt(i);

            if (child.getVisibility() == GONE)
                continue;

            MainView.LayoutParams lp =
                    (MainView.LayoutParams) child.getLayoutParams();

            maxWidth = Math.max(maxWidth, lp.x + child.getMeasuredWidth());
            maxHeight = Math.max(maxHeight, lp.y + child.getMeasuredHeight());
        }

        maxHeight = Math.max(maxHeight, getSuggestedMinimumHeight());
        maxWidth = Math.max(maxWidth, getSuggestedMinimumWidth());

        setMeasuredDimension
        (
                MeasureSpec.getSize (widthMeasureSpec),
                MeasureSpec.getSize (heightMeasureSpec)
        );


        Log.Log ("--- onMeasure for " + this + " (currentWidth " + currentWidth + ", currentHeight " + currentHeight + ")");

    }

    @Override
    public void onSizeChanged (int w, int h, int oldw, int oldh)
    {
    	Log.Log ("onSizeChanged (w " + w + ", h " + h
    			+ ", old values: currentWidth " + currentWidth + ", currentHeight " + currentHeight + ")");

    	currentWidth = w;
    	currentHeight = h;

    	MMFRuntime.inst.updateViewport();


    	if(MMFRuntime.inst.app.joystick != null
    			&& MMFRuntime.inst.app.appRunningState == CRunApp.SL_FRAMELOOP)
    	{
    		MMFRuntime.inst.app.joystick.updatePosition();
    	}

    }

    @Override
	protected ViewGroup.LayoutParams generateDefaultLayoutParams()
    {
        return new LayoutParams(android.view.ViewGroup.LayoutParams.WRAP_CONTENT,
                android.view.ViewGroup.LayoutParams.WRAP_CONTENT, 0, 0);
    }

    @Override
	protected void onLayout(boolean changed, int l, int t, int r, int b)
    {
        int count = getChildCount();

        for (int i = 0; i < count; ++ i)
        {
            View child = getChildAt(i);



            if (child.getVisibility() == GONE)
                continue;

            MainView.LayoutParams layoutParams =
                    (MainView.LayoutParams) child.getLayoutParams();

            child.layout(layoutParams.x, layoutParams.y,
                        layoutParams.x + child.getMeasuredWidth(),
                        layoutParams.y + child.getMeasuredHeight());
        }
    }

    @Override
	protected boolean checkLayoutParams(ViewGroup.LayoutParams p)
    {
        return p instanceof MainView.LayoutParams;
    }

    @Override
	protected ViewGroup.LayoutParams generateLayoutParams(ViewGroup.LayoutParams p)
    {
        return new LayoutParams(p);
    }

    public static class LayoutParams extends ViewGroup.LayoutParams
    {
        public int x;
        public int y;

        public LayoutParams(int width, int height, int x, int y)
        {
            super (width, height);

            this.x = x;
            this.y = y;
        }

        public LayoutParams(ViewGroup.LayoutParams source)
        {
            super(source);
        }
    }
}
