using System;
using System.Security.Principal;
using Sfs2X.WebSocketSharp.Net.WebSockets;

namespace Sfs2X.WebSocketSharp.Net
{
	/// <summary>
	/// Provides the access to the HTTP request and response objects used by
	/// the <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListener" />.
	/// </summary>
	/// <remarks>
	/// This class cannot be inherited.
	/// </remarks>
	public sealed class HttpListenerContext
	{
		private HttpConnection _connection;

		private string _error;

		private int _errorStatus;

		private HttpListener _listener;

		private HttpListenerRequest _request;

		private HttpListenerResponse _response;

		private IPrincipal _user;

		private HttpListenerWebSocketContext _websocketContext;

		internal HttpConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		internal string ErrorMessage
		{
			get
			{
				return _error;
			}
			set
			{
				_error = value;
			}
		}

		internal int ErrorStatus
		{
			get
			{
				return _errorStatus;
			}
			set
			{
				_errorStatus = value;
			}
		}

		internal bool HasError
		{
			get
			{
				return _error != null;
			}
		}

		internal HttpListener Listener
		{
			get
			{
				return _listener;
			}
			set
			{
				_listener = value;
			}
		}

		/// <summary>
		/// Gets the HTTP request object that represents a client request.
		/// </summary>
		/// <value>
		/// A <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerRequest" /> that represents the client request.
		/// </value>
		public HttpListenerRequest Request
		{
			get
			{
				return _request;
			}
		}

		/// <summary>
		/// Gets the HTTP response object used to send a response to the client.
		/// </summary>
		/// <value>
		/// A <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerResponse" /> that represents a response to the client request.
		/// </value>
		public HttpListenerResponse Response
		{
			get
			{
				return _response;
			}
		}

		/// <summary>
		/// Gets the client information (identity, authentication, and security roles).
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Security.Principal.IPrincipal" /> instance that represents the client information.
		/// </value>
		public IPrincipal User
		{
			get
			{
				return _user;
			}
		}

		internal HttpListenerContext(HttpConnection connection)
		{
			_connection = connection;
			_errorStatus = 400;
			_request = new HttpListenerRequest(this);
			_response = new HttpListenerResponse(this);
		}

		internal bool Authenticate()
		{
			AuthenticationSchemes authenticationSchemes = _listener.SelectAuthenticationScheme(_request);
			switch (authenticationSchemes)
			{
			case AuthenticationSchemes.Anonymous:
				return true;
			case AuthenticationSchemes.None:
				_response.Close(HttpStatusCode.Forbidden);
				return false;
			default:
			{
				string realm = _listener.GetRealm();
				IPrincipal principal = HttpUtility.CreateUser(_request.Headers["Authorization"], authenticationSchemes, realm, _request.HttpMethod, _listener.GetUserCredentialsFinder());
				if (principal == null || !principal.Identity.IsAuthenticated)
				{
					_response.CloseWithAuthChallenge(new AuthenticationChallenge(authenticationSchemes, realm).ToString());
					return false;
				}
				_user = principal;
				return true;
			}
			}
		}

		internal bool Register()
		{
			return _listener.RegisterContext(this);
		}

		internal void Unregister()
		{
			_listener.UnregisterContext(this);
		}

		/// <summary>
		/// Accepts a WebSocket handshake request.
		/// </summary>
		/// <returns>
		/// A <see cref="T:Sfs2X.WebSocketSharp.Net.WebSockets.HttpListenerWebSocketContext" /> that represents
		/// the WebSocket handshake request.
		/// </returns>
		/// <param name="protocol">
		/// A <see cref="T:System.String" /> that represents the subprotocol supported on
		/// this WebSocket connection.
		/// </param>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="protocol" /> is empty.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="protocol" /> contains an invalid character.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// This method has already been called.
		/// </exception>
		public HttpListenerWebSocketContext AcceptWebSocket(string protocol)
		{
			if (_websocketContext != null)
			{
				throw new InvalidOperationException("The accepting is already in progress.");
			}
			if (protocol != null)
			{
				if (protocol.Length == 0)
				{
					throw new ArgumentException("An empty string.", "protocol");
				}
				if (!protocol.IsToken())
				{
					throw new ArgumentException("Contains an invalid character.", "protocol");
				}
			}
			_websocketContext = new HttpListenerWebSocketContext(this, protocol);
			return _websocketContext;
		}
	}
}
