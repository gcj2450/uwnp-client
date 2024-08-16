using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sfs2X.Protocol.Serialization;
using Sfs2X.Util;

namespace Sfs2X.Entities.Data
{
	/// <summary>
	/// The SFSObject class is used by SmartFoxServer in client-server data transfer.
	/// </summary>
	///
	/// <remarks>
	/// This class can be thought of as a specialized Dictionary/Map object that can contain any type of data.
	/// <para />
	/// The advantage of using the SFSObject class (for example when sending an <see cref="T:Sfs2X.Requests.ExtensionRequest" /> request) is that you can fine tune the way your data is transmitted over the network.
	/// For instance, a number like 100 can be transmitted as a normal integer (which takes 32 bits), but also a short (16 bit) or even a byte (8 bit).
	/// <para />
	/// SFSObject supports many primitive data types and related arrays of primitives (see the <see cref="T:Sfs2X.Entities.Data.SFSDataType" /> class). It also allows to serialize class instances and rebuild them on the other side (client or server).
	/// Check the SmartFoxServer 2X documentation for more informations on this advanced topic.
	/// <para />
	/// <b>NOTE</b>: UTF-8/multi-byte strings are not supported in key names. In other words you should restrict key names to standard ASCII characters. It is also recommended to keep key names very short to save bandwidth.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.Data.SFSDataType" />
	public class SFSObject : ISFSObject
	{
		private Dictionary<string, SFSDataWrapper> dataHolder;

		private ISFSDataSerializer serializer;

		/// <summary>
		/// Returns a new SFSObject instance.
		/// </summary>
		///
		/// <remarks>
		/// This is an alternative static constructor that builds an SFSObject from a valid SFSObject binary representation.
		/// </remarks>
		///
		/// <param name="ba">The ByteArray representation of a SFSObject.</param>
		///
		/// <returns>A new SFSObject instance.</returns>
		///
		/// <seealso cref="M:Sfs2X.Entities.Data.SFSObject.#ctor" />
		/// <seealso cref="M:Sfs2X.Entities.Data.SFSObject.NewInstance" />
		public static SFSObject NewFromBinaryData(ByteArray ba)
		{
			return DefaultSFSDataSerializer.Instance.Binary2Object(ba) as SFSObject;
		}

		/// <summary>
		/// Returns a new ISFSObject instance.
		/// </summary>
		///
		/// <remarks>
		/// This is an alternative static constructor that builds an ISFSObject from a valid JSON string representation.<br />
		/// This method is not available under Universal Windows Platform.
		/// </remarks>
		///
		/// <param name="js">The JSON representation of a SFSObject.</param>
		///
		/// <returns>A new ISFSObject instance.</returns>
		public static ISFSObject NewFromJsonData(string js)
		{
			return DefaultSFSDataSerializer.Instance.Json2Object(js);
		}

		/// <summary>
		/// Returns a new SFSObject instance.
		/// </summary>
		///
		/// <remarks>
		/// This method is a static alternative to the standard class constructor.
		/// </remarks>
		///
		/// <returns>A new SFSObject instance.</returns>
		///
		/// <seealso cref="M:Sfs2X.Entities.Data.SFSObject.#ctor" />
		/// <seealso cref="M:Sfs2X.Entities.Data.SFSObject.NewFromBinaryData(Sfs2X.Util.ByteArray)" />
		public static SFSObject NewInstance()
		{
			return new SFSObject();
		}

		/// <summary>
		/// Returns a new SFSObject instance.
		/// </summary>
		///
		/// <seealso cref="M:Sfs2X.Entities.Data.SFSObject.NewInstance" />
		/// <seealso cref="M:Sfs2X.Entities.Data.SFSObject.NewFromBinaryData(Sfs2X.Util.ByteArray)" />
		public SFSObject()
		{
			dataHolder = new Dictionary<string, SFSDataWrapper>();
			serializer = DefaultSFSDataSerializer.Instance;
		}

