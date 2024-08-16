using Sfs2X.Util;

namespace Sfs2X.Entities.Data
{
	/// <summary>
	/// The ISFSObject interface defines all the public methods and properties of the SFSObject class used by SmartFoxServer in client-server data transfer.
	/// </summary>
	///
	/// <remarks>
	/// Read the implementor class description for additional informations.
	/// <para />
	/// Check the <see cref="T:Sfs2X.Entities.Data.SFSDataType" /> enumeration for more informations on supported data types.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Entities.Data.SFSObject" />
	public interface ISFSObject
	{
		/// <summary>
		/// Indicates if the value mapped by the specified key is <c>null</c>.
		/// </summary>
		///
		/// <param name="key">The key to be checked.</param>
		///
		/// <returns><c>true</c> if the value mapped by the passed key is <c>null</c> or the mapping doesn't exist for that key.</returns>
		bool IsNull(string key);

		/// <summary>
		/// Indicates whether this object contains a mapping for the specified key or not.
		/// </summary>
		///
		/// <param name="key">The key whose presence in this object is to be tested.</param>
		///
		/// <returns><c>true</c> if this object contains a mapping for the specified key.</returns>
		bool ContainsKey(string key);

		/// <summary>
		/// Removes the element corresponding to the passed key from this object.
		/// </summary>
		///
		/// <param name="key">The key of the element to be removed.</param>
		void RemoveElement(string key);

		/// <summary>
		/// Retrieves a list of all the keys contained in this object.
		/// </summary>
		///
		/// <returns>The list of all the keys in this object.</returns>
		string[] GetKeys();

		/// <summary>
		/// Indicates the number of elements in this object.
		/// </summary>
		///
		/// <returns>The number of elements in this object.</returns>
		int Size();

		/// <summary>
		/// Provides the binary form of this object.
		/// </summary>
		///
		/// <returns>The binary data representing this object.</returns>
		ByteArray ToBinary();

		/// <summary>
		/// Provides the JSON representation of this object.
		/// </summary>
		///
		/// <remarks>
		/// This method is not available under Universal Windows Platform.
		/// </remarks>
		///
		/// <returns>The JSON string representing this object.</returns>
		///
		/// <remarks>This method is not available under Universal Windows Platform.</remarks>
		string ToJson();

		/// <summary>
		/// Provides a formatted string representing this object.
		/// </summary>
		///
		/// <remarks>
		/// The returned string can be logged or traced in the console for debugging purposes.
		/// </remarks>
		///
		/// <param name="format">If <c>true</c>, the output is formatted in a human-readable way.</param>
		///
		/// <returns>The string representation of this object.</returns>
		string GetDump(bool format);

		/// <summary>
		/// See <see cref="M:Sfs2X.Entities.Data.ISFSObject.GetDump(System.Boolean)" />.
		/// </summary>
		string GetDump();

		/// <summary>
		/// Provides a detailed hexadecimal representation of this object.
		/// </summary>
		///
		/// <remarks>
		/// The returned string can be logged or traced in the console for debugging purposes.
		/// </remarks>
		///
		/// <returns>The hexadecimal string representation of this object.</returns>
		string GetHexDump();

