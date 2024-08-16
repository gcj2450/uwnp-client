using System;

namespace Sfs2X.WebSocketSharp.Net
{
	/// <summary>
	/// Provides the HTTP version numbers.
	/// </summary>
	public class HttpVersion
	{
		/// <summary>
		/// Provides a <see cref="T:System.Version" /> instance for the HTTP/1.0.
		/// </summary>
		public static readonly Version Version10 = new Version(1, 0);

		/// <summary>
		/// Provides a <see cref="T:System.Version" /> instance for the HTTP/1.1.
		/// </summary>
		public static readonly Version Version11 = new Version(1, 1);

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.HttpVersion" /> class.
		/// </summary>
		public HttpVersion()
		{
		}
	}
}
