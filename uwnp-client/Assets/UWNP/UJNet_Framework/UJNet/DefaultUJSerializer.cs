using System;
using System.Collections;
using System.Text;
using UJNet.Data;
using LitJson;

namespace UJNet
{
    class DefaultUJSerializer : UJSerializer
    {
        private static int BUFFER_SIZE = 512;
	    private static UJSerializer instance = new DefaultUJSerializer();

	    public static UJSerializer GetInstance() {
		    return instance;
	    }

	    public byte[] Obj2Bin(UJObject obj) {
		    ByteBuffer buffer = ByteBuffer.Allocate(BUFFER_SIZE);
		    buffer.Put((byte) UJDataType.UJ_OBJECT.GetTypeID());
		    buffer.PutShort((short) obj.Size());
		    return obj2bin(obj, buffer);
			
	    }

	    public UJObject Bin2Obj(byte[] data) {
		    ByteBuffer buffer = ByteBuffer.Allocate(data.Length);
		    buffer.Put(data, true);
			buffer.Flip();
		    return decodeUJObject(buffer);
	    }
  
	    public byte[] Array2Bin(UJArray array) {
		    ByteBuffer buffer = ByteBuffer.Allocate(BUFFER_SIZE);
		    buffer.Put((byte) UJDataType.UJ_ARRAY.GetTypeID());
		    buffer.PutShort((short) array.Size());
		    return arr2bin(array, buffer);
	    }

	    public UJArray Bin2Array(byte[] data) {
		    ByteBuffer buffer = ByteBuffer.Allocate(data.Length);
		    buffer.Put(data, true);
			buffer.Flip();
		    return decodeUJArray(buffer);
	    }
		
		public UJObject Json2Obj(string json) {
			if (json == null || json.Length < 2) throw new ArgumentException ("json string too short " + json);
			JsonData data = JsonMapper.ToObject (json);
			return decodeUJObject (data);
		}
		
		public UJArray Json2Array(string json) {
			if (json == null || json.Length < 2) throw new ArgumentException ("json string too short " + json);
			JsonData data = JsonMapper.ToObject (json);
			return decodeUJArray (data);
		}
		

	    private byte[] obj2bin(UJObject obj, ByteBuffer buffer) {
		    ICollection keys = obj.GetKeys();
		    foreach (string key in keys) {
			    UJData ujData = obj.Get(key);
			    Object dataObj = ujData.GetObject();
			    buffer = encodeSFSObjectKey(buffer, key);
			    buffer = encodeObject(buffer, ujData.GetUJType(), dataObj);
		    }
		    return buffer.array();
	    }

	    private byte[] arr2bin(UJArray array, ByteBuffer buffer) {
			object[] arrays = array.ToArray();
		    foreach (object obj in arrays) {
			    UJData ujData = (UJData) obj;
			    Object dataObj = ujData.GetObject();
			    buffer = encodeObject(buffer, ujData.GetUJType(), dataObj);
		    }

		    return buffer.array();
	    }

	    private UJObject decodeUJObject(ByteBuffer buffer) {
		    UJObject ujObj = UJObject.NewInstance();
		    byte headerBuffer = buffer.Get();
		    if (headerBuffer != UJDataType.UJ_OBJECT.GetTypeID()) {
			    throw new ArgumentException(new StringBuilder(
					    "Invalid UJDataType:")
					    .Append(UJDataType.UJ_OBJECT.GetTypeID())
					    .Append(",headerBuffer : ").Append(headerBuffer).ToString()
                        );
		    }

		    short size = buffer.GetShort();
		    if (size < 0) {
			    throw new ArgumentException((new StringBuilder(
					    "Can't decode UJObject. Size  ")).Append(size)
					    .ToString());
		    }

		    for (int i = 0; i < size; i++) {
			    short keySize = buffer.GetShort();
			    if (keySize < 0 || keySize > 255) {
				    throw new ArgumentException();
			    }

			    byte[] keyData = new byte[keySize];
			    buffer.Get(keyData, 0, keyData.Length);
			    String key = Encoding.UTF8.GetString(keyData);
			    UJData decodedObject = decodeObject(buffer);
			    if (decodedObject == null) {
				    throw new ArgumentException();
			    }
			    ujObj.Put(key, decodedObject);
		    }

		    return ujObj;
	    }

	    private UJArray decodeUJArray(ByteBuffer buffer) {
		    UJArray array = UJArray.NewInstance();
		    byte headerBuffer = buffer.Get();
		    if (headerBuffer != UJDataType.UJ_ARRAY.GetTypeID())
			    throw new ArgumentException();

		    short size = buffer.GetShort();
		    if (size < 0)
			    throw new ArgumentException();

		    for (int i = 0; i < size; i++) {
			    UJData decodedObject = decodeObject(buffer);
			    if (decodedObject == null) {
				    throw new ArgumentException();
			    }

			    array.Add(decodedObject);
		    }
		    return array;
	    }
		
