using System;
using System.Collections;
using System.Text;

namespace UJNet.Data
{
    public class UJArray
    {
        private ArrayList dataHolder;

        public UJArray()
        {
            dataHolder = new ArrayList();
        }

        public static UJArray NewInstance()
        {
            return new UJArray();
        }

        public void AddBool(bool value)
        {
            AddObj(value, UJDataType.BOOL); 
        }

        public void AddByte(byte value)
        {
            AddObj(value, UJDataType.BYTE);
        }

        public void AddShort(short value)
        {
            AddObj(value, UJDataType.SHORT);
        }

        public void AddInt(int value)
        {
            AddObj(value, UJDataType.INT);
        }

        public void AddLong(long value)
        {
            AddObj(value, UJDataType.LONG);
        }

        public void AddFloat(float value)
        {
            AddObj(value, UJDataType.FLOAT);
        }

        public void AddDouble(double value)
        {
            AddObj(value, UJDataType.DOUBLE);
        }

        public void AddUtfString(string value)
        {
            AddObj(value, UJDataType.UTF_STRING);
        }

        public void AddUJArray(UJArray array)
        {
            AddObj(array, UJDataType.UJ_ARRAY);
        }

        public void AddUJObject(UJObject obj)
        {
            AddObj(obj, UJDataType.UJ_OBJECT);
        }

        public void Add(UJData data)
        {
            dataHolder.Add(data);
        }

        public bool GetBool(int index)
        {
            UJData data = (UJData)dataHolder[index];
			return Convert.ToBoolean(data.GetObject());
        }

        public byte GetByte(int index)
        {
            UJData data = (UJData)dataHolder[index];
			return Convert.ToByte(data.GetObject());
        }

        public short GetShort(int index)
        {
            UJData data = (UJData)dataHolder[index];
			return Convert.ToInt16(data.GetObject());
        }

        public int GetInt(int index)
        {
            UJData data = (UJData)dataHolder[index];
			return Convert.ToInt32(data.GetObject());
        }

        public long GetLong(int index)
        {
            UJData data = (UJData)dataHolder[index];
			return Convert.ToInt64(data.GetObject());
        }

        public float GetFloat(int index)
        {
           	UJData data = (UJData)dataHolder[index];
			return Convert.ToSingle(data.GetObject());
        }

        public double GetDouble(int index)
        {
            UJData data = (UJData)dataHolder[index];
			return Convert.ToDouble(data.GetObject());
        }

        public string GetUtfString(int index)
        {
           	UJData data = (UJData)dataHolder[index];
			return Convert.ToString(data.GetObject());
        }

        public UJArray GetUJArray(int index)
        {
            UJData data = (UJData)dataHolder[index];
			return data != null ? (UJArray) data.GetObject() : null;
        }

        public UJObject GetUJObject(int index)
        {
            UJData data = (UJData)dataHolder[index];
			return data != null ? (UJObject) data.GetObject() : null;
        }

        public int Size()
        {
            return dataHolder.Count;
        }
		
		public object[] ToArray(){
			return dataHolder.ToArray();
		}

        public byte[] ToBinary()
        {
            return DefaultUJSerializer.GetInstance().Array2Bin(this);
        }

        private void AddObj(Object obj, UJDataType type)
        {
            dataHolder.Add(new UJData(type, obj));
        }

        public String Dump()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');

            Object objDump = null;
            foreach (UJData wrappedObject in dataHolder)
            {
                if (wrappedObject.GetUJType() == UJDataType.UJ_OBJECT)
                    objDump = ((UJObject)wrappedObject.GetObject()).Dump();
                else if (wrappedObject.GetUJType() == UJDataType.UJ_ARRAY)
                    objDump = ((UJArray)wrappedObject.GetObject()).Dump();
                else
                    objDump = wrappedObject.GetObject();

                sb.Append(" (")
                    .Append(wrappedObject.GetUJType().GetName().ToLower())
                    .Append(") ")
                    .Append(objDump).Append(';');
            }

            sb.Append('}');
            return sb.ToString();
        }

    }
}
