using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreServer.UJNet_Framework.UJNet.Data
{
    public interface ISFSArray : ICollection, IEnumerable
    {
        /// <summary>
        /// Indicates whether this array contains the specified object or not.
        /// </summary>
        ///
        /// <param name="obj">The object whose presence in this array is to be tested.</param>
        ///
        /// <returns><c>true</c> if the specified object is present.</returns>
        bool Contains(object obj);

        /// <summary>
        /// Returns the element at the specified position in this array.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element at the specified index in this array.</returns>
        object GetElementAt(int index);

        /// <exclude />
        SFSDataWrapper GetWrappedElementAt(int index);

        //SFSDataWrapper get(int paramInt);

        /// <summary>
        /// Removes the element at the specified position in this array.
        /// </summary>
        ///
        /// <param name="index">The position of the element to be removed.</param>
        ///
        /// <returns>The element that was removed.</returns>
        object RemoveElementAt(int index);

        /// <summary>
        /// Indicates the number of elements in this array.
        /// </summary>
        ///
        /// <returns>The number of elements in this array.</returns>
        int Size();

        /// <summary>
        /// Provides the binary form of this array.
        /// </summary>
        ///
        /// <returns>The binary data representing this array.</returns>
        ByteArray ToBinary();

        /// <summary>
        /// Provides the JSON representation of this array.
        /// </summary>
        ///
        /// <remarks>
        /// This method is not available under Universal Windows Platform.
        /// </remarks>
        ///
        /// <returns>The JSON string representing this array.</returns>
        ///
        /// <remarks>This method is not available under Universal Windows Platform.</remarks>
        string ToJson();

        /// <summary>
        /// Provides a formatted string representing this array.
        /// </summary>
        ///
        /// <remarks>
        /// The returned string can be logged or traced in the console for debugging purposes.
        /// </remarks>
        ///
        /// <param name="format">If <c>true</c>, the output is formatted in a human-readable way.</param>
        ///
        /// <returns>The string representation of this array.</returns>
        string GetDump(bool format);

        /// <summary>
        /// See <see cref="M:Sfs2X.Entities.Data.ISFSArray.GetDump(System.Boolean)" />.
        /// </summary>
        string GetDump();

        /// <summary>
        /// Provides a detailed hexadecimal representation of this array.
        /// </summary>
        ///
        /// <remarks>
        /// The returned string can be logged or traced in the console for debugging purposes.
        /// </remarks>
        ///
        /// <returns>The hexadecimal string representation of this array.</returns>
        string GetHexDump();

        /// <summary>
        /// Appends a <c>null</c> value to the end of this array.
        /// </summary>
        void AddNull();

        /// <summary>
        /// Appends a boolean value to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The value to be appended to this array.</param>
        void AddBool(bool val);

        /// <summary>
        /// Appends a byte (8 bits) value to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The value to be appended to this array.</param>
        void AddByte(byte val);

        /// <summary>
        /// Appends a short integer (16 bits) value to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The value to be appended to this array.</param>
        void AddShort(short val);

        /// <summary>
        /// Appends an integer (32 bits) value to the end of this array.
        /// </summary>
        /// <param name="val">The value to be appended to this array.</param>
        void AddInt(int val);

        /// <summary>
        /// Appends a long integer (64 bits) value to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The value to be appended to this array.</param>
        void AddLong(long val);

        /// <summary>
        /// Appends a floating point number (32 bits) value to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The value to be appended to this array.</param>
        void AddFloat(float val);

        /// <summary>
        /// Appends a double precision number (64 bits) value to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The value to be appended to this array.</param>
        void AddDouble(double val);

        /// <summary>
        /// Appends a UTF-8 string (with max length of 32 KBytes) value to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The value to be appended to this array.</param>
        void AddUtfString(string val);

        /// <summary>
        /// Appends a UTF-8 string (with max length of 2 GBytes) value to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The value to be appended to this array.</param>
        void AddText(string val);

        /// <summary>
        /// Appends an array of boolean values to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The array of booleans to be appended to this array.</param>
        void AddBoolArray(bool[] val);

        /// <summary>
        /// Appends a ByteArray object to the end of this array.
        /// </summary>
        ///
        /// <remarks>
        /// <b>IMPORTANT</b>: ByteArrays transmission is not supported in Unity WebGL.
        /// </remarks>
        ///
        /// <param name="val">The ByteArray object to be appended to this array.</param>
        void AddByteArray(ByteArray val);

        /// <summary>
        /// Appends an array of short integer values to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The array of shorts to be appended to this array.</param>
        void AddShortArray(short[] val);

        /// <summary>
        /// Appends an array of integer values to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The array of ints to be appended to this array.</param>
        void AddIntArray(int[] val);

        /// <summary>
        /// Appends an array of long integer values to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The array of longs to be appended to this array.</param>
        void AddLongArray(long[] val);

        /// <summary>
        /// Appends an array of floating point number values to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The array of floats to be appended to this array.</param>
        void AddFloatArray(float[] val);

        /// <summary>
        /// Appends an array of double precision number values to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The array of doubles to be appended to this array.</param>
        void AddDoubleArray(double[] val);

        /// <summary>
        /// Appends an array of UTF-8 string values to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The array of strings to be appended to this array.</param>
        void AddUtfStringArray(string[] val);

        /// <summary>
        /// Appends an ISFSArray object to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The object implementing the ISFSArray interface to be appended to this array.</param>
        ///
        /// <seealso cref="T:Sfs2X.Entities.Data.SFSArray" />
        void AddSFSArray(ISFSArray val);

        /// <summary>
        /// Appends a ISFSObject object to the end of this array.
        /// </summary>
        ///
        /// <param name="val">The object implementing the ISFSObject interface to be appended to this array.</param>
        ///
        /// <seealso cref="T:Sfs2X.Entities.Data.SFSObject" />
        void AddSFSObject(ISFSObject val);

        /// <summary>
        /// Appends the passed custom class instance to the end of this array.
        /// </summary>
        ///
        /// <remarks>
        /// Read the <see cref="M:Sfs2X.Entities.Data.ISFSArray.GetClass(System.Int32)" /> method description for more informations.
        /// <para />
        /// <b>IMPORTANT</b>: class serialization is not supported in Unity WebGL.
        /// </remarks>
        ///
        /// <param name="val">The custom class instance to be appended to this array.</param>
        void AddClass(object val);

        /// <exclude />
        void Add(SFSDataWrapper val);

        /// <summary>
        /// Indicates if the element at the specified position in this array is <c>null</c>.
        /// </summary>
        ///
        /// <param name="index">The position of the element to be checked.</param>
        ///
        /// <returns><c>true</c> if the element of this array at the specified position is <c>null</c>.</returns>
        bool IsNull(int index);

        /// <summary>
        /// Returns the element at the specified position as a boolean.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array at the specified index.</returns>
        bool GetBool(int index);

        /// <summary>
        /// Returns the element at the specified position as a signed byte (8 bits).
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array at the specified index.</returns>
        byte GetByte(int index);

        //int getUnsignedByte(int paramInt);aa

        /// <summary>
        /// Returns the element at the specified position as a short integer (16 bits).
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array at the specified index.</returns>
        short GetShort(int index);

        /// <summary>
        /// Returns the element at the specified position as an integer (32 bits).
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array at the specified index.</returns>
        int GetInt(int index);

        /// <summary>
        /// Returns the element at the specified position as a long integer (64 bits).
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array at the specified index.</returns>
        long GetLong(int index);

        /// <summary>
        /// Returns the element at the specified position as a floating point number.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array at the specified index.</returns>
        float GetFloat(int index);

        /// <summary>
        /// Returns the element at the specified position as a double precision number.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array at the specified index.</returns>
        double GetDouble(int index);

        /// <summary>
        /// Returns the element at the specified position as an UTF-8 string, with max length of 32 KBytes.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array at the specified index.</returns>
        string GetUtfString(int index);

        /// <summary>
        /// Returns the element at the specified position as an UTF-8 string, with max length of 2 GBytes.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array at the specified index.</returns>
        string GetText(int index);

        /// <summary>
        /// Returns the element at the specified position as an array of booleans.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array as an array of booleans.</returns>
        bool[] GetBoolArray(int index);

        /// <summary>
        /// Returns the element at the specified position as a ByteArray object.
        /// </summary>
        ///
        /// <remarks>
        /// <b>IMPORTANT</b>: ByteArrays transmission is not supported in Unity WebGL.
        /// </remarks>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array as a ByteArray object.</returns>
        ByteArray GetByteArray(int index);

        //int[] getUnsignedByteArray(int paramInt);			ssss

        /// <summary>
        /// Returns the element at the specified position as an array of shorts.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array as an array of shorts.</returns>
        short[] GetShortArray(int index);

        /// <summary>
        /// Returns the element at the specified position as an array of integers.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array as an array of integers.</returns>
        int[] GetIntArray(int index);

        /// <summary>
        /// Returns the element at the specified position as an array of longs.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array as an array of longs.</returns>
        long[] GetLongArray(int index);

        /// <summary>
        /// Returns the element at the specified position as an array of floats.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array as an array of floats.</returns>
        float[] GetFloatArray(int index);

        /// <summary>
        /// Returns the element at the specified position as an array of doubles.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array as an array of doubles.</returns>
        double[] GetDoubleArray(int index);

        /// <summary>
        /// Returns the element at the specified position as an array of UTF-8 strings.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array as an array of UTF-8 strings.</returns>
        string[] GetUtfStringArray(int index);

        /// <summary>
        /// Returns the element at the specified position as an ISFSArray object.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array as an object implementing the ISFSArray interface.</returns>
        ///
        /// <seealso cref="T:Sfs2X.Entities.Data.SFSArray" />
        ISFSArray GetSFSArray(int index);

        /// <summary>
        /// Returns the element at the specified position as an ISFSObject object.
        /// </summary>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array as an object implementing the ISFSObject interface.</returns>
        ///
        /// <seealso cref="T:Sfs2X.Entities.Data.SFSObject" />
        ISFSObject GetSFSObject(int index);

        /// <summary>
        /// Returns the element at the specified position as an instance of a custom class.
        /// </summary>
        ///
        /// <remarks>
        /// This advanced feature allows the transmission of specific object instances between client-side C# and server-side Java provided that:<br />
        /// - the respective class definitions on both sides have the same package name<br />
        /// - the class implements the SerializableSFSType interface on both sides<br />
        /// - the following code is executed right after creating the SmartFox object: <c>DefaultSFSDataSerializer.RunningAssembly = Assembly.GetExecutingAssembly();</c> (requires <c>System.Reflection</c> and <c>Sfs2X.Protocol.Serialization</c>)
        /// <para />
        /// <b>IMPORTANT</b>: class serialization is not supported in Unity WebGL.
        /// </remarks>
        ///
        /// <param name="index">The position of the element to return.</param>
        ///
        /// <returns>The element of this array as a generic object type to be casted to the target class definition.</returns>
        ///
        /// <example>
        /// The following example shows the same class on the client and server sides, which can be transferred back and forth with the <see cref="M:Sfs2X.Entities.Data.ISFSArray.GetClass(System.Int32)" /> and <see cref="M:Sfs2X.Entities.Data.ISFSArray.AddClass(System.Object)" /> methods.
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
        /// A SpaceShip instance is sent by the server to the client in the first position of an array. This is how to retrieve it:
        /// <code>
        /// 	SpaceShip myShipData = (SpaceShip)sfsArray.GetClass(0);
        /// </code>
        /// </example>
        object GetClass(int index);
    }
}
