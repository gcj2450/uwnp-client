using System;

namespace Sfs2X.WebSocketSharp
{
	/// <summary>
	/// Represents the event data for the <see cref="E:Sfs2X.WebSocketSharp.WebSocket.OnClose" /> event.
	/// </summary>
	/// <remarks>
	///   <para>
	///   That event occurs when the WebSocket connection has been closed.
	///   </para>
	///   <para>
	///   If you would like to get the reason for the close, you should access
	///   the <see cref="P:Sfs2X.WebSocketSharp.CloseEventArgs.Code" /> or <see cref="P:Sfs2X.WebSocketSharp.CloseEventArgs.Reason" /> property.
	///   </para>
	/// </remarks>
	public class CloseEventArgs : EventArgs
	{
		private bool _clean;

		private PayloadData _payloadData;

		internal PayloadData PayloadData
		{
			get
			{
				return _payloadData;
			}
		}

		/// <summary>
		/// Gets the status code for the close.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.UInt16" /> that represents the status code for the close if any.
		/// </value>
		public ushort Code
		{
			get
			{
				return _payloadData.Code;
			}
		}

		/// <summary>
		/// Gets the reason for the close.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the reason for the close if any.
		/// </value>
		public string Reason
		{
			get
			{
				return _payloadData.Reason ?? string.Empty;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the connection has been closed cleanly.
		/// </summary>
		/// <value>
		/// <c>true</c> if the connection has been closed cleanly; otherwise, <c>false</c>.
		/// </value>
		public bool WasClean
		{
			get
			{
				return _clean;
			}
			internal set
			{
				_clean = value;
			}
		}

		internal CloseEventArgs()
		{
			_payloadData = PayloadData.Empty;
		}

		internal CloseEventArgs(ushort code)
			: this(code, null)
		{
		}

		internal CloseEventArgs(CloseStatusCode code)
			: this((ushort)code, null)
		{
		}

		internal CloseEventArgs(PayloadData payloadData)
		{
			_payloadData = payloadData;
		}

		internal CloseEventArgs(ushort code, string reason)
		{
			_payloadData = new PayloadData(code, reason);
		}

		internal CloseEventArgs(CloseStatusCode code, string reason)
			: this((ushort)code, reason)
		{
		}
	}
}
