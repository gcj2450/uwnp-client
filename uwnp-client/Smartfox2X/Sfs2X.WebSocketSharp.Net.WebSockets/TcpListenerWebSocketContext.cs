using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;

namespace Sfs2X.WebSocketSharp.Net.WebSockets
{
	/// <summary>
	/// Provides the access to the information in a WebSocket handshake request to
	/// a <see cref="T:System.Net.Sockets.TcpListener" /> instance.
	/// </summary>
	internal class TcpListenerWebSocketContext : WebSocketContext
	{
		private Logger _log;

		private NameValueCollection _queryString;

		private HttpRequest _request;

		private Uri _requestUri;

		private bool _secure;

		private EndPoint _serverEndPoint;

		private Stream _stream;

		private TcpClient _tcpClient;

		private IPrincipal _user;

		private EndPoint _userEndPoint;

		private WebSocket _websocket;

		internal Logger Log
		{
			get
			{
				return _log;
			}
		}

		internal Stream Stream
		{
			get
			{
				return _stream;
			}
		}

		/// <summary>
		/// Gets the HTTP cookies included in the handshake request.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:Sfs2X.WebSocketSharp.Net.CookieCollection" /> that contains
		///   the cookies.
		///   </para>
		///   <para>
		///   An empty collection if not included.
		///   </para>
		/// </value>
		public override CookieCollection CookieCollection
		{
			get
			{
				return _request.Cookies;
			}
		}

		/// <summary>
		/// Gets the HTTP headers included in the handshake request.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> that contains the headers.
		/// </value>
		public override NameValueCollection Headers
		{
			get
			{
				return _request.Headers;
			}
		}

		/// <summary>
		/// Gets the value of the Host header included in the handshake request.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.String" /> that represents the server host name requested
		///   by the client.
		///   </para>
		///   <para>
		///   It includes the port number if provided.
		///   </para>
		/// </value>
		public override string Host
		{
			get
			{
				return _request.Headers["Host"];
			}
		}