		private UJObject decodeUJObject(JsonData jso)
	    {
	        UJObject ujObj = UJObject.NewInstance ();
			foreach (string key in ((IDictionary)jso).Keys) {
				JsonData d = jso[key];
				UJData decodedObject = decodeJsonObject(d);
			    if (decodedObject == null) {
				    throw new ArgumentException ("not decode value for key " + d);
			    }
			    ujObj.Put(key, decodedObject);			
			}
	        return ujObj;
	    }	
		
		private UJArray decodeUJArray (JsonData jso)
		{
			UJArray array = UJArray.NewInstance();
			foreach (JsonData d in jso) {
			    UJData decodedObject = decodeJsonObject(d);
			    if (decodedObject == null) {
				    throw new ArgumentException ("not decode value for key " + d[0]);
			    }
				array.Add(decodedObject);
			}
	        return array;
		}
		

	    private ByteBuffer encodeSFSObjectKey(ByteBuffer buf, string value) {
		    buf.PutShort((short) value.Length);
		    buf.Put(Encoding.UTF8.GetBytes(value), true);
			return buf;
	    }

	    private ByteBuffer addData(ByteBuffer buffer, byte[] newData) {
		    buffer.Put(newData);
		    return buffer;
	    }

	    // ============================Encode Obj============================//
	    private ByteBuffer encodeObject(ByteBuffer buffer, UJDataType type,
			    Object obj) {
		    switch (type.GetTypeID()) {

		    case 1:
			    buffer = binEncode_BOOL(buffer, (bool) obj);
			    break;

		    case 2:
			    buffer = binEncode_BYTE(buffer, (byte) obj);
			    break;

		    case 3:
			    buffer = binEncode_SHORT(buffer, (short) obj);
			    break;

		    case 4:
			    buffer = binEncode_INT(buffer, (int) obj);
			    break;

		    case 5:
			    buffer = binEncode_LONG(buffer, (long) obj);
			    break;

		    case 6:
			    buffer = binEncode_FLOAT(buffer, (float) obj);
			    break;

		    case 7:
			    buffer = binEncode_DOUBLE(buffer, (double) obj);
			    break;

		    case 8:
			    buffer = binEncode_UTF_STRING(buffer, (string) obj);
			    break;

		    case 9:
			    buffer = buffer.Put(Array2Bin((UJArray) obj), true);
			    break;

		    case 10:
			    buffer = buffer.Put(Obj2Bin((UJObject) obj), true);
			    break;

		    default:
			    throw new ArgumentException((new StringBuilder(
					    "Unrecognized type : ")).Append(type).ToString());
		    }
		    return buffer;
	    }

	    private ByteBuffer binEncode_BOOL(ByteBuffer buffer, bool value) {
		    byte[] data = new byte[2];
		    data[0] = (byte) UJDataType.BOOL.GetTypeID();
		    data[1] = ((byte) (value ? 1 : 0));
		    return buffer.Put(data, true);
	    }

	    private ByteBuffer binEncode_BYTE(ByteBuffer buffer, byte value) {
		    byte[] data = new byte[2];
		    data[0] = (byte) UJDataType.BYTE.GetTypeID();
		    data[1] = value;
		    return buffer.Put(data, true);
	    }

	    private ByteBuffer binEncode_SHORT(ByteBuffer buf, short value) {
		    buf.Put((byte) UJDataType.SHORT.GetTypeID());
		    buf.PutShort(value);
			return buf;
	    }

	    private ByteBuffer binEncode_INT(ByteBuffer buf, int value) {
		    buf.Put((byte) UJDataType.INT.GetTypeID());
		    buf.PutInt(value);
			return buf;
	    }

	    private ByteBuffer binEncode_LONG(ByteBuffer buf, long value) {
		    buf.Put((byte) UJDataType.LONG.GetTypeID());
		    buf.PutLong(value);
			return buf;
	    }

	    private ByteBuffer binEncode_FLOAT(ByteBuffer buf, float value) {
		    buf.Put((byte) UJDataType.FLOAT.GetTypeID());
		    buf.PutFloat(value);
			return buf;
	    }

	    private ByteBuffer binEncode_DOUBLE(ByteBuffer buf, double value) {
		    buf.Put((byte) UJDataType.DOUBLE.GetTypeID());
		    buf.PutDouble(value);
			return buf;
	    }

	    private ByteBuffer binEncode_UTF_STRING(ByteBuffer buf, string value) {
		    byte[] stringBytes = Encoding.UTF8.GetBytes(value);
		    buf.Put((byte) UJDataType.UTF_STRING.GetTypeID());
		    buf.PutShort((short) stringBytes.Length);
		    buf.Put(stringBytes, true);
			return buf;
	    }

