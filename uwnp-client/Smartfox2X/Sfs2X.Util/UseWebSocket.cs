namespace Sfs2X.Util
{
	/// <summary>
	/// The available websocket connection modes to be passed to the SmartFox class constructor.
	/// </summary>
	///
	/// <seealso cref="M:Sfs2X.SmartFox.#ctor(Sfs2X.Util.UseWebSocket)" />
	public enum UseWebSocket
	{
		/// <summary>
		/// Unsecure, text-type websocket communication should be established when connecting to a SmartFoxServer 2X instance.
		/// </summary>
		WS,
		/// <summary>
		/// Secure, text-type websocket communication should be established when connecting to a SmartFoxServer 2X instance.
		/// </summary>
		WSS,
		/// <summary>
		/// Unsecure, binary-type websocket communication should be established when connecting to a SmartFoxServer 2X instance.
		/// </summary>
		WS_BIN,
		/// <summary>
		/// Secure, binary-type websocket communication should be established when connecting to a SmartFoxServer 2X instance.
		/// </summary>
		WSS_BIN
	}
}
