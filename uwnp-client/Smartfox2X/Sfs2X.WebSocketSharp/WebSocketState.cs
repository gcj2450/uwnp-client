namespace Sfs2X.WebSocketSharp
{
	/// <summary>
	/// Indicates the state of a WebSocket connection.
	/// </summary>
	/// <remarks>
	/// The values of this enumeration are defined in
	/// <see href="http://www.w3.org/TR/websockets/#dom-websocket-readystate">
	/// The WebSocket API</see>.
	/// </remarks>
	public enum WebSocketState : ushort
	{
		/// <summary>
		/// Equivalent to numeric value 0. Indicates that the connection has not
		/// yet been established.
		/// </summary>
		Connecting,
		/// <summary>
		/// Equivalent to numeric value 1. Indicates that the connection has
		/// been established, and the communication is possible.
		/// </summary>
		Open,
		/// <summary>
		/// Equivalent to numeric value 2. Indicates that the connection is
		/// going through the closing handshake, or the close method has
		/// been invoked.
		/// </summary>
		Closing,
		/// <summary>
		/// Equivalent to numeric value 3. Indicates that the connection has
		/// been closed or could not be established.
		/// </summary>
		Closed
	}
}
