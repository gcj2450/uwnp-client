namespace Sfs2X.WebSocketSharp.Net
{
	/// <summary>
	/// Specifies the scheme for authentication.
	/// </summary>
	public enum AuthenticationSchemes
	{
		/// <summary>
		/// No authentication is allowed.
		/// </summary>
		None = 0,
		/// <summary>
		/// Specifies digest authentication.
		/// </summary>
		Digest = 1,
		/// <summary>
		/// Specifies basic authentication.
		/// </summary>
		Basic = 8,
		/// <summary>
		/// Specifies anonymous authentication.
		/// </summary>
		Anonymous = 0x8000
	}
}