	    // ========================Decode Obj ==========================//
	    private UJData decodeObject(ByteBuffer buffer) {
		    UJData decodedObject = null;
		    byte headerByte = buffer.Get();

		    if (headerByte == UJDataType.BOOL.GetTypeID()) {
			    decodedObject = binDecode_BOOL(buffer);
		    } else if (headerByte == UJDataType.BYTE.GetTypeID()) {
			    decodedObject = binDecode_BYTE(buffer);
		    } else if (headerByte == UJDataType.SHORT.GetTypeID()) {
			    decodedObject = binDecode_SHORT(buffer);
		    } else if (headerByte == UJDataType.INT.GetTypeID()) {
			    decodedObject = binDecode_INT(buffer);
		    } else if (headerByte == UJDataType.LONG.GetTypeID()) {
			    decodedObject = binDecode_LONG(buffer);
		    } else if (headerByte == UJDataType.FLOAT.GetTypeID()) {
			    decodedObject = binDecode_FLOAT(buffer);
		    } else if (headerByte == UJDataType.DOUBLE.GetTypeID()) {
			    decodedObject = binDecode_DOUBLE(buffer);
		    } else if (headerByte == UJDataType.UTF_STRING.GetTypeID()) {
			    decodedObject = binDecode_UTF_STRING(buffer);
		    } else if (headerByte == UJDataType.UJ_ARRAY.GetTypeID()) {
			    buffer.Position((long) (buffer.Position() - 1));
			    decodedObject = new UJData(UJDataType.UJ_ARRAY,
					    decodeUJArray(buffer));
		    } else if (headerByte == UJDataType.UJ_OBJECT.GetTypeID()) {
			    buffer.Position((long) (buffer.Position() - 1) );
			    UJObject ujObj = decodeUJObject(buffer);
			    decodedObject = new UJData(UJDataType.UJ_OBJECT, ujObj);
		    } else {
			    throw new ArgumentException();
		    }
		    return decodedObject;
	    }
		
		private UJData decodeJsonObject (JsonData o)
		{
	       if(o.IsInt)
	            return new UJData(UJDataType.INT, (int)o);
	        if(o.IsLong)
	            return new UJData(UJDataType.LONG, (long)o);
	        if(o.IsDouble)
	            return new UJData(UJDataType.DOUBLE, (double)o);
	        if(o.IsString)
	            return new UJData(UJDataType.UTF_STRING, (string)o);
	        if(o.IsObject)
	        {
	            JsonData jso = (JsonData)o;
	            return new UJData(UJDataType.UJ_OBJECT, decodeUJObject(jso));
	        }
	        if(o.IsArray)
	            return new UJData(UJDataType.UJ_ARRAY, decodeUJArray((JsonData)o));
	        else
	             throw new ArgumentException();
		}

	    private UJData binDecode_BOOL(ByteBuffer buffer) {
		    byte boolByte = buffer.Get();
		    bool val = false;
		    if (boolByte == 0)
			    val = false;
		    else if (boolByte == 1)
			    val = true;
		    else
			    throw new ArgumentException();
		    return new UJData(UJDataType.BOOL, val);
	    }

	    private UJData binDecode_BYTE(ByteBuffer buffer) {
		    byte boolByte = buffer.Get();
		    return new UJData(UJDataType.BYTE, boolByte);
	    }

	    private UJData binDecode_SHORT(ByteBuffer buffer) {
		    short shortValue = buffer.GetShort();
		    return new UJData(UJDataType.SHORT, shortValue);
	    }

	    private UJData binDecode_INT(ByteBuffer buffer) {
		    int intValue = buffer.GetInt();
		    return new UJData(UJDataType.INT, intValue);
	    }

	    private UJData binDecode_LONG(ByteBuffer buffer) {
		    long longValue = buffer.GetLong();
		    return new UJData(UJDataType.LONG, longValue);
	    }

	    private UJData binDecode_FLOAT(ByteBuffer buffer) {
		    float floatValue = buffer.GetFloat();
		    return new UJData(UJDataType.FLOAT, floatValue);
	    }

	    private UJData binDecode_DOUBLE(ByteBuffer buffer) {
		    double doubleValue = buffer.GetDouble();
		    return new UJData(UJDataType.DOUBLE, doubleValue);
	    }

	    private UJData binDecode_UTF_STRING(ByteBuffer buffer) {
		    short strLen = buffer.GetShort();
		    if (strLen < 0) {
			    throw new ArgumentException();
		    } else {
			    byte[] strData = new byte[strLen];
			    buffer.Get(strData, 0, strLen);
			    String decodedString = Encoding.UTF8.GetString(strData);
			    return new UJData(UJDataType.UTF_STRING, decodedString);
		    }
	    }

    }
}
