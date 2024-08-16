using System;

namespace Sfs2X.WebSocketSharp
{
	/// <summary>
	/// Represents the event data for the <see cref="E:Sfs2X.WebSocketSharp.WebSocket.OnError" /> event.
	/// </summary>
	/// <remarks>
	///   <para>
	///   That event occurs when the <see cref="T:Sfs2X.WebSocketSharp.WebSocket" /> gets an error.
	///   </para>
	///   <para>
	///   If you would like to get the error message, you should access
	///   the <see cref="P:Sfs2X.WebSocketSharp.ErrorEventArgs.Message" /> property.
	///   </para>
	///   <para>
	///   And if the error is due to an exception, you can get it by accessing
	///   the <see cref="P:Sfs2X.WebSocketSharp.ErrorEventArgs.Exception" /> property.
	///   </para>
	/// </remarks>
	public class ErrorEventArgs : EventArgs
	{
		private Exception _exception;

		private string _message;

		/// <summary>
		/// Gets the exception that caused the error.
		/// </summary>
		/// <value>
		/// An <see cref="T:System.Exception" /> instance that represents the cause of
		/// the error if it is due to an exception; otherwise, <see langword="null" />.
		/// </value>
		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the error message.
		/// </value>
		public string Message
		{
			get
			{
				return _message;
			}
		}

		internal ErrorEventArgs(string message)
			: this(message, null)
		{
		}

		internal ErrorEventArgs(string message, Exception exception)
		{
			_message = message;
			_exception = exception;
		}
	}
}
