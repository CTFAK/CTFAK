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
// CVALUE : classe de calcul et de stockage de valeurs
//
//----------------------------------------------------------------------------------
package Expressions;


public class CValue 
{
    public static final byte TYPE_INT=0;
    public static final byte TYPE_DOUBLE=1;
    public static final byte TYPE_STRING=2;

    public byte type;
    public int intValue;
    public double doubleValue;
    public String stringValue;
   
    public CValue() 
    {
		type=TYPE_INT;
		intValue=0;
    }
    public CValue(CValue value) 
    {
		switch(value.type)
		{
		    case 0:
				intValue=value.intValue;
				break;
		    case 1:
				doubleValue=value.doubleValue;
				break;
		    case 2:
				stringValue=new String(value.stringValue);
				break;
		}
		type=value.type;
    }
    public CValue(int i) 
    {
		type=TYPE_INT;
		intValue=i;
    }
    public CValue(double d) 
    {
		type=TYPE_DOUBLE;
		doubleValue=d;
    }
    public CValue(String s) 
    {
		type=TYPE_STRING;
		stringValue=s;
    }
    public byte getType()
    {
    	return type;
    }
    public int getInt()
    {
		switch(type)
		{
		    case 0:
				return intValue;
		    case 1:
				return (int)doubleValue;
		}
		return 0;
    }
    public double getDouble()
    {
		switch(type)
		{
		    case 0:
				return (double)intValue;
		    case 1:
				return doubleValue;
		}
		return 0;
    }
    public String getString()
    {
		if (type==TYPE_STRING)
		    return stringValue;
		return "";
    }
    public void forceInt(int value)
    {
		type=TYPE_INT;
		intValue=value;
    }
    public void forceDouble(double value)
    {
		type=TYPE_DOUBLE;
		doubleValue=value;
    }
    public void forceString(String value)
    {
		type=TYPE_STRING;
		stringValue=new String(value);
    }
    public void forceStringFromInt(int value)
    {
		type=TYPE_STRING;
		stringValue=Integer.toString(value);
    }
    public void forceStringFromDouble(double value)
    {
		type=TYPE_STRING;
		stringValue=Double.toString(value);
    }
    public void forceValue(CValue value)
    {
		type=value.type;
		switch (type)
		{
		    case 0:
				intValue=value.intValue;
				break;
		    case 1:
				doubleValue=value.doubleValue;
				break;
		    case 2:
				stringValue=new String(value.stringValue);
				break;
		}
    }
    public void setValue(CValue value)
    {
		switch (type)
		{
		    case 0:
				intValue=value.getInt();
				break;
		    case 1:
				doubleValue=value.getDouble();
				break;
		    case 2:
				stringValue=new String(value.stringValue);
				break;
		}
    }
    public void convertToDouble()
    {
		if (type==TYPE_INT)
		{
		    doubleValue=(double)intValue;
		    type=TYPE_DOUBLE;
		}
    }
    public void convertToInt()
    {
		if (type==TYPE_DOUBLE)
		{
		    intValue=(int)doubleValue;
		    type=TYPE_INT;
		}
    }
    public void add(CValue value)
    {
		switch (type)
		{
			case 0:	// TYPE_INT:
				if ( value.type == TYPE_INT )
					intValue+=value.intValue;
				else if ( value.type == TYPE_DOUBLE )
				{
					doubleValue=(double)intValue;
					type=TYPE_DOUBLE;
					doubleValue+=value.doubleValue;
				}
				break;
			case 1:	// TYPE_DOUBLE:
				if ( value.type == TYPE_DOUBLE )
					doubleValue+=value.doubleValue;
				else if ( value.type == TYPE_INT )
					doubleValue+=(double)value.intValue;
				break;
			case TYPE_STRING:
				stringValue=new String(stringValue+value.stringValue);
				break;
		}
    }
	public void add(int value)
	{
		switch (type)
		{
		case 0:	// TYPE_INT:
			intValue += value;
			break;
		case 1:	// TYPE_DOUBLE:
			doubleValue += (double)value;
			break;
		}
	}
	public void add(double value)
	{
		switch (type)
		{
		case 0:	// TYPE_INT:
			doubleValue = (double)intValue;
			type = TYPE_DOUBLE;
			doubleValue += value;
			break;
		case 1:	// TYPE_DOUBLE:
			doubleValue += value;
			break;
		}
	}
    public void sub(CValue value)
    {
		switch (type)
		{
			case 0:	// TYPE_INT:
				if ( value.type == TYPE_INT )
					intValue-=value.intValue;
				else if ( value.type == TYPE_DOUBLE )
				{
					doubleValue=(double)intValue;
					type=TYPE_DOUBLE;
					doubleValue-=value.doubleValue;
				}
				break;
			case 1:	// TYPE_DOUBLE:
				if ( value.type == TYPE_DOUBLE )
					doubleValue-=value.doubleValue;
				else if ( value.type == TYPE_INT )
					doubleValue-=(double)value.intValue;
				break;
		}
    }
	public void sub(int value)
	{
		switch (type)
		{
		case 0:	// TYPE_INT:
			intValue -= value;
			break;
		case 1:	// TYPE_DOUBLE:
			doubleValue -= (double)value;
			break;
		}
	}
	public void sub(double value)
	{
		switch (type)
		{
		case 0:	// TYPE_INT:
			doubleValue = (double)intValue;
			type = TYPE_DOUBLE;
			doubleValue -= value;
			break;
		case 1:	// TYPE_DOUBLE:
			doubleValue -= value;
			break;
		}
	}
    public void negate()
    {
		switch (type)
		{
		    case 0:
				intValue=-intValue;
				break;
		    case 1:
				doubleValue=-doubleValue;
				break;
		}
    }
    public void mul(CValue value)
    {
		switch (type)
		{
			case 0:	// TYPE_INT:
				if ( value.type == TYPE_INT )
					intValue*=value.intValue;
				else if ( value.type == TYPE_DOUBLE )
				{
					doubleValue=(double)intValue;
					type=TYPE_DOUBLE;
					doubleValue*=value.doubleValue;
				}
				break;
			case 1:	// TYPE_DOUBLE:
				if ( value.type == TYPE_DOUBLE )
					doubleValue*=value.doubleValue;
				else if ( value.type == TYPE_INT )
					doubleValue*=(double)value.intValue;
				break;
		}
    }
    public void div(CValue value)
    {
		switch (type)
		{
			case 0:	// TYPE_INT:
				if ( value.type == TYPE_INT )
				{
					if ( value.intValue != 0 )
						intValue/=value.intValue;
					else
						intValue=0;
				}
				else if ( value.type == TYPE_DOUBLE )
				{
					doubleValue=(double)intValue;
					type=TYPE_DOUBLE;
					if ( value.doubleValue != 0.0 )
						doubleValue/=value.doubleValue;
					else
						doubleValue=0.0;
				}
				break;
			case 1:	// TYPE_DOUBLE:
				if ( value.type == TYPE_DOUBLE )
				{
					if ( value.doubleValue != 0.0 )
						doubleValue/=value.doubleValue;
					else
						doubleValue=0.0;
				}
				else if ( value.type == TYPE_INT )
				{
					if ( value.intValue != 0 )
						doubleValue/=(double)value.intValue;
					else
						doubleValue=0.0;
				}
				break;
		}
    }
    public void pow(CValue value)
    {
		switch (type)
		{
			case 0:	// TYPE_INT:
				if ( value.type == TYPE_INT )
					intValue = (int) Math.pow(intValue, value.intValue);
				else if ( value.type == TYPE_DOUBLE )
				{
					doubleValue=(double)intValue;
					type=TYPE_DOUBLE;
					doubleValue = Math.pow(doubleValue, value.doubleValue);
				}
				break;
			case 1:	// TYPE_DOUBLE:
				if ( value.type == TYPE_DOUBLE )
					doubleValue = Math.pow(doubleValue, value.doubleValue);
				else if ( value.type == TYPE_INT )
					doubleValue = Math.pow(doubleValue, (double)value.intValue);
				break;
		}
    }
    public void mod(CValue value)
    {
		switch (type)
		{
			case 0:	// TYPE_INT:
				if ( value.type == TYPE_INT )
				{
					if ( value.intValue != 0 )
						intValue%=value.intValue;
					else
						intValue=0;
				}
				else if ( value.type == TYPE_DOUBLE )
				{
					doubleValue=(double)intValue;
					type=TYPE_DOUBLE;
					if ( value.doubleValue != 0.0 )
						doubleValue%=value.doubleValue;
					else
						doubleValue=0.0;
				}
				break;
			case 1:	// TYPE_DOUBLE:
				if ( value.type == TYPE_DOUBLE )
				{
					if ( value.doubleValue != 0.0 )
						doubleValue%=value.doubleValue;
					else
						doubleValue=0.0;
				}
				else if ( value.type == TYPE_INT )
				{
					if ( value.intValue != 0 )
						doubleValue%=(double)value.intValue;
					else
						doubleValue=0.0;
				}
				break;
		}
    }
    public void andLog(CValue value)
    {
		switch (type)
		{
			case 1:		// TYPE_DOUBLE:
				intValue = (int)doubleValue;
				type = TYPE_INT;
			case 0:		// TYPE_INT:
				if ( value.type == TYPE_INT )
					intValue &= value.intValue;
				else if ( value.type == TYPE_DOUBLE )
					intValue &= (int)value.doubleValue;
				break;
		}
    }
    public void orLog(CValue value)
    {
		switch (type)
		{
			case 1:		// TYPE_DOUBLE:
				intValue = (int)doubleValue;
				type = TYPE_INT;
			case 0:		// TYPE_INT:
				if ( value.type == TYPE_INT )
					intValue |= value.intValue;
				else if ( value.type == TYPE_DOUBLE )
					intValue |= (int)value.doubleValue;
				break;
		}
    }
    public void xorLog(CValue value)
    {
		switch (type)
		{
			case 1:		// TYPE_DOUBLE:
				intValue = (int)doubleValue;
				type = TYPE_INT;
			case 0:		// TYPE_INT:
				if ( value.type == TYPE_INT )
					intValue ^= value.intValue;
				else if ( value.type == TYPE_DOUBLE )
					intValue ^= (int)value.doubleValue;
				break;
		}
    }
    public boolean equal(CValue value)
    {
		switch (type)
		{
			case 0:		// TYPE_INT:
				if ( value.type == TYPE_INT )
					return (intValue==value.intValue);
				else if ( value.type == TYPE_DOUBLE )
					return ((double)intValue==value.doubleValue);
				break;
			case 1:		// TYPE_DOUBLE:
				if ( value.type == TYPE_DOUBLE )
					return (doubleValue==value.doubleValue);
				else if ( value.type == TYPE_INT )
					return (doubleValue==(double)value.intValue);
				break;
			case 2:		// TYPE_STRING:
				if ( value.type == TYPE_STRING )
					return stringValue.equals(value.stringValue);
				break;
		}
		return false;
    }
	public boolean equal(int value)
	{
		switch (type) {
		case 0:		// TYPE_INT:
			return (intValue==value);
		case 1:		// TYPE_DOUBLE:
			return (doubleValue==(double)value);
		}
		return false;
	}
	public boolean equal(double value)
	{
		switch (type) {
		case 0:		// TYPE_INT:
			return ((double)intValue==value);
		case 1:		// TYPE_DOUBLE:
			return (doubleValue==value);
		}
		return false;
	}
    public boolean greater(CValue value)
    {
		switch (type)
		{
			case 0:		// TYPE_INT:
				if ( value.type == TYPE_INT )
					return (intValue>=value.intValue);
				else if ( value.type == TYPE_DOUBLE )
					return ((double)intValue>=value.doubleValue);
				break;
			case 1:		// TYPE_DOUBLE:
				if ( value.type == TYPE_DOUBLE )
					return (doubleValue>=value.doubleValue);
				else if ( value.type == TYPE_INT )
					return (doubleValue>=(double)value.intValue);
				break;
			case 2:		// TYPE_STRING:
				if ( value.type == TYPE_STRING )
					return stringValue.compareTo(value.stringValue)>=0;
				break;
		}
		return false;
    }
	public boolean greater(int value)
	{
		switch (type) {
		case 0:		// TYPE_INT:
			return (intValue>=value);
		case 1:		// TYPE_DOUBLE:
			return (doubleValue>=(double)value);
		}
		return false;
	}
	public boolean greater(double value)
	{
		switch (type) {
		case 0:		// TYPE_INT:
			return ((double)intValue>=value);
		case 1:		// TYPE_DOUBLE:
			return (doubleValue>=value);
		}
		return false;
	}
    public boolean lower(CValue value)
    {
		switch (type)
		{
			case 0:		// TYPE_INT:
				if ( value.type == TYPE_INT )
					return (intValue<=value.intValue);
				else if ( value.type == TYPE_DOUBLE )
					return ((double)intValue<=value.doubleValue);
				break;
			case 1:		// TYPE_DOUBLE:
				if ( value.type == TYPE_DOUBLE )
					return (doubleValue<=value.doubleValue);
				else if ( value.type == TYPE_INT )
					return (doubleValue<=(double)value.intValue);
				break;
			case 2:		// TYPE_STRING:
				if ( value.type == TYPE_STRING )
					return stringValue.compareTo(value.stringValue)<=0;
				break;
		}
		return false;
    }
	public boolean lower(int value)
	{
		switch (type) {
		case 0:		// TYPE_INT:
			return (intValue<=value);
		case 1:		// TYPE_DOUBLE:
			return (doubleValue<=(double)value);
		}
		return false;
	}
	public boolean lower(double value)
	{
		switch (type) {
		case 0:		// TYPE_INT:
			return ((double)intValue<=value);
		case 1:		// TYPE_DOUBLE:
			return (doubleValue<=value);
		}
		return false;
	}
    public boolean greaterThan(CValue value)
    {
		switch (type)
		{
			case 0:		// TYPE_INT:
				if ( value.type == TYPE_INT )
					return (intValue>value.intValue);
				else if ( value.type == TYPE_DOUBLE )
					return ((double)intValue>value.doubleValue);
				break;
			case 1:		// TYPE_DOUBLE:
				if ( value.type == TYPE_DOUBLE )
					return (doubleValue>value.doubleValue);
				else if ( value.type == TYPE_INT )
					return (doubleValue>(double)value.intValue);
				break;
			case 2:		// TYPE_STRING:
				if ( value.type == TYPE_STRING )
					return stringValue.compareTo(value.stringValue)>0;
				break;
		}
		return false;
    }
	public boolean greaterThan(int value)
	{
		switch (type) {
		case 0:		// TYPE_INT:
			return (intValue>value);
		case 1:		// TYPE_DOUBLE:
			return (doubleValue>(double)value);
		}
		return false;
	}
	public boolean greaterThan(double value)
	{
		switch (type) {
		case 0:		// TYPE_INT:
			return ((double)intValue>value);
		case 1:		// TYPE_DOUBLE:
			return (doubleValue>value);
		}
		return false;
	}
    public boolean lowerThan(CValue value)
    {
		switch (type)
		{
			case 0:		// TYPE_INT:
				if ( value.type == TYPE_INT )
					return (intValue<value.intValue);
				else if ( value.type == TYPE_DOUBLE )
					return ((double)intValue<value.doubleValue);
				break;
			case 1:		// TYPE_DOUBLE:
				if ( value.type == TYPE_DOUBLE )
					return (doubleValue<value.doubleValue);
				else if ( value.type == TYPE_INT )
					return (doubleValue<(double)value.intValue);
				break;
			case 2:		// TYPE_STRING:
				if ( value.type == TYPE_STRING )
					return stringValue.compareTo(value.stringValue)<0;
				break;
		}
		return false;
    }
	public boolean lowerThan(int value)
	{
		switch (type) {
		case 0:		// TYPE_INT:
			return (intValue<value);
		case 1:		// TYPE_DOUBLE:
			return (doubleValue<(double)value);
		}
		return false;
	}
	public boolean lowerThan(double value)
	{
		switch (type) {
		case 0:		// TYPE_INT:
			return ((double)intValue<value);
		case 1:		// TYPE_DOUBLE:
			return (doubleValue<value);
		}
		return false;
	}
    public boolean notEqual(CValue value)
    {
		switch (type)
		{
			case 0:		// TYPE_INT:
				if ( value.type == TYPE_INT )
					return (intValue!=value.intValue);
				else if ( value.type == TYPE_DOUBLE )
					return ((double)intValue!=value.doubleValue);
				break;
			case 1:		// TYPE_DOUBLE:
				if ( value.type == TYPE_DOUBLE )
					return (doubleValue!=value.doubleValue);
				else if ( value.type == TYPE_INT )
					return (doubleValue!=(double)value.intValue);
				break;
			case 2:		// TYPE_STRING:
				if ( value.type == TYPE_STRING )
					return stringValue.compareTo(value.stringValue)!=0;
				break;
		}
		return false;
    }
	public boolean notEqual(int value)
	{
		switch (type) {
		case 0:		// TYPE_INT:
			return (intValue!=value);
		case 1:		// TYPE_DOUBLE:
			return (doubleValue!=(double)value);
		}
		return false;
	}
	public boolean notEqual(double value)
	{
		switch (type) {
		case 0:		// TYPE_INT:
			return ((double)intValue!=value);
		case 1:		// TYPE_DOUBLE:
			return (doubleValue!=value);
		}
		return false;
	}
}