		/// <exclude />
		SFSDataWrapper GetData(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as a boolean.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object associated with the specified key; <c>false</c> if a mapping for the passed key doesn't exist.</returns>
		bool GetBool(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as a signed byte (8 bits).
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object associated with the specified key; <c>0</c> if a mapping for the passed key doesn't exist.</returns>
		byte GetByte(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as a short integer (16 bits).
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object associated with the specified key; <c>0</c> if a mapping for the passed key doesn't exist.</returns>
		short GetShort(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an integer (32 bits).
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object associated with the specified key; <c>0</c> if a mapping for the passed key doesn't exist.</returns>
		int GetInt(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as a long integer (64 bits).
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object associated with the specified key; <c>0</c> if a mapping for the passed key doesn't exist.</returns>
		long GetLong(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as a floating point number.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object associated with the specified key; <c>0</c> if a mapping for the passed key doesn't exist.</returns>
		float GetFloat(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as a double precision number.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object associated with the specified key; <c>0</c> if a mapping for the passed key doesn't exist.</returns>
		double GetDouble(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an UTF-8 string, with max length of 32 KBytes.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object associated with the specified key; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		string GetUtfString(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an UTF-8 string, with max length of 2 GBytes.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object associated with the specified key; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		string GetText(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an array of booleans.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object as an array of booleans; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		bool[] GetBoolArray(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as a ByteArray object.
		/// </summary>
		///
		/// <remarks>
		/// <b>IMPORTANT</b>: ByteArrays transmission is not supported in Unity WebGL.
		/// </remarks>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object as a ByteArray object; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		ByteArray GetByteArray(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an array of shorts.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object as an array of shorts; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		short[] GetShortArray(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an array of integers.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object as an array of integers; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		int[] GetIntArray(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an array of longs.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object as an array of longs; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		long[] GetLongArray(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an array of floats.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object as an array of floats; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		float[] GetFloatArray(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an array of doubles.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object as an array of doubles; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		double[] GetDoubleArray(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an array of UTF-8 strings.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object as an array of UTF-8 strings; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		string[] GetUtfStringArray(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an ISFSArray object.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object as an object implementing the ISFSArray interface; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		///
		/// <seealso cref="T:Sfs2X.Entities.Data.SFSArray" />
		ISFSArray GetSFSArray(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an ISFSObject object.
		/// </summary>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object as an object implementing the ISFSObject interface; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		///
		/// <seealso cref="T:Sfs2X.Entities.Data.SFSObject" />
		ISFSObject GetSFSObject(string key);

		/// <summary>
		/// Returns the element corresponding to the specified key as an instance of a custom class.
		/// </summary>
		///
		/// <remarks>
		/// This advanced feature allows the transmission of specific object instances between client-side C# and server-side Java provided that:<br />
		/// - the respective class definitions on both sides have the same package/namespace name<br />
		/// - the class implements the SerializableSFSType interface on both sides<br />
		/// - the following code is executed right after creating the SmartFox object: <c>DefaultSFSDataSerializer.RunningAssembly = Assembly.GetExecutingAssembly();</c> (requires <c>System.Reflection</c> and <c>Sfs2X.Protocol.Serialization</c>)
		/// <para />
		/// <b>IMPORTANT</b>: class serialization is not supported in Unity WebGL.
		/// </remarks>
		///
		/// <param name="key">The key whose associated value is to be returned.</param>
		///
		/// <returns>The element of this object as a generic object type to be casted to the target class definition; <c>null</c> if a mapping for the passed key doesn't exist.</returns>
		///
		/// <example>
		/// The following example shows the same class on the client and server sides, which can be transferred back and forth with the <see cref="M:Sfs2X.Entities.Data.ISFSObject.GetClass(System.String)" /> and <see cref="M:Sfs2X.Entities.Data.ISFSObject.PutClass(System.String,System.Object)" /> methods.
		/// <para />
		/// The server-side Java definition of a SpaceShip class is:
		/// <code>
		/// package my.game.spacecombat
		///
		/// public class SpaceShip implements SerializableSFSType
		/// {
		/// 	private String type;
		/// 	private String name;
		/// 	private int firePower;
		/// 	private int maxSpeed;
		/// 	private List&lt;String&gt; weapons;
		///
		/// 	public SpaceShip(String name, String type)
		/// 	{
		/// 		this.name = name;
		/// 		this.type = type;
		/// 	}
		///
		/// 	// ... Getters / Setters ...
		/// }
		/// </code>
		/// <para />
		/// The client-side C# definition of the SpaceShip class is:
		/// <code>
		/// namespace my.game.spacecombat
		/// {
		/// 	public class SpaceShip : SerializableSFSType
		/// 	{
		/// 		private string _type;
		/// 		private string _name;
		/// 		private int _firePower;
		/// 		private int _maxSpeed;
		/// 		private Array _weapons;
		///
		/// 		public SpaceShip(string name, string type)
		/// 		{
		/// 			_name = name
		/// 			_type = type
		/// 		}
		///
		/// 		// ... Getters / Setters ...
		/// 	}
		/// }
		/// </code>
		/// <para />
		/// The SpaceShip instance is sent by the server to the client. This is how to retrieve it:
		/// <code>
		/// 	SpaceShip myShipData = (SpaceShip)sfsObject.GetClass(key);
		/// </code>
		/// </example>
		object GetClass(string key);

		/// <exclude />
		void PutNull(string key);

		/// <summary>
		/// Associates the passed boolean value with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified value is to be associated.</param>
		/// <param name="val">The value to be associated with the specified key.</param>
		void PutBool(string key, bool val);

		/// <summary>
		/// Associates the passed byte value with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified value is to be associated.</param>
		/// <param name="val">The value to be associated with the specified key.</param>
		void PutByte(string key, byte val);

		/// <summary>
		/// Associates the passed short value with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified value is to be associated.</param>
		/// <param name="val">The value to be associated with the specified key.</param>
		void PutShort(string key, short val);

		/// <summary>
		/// Associates the passed integer value with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified value is to be associated.</param>
		/// <param name="val">The value to be associated with the specified key.</param>
		void PutInt(string key, int val);

		/// <summary>
		/// Associates the passed long value with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified value is to be associated.</param>
		/// <param name="val">The value to be associated with the specified key.</param>
		void PutLong(string key, long val);

		/// <summary>
		/// Associates the passed float value with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified value is to be associated.</param>
		/// <param name="val">The value to be associated with the specified key.</param>
		void PutFloat(string key, float val);

		/// <summary>
		/// Associates the passed double value with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified value is to be associated.</param>
		/// <param name="val">The value to be associated with the specified key.</param>
		void PutDouble(string key, double val);

		/// <summary>
		/// Associates the passed UTF-8 string value (max length: 32 KBytes) with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified value is to be associated.</param>
		/// <param name="val">The value to be associated with the specified key.</param>
		void PutUtfString(string key, string val);

		/// <summary>
		/// Associates the passed UTF-8 string value (max length: 2 GBytes) with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified value is to be associated.</param>
		/// <param name="val">The value to be associated with the specified key.</param>
		void PutText(string key, string val);

		/// <summary>
		/// Associates the passed array of booleans with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified array is to be associated.</param>
		/// <param name="val">The array of booleans to be associated with the specified key.</param>
		void PutBoolArray(string key, bool[] val);

		/// <summary>
		/// Associates the passed ByteArray object with the specified key in this object.
		/// </summary>
		///
		/// <remarks>
		/// <b>IMPORTANT</b>: ByteArrays transmission is not supported in Unity WebGL.
		/// </remarks>
		///
		/// <param name="key">The key with which the specified object is to be associated.</param>
		/// <param name="val">The object to be associated with the specified key.</param>
		void PutByteArray(string key, ByteArray val);

		/// <summary>
		/// Associates the passed array of shorts with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified array is to be associated.</param>
		/// <param name="val">The array of shorts to be associated with the specified key.</param>
		void PutShortArray(string key, short[] val);

		/// <summary>
		/// Associates the passed array of integers with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified array is to be associated.</param>
		/// <param name="val">The array of integers to be associated with the specified key.</param>
		void PutIntArray(string key, int[] val);

		/// <summary>
		/// Associates the passed array of longs with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified array is to be associated.</param>
		/// <param name="val">The array of longs to be associated with the specified key.</param>
		void PutLongArray(string key, long[] val);

		/// <summary>
		/// Associates the passed array of floats with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified array is to be associated.</param>
		/// <param name="val">The array of floats to be associated with the specified key.</param>
		void PutFloatArray(string key, float[] val);

		/// <summary>
		/// Associates the passed array of doubles with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified array is to be associated.</param>
		/// <param name="val">The array of doubles to be associated with the specified key.</param>
		void PutDoubleArray(string key, double[] val);

		/// <summary>
		/// Associates the passed array of UTF-8 strings with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified array is to be associated.</param>
		/// <param name="val">The array of UTF-8 strings to be associated with the specified key.</param>
		void PutUtfStringArray(string key, string[] val);

		/// <summary>
		/// Associates the passed ISFSArray object with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified object is to be associated.</param>
		/// <param name="val">The object to be associated with the specified key.</param>
		void PutSFSArray(string key, ISFSArray val);

		/// <summary>
		/// Associates the passed ISFSObject object with the specified key in this object.
		/// </summary>
		///
		/// <param name="key">The key with which the specified object is to be associated.</param>
		/// <param name="val">The object to be associated with the specified key.</param>
		void PutSFSObject(string key, ISFSObject val);

		/// <summary>
		/// Associates the passed custom class instance with the specified key in this object.
		/// </summary>
		///
		/// <remarks>
		/// Read the <see cref="M:Sfs2X.Entities.Data.ISFSObject.GetClass(System.String)" /> method description for more informations.
		/// <para />
		/// <b>IMPORTANT</b>: class serialization is not supported in Unity WebGL.
		/// </remarks>
		///
		/// <param name="key">The key with which the specified custom class instance is to be associated.</param>
		/// <param name="val">The custom class instance to be associated with the specified key.</param>
		///
		/// <seealso cref="M:Sfs2X.Entities.Data.ISFSObject.GetClass(System.String)" />
		void PutClass(string key, object val);

		/// <exclude />
		void Put(string key, SFSDataWrapper val);
	}
}
