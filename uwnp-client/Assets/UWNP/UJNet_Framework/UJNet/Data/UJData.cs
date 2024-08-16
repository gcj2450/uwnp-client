using System;
using System.Collections;
using System.Text;

namespace UJNet.Data
{
    public class UJData
    {
        private SFSDataType ujType;
        private object obj;

        public UJData(SFSDataType type, Object obj)
        {
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

        public SFSDataType GetUJType()
        {
            return ujType;
        }

        public void SetUJType(SFSDataType type)
        {
            this.ujType = type;
        }
    }
}
