namespace Sfs2X.Util
{
	/// <summary>
	/// The ProxyCfg class stores the proxy server settings for the BlueBox.
	/// </summary>
	///
	/// <remarks>
	/// Connection via proxy server is not supported under Universal Windows Platform.
	/// </remarks>
	public class ProxyCfg
	{
		/// <summary>
		/// Specifies the IP address of the proxy server.
		/// </summary>
		public string Host { get; set; }

		private int port = 0;
		/// <summary>
		/// Specifies the port number of the proxy server.
		/// </summary>
		public int Port { get { return port; } set { port = value; } }

		private bool bypassLocal=true;
        /// <summary>
        /// Indicates whether the proxy server should be bypassed for local addresses or not.
        /// </summary>
        ///
        /// <remarks>
        /// Default value is <c>true</c>.
        /// </remarks>
        public bool BypassLocal { get { return bypassLocal; } set { bypassLocal = value; } }


		/// <summary>
		/// User name for the proxy server authentication.
		/// </summary>
		///
		/// <remarks>
		/// Use only if the proxy server requires authentication.
		/// </remarks>
		public string UserName { get; set; }

		/// <summary>
		/// Password for the proxy server authentication.
		/// </summary>
		/// /// 
		/// <remarks>
		/// Use only if the proxy server requires authentication.
		/// </remarks>
		public string Password { get; set; }
	}
}
