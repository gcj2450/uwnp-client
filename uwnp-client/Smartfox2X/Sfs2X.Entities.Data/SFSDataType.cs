namespace Sfs2X.Entities.Data
{
	/// <summary>
	/// The SFSDataType class contains the costants defining the data types supported by SFSObject and SFSArray classes.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Entities.Data.SFSObject" />
	/// <seealso cref="T:Sfs2X.Entities.Data.SFSArray" />
	public enum SFSDataType
	{
		/// <summary>
		/// Null value.
		/// </summary>
		NULL,
		/// <summary>
		/// Boolean.
		/// </summary>
		BOOL,
		/// <summary>
		/// Byte, signed 8 bits.
		/// </summary>
		BYTE,
		/// <summary>
		/// Short integer, signed 16 bits.
		/// </summary>
		SHORT,
		/// <summary>
		/// Integer, signed 32 bits.
		/// </summary>
		INT,
		/// <summary>
		/// Long integer, signed 64 bits.
		/// </summary>
		LONG,
		/// <summary>
		/// Floating point decimal, signed 32 bits.
		/// </summary>
		FLOAT,
		/// <summary>
		/// Double precision decimal, signed 64 bits.
		/// </summary>
		DOUBLE,
		/// <summary>
		/// UTF-8 encoded string, with length up to 32 KBytes.
		/// </summary>
		UTF_STRING,
		/// <summary>
		/// Array of booleans.
		/// </summary>
		BOOL_ARRAY,
		/// <summary>
		/// Array of bytes (treated as ByteArray).
		/// </summary>
		BYTE_ARRAY,
		/// <summary>
		/// Array of shorts.
		/// </summary>
		SHORT_ARRAY,
		/// <summary>
		/// Array of integers.
		/// </summary>
		INT_ARRAY,
		/// <summary>
		/// Array of long integers.
		/// </summary>
		LONG_ARRAY,
		/// <summary>
		/// Array of floats.
		/// </summary>
		FLOAT_ARRAY,
		/// <summary>
		/// Array of Doubles.
		/// </summary>
		DOUBLE_ARRAY,
		/// <summary>
		/// Array of UTF-8 strings.
		/// </summary>
		UTF_STRING_ARRAY,
		/// <summary>
		/// SFSArray.
		/// </summary>
		///
		/// <seealso cref="T:Sfs2X.Entities.Data.SFSArray" />
		SFS_ARRAY,
		/// <summary>
		/// SFSObject.
		/// </summary>
		///
		/// <seealso cref="T:Sfs2X.Entities.Data.SFSObject" />
		SFS_OBJECT,
		/// <summary>
		/// Serialized class instance.
		/// </summary>
		CLASS,
		/// <summary>
		/// UTF-8 encoded string, with length up to 2 GBytes.
		/// </summary>
		TEXT
	}
}
