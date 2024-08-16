using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Threading;
using Sfs2X.WebSocketSharp.Net;
using Sfs2X.WebSocketSharp.Net.WebSockets;

namespace Sfs2X.WebSocketSharp.Server
{
	/// <summary>
	/// Provides a WebSocket protocol server.
	/// </summary>
	/// <remarks>
	/// This class can provide multiple WebSocket services.
	/// </remarks>
	public class WebSocketServer
	{
		private IPAddress _address;

		private bool _allowForwardedRequest;

		private Sfs2X.WebSocketSharp.Net.AuthenticationSchemes _authSchemes;

		private static readonly string _defaultRealm;

		private bool _dnsStyle;

		private string _hostname;

		private TcpListener _listener;

		private Logger _log;

		private int _port;

		private string _realm;

		private string _realmInUse;

		private Thread _receiveThread;

		private bool _reuseAddress;

		private bool _secure;

		private WebSocketServiceManager _services;

		private ServerSslConfiguration _sslConfig;

		private ServerSslConfiguration _sslConfigInUse;

		private volatile ServerState _state;

		private object _sync;

		private Func<IIdentity, Sfs2X.WebSocketSharp.Net.NetworkCredential> _userCredFinder;

		/// <summary>
		/// Gets the IP address of the server.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Net.IPAddress" /> that represents the local
		/// IP address on which to listen for incoming handshake requests.
		/// </value>
		public IPAddress Address
		{
			get
			{
				return _address;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the server accepts every
		/// handshake request without checking the request URI.
		/// </summary>
		/// <remarks>
		/// The set operation does nothing if the server has already started or
		/// it is shutting down.
		/// </remarks>
		/// <value>
		///   <para>
		///   <c>true</c> if the server accepts every handshake request without
		///   checking the request URI; otherwise, <c>false</c>.
		///   </para>
		///   <para>
		///   The default value is <c>false</c>.
		///   </para>
		/// </value>
		public bool AllowForwardedRequest
		{
			get
			{
				return _allowForwardedRequest;
			}
			set
			{
				string message;
				if (!canSet(out message))
				{
					_log.Warn(message);
					return;
				}
				lock (_sync)
				{
					if (!canSet(out message))
					{
						_log.Warn(message);
					}
					else
					{
						_allowForwardedRequest = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the scheme used to authenticate the clients.
		/// </summary>
		/// <remarks>
		/// The set operation does nothing if the server has already started or
		/// it is shutting down.
		/// </remarks>
		/// <value>
		///   <para>
		///   One of the <see cref="T:Sfs2X.WebSocketSharp.Net.AuthenticationSchemes" />
		///   enum values.
		///   </para>
		///   <para>
		///   It represents the scheme used to authenticate the clients.
		///   </para>
		///   <para>
		///   The default value is
		///   <see cref="F:Sfs2X.WebSocketSharp.Net.AuthenticationSchemes.Anonymous" />.
		///   </para>
		/// </value>
		public Sfs2X.WebSocketSharp.Net.AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				return _authSchemes;
			}
			set
			{
				string message;
				if (!canSet(out message))
				{
					_log.Warn(message);
					return;
				}
				lock (_sync)
				{
					if (!canSet(out message))
					{
						_log.Warn(message);
					}
					else
					{
						_authSchemes = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the server has started.
		/// </summary>
		/// <value>
		/// <c>true</c> if the server has started; otherwise, <c>false</c>.
		/// </value>
		public bool IsListening
		{
			get
			{
				return _state == ServerState.Start;
			}
		}

		/// <summary>
		/// Gets a value indicating whether secure connections are provided.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance provides secure connections; otherwise,
		/// <c>false</c>.
		/// </value>
		public bool IsSecure
		{
			get
			{
				return _secure;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the server cleans up
		/// the inactive sessions periodically.
		/// </summary>
		/// <remarks>
		/// The set operation does nothing if the server has already started or
		/// it is shutting down.
		/// </remarks>
		/// <value>
		///   <para>
		///   <c>true</c> if the server cleans up the inactive sessions every
		///   60 seconds; otherwise, <c>false</c>.
		///   </para>
		///   <para>
		///   The default value is <c>true</c>.
		///   </para>
		/// </value>
		public bool KeepClean
		{
			get
			{
				return _services.KeepClean;
			}
			set
			{
				_services.KeepClean = value;
			}
		}

		/// <summary>
		/// Gets the logging function for the server.
		/// </summary>
		/// <remarks>
		/// The default logging level is <see cref="F:Sfs2X.WebSocketSharp.LogLevel.Error" />.
		/// </remarks>
		/// <value>
		/// A <see cref="T:Sfs2X.WebSocketSharp.Logger" /> that provides the logging function.
		/// </value>
		public Logger Log
		{
			get
			{
				return _log;
			}
		}

		/// <summary>
		/// Gets the port of the server.
		/// </summary>
		/// <value>
		/// An <see cref="T:System.Int32" /> that represents the number of the port
		/// on which to listen for incoming handshake requests.
		/// </value>
		public int Port
		{
			get
			{
				return _port;
			}
		}

		/// <summary>
		/// Gets or sets the realm used for authentication.
		/// </summary>
		/// <remarks>
		///   <para>
		///   "SECRET AREA" is used as the realm if the value is
		///   <see langword="null" /> or an empty string.
		///   </para>
		///   <para>
		///   The set operation does nothing if the server has
		///   already started or it is shutting down.
		///   </para>
		/// </remarks>
		/// <value>
		///   <para>
		///   A <see cref="T:System.String" /> or <see langword="null" /> by default.
		///   </para>
		///   <para>
		///   That string represents the name of the realm.
		///   </para>
		/// </value>
		public string Realm
		{
			get
			{
				return _realm;
			}
			set
			{
				string message;
				if (!canSet(out message))
				{
					_log.Warn(message);
					return;
				}
				lock (_sync)
				{
					if (!canSet(out message))
					{
						_log.Warn(message);
					}
					else
					{
						_realm = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the server is allowed to
		/// be bound to an address that is already in use.
		/// </summary>
		/// <remarks>
		///   <para>
		///   You should set this property to <c>true</c> if you would
		///   like to resolve to wait for socket in TIME_WAIT state.
		///   </para>
		///   <para>
		///   The set operation does nothing if the server has already
		///   started or it is shutting down.
		///   </para>
		/// </remarks>
		/// <value>
		///   <para>
		///   <c>true</c> if the server is allowed to be bound to an address
		///   that is already in use; otherwise, <c>false</c>.
		///   </para>
		///   <para>
		///   The default value is <c>false</c>.
		///   </para>
		/// </value>
		public bool ReuseAddress
		{
			get
			{
				return _reuseAddress;
			}
			set
			{
				string message;
				if (!canSet(out message))
				{
					_log.Warn(message);
					return;
				}
				lock (_sync)
				{
					if (!canSet(out message))
					{
						_log.Warn(message);
					}
					else
					{
						_reuseAddress = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets the configuration for secure connection.
		/// </summary>
		/// <remarks>
		/// This configuration will be referenced when attempts to start,
		/// so it must be configured before the start method is called.
		/// </remarks>
		/// <value>
		/// A <see cref="T:Sfs2X.WebSocketSharp.Net.ServerSslConfiguration" /> that represents
		/// the configuration used to provide secure connections.
		/// </value>
		/// <exception cref="T:System.InvalidOperationException">
		/// This instance does not provide secure connections.
		/// </exception>
		public ServerSslConfiguration SslConfiguration
		{
			get
			{
				if (!_secure)
				{
					string message = "This instance does not provide secure connections.";
					throw new InvalidOperationException(message);
				}
				return getSslConfiguration();
			}
		}

		/// <summary>
		/// Gets or sets the delegate used to find the credentials
		/// for an identity.
		/// </summary>
		/// <remarks>
		///   <para>
		///   No credentials are found if the method invoked by
		///   the delegate returns <see langword="null" /> or
		///   the value is <see langword="null" />.
		///   </para>
		///   <para>
		///   The set operation does nothing if the server has
		///   already started or it is shutting down.
		///   </para>
		/// </remarks>
		/// <value>
		///   <para>
		///   A <c>Func&lt;<see cref="T:System.Security.Principal.IIdentity" />,
		///   <see cref="T:Sfs2X.WebSocketSharp.Net.NetworkCredential" />&gt;</c> delegate or
		///   <see langword="null" /> if not needed.
		///   </para>
		///   <para>
		///   That delegate invokes the method called for finding
		///   the credentials used to authenticate a client.
		///   </para>
		///   <para>
		///   The default value is <see langword="null" />.
		///   </para>
		/// </value>
		public Func<IIdentity, Sfs2X.WebSocketSharp.Net.NetworkCredential> UserCredentialsFinder
		{
			get
			{
				return _userCredFinder;
			}
			set
			{
				string message;
				if (!canSet(out message))
				{
					_log.Warn(message);
					return;
				}
				lock (_sync)
				{
					if (!canSet(out message))
					{
						_log.Warn(message);
					}
					else
					{
						_userCredFinder = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the time to wait for the response to the WebSocket Ping or
		/// Close.
		/// </summary>
		/// <remarks>
		/// The set operation does nothing if the server has already started or
		/// it is shutting down.
		/// </remarks>
		/// <value>
		///   <para>
		///   A <see cref="T:System.TimeSpan" /> to wait for the response.
		///   </para>
		///   <para>
		///   The default value is the same as 1 second.
		///   </para>
		/// </value>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// The value specified for a set operation is zero or less.
		/// </exception>
		public TimeSpan WaitTime
		{
			get
			{
				return _services.WaitTime;
			}
			set
			{
				_services.WaitTime = value;
			}
		}

		/// <summary>
		/// Gets the management function for the WebSocket services
		/// provided by the server.
		/// </summary>
		/// <value>
		/// A <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketServiceManager" /> that manages
		/// the WebSocket services provided by the server.
		/// </value>
		public WebSocketServiceManager WebSocketServices
		{
			get
			{
				return _services;
			}
		}

		static WebSocketServer()
		{
			_defaultRealm = "SECRET AREA";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketServer" /> class.
		/// </summary>
		/// <remarks>
		/// The new instance listens for incoming handshake requests on
		/// <see cref="F:System.Net.IPAddress.Any" /> and port 80.
		/// </remarks>
		public WebSocketServer()
		{
			IPAddress any = IPAddress.Any;
			init(any.ToString(), any, 80, false);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketServer" /> class
		/// with the specified <paramref name="port" />.
		/// </summary>
		/// <remarks>
		///   <para>
		///   The new instance listens for incoming handshake requests on
		///   <see cref="F:System.Net.IPAddress.Any" /> and <paramref name="port" />.
		///   </para>
		///   <para>
		///   It provides secure connections if <paramref name="port" /> is 443.
		///   </para>
		/// </remarks>
		/// <param name="port">
		/// An <see cref="T:System.Int32" /> that represents the number of the port
		/// on which to listen.
		/// </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="port" /> is less than 1 or greater than 65535.
		/// </exception>
		public WebSocketServer(int port)
			: this(port, port == 443)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketServer" /> class
		/// with the specified <paramref name="url" />.
		/// </summary>
		/// <remarks>
		///   <para>
		///   The new instance listens for incoming handshake requests on
		///   the IP address of the host of <paramref name="url" /> and
		///   the port of <paramref name="url" />.
		///   </para>
		///   <para>
		///   Either port 80 or 443 is used if <paramref name="url" /> includes
		///   no port. Port 443 is used if the scheme of <paramref name="url" />
		///   is wss; otherwise, port 80 is used.
		///   </para>
		///   <para>
		///   The new instance provides secure connections if the scheme of
		///   <paramref name="url" /> is wss.
		///   </para>
		/// </remarks>
		/// <param name="url">
		/// A <see cref="T:System.String" /> that represents the WebSocket URL of the server.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="url" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="url" /> is an empty string.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="url" /> is invalid.
		///   </para>
		/// </exception>
		public WebSocketServer(string url)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			if (url.Length == 0)
			{
				throw new ArgumentException("An empty string.", "url");
			}
			Uri result;
			string message;
			if (!tryCreateUri(url, out result, out message))
			{
				throw new ArgumentException(message, "url");
			}
			string dnsSafeHost = result.DnsSafeHost;
			IPAddress iPAddress = dnsSafeHost.ToIPAddress();
			if (iPAddress == null)
			{
				message = "The host part could not be converted to an IP address.";
				throw new ArgumentException(message, "url");
			}
			if (!iPAddress.IsLocal())
			{
				message = "The IP address of the host is not a local IP address.";
				throw new ArgumentException(message, "url");
			}
			init(dnsSafeHost, iPAddress, result.Port, result.Scheme == "wss");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketServer" /> class
		/// with the specified <paramref name="port" /> and <paramref name="secure" />.
		/// </summary>
		/// <remarks>
		/// The new instance listens for incoming handshake requests on
		/// <see cref="F:System.Net.IPAddress.Any" /> and <paramref name="port" />.
		/// </remarks>
		/// <param name="port">
		/// An <see cref="T:System.Int32" /> that represents the number of the port
		/// on which to listen.
		/// </param>
		/// <param name="secure">
		/// A <see cref="T:System.Boolean" />: <c>true</c> if the new instance provides
		/// secure connections; otherwise, <c>false</c>.
		/// </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="port" /> is less than 1 or greater than 65535.
		/// </exception>
		public WebSocketServer(int port, bool secure)
		{
			if (!port.IsPortNumber())
			{
				string message = "Less than 1 or greater than 65535.";
				throw new ArgumentOutOfRangeException("port", message);
			}
			IPAddress any = IPAddress.Any;
			init(any.ToString(), any, port, secure);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketServer" /> class
		/// with the specified <paramref name="address" /> and <paramref name="port" />.
		/// </summary>
		/// <remarks>
		///   <para>
		///   The new instance listens for incoming handshake requests on
		///   <paramref name="address" /> and <paramref name="port" />.
		///   </para>
		///   <para>
		///   It provides secure connections if <paramref name="port" /> is 443.
		///   </para>
		/// </remarks>
		/// <param name="address">
		/// A <see cref="T:System.Net.IPAddress" /> that represents the local
		/// IP address on which to listen.
		/// </param>
		/// <param name="port">
		/// An <see cref="T:System.Int32" /> that represents the number of the port
		/// on which to listen.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="address" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="address" /> is not a local IP address.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="port" /> is less than 1 or greater than 65535.
		/// </exception>
		public WebSocketServer(IPAddress address, int port)
			: this(address, port, port == 443)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketServer" /> class
		/// with the specified <paramref name="address" />, <paramref name="port" />,
		/// and <paramref name="secure" />.
		/// </summary>
		/// <remarks>
		/// The new instance listens for incoming handshake requests on
		/// <paramref name="address" /> and <paramref name="port" />.
		/// </remarks>
		/// <param name="address">
		/// A <see cref="T:System.Net.IPAddress" /> that represents the local
		/// IP address on which to listen.
		/// </param>
		/// <param name="port">
		/// An <see cref="T:System.Int32" /> that represents the number of the port
		/// on which to listen.
		/// </param>
		/// <param name="secure">
		/// A <see cref="T:System.Boolean" />: <c>true</c> if the new instance provides
		/// secure connections; otherwise, <c>false</c>.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="address" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="address" /> is not a local IP address.
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// <paramref name="port" /> is less than 1 or greater than 65535.
		/// </exception>
		public WebSocketServer(IPAddress address, int port, bool secure)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (!address.IsLocal())
			{
				throw new ArgumentException("Not a local IP address.", "address");
			}
			if (!port.IsPortNumber())
			{
				string message = "Less than 1 or greater than 65535.";
				throw new ArgumentOutOfRangeException("port", message);
			}
			init(address.ToString(), address, port, secure);
		}

		private void abort()
		{
			lock (_sync)
			{
				if (_state != ServerState.Start)
				{
					return;
				}
				_state = ServerState.ShuttingDown;
			}
			try
			{
				try
				{
					_listener.Stop();
				}
				finally
				{
					_services.Stop(1006, string.Empty);
				}
			}
			catch
			{
			}
			_state = ServerState.Stop;
		}

		private bool authenticateClient(TcpListenerWebSocketContext context)
		{
			if (_authSchemes == Sfs2X.WebSocketSharp.Net.AuthenticationSchemes.Anonymous)
			{
				return true;
			}
			if (_authSchemes == Sfs2X.WebSocketSharp.Net.AuthenticationSchemes.None)
			{
				return false;
			}
			return context.Authenticate(_authSchemes, _realmInUse, _userCredFinder);
		}

		private bool canSet(out string message)
		{
			message = null;
			if (_state == ServerState.Start)
			{
				message = "The server has already started.";
				return false;
			}
			if (_state == ServerState.ShuttingDown)
			{
				message = "The server is shutting down.";
				return false;
			}
			return true;
		}

		private bool checkHostNameForRequest(string name)
		{
			return !_dnsStyle || Uri.CheckHostName(name) != UriHostNameType.Dns || name == _hostname;
		}

		private static bool checkSslConfiguration(ServerSslConfiguration configuration, out string message)
		{
			message = null;
			if (configuration.ServerCertificate == null)
			{
				message = "There is no server certificate for secure connection.";
				return false;
			}
			return true;
		}

		private string getRealm()
		{
			string realm = _realm;
			return (realm != null && realm.Length > 0) ? realm : _defaultRealm;
		}

		private ServerSslConfiguration getSslConfiguration()
		{
			if (_sslConfig == null)
			{
				_sslConfig = new ServerSslConfiguration();
			}
			return _sslConfig;
		}

		private void init(string hostname, IPAddress address, int port, bool secure)
		{
			_hostname = hostname;
			_address = address;
			_port = port;
			_secure = secure;
			_authSchemes = Sfs2X.WebSocketSharp.Net.AuthenticationSchemes.Anonymous;
			_dnsStyle = Uri.CheckHostName(hostname) == UriHostNameType.Dns;
			_listener = new TcpListener(address, port);
			_log = new Logger();
			_services = new WebSocketServiceManager(_log);
			_sync = new object();
		}

		private void processRequest(TcpListenerWebSocketContext context)
		{
			if (!authenticateClient(context))
			{
				context.Close(Sfs2X.WebSocketSharp.Net.HttpStatusCode.Forbidden);
				return;
			}
			Uri requestUri = context.RequestUri;
			if (requestUri == null)
			{
				context.Close(Sfs2X.WebSocketSharp.Net.HttpStatusCode.BadRequest);
				return;
			}
			if (!_allowForwardedRequest)
			{
				if (requestUri.Port != _port)
				{
					context.Close(Sfs2X.WebSocketSharp.Net.HttpStatusCode.BadRequest);
					return;
				}
				if (!checkHostNameForRequest(requestUri.DnsSafeHost))
				{
					context.Close(Sfs2X.WebSocketSharp.Net.HttpStatusCode.NotFound);
					return;
				}
			}
			WebSocketServiceHost host;
			if (!_services.InternalTryGetServiceHost(requestUri.AbsolutePath, out host))
			{
				context.Close(Sfs2X.WebSocketSharp.Net.HttpStatusCode.NotImplemented);
			}
			else
			{
				host.StartSession(context);
			}
		}

		private void receiveRequest()
		{
			while (true)
			{
				TcpClient cl = null;
				try
				{
					cl = _listener.AcceptTcpClient();
					ThreadPool.QueueUserWorkItem(delegate
					{
						try
						{
							TcpListenerWebSocketContext context = new TcpListenerWebSocketContext(cl, null, _secure, _sslConfigInUse, _log);
							processRequest(context);
						}
						catch (Exception ex3)
						{
							_log.Error(ex3.Message);
							_log.Debug(ex3.ToString());
							cl.Close();
						}
					});
				}
				catch (SocketException ex)
				{
					if (_state == ServerState.ShuttingDown)
					{
						_log.Info("The underlying listener is stopped.");
						break;
					}
					_log.Fatal(ex.Message);
					_log.Debug(ex.ToString());
					break;
				}
				catch (Exception ex2)
				{
					_log.Fatal(ex2.Message);
					_log.Debug(ex2.ToString());
					if (cl != null)
					{
						cl.Close();
					}
					break;
				}
			}
			if (_state != ServerState.ShuttingDown)
			{
				abort();
			}
		}

		private void start(ServerSslConfiguration sslConfig)
		{
			if (_state == ServerState.Start)
			{
				_log.Info("The server has already started.");
				return;
			}
			if (_state == ServerState.ShuttingDown)
			{
				_log.Warn("The server is shutting down.");
				return;
			}
			lock (_sync)
			{
				if (_state == ServerState.Start)
				{
					_log.Info("The server has already started.");
					return;
				}
				if (_state == ServerState.ShuttingDown)
				{
					_log.Warn("The server is shutting down.");
					return;
				}
				_sslConfigInUse = sslConfig;
				_realmInUse = getRealm();
				_services.Start();
				try
				{
					startReceiving();
				}
				catch
				{
					_services.Stop(1011, string.Empty);
					throw;
				}
				_state = ServerState.Start;
			}
		}

		private void startReceiving()
		{
			if (_reuseAddress)
			{
				_listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			}
			try
			{
				_listener.Start();
			}
			catch (Exception innerException)
			{
				string message = "The underlying listener has failed to start.";
				throw new InvalidOperationException(message, innerException);
			}
			_receiveThread = new Thread(receiveRequest);
			_receiveThread.IsBackground = true;
			_receiveThread.Start();
		}

		private void stop(ushort code, string reason)
		{
			if (_state == ServerState.Ready)
			{
				_log.Info("The server is not started.");
				return;
			}
			if (_state == ServerState.ShuttingDown)
			{
				_log.Info("The server is shutting down.");
				return;
			}
			if (_state == ServerState.Stop)
			{
				_log.Info("The server has already stopped.");
				return;
			}
			lock (_sync)
			{
				if (_state == ServerState.ShuttingDown)
				{
					_log.Info("The server is shutting down.");
					return;
				}
				if (_state == ServerState.Stop)
				{
					_log.Info("The server has already stopped.");
					return;
				}
				_state = ServerState.ShuttingDown;
			}
			try
			{
				bool flag = false;
				try
				{
					stopReceiving(5000);
				}
				catch
				{
					flag = true;
					throw;
				}
				finally
				{
					try
					{
						_services.Stop(code, reason);
					}
					catch
					{
						if (!flag)
						{
							throw;
						}
					}
				}
			}
			finally
			{
				_state = ServerState.Stop;
			}
		}

		private void stopReceiving(int millisecondsTimeout)
		{
			try
			{
				_listener.Stop();
			}
			catch (Exception innerException)
			{
				string message = "The underlying listener has failed to stop.";
				throw new InvalidOperationException(message, innerException);
			}
			_receiveThread.Join(millisecondsTimeout);
		}

		private static bool tryCreateUri(string uriString, out Uri result, out string message)
		{
			if (!uriString.TryCreateWebSocketUri(out result, out message))
			{
				return false;
			}
			if (result.PathAndQuery != "/")
			{
				result = null;
				message = "It includes either or both path and query components.";
				return false;
			}
			return true;
		}

		/// <summary>
		/// Adds a WebSocket service with the specified behavior,
		/// <paramref name="path" />, and <paramref name="creator" />.
		/// </summary>
		/// <remarks>
		/// <paramref name="path" /> is converted to a URL-decoded string and
		/// '/' is trimmed from the end of the converted string if any.
		/// </remarks>
		/// <param name="path">
		/// A <see cref="T:System.String" /> that represents an absolute path to
		/// the service to add.
		/// </param>
		/// <param name="creator">
		///   <para>
		///   A <c>Func&lt;TBehavior&gt;</c> delegate.
		///   </para>
		///   <para>
		///   It invokes the method called for creating a new session
		///   instance for the service.
		///   </para>
		///   <para>
		///   The method must create a new instance of the specified
		///   behavior class and return it.
		///   </para>
		/// </param>
		/// <typeparam name="TBehavior">
		///   <para>
		///   The type of the behavior for the service.
		///   </para>
		///   <para>
		///   It must inherit the <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketBehavior" /> class.
		///   </para>
		/// </typeparam>
		/// <exception cref="T:System.ArgumentNullException">
		///   <para>
		///   <paramref name="path" /> is <see langword="null" />.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="creator" /> is <see langword="null" />.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="path" /> is an empty string.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="path" /> is not an absolute path.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="path" /> includes either or both
		///   query and fragment components.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="path" /> is already in use.
		///   </para>
		/// </exception>
		[Obsolete("This method will be removed. Use added one instead.")]
		public void AddWebSocketService<TBehavior>(string path, Func<TBehavior> creator) where TBehavior : WebSocketBehavior
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (creator == null)
			{
				throw new ArgumentNullException("creator");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("An empty string.", "path");
			}
			if (path[0] != '/')
			{
				throw new ArgumentException("Not an absolute path.", "path");
			}
			if (path.IndexOfAny(new char[2] { '?', '#' }) > -1)
			{
				string message = "It includes either or both query and fragment components.";
				throw new ArgumentException(message, "path");
			}
			_services.Add(path, creator);
		}

		/// <summary>
		/// Adds a WebSocket service with the specified behavior and
		/// <paramref name="path" />.
		/// </summary>
		/// <remarks>
		/// <paramref name="path" /> is converted to a URL-decoded string and
		/// '/' is trimmed from the end of the converted string if any.
		/// </remarks>
		/// <param name="path">
		/// A <see cref="T:System.String" /> that represents an absolute path to
		/// the service to add.
		/// </param>
		/// <typeparam name="TBehaviorWithNew">
		///   <para>
		///   The type of the behavior for the service.
		///   </para>
		///   <para>
		///   It must inherit the <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketBehavior" /> class and
		///   have a public parameterless constructor.
		///   </para>
		/// </typeparam>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="path" /> is an empty string.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="path" /> is not an absolute path.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="path" /> includes either or both
		///   query and fragment components.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="path" /> is already in use.
		///   </para>
		/// </exception>
		public void AddWebSocketService<TBehaviorWithNew>(string path) where TBehaviorWithNew : WebSocketBehavior, new()
		{
			_services.AddService<TBehaviorWithNew>(path, null);
		}

		/// <summary>
		/// Adds a WebSocket service with the specified behavior,
		/// <paramref name="path" />, and <paramref name="initializer" />.
		/// </summary>
		/// <remarks>
		/// <paramref name="path" /> is converted to a URL-decoded string and
		/// '/' is trimmed from the end of the converted string if any.
		/// </remarks>
		/// <param name="path">
		/// A <see cref="T:System.String" /> that represents an absolute path to
		/// the service to add.
		/// </param>
		/// <param name="initializer">
		///   <para>
		///   An <c>Action&lt;TBehaviorWithNew&gt;</c> delegate or
		///   <see langword="null" /> if not needed.
		///   </para>
		///   <para>
		///   That delegate invokes the method called for initializing
		///   a new session instance for the service.
		///   </para>
		/// </param>
		/// <typeparam name="TBehaviorWithNew">
		///   <para>
		///   The type of the behavior for the service.
		///   </para>
		///   <para>
		///   It must inherit the <see cref="T:Sfs2X.WebSocketSharp.Server.WebSocketBehavior" /> class and
		///   have a public parameterless constructor.
		///   </para>
		/// </typeparam>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="path" /> is an empty string.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="path" /> is not an absolute path.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="path" /> includes either or both
		///   query and fragment components.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="path" /> is already in use.
		///   </para>
		/// </exception>
		public void AddWebSocketService<TBehaviorWithNew>(string path, Action<TBehaviorWithNew> initializer) where TBehaviorWithNew : WebSocketBehavior, new()
		{
			_services.AddService(path, initializer);
		}

		/// <summary>
		/// Removes a WebSocket service with the specified <paramref name="path" />.
		/// </summary>
		/// <remarks>
		///   <para>
		///   <paramref name="path" /> is converted to a URL-decoded string and
		///   '/' is trimmed from the end of the converted string if any.
		///   </para>
		///   <para>
		///   The service is stopped with close status 1001 (going away)
		///   if it has already started.
		///   </para>
		/// </remarks>
		/// <returns>
		/// <c>true</c> if the service is successfully found and removed;
		/// otherwise, <c>false</c>.
		/// </returns>
		/// <param name="path">
		/// A <see cref="T:System.String" /> that represents an absolute path to
		/// the service to remove.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="path" /> is an empty string.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="path" /> is not an absolute path.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="path" /> includes either or both
		///   query and fragment components.
		///   </para>
		/// </exception>
		public bool RemoveWebSocketService(string path)
		{
			return _services.RemoveService(path);
		}

		/// <summary>
		/// Starts receiving incoming handshake requests.
		/// </summary>
		/// <remarks>
		/// This method does nothing if the server has already started or
		/// it is shutting down.
		/// </remarks>
		/// <exception cref="T:System.InvalidOperationException">
		///   <para>
		///   There is no server certificate for secure connection.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   The underlying <see cref="T:System.Net.Sockets.TcpListener" /> has failed to start.
		///   </para>
		/// </exception>
		public void Start()
		{
			ServerSslConfiguration serverSslConfiguration = null;
			if (_secure)
			{
				serverSslConfiguration = new ServerSslConfiguration(getSslConfiguration());
				string message;
				if (!checkSslConfiguration(serverSslConfiguration, out message))
				{
					throw new InvalidOperationException(message);
				}
			}
			start(serverSslConfiguration);
		}

		/// <summary>
		/// Stops receiving incoming handshake requests.
		/// </summary>
		/// <exception cref="T:System.InvalidOperationException">
		/// The underlying <see cref="T:System.Net.Sockets.TcpListener" /> has failed to stop.
		/// </exception>
		public void Stop()
		{
			stop(1001, string.Empty);
		}

		/// <summary>
		/// Stops receiving incoming handshake requests and closes each connection
		/// with the specified code and reason.
		/// </summary>
		/// <param name="code">
		///   <para>
		///   A <see cref="T:System.UInt16" /> that represents the status code indicating
		///   the reason for the close.
		///   </para>
		///   <para>
		///   The status codes are defined in
		///   <see href="http://tools.ietf.org/html/rfc6455#section-7.4">
		///   Section 7.4</see> of RFC 6455.
		///   </para>
		/// </param>
		/// <param name="reason">
		///   <para>
		///   A <see cref="T:System.String" /> that represents the reason for the close.
		///   </para>
		///   <para>
		///   The size must be 123 bytes or less in UTF-8.
		///   </para>
		/// </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <para>
		///   <paramref name="code" /> is less than 1000 or greater than 4999.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   The size of <paramref name="reason" /> is greater than 123 bytes.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="code" /> is 1010 (mandatory extension).
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="code" /> is 1005 (no status) and there is reason.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="reason" /> could not be UTF-8-encoded.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The underlying <see cref="T:System.Net.Sockets.TcpListener" /> has failed to stop.
		/// </exception>
		[Obsolete("This method will be removed.")]
		public void Stop(ushort code, string reason)
		{
			if (!code.IsCloseStatusCode())
			{
				string message = "Less than 1000 or greater than 4999.";
				throw new ArgumentOutOfRangeException("code", message);
			}
			if (code == 1010)
			{
				string message2 = "1010 cannot be used.";
				throw new ArgumentException(message2, "code");
			}
			if (!reason.IsNullOrEmpty())
			{
				if (code == 1005)
				{
					string message3 = "1005 cannot be used.";
					throw new ArgumentException(message3, "code");
				}
				byte[] bytes;
				if (!reason.TryGetUTF8EncodedBytes(out bytes))
				{
					string message4 = "It could not be UTF-8-encoded.";
					throw new ArgumentException(message4, "reason");
				}
				if (bytes.Length > 123)
				{
					string message5 = "Its size is greater than 123 bytes.";
					throw new ArgumentOutOfRangeException("reason", message5);
				}
			}
			stop(code, reason);
		}

		/// <summary>
		/// Stops receiving incoming handshake requests and closes each connection
		/// with the specified code and reason.
		/// </summary>
		/// <param name="code">
		///   <para>
		///   One of the <see cref="T:Sfs2X.WebSocketSharp.CloseStatusCode" /> enum values.
		///   </para>
		///   <para>
		///   It represents the status code indicating the reason for the close.
		///   </para>
		/// </param>
		/// <param name="reason">
		///   <para>
		///   A <see cref="T:System.String" /> that represents the reason for the close.
		///   </para>
		///   <para>
		///   The size must be 123 bytes or less in UTF-8.
		///   </para>
		/// </param>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="code" /> is
		///   <see cref="F:Sfs2X.WebSocketSharp.CloseStatusCode.MandatoryExtension" />.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="code" /> is
		///   <see cref="F:Sfs2X.WebSocketSharp.CloseStatusCode.NoStatus" /> and there is reason.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="reason" /> could not be UTF-8-encoded.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// The size of <paramref name="reason" /> is greater than 123 bytes.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The underlying <see cref="T:System.Net.Sockets.TcpListener" /> has failed to stop.
		/// </exception>
		[Obsolete("This method will be removed.")]
		public void Stop(CloseStatusCode code, string reason)
		{
			if (code == CloseStatusCode.MandatoryExtension)
			{
				string message = "MandatoryExtension cannot be used.";
				throw new ArgumentException(message, "code");
			}
			if (!reason.IsNullOrEmpty())
			{
				if (code == CloseStatusCode.NoStatus)
				{
					string message2 = "NoStatus cannot be used.";
					throw new ArgumentException(message2, "code");
				}
				byte[] bytes;
				if (!reason.TryGetUTF8EncodedBytes(out bytes))
				{
					string message3 = "It could not be UTF-8-encoded.";
					throw new ArgumentException(message3, "reason");
				}
				if (bytes.Length > 123)
				{
					string message4 = "Its size is greater than 123 bytes.";
					throw new ArgumentOutOfRangeException("reason", message4);
				}
			}
			stop((ushort)code, reason);
		}
	}
}
