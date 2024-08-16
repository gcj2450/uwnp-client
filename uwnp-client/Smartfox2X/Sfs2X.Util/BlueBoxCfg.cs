namespace Sfs2X.Util
{
	/// <summary>
	/// The BlueBoxCfg class stores the configuration parameters for the BlueBox connection. When a socket connection fails the BlueBox can help the client connect to the server via HTTP tunnel.
	/// </summary>
	public class BlueBoxCfg
	{
		/// <summary>
		/// Indicates whether the SmartFoxServer's BlueBox should be enabled or not.
		/// </summary>
		///
		/// <remarks>
		/// The default value is <c>true</c>.
		/// </remarks>
		private bool isActive = true;
        public bool IsActive { get { return isActive; } set { isActive = value; } }

        /// <summary>
        /// Forces the BlueBox connection to use an HTTPS tunnel instead of standard HTTP.
        /// </summary>
        ///
        /// <remarks>
        /// <b>IMPORTANT NOTE</b>: this mode should be used exclusively if you're deploying your app to iOS devices, since Apple doesn't accept HTTP connections.
        /// All other platforms should <b>always use standard HTTP</b>, because the protocol is already encrypted when cryptography is activated. In other words the BlueBox transmits an encrypted protocol over a standard HTTP tunnel, which is perfectly secure.
        /// </remarks>
        private bool useHttps = false;
        public bool UseHttps { get { return useHttps; } set { useHttps = value; } }

        /// <summary>
        /// Specifies the BlueBox polling speed.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is <c>750</c>.
        /// </remarks>
        private int pollingRate = 750;
        public int PollingRate { get { return pollingRate; } set { pollingRate = value; } }

        /// <summary>
        /// Proxy server settings for the BlueBox.  
        /// </summary>
        ///
        /// <remarks>
        /// These should be used only if the client can only access HTTP services via a proxy server.
        /// Connection via proxy server is not supported under Universal Windows Platform.
        /// </remarks>
        public ProxyCfg proxy  = new ProxyCfg();
        public ProxyCfg Proxy { get { return proxy; } set { proxy = value; } }
    }
}
