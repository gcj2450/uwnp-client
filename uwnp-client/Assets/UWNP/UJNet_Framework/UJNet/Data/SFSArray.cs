using NetCoreServer.UJNet_Framework.UJNet;
using NetCoreServer.UJNet_Framework.UJNet.Data;
using Sfs2X.Exceptions;
using Sfs2X.Protocol.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UJNet.Data
{
    public class SFSArray : ISFSArray, ICollection, IEnumerable
    {
        private ISFSDataSerializer serializer;

        private List<SFSDataWrapper> dataHolder;

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        int ICollection.Count
        {
            get
            {
                return dataHolder.Count;
            }
        }

        /// <summary>
        /// Returns a new SFSArray instance.
        /// </summary>
        ///
        /// <remarks>
        /// This is an alternative static constructor that builds an SFSArray from a valid SFSArray binary representation.
        /// </remarks>
        ///
        /// <param name="ba">The ByteArray representation of a SFSArray.</param>
        ///
        /// <returns>A new SFSArray instance.</returns>
        ///
        /// <seealso cref="M:Sfs2X.Entities.Data.SFSArray.#ctor" />
        /// <seealso cref="M:Sfs2X.Entities.Data.SFSArray.NewInstance" />
        public static SFSArray NewFromBinaryData(ByteArray ba)
        {
            return DefaultSFSDataSerializer.Instance.Binary2Array(ba) as SFSArray;
        }

        /// <summary>
        /// Returns a new ISFSArray instance.
        /// </summary>
        ///
        /// <remarks>
        /// This is an alternative static constructor that builds an ISFSArray from a valid JSON string representation.<br />
        /// This method is not available under Universal Windows Platform.
        /// </remarks>
        ///
        /// <param name="js">The JSON representation of a SFSArray.</param>
        ///
        /// <returns>A new ISFSArray instance.</returns>
        public static ISFSArray NewFromJsonData(string js)
        {
            return DefaultSFSDataSerializer.Instance.Json2Array(js);
        }

        /// <summary>
        /// Returns a new SFSArray instance.
        /// </summary>
        ///
        /// <remarks>
        /// This method is a static alternative to the standard class constructor.
        /// </remarks>
        ///
        /// <returns>A new SFSArray instance.</returns>
        ///
        /// <seealso cref="M:Sfs2X.Entities.Data.SFSArray.#ctor" />
        /// <seealso cref="M:Sfs2X.Entities.Data.SFSArray.NewFromBinaryData(Sfs2X.Util.ByteArray)" />
        public static SFSArray NewInstance()
        {
            return new SFSArray();
        }

        /// <summary>
        /// Returns a new SFSArray instance.
        /// </summary>
        ///
        /// <seealso cref="M:Sfs2X.Entities.Data.SFSArray.NewInstance" />
        /// <seealso cref="M:Sfs2X.Entities.Data.SFSArray.NewFromBinaryData(Sfs2X.Util.ByteArray)" />
        public SFSArray()
        {
            dataHolder = new List<SFSDataWrapper>();
            serializer = DefaultSFSDataSerializer.Instance;
        }

        /// <inheritdoc />
        ///
        /// <remarks>Checking if this object contains an inner SFSObject or SFSArray is not supported.</remarks>
        public bool Contains(object obj)
        {
            if (obj is ISFSArray || obj is ISFSObject)
            {
                throw new SFSError("ISFSArray and ISFSObject are not supported by this method.");
            }
            for (int i = 0; i < Size(); i++)
            {
                object elementAt = GetElementAt(i);
                if (object.Equals(elementAt, obj))
                {
                    return true;
                }
            }
            return false;
        }

        /// <exclude />
        public SFSDataWrapper GetWrappedElementAt(int index)
        {
            return dataHolder[index];
        }

        /// <inheritdoc />
        public object GetElementAt(int index)
        {
            object result = null;
            if (dataHolder[index] != null)
            {
                result = dataHolder[index].Data;
            }
            return result;
        }

        /// <inheritdoc />
        public object RemoveElementAt(int index)
        {
            if (index >= dataHolder.Count)
            {
                return null;
            }
            SFSDataWrapper sFSDataWrapper = dataHolder[index];
            dataHolder.RemoveAt(index);
            return sFSDataWrapper.Data;
        }

        /// <inheritdoc />
        public int Size()
        {
            return dataHolder.Count;
        }

        /// <inheritdoc />
        public ByteArray ToBinary()
        {
            return serializer.Array2Binary(this);
        }

        /// <inheritdoc />
        public string ToJson()
        {
            return serializer.Array2Json(flatten());
        }

        private List<object> flatten()
        {
            List<object> list = new List<object>();
            DefaultSFSDataSerializer.Instance.flattenArray(list, this);
            return list;
        }

        /// <inheritdoc />
        public string GetDump()
        {
            return GetDump(true);
        }

        /// <inheritdoc />
        public string GetDump(bool format)
        {
            if (!format)
            {
                return Dump();
            }
            return DefaultObjectDumpFormatter.PrettyPrintDump(Dump());
        }

        private string Dump()
        {
            StringBuilder stringBuilder = new StringBuilder(Convert.ToString(DefaultObjectDumpFormatter.TOKEN_INDENT_OPEN));
            for (int i = 0; i < dataHolder.Count; i++)
            {
                SFSDataWrapper sFSDataWrapper = dataHolder[i];
                int type = sFSDataWrapper.Type;
                string value;
                if (type == 18)
                {
                    value = (sFSDataWrapper.Data as SFSObject).GetDump(false);
                }
                else if (type == 17)
                {
                    value = (sFSDataWrapper.Data as SFSArray).GetDump(false);
                }
                else if (type == 0)
                {
                    value = "NULL";
                }
                else if (type > 8 && type < 19)
                {
                    object data = sFSDataWrapper.Data;
                    value = "[" + ((data != null) ? data.ToString() : null) + "]";
                }
                else
                {
                    value = sFSDataWrapper.Data.ToString();
                }
                SFSDataType sFSDataType = (SFSDataType)type;
                stringBuilder.Append("(" + sFSDataType.ToString().ToLower() + ") ");
                stringBuilder.Append(value);
                stringBuilder.Append(Convert.ToString(DefaultObjectDumpFormatter.TOKEN_DIVIDER));
            }
            string text = stringBuilder.ToString();
            if (Size() > 0)
            {
                text = text.Substring(0, text.Length - 1);
            }
            return text + Convert.ToString(DefaultObjectDumpFormatter.TOKEN_INDENT_CLOSE);
        }

        /// <inheritdoc />
        public string GetHexDump()
        {
            return DefaultObjectDumpFormatter.HexDump(ToBinary());
        }

        /// <inheritdoc />
        public void AddNull()
        {
            AddObject(null, SFSDataType.NULL);
        }

        /// <inheritdoc />
        public void AddBool(bool val)
        {
            AddObject(val, SFSDataType.BOOL);
        }

        /// <inheritdoc />
        public void AddByte(byte val)
        {
            AddObject(val, SFSDataType.BYTE);
        }

        /// <inheritdoc />
        public void AddShort(short val)
        {
            AddObject(val, SFSDataType.SHORT);
        }

        /// <inheritdoc />
        public void AddInt(int val)
        {
            AddObject(val, SFSDataType.INT);
        }

        /// <inheritdoc />
        public void AddLong(long val)
        {
            AddObject(val, SFSDataType.LONG);
        }

        /// <inheritdoc />
        public void AddFloat(float val)
        {
            AddObject(val, SFSDataType.FLOAT);
        }

        /// <inheritdoc />
        public void AddDouble(double val)
        {
            AddObject(val, SFSDataType.DOUBLE);
        }

        /// <inheritdoc />
        public void AddUtfString(string val)
        {
            AddObject(val, SFSDataType.UTF_STRING);
        }

        /// <inheritdoc />
        public void AddText(string val)
        {
            AddObject(val, SFSDataType.TEXT);
        }

        /// <inheritdoc />
        public void AddBoolArray(bool[] val)
        {
            AddObject(val, SFSDataType.BOOL_ARRAY);
        }

        /// <inheritdoc />
        public void AddByteArray(ByteArray val)
        {
            AddObject(val, SFSDataType.BYTE_ARRAY);
        }

        /// <inheritdoc />
        public void AddShortArray(short[] val)
        {
            AddObject(val, SFSDataType.SHORT_ARRAY);
        }

        /// <inheritdoc />
        public void AddIntArray(int[] val)
        {
            AddObject(val, SFSDataType.INT_ARRAY);
        }

        /// <inheritdoc />
        public void AddLongArray(long[] val)
        {
            AddObject(val, SFSDataType.LONG_ARRAY);
        }

        /// <inheritdoc />
        public void AddFloatArray(float[] val)
        {
            AddObject(val, SFSDataType.FLOAT_ARRAY);
        }

        /// <inheritdoc />
        public void AddDoubleArray(double[] val)
        {
            AddObject(val, SFSDataType.DOUBLE_ARRAY);
        }

        /// <inheritdoc />
        public void AddUtfStringArray(string[] val)
        {
            AddObject(val, SFSDataType.UTF_STRING_ARRAY);
        }

        /// <inheritdoc />
        public void AddSFSArray(ISFSArray val)
        {
            AddObject(val, SFSDataType.SFS_ARRAY);
        }

        /// <inheritdoc />
        public void AddSFSObject(ISFSObject val)
        {
            AddObject(val, SFSDataType.SFS_OBJECT);
        }

        /// <inheritdoc />
        public void AddClass(object val)
        {
            AddObject(val, SFSDataType.CLASS);
        }

        /// <exclude />
        public void Add(SFSDataWrapper wrappedObject)
        {
            dataHolder.Add(wrappedObject);
        }

        private void AddObject(object val, SFSDataType tp)
        {
            Add(new SFSDataWrapper((int)tp, val));
        }

        private T GetValue<T>(int index)
        {
            SFSDataWrapper sFSDataWrapper = dataHolder[index];
            return (T)sFSDataWrapper.Data;
        }

        /// <inheritdoc />
        public bool IsNull(int index)
        {
            SFSDataWrapper sFSDataWrapper = dataHolder[index];
            return sFSDataWrapper.Type == 0;
        }

        /// <inheritdoc />
        public virtual bool GetBool(int index)
        {
            return GetValue<bool>(index);
        }

        /// <inheritdoc />
        public virtual byte GetByte(int index)
        {
            return GetValue<byte>(index);
        }

        /// <inheritdoc />
        public virtual short GetShort(int index)
        {
            return GetValue<short>(index);
        }

        /// <inheritdoc />
        public virtual int GetInt(int index)
        {
            return GetValue<int>(index);
        }

        /// <inheritdoc />
        public virtual long GetLong(int index)
        {
            return GetValue<long>(index);
        }

        /// <inheritdoc />
        public virtual float GetFloat(int index)
        {
            return GetValue<float>(index);
        }

        /// <inheritdoc />
        public virtual double GetDouble(int index)
        {
            return GetValue<double>(index);
        }

        /// <inheritdoc />
        public string GetUtfString(int index)
        {
            return GetValue<string>(index);
        }

        /// <inheritdoc />
        public string GetText(int index)
        {
            return GetValue<string>(index);
        }

        private ICollection GetArray(int index)
        {
            return GetValue<ICollection>(index);
        }

        /// <inheritdoc />
        public virtual bool[] GetBoolArray(int index)
        {
            return (bool[])GetArray(index);
        }

        /// <inheritdoc />
        public virtual ByteArray GetByteArray(int index)
        {
            return GetValue<ByteArray>(index);
        }

        /// <inheritdoc />
        public virtual short[] GetShortArray(int index)
        {
            return (short[])GetArray(index);
        }

        /// <inheritdoc />	
        public virtual int[] GetIntArray(int index)
        {
            return (int[])GetArray(index);
        }

        /// <inheritdoc />
        public virtual long[] GetLongArray(int index)
        {
            return (long[])GetArray(index);
        }

        /// <inheritdoc />
        public virtual float[] GetFloatArray(int index)
        {
            return (float[])GetArray(index);
        }

        /// <inheritdoc />
        public virtual double[] GetDoubleArray(int index)
        {
            return (double[])GetArray(index);
        }

        /// <inheritdoc />
        public virtual string[] GetUtfStringArray(int index)
        {
            return (string[])GetArray(index);
        }

        /// <inheritdoc />
        public virtual ISFSArray GetSFSArray(int index)
        {
            return GetValue<ISFSArray>(index);
        }

        /// <inheritdoc />
        public virtual object GetClass(int index)
        {
            SFSDataWrapper sFSDataWrapper = dataHolder[index];
            return (sFSDataWrapper != null) ? sFSDataWrapper.Data : null;
        }

        /// <inheritdoc />
        public virtual ISFSObject GetSFSObject(int index)
        {
            return GetValue<ISFSObject>(index);
        }

        void ICollection.CopyTo(Array toArray, int index)
        {
            foreach (SFSDataWrapper item in dataHolder)
            {
                toArray.SetValue(item, index);
                index++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SFSArrayEnumerator(dataHolder);
        }
    }
}
