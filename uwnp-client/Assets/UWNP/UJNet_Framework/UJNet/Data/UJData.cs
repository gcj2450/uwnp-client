using System;
using System.Collections;
using System.Text;

namespace UJNet.Data
{
    public class UJData
    {
        private UJDataType ujType;
	    private object obj;

	    public UJData(UJDataType type, Object obj) {
            this.ujType = type;
		    this.obj = obj;
	    }

        public object GetObject()
        {
		    return obj;
	    }

        public void SetObject(object obj)
        {
            this.obj = obj;
	    }

        public UJDataType GetUJType()
        {
            return ujType;
	    }

	    public void SetUJType(UJDataType type) {
            this.ujType = type;
	    }
    }
}
