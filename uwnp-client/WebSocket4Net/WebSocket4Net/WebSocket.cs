using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Threading;
using SuperSocket.ClientEngine;
using WebSocket4Net.Command;
using WebSocket4Net.Common;
using WebSocket4Net.Protocol;

namespace WebSocket4Net
{
	public class WebSocket : IDisposable
	{
		private EndPoint m_RemoteEndPoint;

		protected const string UserAgentKey = "User-Agent";

		public const int DefaultReceiveBufferSize = 4096;

		private int m_StateCode;

		private EndPoint m_HttpConnectProxy;

		private Dictionary<string, ICommand<WebSocket, WebSocketCommandInfo>> m_CommandDict = new Dictionary<string, ICommand<WebSocket, WebSocketCommandInfo>>(StringComparer.OrdinalIgnoreCase);

		private static ProtocolProcessorFactory m_ProtocolProcessorFactory;

		private Timer m_WebSocketTimer;

		private string m_LastPingRequest;

		private const string m_UriScheme = "ws";

		private const string m_UriPrefix = "ws://";

		private const string m_SecureUriScheme = "wss";

		private const int m_SecurePort = 443;

		private const string m_SecureUriPrefix = "wss://";

		private SecurityOption m_Security;

		private bool m_Disposed;

		private EventHandler m_Opened;

		private EventHandler<MessageReceivedEventArgs> m_MessageReceived;

		private EventHandler<DataReceivedEventArgs> m_DataReceived;

		private const string m_NotOpenSendingMessage = "You must send data by websocket after websocket is opened!";

		private ClosedEventArgs m_ClosedArgs;

		private EventHandler m_Closed;

		private EventHandler<ErrorEventArgs> m_Error;

		private static List<KeyValuePair<string, string>> EmptyCookies;

		private SslProtocols m_SecureProtocols = SslProtocols.Default;

		internal TcpClientSession Client { get; private set; }

		public WebSocketVersion Version { get; private set; }

		public DateTime LastActiveTime { get; internal set; }

		public bool EnableAutoSendPing { get; set; }

		public int AutoSendPingInterval { get; set; }

		internal IProtocolProcessor ProtocolProcessor { get; private set; }

		public bool SupportBinary
		{
			get
			{
				return ProtocolProcessor.SupportBinary;
			}
		}

		internal Uri TargetUri { get; private set; }

		internal string SubProtocol { get; private set; }

		internal IDictionary<string, object> Items { get; private set; }

		internal List<KeyValuePair<string, string>> Cookies { get; private set; }

		internal List<KeyValuePair<string, string>> CustomHeaderItems { get; private set; }

		internal int StateCode
		{
			get
			{
				return m_StateCode;
			}
		}

		public WebSocketState State
		{
			get
			{
				return (WebSocketState)m_StateCode;
			}
		}

		public bool Handshaked { get; private set; }

		public IProxyConnector Proxy { get; set; }

		internal EndPoint HttpConnectProxy
		{
			get
			{
				return m_HttpConnectProxy;
			}
		}

		protected IClientCommandReader<WebSocketCommandInfo> CommandReader { get; private set; }

		internal bool NotSpecifiedVersion { get; private set; }

		internal string LastPongResponse { get; set; }

		internal string HandshakeHost { get; private set; }

		internal string Origin { get; private set; }

		public bool NoDelay { get; set; }

		public EndPoint LocalEndPoint
		{
			get
			{
				if (Client == null)
				{
					return null;
				}
				return Client.LocalEndPoint;
			}
			set
			{
				if (Client == null)
				{
					throw new Exception("Websocket client is not initilized.");
				}
				Client.LocalEndPoint = value;
			}
		}

		public SecurityOption Security
		{
			get
			{
				if (m_Security != null)
				{
					return m_Security;
				}
				SslStreamTcpSession sslStreamTcpSession = Client as SslStreamTcpSession;
				if (sslStreamTcpSession == null)
				{
					return m_Security = new SecurityOption();
				}
				return m_Security = sslStreamTcpSession.Security;
			}
		}

