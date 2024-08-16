using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sfs2X.Util
{
	/// <summary>
	/// The ConfigData class stores the client configuration data loaded from an external XML file or passed directly to the deputy connect method.
	/// </summary>
	///
	/// <remarks>
	/// The external configuration file is loaded by the <em>SmartFox</em> class when its <see cref="M:Sfs2X.SmartFox.LoadConfig">SmartFox.LoadConfig()</see> method is called.
	/// Otherwise it can be passed directly to one of the <see cref="M:Sfs2X.SmartFox.Connect(Sfs2X.Util.ConfigData)">SmartFox.Connect(ConfigData)</see> method overloads of the <em>SmartFox</em> class.
	/// </remarks>
	///
	/// <seealso cref="P:Sfs2X.SmartFox.Config" />
	public class ConfigData
	{
		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly BlueBoxCfg _003CBlueBox_003Ek__BackingField = new BlueBoxCfg();

		/// <summary>
		/// Specifies the IP address or host name of the SmartFoxServer 2X instance to connect to (TCP connection).
		/// </summary>
		///
		/// <remarks>
		/// The default value is <c>127.0.0.1</c>.
		/// <para />
		/// When using a websocket connection to an IPv6 address, always wrap the address in square brackets.
		/// </remarks>
		public string Host { get { return host; } set { host = value; } }
		private string host = "127.0.0.1";

		/// <summary>
		/// Specifies the TCP port of the SmartFoxServer 2X instance to connect to (TCP connection).
		/// </summary>
		///
		/// <remarks>
		/// The default value is <c>9933</c>.
		/// </remarks>
		public int Port { get { return port; } set { port = value; } }
		private int port = 9933;

		/// <summary>
		/// Specifies the IP address of the SmartFoxServer 2X instance to connect to (UDP connection).
		/// </summary>
		///
		/// <remarks>
		/// The default value is <c>127.0.0.1</c>.
		/// </remarks>
        public string UdpHost { get { return udpHost; } set { udpHost = value; } }
        private string udpHost = "127.0.0.1";

        /// <summary>
        /// Specifies the UDP port of the SmartFoxServer 2X instance to connect to (UDP connection).
        /// </summary>
        ///
        /// <remarks>
        /// The default value is <c>9933</c>.
        /// </remarks>
        public int UdpPort { get { return udpPort; } set { udpPort = value; } }
        private int udpPort = 9933;

        /// <summary>
        /// Specifies the Zone of the SmartFoxServer 2X instance to join.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is <c>null</c>.
        /// </remarks>
        public string Zone { get; set; }

        /// <summary>
        /// Indicates whether the client-server messages debug should be enabled or not.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is <c>false</c>.
        /// </remarks>
        private bool debug = false;
        public bool Debug { get { return debug; } set { debug = value; } }


        /// <summary>
        /// Specifies the port for generic HTTP communication.
        /// </summary>
        ///
        /// <remarks>
        /// The default value is <c>8080</c>.
        /// </remarks>
        private int httpPort = 8080;
        public int HttpPort { get { return httpPort; } set { httpPort = value; } }

        /// <summary>
        /// Specifies the port for HTTPS communication.
        /// </summary>
        ///
        /// <remarks>
        /// For example this parameter is required during the initialization of an encrypted connection.
        /// The default value is <c>8443</c>.
        /// </remarks>
        public int httpsPort = 8443;
        public int HttpsPort { get { return httpsPort; } set { httpsPort = value; } }

        /// <summary>
        /// Indicates whether SmartFoxServer's TCP socket is using the Nagle algorithm or not.
        /// </summary>
        ///
        /// <remarks>
        /// This setting must be <c>false</c> to use the Nagle algorithm; otherwise, <c>true</c>.
        /// The default value is <c>false</c>.
        /// </remarks>
        private bool tcpNoDelay = false;
        public bool TcpNoDelay { get { return tcpNoDelay; } set { tcpNoDelay = value; } }

        /// <summary>
        /// Configuration parameters for the BlueBox connection. When a socket connection fails the BlueBox can help the client connect to the server via HTTP tunnel.
        /// </summary>
        public BlueBoxCfg BlueBox
		{
			[CompilerGenerated]
			get
			{
				return _003CBlueBox_003Ek__BackingField;
			}
		}
	}
}
