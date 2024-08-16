using System;

namespace Sfs2X.WebSocketSharp
{
	/// <summary>
	/// Represents the event data for the <see cref="E:Sfs2X.WebSocketSharp.WebSocket.OnMessage" /> event.
	/// </summary>
	/// <remarks>
	///   <para>
	///   That event occurs when the <see cref="T:Sfs2X.WebSocketSharp.WebSocket" /> receives
	///   a message or a ping if the <see cref="P:Sfs2X.WebSocketSharp.WebSocket.EmitOnPing" />
	///   property is set to <c>true</c>.
	///   </para>
	///   <para>
	///   If you would like to get the message data, you should access
	///   the <see cref="P:Sfs2X.WebSocketSharp.MessageEventArgs.Data" /> or <see cref="P:Sfs2X.WebSocketSharp.MessageEventArgs.RawData" /> property.
	///   </para>
	/// </remarks>
	public class MessageEventArgs : EventArgs
	{
		private string _data;

		private bool _dataSet;

		private Opcode _opcode;

		private byte[] _rawData;

		/// <summary>
		/// Gets the opcode for the message.
		/// </summary>
		/// <value>
		/// <see cref="F:Sfs2X.WebSocketSharp.Opcode.Text" />, <see cref="F:Sfs2X.WebSocketSharp.Opcode.Binary" />,
		/// or <see cref="F:Sfs2X.WebSocketSharp.Opcode.Ping" />.
		/// </value>
		internal Opcode Opcode
		{
			get
			{
				return _opcode;
			}
		}

		/// <summary>
		/// Gets the message data as a <see cref="T:System.String" />.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the message data if its type is
		/// text or ping and if decoding it to a string has successfully done;
		/// otherwise, <see langword="null" />.
		/// </value>
		public string Data
		{
			get
			{
				setData();
				return _data;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the message type is binary.
		/// </summary>
		/// <value>
		/// <c>true</c> if the message type is binary; otherwise, <c>false</c>.
		/// </value>
		public bool IsBinary
		{
			get
			{
				return _opcode == Opcode.Binary;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the message type is ping.
		/// </summary>
		/// <value>
		/// <c>true</c> if the message type is ping; otherwise, <c>false</c>.
		/// </value>
		public bool IsPing
		{
			get
			{
				return _opcode == Opcode.Ping;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the message type is text.
		/// </summary>
		/// <value>
		/// <c>true</c> if the message type is text; otherwise, <c>false</c>.
		/// </value>
		public bool IsText
		{
			get
			{
				return _opcode == Opcode.Text;
			}
		}

		/// <summary>
		/// Gets the message data as an array of <see cref="T:System.Byte" />.
		/// </summary>
		/// <value>
		/// An array of <see cref="T:System.Byte" /> that represents the message data.
		/// </value>
		public byte[] RawData
		{
			get
			{
				setData();
				return _rawData;
			}
		}

		internal MessageEventArgs(WebSocketFrame frame)
		{
			_opcode = frame.Opcode;
			_rawData = frame.PayloadData.ApplicationData;
		}

		internal MessageEventArgs(Opcode opcode, byte[] rawData)
		{
			if ((ulong)rawData.LongLength > PayloadData.MaxLength)
			{
				throw new WebSocketException(CloseStatusCode.TooBig);
			}
			_opcode = opcode;
			_rawData = rawData;
		}

		private void setData()
		{
			if (!_dataSet)
			{
				if (_opcode == Opcode.Binary)
				{
					_dataSet = true;
					return;
				}
				_data = _rawData.UTF8Decode();
				_dataSet = true;
			}
		}
	}
}