		public int ReceiveBufferSize
		{
			get
			{
				return Client.ReceiveBufferSize;
			}
			set
			{
				Client.ReceiveBufferSize = value;
			}
		}

		public event EventHandler Opened
		{
			add
			{
				m_Opened = (EventHandler)Delegate.Combine(m_Opened, value);
			}
			remove
			{
				m_Opened = (EventHandler)Delegate.Remove(m_Opened, value);
			}
		}

		public event EventHandler<MessageReceivedEventArgs> MessageReceived
		{
			add
			{
				m_MessageReceived = (EventHandler<MessageReceivedEventArgs>)Delegate.Combine(m_MessageReceived, value);
			}
			remove
			{
				m_MessageReceived = (EventHandler<MessageReceivedEventArgs>)Delegate.Remove(m_MessageReceived, value);
			}
		}

		public event EventHandler<DataReceivedEventArgs> DataReceived
		{
			add
			{
				m_DataReceived = (EventHandler<DataReceivedEventArgs>)Delegate.Combine(m_DataReceived, value);
			}
			remove
			{
				m_DataReceived = (EventHandler<DataReceivedEventArgs>)Delegate.Remove(m_DataReceived, value);
			}
		}

		public event EventHandler Closed
		{
			add
			{
				m_Closed = (EventHandler)Delegate.Combine(m_Closed, value);
			}
			remove
			{
				m_Closed = (EventHandler)Delegate.Remove(m_Closed, value);
			}
		}

		public event EventHandler<ErrorEventArgs> Error
		{
			add
			{
				m_Error = (EventHandler<ErrorEventArgs>)Delegate.Combine(m_Error, value);
			}
			remove
			{
				m_Error = (EventHandler<ErrorEventArgs>)Delegate.Remove(m_Error, value);
			}
		}

		static WebSocket()
		{
			m_ProtocolProcessorFactory = new ProtocolProcessorFactory(new Rfc6455Processor(), new DraftHybi10Processor(), new DraftHybi00Processor());
		}

		private EndPoint ResolveUri(string uri, int defaultPort, out int port)
		{
			TargetUri = new Uri(uri);
			if (string.IsNullOrEmpty(Origin))
			{
				Origin = TargetUri.GetOrigin();
			}
			port = TargetUri.Port;
			if (port <= 0)
			{
				port = defaultPort;
			}
			IPAddress address;
			if (IPAddress.TryParse(TargetUri.Host, out address))
			{
				return new IPEndPoint(address, port);
			}
			return new DnsEndPoint(TargetUri.Host, port);
		}

		private TcpClientSession CreateClient(string uri)
		{
			int port;
			m_RemoteEndPoint = ResolveUri(uri, 80, out port);
			if (port == 80)
			{
				HandshakeHost = TargetUri.Host;
			}
			else
			{
				HandshakeHost = TargetUri.Host + ":" + port;
			}
			return new AsyncTcpSession();
		}

		private TcpClientSession CreateSecureClient(string uri)
		{
			int num = uri.IndexOf('/', "wss://".Length);
			if (num < 0)
			{
				num = uri.IndexOf(':', "wss://".Length, uri.Length - "wss://".Length);
				uri = ((num >= 0) ? (uri + "/") : (uri + ":" + 443 + "/"));
			}
			else
			{
				if (num == "wss://".Length)
				{
					throw new ArgumentException("Invalid uri", "uri");
				}
				if (uri.IndexOf(':', "wss://".Length, num - "wss://".Length) < 0)
				{
					uri = uri.Substring(0, num) + ":" + 443 + uri.Substring(num);
				}
			}
			int port;
			m_RemoteEndPoint = ResolveUri(uri, 443, out port);
			if (m_HttpConnectProxy != null)
			{
				m_RemoteEndPoint = m_HttpConnectProxy;
			}
			if (port == 443)
			{
				HandshakeHost = TargetUri.Host;
			}
			else
			{
				HandshakeHost = TargetUri.Host + ":" + port;
			}
			return CreateSecureTcpSession();
		}