		private string Dump()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Convert.ToString(DefaultObjectDumpFormatter.TOKEN_INDENT_OPEN));
			foreach (KeyValuePair<string, SFSDataWrapper> item in dataHolder)
			{
				SFSDataWrapper value = item.Value;
				string key = item.Key;
				int type = value.Type;
				SFSDataType sFSDataType = (SFSDataType)type;
				stringBuilder.Append("(" + sFSDataType.ToString().ToLower() + ")");
				stringBuilder.Append(" " + key + ": ");
				if (type == 18)
				{
					stringBuilder.Append((value.Data as SFSObject).GetDump(false));
				}
				else if (type == 17)
				{
					stringBuilder.Append((value.Data as SFSArray).GetDump(false));
				}
				else if (type > 8 && type < 19)
				{
					object data = value.Data;
					stringBuilder.Append("[" + ((data != null) ? data.ToString() : null) + "]");
				}
				else
				{
					stringBuilder.Append(value.Data);
				}
				stringBuilder.Append(DefaultObjectDumpFormatter.TOKEN_DIVIDER);
			}
			string text = stringBuilder.ToString();
			if (Size() > 0)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text + DefaultObjectDumpFormatter.TOKEN_INDENT_CLOSE;
		}

		private T GetValue<T>(string key)
		{
			if (!dataHolder.ContainsKey(key))
			{
				return default(T);
			}
			return (T)dataHolder[key].Data;
		}

		/// <exclude />
		public SFSDataWrapper GetData(string key)
		{
			return dataHolder[key];
		}

		/// <inheritdoc />
		public bool IsNull(string key)
		{
			if (!ContainsKey(key))
			{
				return true;
			}
			SFSDataWrapper sFSDataWrapper = dataHolder[key];
			return sFSDataWrapper.Type == 0 || sFSDataWrapper.Data == null;
		}

		/// <inheritdoc />
		public virtual bool GetBool(string key)
		{
			return GetValue<bool>(key);
		}

		/// <inheritdoc />
		public virtual byte GetByte(string key)
		{
			return GetValue<byte>(key);
		}

		/// <inheritdoc />
		public virtual short GetShort(string key)
		{
			return GetValue<short>(key);
		}

		/// <inheritdoc />
		public virtual int GetInt(string key)
		{
			return GetValue<int>(key);
		}

		/// <inheritdoc />
		public virtual long GetLong(string key)
		{
			return GetValue<long>(key);
		}

		/// <inheritdoc />
		public virtual float GetFloat(string key)
		{
			return GetValue<float>(key);
		}

		/// <inheritdoc />
		public virtual double GetDouble(string key)
		{
			return GetValue<double>(key);
		}

		/// <inheritdoc />
		public virtual string GetUtfString(string key)
		{
			return GetValue<string>(key);
		}

		/// <inheritdoc />
		public virtual string GetText(string key)
		{
			return GetValue<string>(key);
		}

		private ICollection GetArray(string key)
		{
			return GetValue<ICollection>(key);
		}

		/// <inheritdoc />
		public virtual bool[] GetBoolArray(string key)
		{
			return (bool[])GetArray(key);
		}

		/// <inheritdoc />
		public virtual ByteArray GetByteArray(string key)
		{
			return GetValue<ByteArray>(key);
		}

		/// <inheritdoc />
		public virtual short[] GetShortArray(string key)
		{
			return (short[])GetArray(key);
		}

		/// <inheritdoc />
		public virtual int[] GetIntArray(string key)
		{
			return (int[])GetArray(key);
		}

		/// <inheritdoc />
		public virtual long[] GetLongArray(string key)
		{
			return (long[])GetArray(key);
		}

		/// <inheritdoc />
		public virtual float[] GetFloatArray(string key)
		{
			return (float[])GetArray(key);
		}

		/// <inheritdoc />
		public virtual double[] GetDoubleArray(string key)
		{
			return (double[])GetArray(key);
		}

		/// <inheritdoc />
		public virtual string[] GetUtfStringArray(string key)
		{
			return (string[])GetArray(key);
		}

		/// <inheritdoc />
		public virtual ISFSArray GetSFSArray(string key)
		{
			return GetValue<ISFSArray>(key);
		}

		/// <inheritdoc />
		public virtual ISFSObject GetSFSObject(string key)
		{
			return GetValue<ISFSObject>(key);
		}

		/// <inheritdoc />		
		public virtual object GetClass(string key)
		{
			if (!ContainsKey(key))
			{
				return null;
			}
			SFSDataWrapper sFSDataWrapper = dataHolder[key];
			if (sFSDataWrapper != null)
			{
				return sFSDataWrapper.Data;
			}
			return null;
		}

		/// <exclude />
		public void PutNull(string key)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.NULL, null);
		}

		/// <inheritdoc />
		public void PutBool(string key, bool val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.BOOL, val);
		}

		/// <inheritdoc />
		public void PutByte(string key, byte val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.BYTE, val);
		}

		/// <inheritdoc />
		public void PutShort(string key, short val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.SHORT, val);
		}

		/// <inheritdoc />
		public void PutInt(string key, int val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.INT, val);
		}

		/// <inheritdoc />
		public void PutLong(string key, long val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.LONG, val);
		}

		/// <inheritdoc />
		public void PutFloat(string key, float val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.FLOAT, val);
		}

		/// <inheritdoc />
		public void PutDouble(string key, double val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.DOUBLE, val);
		}

		/// <inheritdoc />
		public void PutUtfString(string key, string val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.UTF_STRING, val);
		}

		/// <inheritdoc />
		public void PutText(string key, string val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.TEXT, val);
		}

		/// <inheritdoc />
		public void PutBoolArray(string key, bool[] val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.BOOL_ARRAY, val);
		}

		/// <inheritdoc />
		public void PutByteArray(string key, ByteArray val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.BYTE_ARRAY, val);
		}

		/// <inheritdoc />
		public void PutShortArray(string key, short[] val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.SHORT_ARRAY, val);
		}

		/// <inheritdoc />
		public void PutIntArray(string key, int[] val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.INT_ARRAY, val);
		}

		/// <inheritdoc />
		public void PutLongArray(string key, long[] val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.LONG_ARRAY, val);
		}

		/// <inheritdoc />
		public void PutFloatArray(string key, float[] val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.FLOAT_ARRAY, val);
		}

		/// <inheritdoc />
		public void PutDoubleArray(string key, double[] val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.DOUBLE_ARRAY, val);
		}

		/// <inheritdoc />
		public void PutUtfStringArray(string key, string[] val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.UTF_STRING_ARRAY, val);
		}

		/// <inheritdoc />
		public void PutSFSArray(string key, ISFSArray val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.SFS_ARRAY, val);
		}

		/// <inheritdoc />
		public void PutSFSObject(string key, ISFSObject val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.SFS_OBJECT, val);
		}

		/// <inheritdoc />
		public virtual void PutClass(string key, object val)
		{
			dataHolder[key] = new SFSDataWrapper(SFSDataType.CLASS, val);
		}

		/// <exclude />
		public void Put(string key, SFSDataWrapper val)
		{
			dataHolder[key] = val;
		}

		/// <inheritdoc />
		public bool ContainsKey(string key)
		{
			return dataHolder.ContainsKey(key);
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

		/// <inheritdoc />
		public string GetDump()
		{
			return GetDump(true);
		}

		/// <inheritdoc />
		public string GetHexDump()
		{
			return DefaultObjectDumpFormatter.HexDump(ToBinary());
		}

		/// <inheritdoc />
		public string[] GetKeys()
		{
			string[] array = new string[dataHolder.Keys.Count];
			dataHolder.Keys.CopyTo(array, 0);
			return array;
		}

		/// <inheritdoc />
		public void RemoveElement(string key)
		{
			dataHolder.Remove(key);
		}

		/// <inheritdoc />
		public int Size()
		{
			return dataHolder.Count;
		}

		/// <inheritdoc />	
		public ByteArray ToBinary()
		{
			return serializer.Object2Binary(this);
		}

		/// <inheritdoc />	
		public string ToJson()
		{
			return serializer.Object2Json(flatten());
		}

		private Dictionary<string, object> flatten()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			DefaultSFSDataSerializer.Instance.flattenObject(dictionary, this);
			return dictionary;
		}
	}
}
