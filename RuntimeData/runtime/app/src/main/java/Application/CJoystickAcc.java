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
package Application;

import Runtime.MMFRuntime;
import Services.CServices;
import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;

public class CJoystickAcc
{
	public CRunApp app;

	public int joystick = 0;

    private SensorManager manager = null;
    private double[] direct = new double[3];
    private double[] filtered = new double[3];
    private double[] instant = new double[3];
    
    private static double DEADRANGE = 0.1;
    private double x_axis;
    private double y_axis;
    
    private float nominalG = 9.8f;

    private SensorEventListener accelerometerListener = new SensorEventListener()
    {
        @Override
		public void onSensorChanged(SensorEvent e)
        {
            CServices.filterAccelerometer(e, direct, filtered, instant, nominalG);
            
            x_axis = filtered[0];
            y_axis = filtered[1];
            
            joystick = 0;

            if(x_axis < -DEADRANGE)
                joystick |= 0x04;

            if(x_axis >  DEADRANGE)
                joystick |= 0x08;

            if(y_axis < -DEADRANGE)
                joystick |= 0x02;

            if(y_axis >  DEADRANGE)
                joystick |= 0x01;
        }

        @Override
		public void onAccuracyChanged(Sensor sensor, int accuracy)
        {
        }
    };

    public CJoystickAcc(CRunApp app)
    {
        manager = (SensorManager) MMFRuntime.inst.getSystemService(Context.SENSOR_SERVICE);
        if(manager != null) {
	        manager.registerListener(accelerometerListener,
	                manager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER),
	                SensorManager.SENSOR_DELAY_GAME);
	        
        }
    }

	public void deactivate()
    {
        if (accelerometerListener == null)
            return;

        manager.unregisterListener(accelerometerListener);
        accelerometerListener = null;
    }
	
}