		private void Initialize(string uri, string subProtocol, List<KeyValuePair<string, string>> cookies, List<KeyValuePair<string, string>> customHeaderItems, string userAgent, string origin, WebSocketVersion version, EndPoint httpConnectProxy, int receiveBufferSize)
		{
			if (version == WebSocketVersion.None)
			{
				NotSpecifiedVersion = true;
				version = WebSocketVersion.Rfc6455;
			}
			Version = version;
			ProtocolProcessor = GetProtocolProcessor(version);
			Cookies = cookies;
			Origin = origin;
			if (!string.IsNullOrEmpty(userAgent))
			{
				if (customHeaderItems == null)
				{
					customHeaderItems = new List<KeyValuePair<string, string>>();
				}
				customHeaderItems.Add(new KeyValuePair<string, string>("User-Agent", userAgent));
			}
			if (customHeaderItems != null && customHeaderItems.Count > 0)
			{
				CustomHeaderItems = customHeaderItems;
			}
			Handshake handshake = new Handshake();
			m_CommandDict.Add(handshake.Name, handshake);
			Text text = new Text();
			m_CommandDict.Add(text.Name, text);
			Binary binary = new Binary();
			m_CommandDict.Add(binary.Name, binary);
			Close close = new Close();
			m_CommandDict.Add(close.Name, close);
			Ping ping = new Ping();
			m_CommandDict.Add(ping.Name, ping);
			Pong pong = new Pong();
			m_CommandDict.Add(pong.Name, pong);
			BadRequest badRequest = new BadRequest();
			m_CommandDict.Add(badRequest.Name, badRequest);
			m_StateCode = -1;
			SubProtocol = subProtocol;
			Items = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			m_HttpConnectProxy = httpConnectProxy;
			TcpClientSession tcpClientSession;
			if (uri.StartsWith("ws://", StringComparison.OrdinalIgnoreCase))
			{
				tcpClientSession = CreateClient(uri);
			}
			else
			{
				if (!uri.StartsWith("wss://", StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException("Invalid uri", "uri");
				}
				tcpClientSession = CreateSecureClient(uri);
			}
			tcpClientSession.ReceiveBufferSize = ((receiveBufferSize > 0) ? receiveBufferSize : 4096);
			tcpClientSession.Connected += client_Connected;
			tcpClientSession.Closed += client_Closed;
			tcpClientSession.Error += client_Error;
			tcpClientSession.DataReceived += client_DataReceived;
			Client = tcpClientSession;
			EnableAutoSendPing = true;
		}

		private void client_DataReceived(object sender, DataEventArgs e)
		{
			OnDataReceived(e.Data, e.Offset, e.Length);
		}

		private void client_Error(object sender, ErrorEventArgs e)
		{
			OnError(e);
			OnClosed();
		}

		private void client_Closed(object sender, EventArgs e)
		{
			OnClosed();
		}

		private void client_Connected(object sender, EventArgs e)
		{
			OnConnected();
		}

		internal bool GetAvailableProcessor(int[] availableVersions)
		{
			IProtocolProcessor preferedProcessorFromAvialable = m_ProtocolProcessorFactory.GetPreferedProcessorFromAvialable(availableVersions);
			if (preferedProcessorFromAvialable == null)
			{
				return false;
			}
			ProtocolProcessor = preferedProcessorFromAvialable;
			return true;
		}

		public void Open()
		{
			m_StateCode = 0;
			if (Proxy != null)
			{
				Client.Proxy = Proxy;
			}
			Client.NoDelay = NoDelay;
			Client.Connect(m_RemoteEndPoint);
		}

		private static IProtocolProcessor GetProtocolProcessor(WebSocketVersion version)
		{
			IProtocolProcessor processorByVersion = m_ProtocolProcessorFactory.GetProcessorByVersion(version);
			if (processorByVersion == null)
			{
				throw new ArgumentException("Invalid websocket version");
			}
			return processorByVersion;
		}

		private void OnConnected()
		{
			CommandReader = ProtocolProcessor.CreateHandshakeReader(this);
			if (Items.Count > 0)
			{
				Items.Clear();
			}
			ProtocolProcessor.SendHandshake(this);
		}

		protected internal virtual void OnHandshaked()
		{
			m_StateCode = 1;
			Handshaked = true;
			if (EnableAutoSendPing && ProtocolProcessor.SupportPingPong)
			{
				if (AutoSendPingInterval <= 0)
				{
					AutoSendPingInterval = 60;
				}
				m_WebSocketTimer = new Timer(OnPingTimerCallback, ProtocolProcessor, AutoSendPingInterval * 1000, AutoSendPingInterval * 1000);
			}
			EventHandler opened = m_Opened;
			if (opened != null)
			{
				opened(this, EventArgs.Empty);
			}
		}

		private void OnPingTimerCallback(object state)
		{
			IProtocolProcessor protocolProcessor = state as IProtocolProcessor;
			if (!string.IsNullOrEmpty(m_LastPingRequest) && !m_LastPingRequest.Equals(LastPongResponse))
			{
				try
				{
					protocolProcessor.SendPong(this, "");
				}
				catch (Exception e)
				{
					OnError(e);
					return;
				}
			}
			m_LastPingRequest = DateTime.Now.ToString();
			try
			{
				protocolProcessor.SendPing(this, m_LastPingRequest);
			}
			catch (Exception e2)
			{
				OnError(e2);
			}
		}

		internal void FireMessageReceived(string message)
		{
			if (m_MessageReceived != null)
			{
				m_MessageReceived(this, new MessageReceivedEventArgs(message));
			}
		}

		internal void FireDataReceived(byte[] data)
		{
			if (m_DataReceived != null)
			{
				m_DataReceived(this, new DataReceivedEventArgs(data));
			}
		}

		private bool EnsureWebSocketOpen()
		{
			if (!Handshaked)
			{
				OnError(new Exception("You must send data by websocket after websocket is opened!"));
				return false;
			}
			return true;
		}

		public void Send(string message)
		{
			if (EnsureWebSocketOpen())
			{
				ProtocolProcessor.SendMessage(this, message);
			}
		}

		public void Send(byte[] data, int offset, int length)
		{
			if (EnsureWebSocketOpen())
			{
				ProtocolProcessor.SendData(this, data, offset, length);
			}
		}

		public void Send(IList<ArraySegment<byte>> segments)
		{
			if (EnsureWebSocketOpen())
			{
				ProtocolProcessor.SendData(this, segments);
			}
		}

		private void OnClosed()
		{
			bool flag = false;
			if (m_StateCode == 2 || m_StateCode == 1 || m_StateCode == 0)
			{
				flag = true;
			}
			m_StateCode = 3;
			if (flag)
			{
				FireClosed();
			}
		}

		public void Close()
		{
			Close(string.Empty);
		}

		public void Close(string reason)
		{
			Close(ProtocolProcessor.CloseStatusCode.NormalClosure, reason);
		}

		public void Close(int statusCode, string reason)
		{
			m_ClosedArgs = new ClosedEventArgs((short)statusCode, reason);
			if (Interlocked.CompareExchange(ref m_StateCode, 3, -1) == -1)
			{
				OnClosed();
				return;
			}
			if (Interlocked.CompareExchange(ref m_StateCode, 2, 0) == 0)
			{
				TcpClientSession client = Client;
				if (client != null && client.IsConnected)
				{
					client.Close();
				}
				else
				{
					OnClosed();
				}
				return;
			}
			m_StateCode = 2;
			ClearTimer();
			m_WebSocketTimer = new Timer(CheckCloseHandshake, null, 5000, -1);
			try
			{
				ProtocolProcessor.SendCloseHandshake(this, statusCode, reason);
			}
			catch (Exception e)
			{
				if (Client != null)
				{
					OnError(e);
				}
			}
		}

		private void CheckCloseHandshake(object state)
		{
			if (m_StateCode == 3)
			{
				return;
			}
			try
			{
				CloseWithoutHandshake();
			}
			catch (Exception e)
			{
				OnError(e);
			}
		}

		internal void CloseWithoutHandshake()
		{
			TcpClientSession client = Client;
			if (client != null)
			{
				client.Close();
			}
		}

		protected void ExecuteCommand(WebSocketCommandInfo commandInfo)
		{
			ICommand<WebSocket, WebSocketCommandInfo> value;
			if (m_CommandDict.TryGetValue(commandInfo.Key, out value))
			{
				value.ExecuteCommand(this, commandInfo);
			}
		}

		private void OnDataReceived(byte[] data, int offset, int length)
		{
			while (true)
			{
				int left;
				WebSocketCommandInfo commandInfo = CommandReader.GetCommandInfo(data, offset, length, out left);
				if (CommandReader.NextCommandReader != null)
				{
					CommandReader = CommandReader.NextCommandReader;
				}
				if (commandInfo != null)
				{
					ExecuteCommand(commandInfo);
				}
				if (left > 0)
				{
					offset = offset + length - left;
					length = left;
					continue;
				}
				break;
			}
		}

		internal void FireError(Exception error)
		{
			OnError(error);
		}

		private void ClearTimer()
		{
			Timer webSocketTimer = m_WebSocketTimer;
			if (webSocketTimer == null)
			{
				return;
			}
			lock (this)
			{
				if (m_WebSocketTimer != null)
				{
					webSocketTimer.Change(-1, -1);
					webSocketTimer.Dispose();
					m_WebSocketTimer = null;
				}
			}
		}

		private void FireClosed()
		{
			ClearTimer();
			EventHandler closed = m_Closed;
			if (closed != null)
			{
				closed(this, m_ClosedArgs ?? EventArgs.Empty);
			}
		}

		private void OnError(ErrorEventArgs e)
		{
			EventHandler<ErrorEventArgs> error = m_Error;
			if (error != null)
			{
				error(this, e);
			}
		}

		private void OnError(Exception e)
		{
			OnError(new ErrorEventArgs(e));
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (m_Disposed)
			{
				return;
			}
			if (disposing)
			{
				TcpClientSession client = Client;
				if (client != null)
				{
					client.Connected -= client_Connected;
					client.Closed -= client_Closed;
					client.Error -= client_Error;
					client.DataReceived -= client_DataReceived;
					if (client.IsConnected)
					{
						client.Close();
					}
					Client = null;
				}
				ClearTimer();
			}
			m_Disposed = true;
		}

		~WebSocket()
		{
			Dispose(false);
		}

		public WebSocket(string uri, string subProtocol, WebSocketVersion version)
			: this(uri, subProtocol, EmptyCookies, null, string.Empty, string.Empty, version)
		{
		}

		public WebSocket(string uri, string subProtocol = "", List<KeyValuePair<string, string>> cookies = null, List<KeyValuePair<string, string>> customHeaderItems = null, string userAgent = "", string origin = "", WebSocketVersion version = WebSocketVersion.None, EndPoint httpConnectProxy = null, SslProtocols sslProtocols = SslProtocols.None, int receiveBufferSize = 0)
		{
			if (sslProtocols != 0)
			{
				m_SecureProtocols = sslProtocols;
			}
			Initialize(uri, subProtocol, cookies, customHeaderItems, userAgent, origin, version, httpConnectProxy, receiveBufferSize);
		}

		private TcpClientSession CreateSecureTcpSession()
		{
			SslStreamTcpSession sslStreamTcpSession = new SslStreamTcpSession();
			SecurityOption securityOption2 = (sslStreamTcpSession.Security = new SecurityOption());
			securityOption2.EnabledSslProtocols = m_SecureProtocols;
			return sslStreamTcpSession;
		}
	}
}
