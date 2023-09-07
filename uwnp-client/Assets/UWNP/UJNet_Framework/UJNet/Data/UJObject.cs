using System;
using System.Collections;
using System.Text;

namespace UJNet.Data
{
    public class UJObject
    {
        private Hashtable dataHolder;

        public UJObject()
        {
            dataHolder = new Hashtable();
        }

        public static UJObject NewInstance()
        {
            return new UJObject();
        }
		
		public bool Contains(string key){
			return dataHolder.ContainsKey(key);
		}

        public bool GetBool(string key)
        {
            UJData data = (UJData)dataHolder[key];
			return data != null ? Convert.ToBoolean(data.GetObject()) : false;
        }

        public byte GetByte(string key)
        {
            UJData data = (UJData)dataHolder[key];
			return data != null ? Convert.ToByte(data.GetObject()) : (byte)0;
        }

        public short GetShort(string key)
        {
            UJData data = (UJData)dataHolder[key];
			return data != null ? Convert.ToInt16(data.GetObject()) : (short)0;
        }

        public int GetInt(string key)
        {
            UJData data = (UJData)dataHolder[key];
			return data != null ? Convert.ToInt32(data.GetObject()) : 0;
        }

        public long GetLong(string key)
        {
            UJData data = (UJData)dataHolder[key];
			return data != null ? Convert.ToInt64(data.GetObject()) : 0;
        }

        public float GetFloat(string key)
        {
            UJData data = (UJData)dataHolder[key];
			return data != null ? Convert.ToSingle(data.GetObject()) : 0.0f;
        }

        public double GetDouble(string key)
        {
            UJData data = (UJData)dataHolder[key];
			return data != null ? Convert.ToDouble(data.GetObject()) : 0.0d;
        }

        public string GetUtfString(string key)
        {
            UJData data = (UJData)dataHolder[key];
			return data != null ? Convert.ToString(data.GetObject()) : null;
        }

        public UJArray GetUJArray(string key)
        {
            UJData data = (UJData)dataHolder[key];
			return data != null ? (UJArray)data.GetObject() : null;
        }

        public UJObject GetUJObject(string key)
        {
            UJData data = (UJData)dataHolder[key];
			return data != null ? (UJObject)data.GetObject() : null;
        }

        public UJData Get(string key)
        {
            return (UJData)dataHolder[key];
        }

        public void Put(string key, UJData data)
        {
            dataHolder.Add(key, data);
        }

        public void PutBool(string key, bool value)
        {
            PutObj(key, value, UJDataType.BOOL);
        }

        public void PutByte(string key, byte value)
        {
            PutObj(key, value, UJDataType.BYTE);
        }

        public void PutShort(string key, short value)
        {
            PutObj(key, value, UJDataType.SHORT);
        }

        public void PutInt(string key, int value)
        {
            PutObj(key, value, UJDataType.INT);
        }

        public void PutLong(string key, long value)
        {
            PutObj(key, value, UJDataType.LONG);
        }

        public void PutFloat(string key, float value)
        {
            PutObj(key, value, UJDataType.FLOAT);
        }

        public void PutDouble(string key, double value)
        {
            PutObj(key, value, UJDataType.DOUBLE);
        }

        public void PutUtfString(string key, String value)
        {
            PutObj(key, value, UJDataType.UTF_STRING);
        }

        public void PutUJArray(string key, UJArray value)
        {
            PutObj(key, value, UJDataType.UJ_ARRAY);
        }

        public void PutUJObject(string key, UJObject value)
        {
            PutObj(key, value, UJDataType.UJ_OBJECT);
        }

        public int Size()
        {
            return dataHolder.Count;
        }
		
		public ICollection GetKeys(){
			return dataHolder.Keys;
		}

        public byte[] ToBinary()
        {
            return DefaultUJSerializer.GetInstance().Obj2Bin(this);
        }

        public static UJObject NewFromBinaryData(byte[] data)
        {
            return DefaultUJSerializer.GetInstance().Bin2Obj(data);
        }
		
		public static UJObject newFromJsonData (string json)
		{
			return  DefaultUJSerializer.GetInstance().Json2Obj(json);
		}

        private void PutObj(string key, Object value, UJDataType type)
        {
            if (key == null || value == null || type == null)
            {
                throw new ArgumentException();
            }

		    if (value is UJData) 
            {
			    dataHolder.Add(key, (UJData) value);
		    }
            else
            {
                dataHolder.Add(key, new UJData(type, value));
		    }
	    }

        public string Dump()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append('{');

            foreach(string key in dataHolder.Keys)
            {
                UJData wrapper = Get(key);

                buffer.Append("(")
                    .Append(wrapper.GetUJType().GetName().ToLower())
                    .Append(") ")
                    .Append(key).Append(": ");

                if (wrapper.GetUJType() == UJDataType.UJ_OBJECT)
                    buffer.Append(((UJObject)wrapper.GetObject()).Dump());
                else if (wrapper.GetUJType() == UJDataType.UJ_ARRAY)
                    buffer.Append(((UJArray)wrapper.GetObject()).Dump());
                else
                    buffer.Append(wrapper.GetObject());

                buffer.Append(';');
            }

            buffer.Append('}');
            return buffer.ToString();
        }
    }
}
