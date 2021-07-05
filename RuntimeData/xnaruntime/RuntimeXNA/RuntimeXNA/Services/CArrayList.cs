//----------------------------------------------------------------------------------
//
// CARRAYLIST : classe extensible de stockage
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuntimeXNA.Services
{
    public class CArrayList
    {
        const int GROWTH_STEP=5;
        object[] array=null;
        int numberOfEntries=0;

        void getArray(int max)
        {
            if (array==null)
            {            
                array=new object[max+GROWTH_STEP];
            }
            else if (max>=array.Length)
            {
                object[] tempArray=new Object[max+GROWTH_STEP];
                int n;
                for (n=0; n<array.Length; n++)
                {
                    tempArray[n]=array[n];
                }
                array=tempArray;
            }
        }
        public void ensureCapacity(int max)
        {
            getArray(max);
        }
        public bool isEmpty()
        {
            return numberOfEntries == 0;
        }
        public void add(object o)
        {
            getArray(numberOfEntries);
            array[numberOfEntries++] = o;
        }
        public void add(int index, object o)
        {
            getArray(numberOfEntries);
            int n;
            for (n = numberOfEntries; n > index; n--)
            {
                array[n] = array[n - 1];
            }
            array[index] = o;
            numberOfEntries++;
        }
        public object get(int index)
        {
            if (array != null)
            {
                if (index < array.Length)
                {
                    return array[index];
                }
            }
            return null;
        }
        public void set(int index, object o)
        {
            if (array != null)
            {
                if (index < array.Length)
                {
                    array[index] = o;
                }
            }
        }
	    public void insert(int index, object o)
	    {
	        getArray(numberOfEntries);
	        int n;
	        for (n=numberOfEntries; n>index; n--)
	        {
	            array[n]=array[n-1];
	        }
	        array[index]=o;
	        numberOfEntries++;
	    }
        public void swap(int index1, int index2)
        {
            if (array != null)
            {
                object temp = array[index1];
                array[index1] = array[index2];
                array[index2] = temp;
            }
        }
        public void swap(object o1, object o2)
        {
            if (array != null)
            {
                int i1=indexOf(o1);
                int i2=indexOf(o2);
                if (i1>=0 && i2>=0)
                {
                    swap(i1, i2);
                }
            }
        }
        public void remove(int index)
        {
            if (array != null)
            {
                if (index < array.Length && numberOfEntries > 0)
                {
                    int n;
                    for (n = index; n < numberOfEntries - 1; n++)
                    {
                        array[n] = array[n + 1];
                    }
                    numberOfEntries--;
                    array[numberOfEntries] = null;
                }
            }
        }
        public int indexOf(object o)
        {
            int n;
            for (n = 0; n < numberOfEntries; n++)
            {
                if (array[n] == o)
                {
                    return n;
                }
            }
            return -1;
        }
        public void remove(object o)
        {
            int n = indexOf(o);
            if (n >= 0)
            {
                remove(n);
            }
        }
        public int size()
        {
            return numberOfEntries;
        }
        public void clear()
        {
            numberOfEntries = 0;
        }
    }
}
