namespace Sfs2X.WebSocketSharp
{
	/// <summary>
	/// Indicates the WebSocket frame type.
	/// </summary>
	/// <remarks>
	/// The values of this enumeration are defined in
	/// <see href="http://tools.ietf.org/html/rfc6455#section-5.2">
	/// Section 5.2</see> of RFC 6455.
	/// </remarks>
	internal enum Opcode : byte
	{
		/// <summary>
		/// Equivalent to numeric value 0. Indicates continuation frame.
		/// </summary>
		Cont = 0,
		/// <summary>
		/// Equivalent to numeric value 1. Indicates text frame.
		/// </summary>
		Text = 1,
		/// <summary>
		/// Equivalent to numeric value 2. Indicates binary frame.
		/// </summary>
		Binary = 2,
		/// <summary>
		/// Equivalent to numeric value 8. Indicates connection close frame.
		/// </summary>
		Close = 8,
		/// <summary>
		/// Equivalent to numeric value 9. Indicates ping frame.
		/// </summary>
		Ping = 9,
		/// <summary>
		/// Equivalent to numeric value 10. Indicates pong frame.
		/// </summary>
		Pong = 10
	}
}
