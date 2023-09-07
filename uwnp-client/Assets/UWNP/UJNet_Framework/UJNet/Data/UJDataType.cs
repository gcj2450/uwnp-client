using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UJNet.Data
{
    public class UJDataType
    {
        public static readonly UJDataType BOOL = new UJDataType("BOOL", 1);
        public static readonly UJDataType BYTE = new UJDataType("BYTE", 2);
	    public static readonly UJDataType SHORT = new UJDataType("SHORT", 3);
	    public static readonly UJDataType INT = new UJDataType("INT", 4);
	    public static readonly UJDataType LONG = new UJDataType("LONG", 5);
	    public static readonly UJDataType FLOAT = new UJDataType("FLOAT", 6);
	    public static readonly UJDataType DOUBLE = new UJDataType("DOUBLE", 7);
	    public static readonly UJDataType UTF_STRING = new UJDataType("UTF_STRING", 8);
        public static readonly UJDataType UJ_ARRAY = new UJDataType("UJ_ARRAY", 9);
        public static readonly UJDataType UJ_OBJECT = new UJDataType("UJ_OBJECT", 10);

	    private string name;
	    private int typeID;
    	
	    public UJDataType(string name, int typeID)
        {
		    this.name = name;
		    this.typeID = typeID;
	    }

        public string GetName()
        {
            return this.name;
        }

        public int GetTypeID()
        {
            return this.typeID;
        }

        public static IEnumerable<UJDataType> Values
        {
            get
            {
                yield return BOOL;
                yield return BYTE;
                yield return SHORT;
                yield return INT;
                yield return LONG;
                yield return FLOAT;
                yield return DOUBLE;
                yield return UTF_STRING;
                yield return UJ_ARRAY;
                yield return UJ_OBJECT;
            }
        }
    }
}
