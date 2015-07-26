using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
/*Author:Supriya Kulkarni
  ClassFileName:Attribute.cs
  Purpose:This class is used to store attribute data(name,possible values)
          The attribute class objects are also the building blocks of the decision tree*/
namespace MachineLearningID3
{
    public class Attribute
    {
       public ArrayList aValues;
       public string aName;
       public object aLabel;
       public int postives, negatives;
        public Attribute(string name, string[] values)
        {
            aName = name;
            aValues = new ArrayList(values);
            aValues.Sort();
            postives = negatives;
            
        }

        public Attribute(object Label)
        {
            aLabel = Label;
            aName = string.Empty;
            aValues = null;
        }

   
        public string AttributeName
        {
            get
            {
                return aName;
            }
        }

        public string[] values
        {
            get
            {
                if (aValues != null)
                    return (string[])aValues.ToArray(typeof(string));
                else
                    return null;
            }
        }

       
        public bool isValidValue(string value)
        {
            return indexValue(value) >= 0;
        }

        public int indexValue(string value)
        {
            if (aValues != null)
                return aValues.BinarySearch(value);
            else
                return -1;
        }

        public override string ToString()
        {
            if (aName != string.Empty)
            {
                return aName;
            }
            else
            {
                return aLabel.ToString();
            }
        }
    }

}