		/// <summary>
		/// Gets a value indicating whether the client is authenticated.
		/// </summary>
		/// <value>
		/// <c>true</c> if the client is authenticated; otherwise, <c>false</c>.
		/// </value>
		public override bool IsAuthenticated
		{
			get
			{
				return _user != null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the handshake request is sent from
		/// the local computer.
		/// </summary>
		/// <value>
		/// <c>true</c> if the handshake request is sent from the same computer
		/// as the server; otherwise, <c>false</c>.
		/// </value>
		public override bool IsLocal
		{
			get
			{
				return UserEndPoint.Address.IsLocal();
			}
		}

		/// <summary>
		/// Gets a value indicating whether a secure connection is used to send
		/// the handshake request.
		/// </summary>
		/// <value>
		/// <c>true</c> if the connection is secure; otherwise, <c>false</c>.
		/// </value>
		public override bool IsSecureConnection
		{
			get
			{
				return _secure;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the request is a WebSocket handshake
		/// request.
		/// </summary>
		/// <value>
		/// <c>true</c> if the request is a WebSocket handshake request; otherwise,
		/// <c>false</c>.
		/// </value>
		public override bool IsWebSocketRequest
		{
			get
			{
				return _request.IsWebSocketRequest;
			}
		}

		/// <summary>
		/// Gets the value of the Origin header included in the handshake request.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.String" /> that represents the value of the Origin header.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the header is not present.
		///   </para>
		/// </value>
		public override string Origin
		{
			get
			{
				return _request.Headers["Origin"];
			}
		}

		/// <summary>
		/// Gets the query string included in the handshake request.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.Collections.Specialized.NameValueCollection" /> that contains the query
		///   parameters.
		///   </para>
		///   <para>
		///   An empty collection if not included.
		///   </para>
		/// </value>
		public override NameValueCollection QueryString
		{
			get
			{
				if (_queryString == null)
				{
					Uri requestUri = RequestUri;
					_queryString = QueryStringCollection.Parse((requestUri != null) ? requestUri.Query : null, Encoding.UTF8);
				}
				return _queryString;
			}
		}

		/// <summary>
		/// Gets the URI requested by the client.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.Uri" /> that represents the URI parsed from the request.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the URI cannot be parsed.
		///   </para>
		/// </value>
		public override Uri RequestUri
		{
			get
			{
				if (_requestUri == null)
				{
					_requestUri = HttpUtility.CreateRequestUrl(_request.RequestUri, _request.Headers["Host"], _request.IsWebSocketRequest, _secure);
				}
				return _requestUri;
			}
		}

		/// <summary>
		/// Gets the value of the Sec-WebSocket-Key header included in
		/// the handshake request.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.String" /> that represents the value of
		///   the Sec-WebSocket-Key header.
		///   </para>
		///   <para>
		///   The value is used to prove that the server received
		///   a valid WebSocket handshake request.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the header is not present.
		///   </para>
		/// </value>
		public override string SecWebSocketKey
		{
			get
			{
				return _request.Headers["Sec-WebSocket-Key"];
			}
		}

		/// <summary>
		/// Gets the names of the subprotocols from the Sec-WebSocket-Protocol
		/// header included in the handshake request.
		/// </summary>
		/// <value>
		///   <para>
		///   An <see cref="T:System.Collections.Generic.IEnumerable{string}" />
		///   instance.
		///   </para>
		///   <para>
		///   It provides an enumerator which supports the iteration over
		///   the collection of the names of the subprotocols.
		///   </para>
		/// </value>
		public override IEnumerable<string> SecWebSocketProtocols
		{
			get
			{
				string val = _request.Headers["Sec-WebSocket-Protocol"];
				if (val == null || val.Length == 0)
				{
					yield break;
				}
				string[] array = val.Split(',');
				foreach (string elm in array)
				{
					string protocol = elm.Trim();
					if (protocol.Length != 0)
					{
						yield return protocol;
					}
				}
			}
		}

		/// <summary>
		/// Gets the value of the Sec-WebSocket-Version header included in
		/// the handshake request.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.String" /> that represents the WebSocket protocol
		///   version specified by the client.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the header is not present.
		///   </para>
		/// </value>
		public override string SecWebSocketVersion
		{
			get
			{
				return _request.Headers["Sec-WebSocket-Version"];
			}
		}

		/// <summary>
		/// Gets the endpoint to which the handshake request is sent.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Net.IPEndPoint" /> that represents the server IP
		/// address and port number.
		/// </value>
		public override IPEndPoint ServerEndPoint
		{
			get
			{
				return (IPEndPoint)_serverEndPoint;
			}
		}

		/// <summary>
		/// Gets the client information.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.Security.Principal.IPrincipal" /> instance that represents identity,
		///   authentication, and security roles for the client.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the client is not authenticated.
		///   </para>
		/// </value>
		public override IPrincipal User
		{
			get
			{
				return _user;
			}
		}

		/// <summary>
		/// Gets the endpoint from which the handshake request is sent.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Net.IPEndPoint" /> that represents the client IP
		/// address and port number.
		/// </value>
		public override IPEndPoint UserEndPoint
		{
			get
			{
				return (IPEndPoint)_userEndPoint;
			}
		}

		/// <summary>
		/// Gets the WebSocket instance used for two-way communication between
		/// the client and server.
		/// </summary>
		/// <value>
		/// A <see cref="T:Sfs2X.WebSocketSharp.WebSocket" />.
		/// </value>
		public override WebSocket WebSocket
		{
			get
			{
				return _websocket;
			}
		}

		internal TcpListenerWebSocketContext(TcpClient tcpClient, string protocol, bool secure, ServerSslConfiguration sslConfig, Logger log)
		{
			_tcpClient = tcpClient;
			_secure = secure;
			_log = log;
			NetworkStream stream = tcpClient.GetStream();
			if (secure)
			{
				SslStream sslStream = new SslStream(stream, false, sslConfig.ClientCertificateValidationCallback);
				sslStream.AuthenticateAsServer(sslConfig.ServerCertificate, sslConfig.ClientCertificateRequired, sslConfig.EnabledSslProtocols, sslConfig.CheckCertificateRevocation);
				_stream = sslStream;
			}
			else
			{
				_stream = stream;
			}
			Socket client = tcpClient.Client;
			_serverEndPoint = client.LocalEndPoint;
			_userEndPoint = client.RemoteEndPoint;
			_request = HttpRequest.Read(_stream, 90000);
			_websocket = new WebSocket(this, protocol);
		}

		private HttpRequest sendAuthenticationChallenge(string challenge)
		{
			HttpResponse httpResponse = HttpResponse.CreateUnauthorizedResponse(challenge);
			byte[] array = httpResponse.ToByteArray();
			_stream.Write(array, 0, array.Length);
			return HttpRequest.Read(_stream, 15000);
		}

		internal bool Authenticate(AuthenticationSchemes scheme, string realm, Func<IIdentity, NetworkCredential> credentialsFinder)
		{
			string chal = new AuthenticationChallenge(scheme, realm).ToString();
			int retry = -1;
			Func<bool> auth = null;
			auth = delegate
			{
				retry++;
				if (retry > 99)
				{
					return false;
				}
				IPrincipal principal = HttpUtility.CreateUser(_request.Headers["Authorization"], scheme, realm, _request.HttpMethod, credentialsFinder);
				if (principal != null && principal.Identity.IsAuthenticated)
				{
					_user = principal;
					return true;
				}
				_request = sendAuthenticationChallenge(chal);
				return auth();
			};
			return auth();
		}

		internal void Close()
		{
			_stream.Close();
			_tcpClient.Close();
		}

		internal void Close(HttpStatusCode code)
		{
			HttpResponse httpResponse = HttpResponse.CreateCloseResponse(code);
			byte[] array = httpResponse.ToByteArray();
			_stream.Write(array, 0, array.Length);
			_stream.Close();
			_tcpClient.Close();
		}

		/// <summary>
		/// Returns a string that represents the current instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that contains the request line and headers
		/// included in the handshake request.
		/// </returns>
		public override string ToString()
		{
			return _request.ToString();
		}
	}
}
