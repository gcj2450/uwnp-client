using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Sfs2X.WebSocketSharp.Net
{
	/// <summary>
	/// The exception that is thrown when a <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListener" /> gets an error
	/// processing an HTTP request.
	/// </summary>
	[Serializable]
	public class HttpListenerException : Win32Exception
	{
		/// <summary>
		/// Gets the error code that identifies the error that occurred.
		/// </summary>
		/// <value>
		/// An <see cref="T:System.Int32" /> that identifies the error.
		/// </value>
		public override int ErrorCode
		{
			get
			{
				return base.NativeErrorCode;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerException" /> class from
		/// the specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> and <see cref="T:System.Runtime.Serialization.StreamingContext" />.
		/// </summary>
		/// <param name="serializationInfo">
		/// A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that contains the serialized object data.
		/// </param>
		/// <param name="streamingContext">
		/// A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that specifies the source for the deserialization.
		/// </param>
		protected HttpListenerException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerException" /> class.
		/// </summary>
		public HttpListenerException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerException" /> class
		/// with the specified <paramref name="errorCode" />.
		/// </summary>
		/// <param name="errorCode">
		/// An <see cref="T:System.Int32" /> that identifies the error.
		/// </param>
		public HttpListenerException(int errorCode)
			: base(errorCode)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerException" /> class
		/// with the specified <paramref name="errorCode" /> and <paramref name="message" />.
		/// </summary>
		/// <param name="errorCode">
		/// An <see cref="T:System.Int32" /> that identifies the error.
		/// </param>
		/// <param name="message">
		/// A <see cref="T:System.String" /> that describes the error.
		/// </param>
		public HttpListenerException(int errorCode, string message)
			: base(errorCode, message)
		{
		}
	}
}
