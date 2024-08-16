using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Sfs2X.WebSocketSharp.Net
{
	/// <summary>
	/// The exception that is thrown when a <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" /> gets an error.
	/// </summary>
	[Serializable]
	public class CookieException : FormatException, ISerializable
	{
		internal CookieException(string message)
			: base(message)
		{
		}

		internal CookieException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.CookieException" /> class from
		/// the specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> and <see cref="T:System.Runtime.Serialization.StreamingContext" />.
		/// </summary>
		/// <param name="serializationInfo">
		/// A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that contains the serialized object data.
		/// </param>
		/// <param name="streamingContext">
		/// A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that specifies the source for the deserialization.
		/// </param>
		protected CookieException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.CookieException" /> class.
		/// </summary>
		public CookieException()
		{
		}

		/// <summary>
		/// Populates the specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize
		/// the current <see cref="T:Sfs2X.WebSocketSharp.Net.CookieException" />.
		/// </summary>
		/// <param name="serializationInfo">
		/// A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data.
		/// </param>
		/// <param name="streamingContext">
		/// A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that specifies the destination for the serialization.
		/// </param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			base.GetObjectData(serializationInfo, streamingContext);
		}

		/// <summary>
		/// Populates the specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize
		/// the current <see cref="T:Sfs2X.WebSocketSharp.Net.CookieException" />.
		/// </summary>
		/// <param name="serializationInfo">
		/// A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data.
		/// </param>
		/// <param name="streamingContext">
		/// A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that specifies the destination for the serialization.
		/// </param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter, SerializationFormatter = true)]
		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			base.GetObjectData(serializationInfo, streamingContext);
		}
	}
}
