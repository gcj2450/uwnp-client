using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Security.Principal;

namespace Sfs2X.WebSocketSharp.Net.WebSockets
{
	/// <summary>
	/// Exposes the access to the information in a WebSocket handshake request.
	/// </summary>
	/// <remarks>
	/// This class is an abstract class.
	/// </remarks>
	public abstract class WebSocketContext
	{
		/// <summary>
		/// Gets the HTTP cookies included in the handshake request.
		/// </summary>
		/// <value>
		/// A <see cref="T:Sfs2X.WebSocketSharp.Net.CookieCollection" /> that contains
		/// the cookies.
		/// </value>
		public abstract CookieCollection CookieCollection { get; }

		/// <summary>
		/// Gets the HTTP headers included in the handshake request.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> that contains the headers.
		/// </value>
		public abstract NameValueCollection Headers { get; }

		/// <summary>
		/// Gets the value of the Host header included in the handshake request.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the server host name requested
		/// by the client.
		/// </value>
		public abstract string Host { get; }

		/// <summary>
		/// Gets a value indicating whether the client is authenticated.
		/// </summary>
		/// <value>
		/// <c>true</c> if the client is authenticated; otherwise, <c>false</c>.
		/// </value>
		public abstract bool IsAuthenticated { get; }

		/// <summary>
		/// Gets a value indicating whether the handshake request is sent from
		/// the local computer.
		/// </summary>
		/// <value>
		/// <c>true</c> if the handshake request is sent from the same computer
		/// as the server; otherwise, <c>false</c>.
		/// </value>
		public abstract bool IsLocal { get; }

		/// <summary>
		/// Gets a value indicating whether a secure connection is used to send
		/// the handshake request.
		/// </summary>
		/// <value>
		/// <c>true</c> if the connection is secure; otherwise, <c>false</c>.
		/// </value>
		public abstract bool IsSecureConnection { get; }

		/// <summary>
		/// Gets a value indicating whether the request is a WebSocket handshake
		/// request.
		/// </summary>
		/// <value>
		/// <c>true</c> if the request is a WebSocket handshake request; otherwise,
		/// <c>false</c>.
		/// </value>
		public abstract bool IsWebSocketRequest { get; }

		/// <summary>
		/// Gets the value of the Origin header included in the handshake request.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the value of the Origin header.
		/// </value>
		public abstract string Origin { get; }

		/// <summary>
		/// Gets the query string included in the handshake request.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> that contains the query parameters.
		/// </value>
		public abstract NameValueCollection QueryString { get; }

		/// <summary>
		/// Gets the URI requested by the client.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Uri" /> that represents the URI parsed from the request.
		/// </value>
		public abstract Uri RequestUri { get; }

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
		/// </value>
		public abstract string SecWebSocketKey { get; }

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
		public abstract IEnumerable<string> SecWebSocketProtocols { get; }

		/// <summary>
		/// Gets the value of the Sec-WebSocket-Version header included in
		/// the handshake request.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the WebSocket protocol
		/// version specified by the client.
		/// </value>
		public abstract string SecWebSocketVersion { get; }

		/// <summary>
		/// Gets the endpoint to which the handshake request is sent.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Net.IPEndPoint" /> that represents the server IP
		/// address and port number.
		/// </value>
		public abstract IPEndPoint ServerEndPoint { get; }

		/// <summary>
		/// Gets the client information.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Security.Principal.IPrincipal" /> instance that represents identity,
		/// authentication, and security roles for the client.
		/// </value>
		public abstract IPrincipal User { get; }

		/// <summary>
		/// Gets the endpoint from which the handshake request is sent.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Net.IPEndPoint" /> that represents the client IP
		/// address and port number.
		/// </value>
		public abstract IPEndPoint UserEndPoint { get; }

		/// <summary>
		/// Gets the WebSocket instance used for two-way communication between
		/// the client and server.
		/// </summary>
		/// <value>
		/// A <see cref="T:Sfs2X.WebSocketSharp.WebSocket" />.
		/// </value>
		public abstract WebSocket WebSocket { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.WebSockets.WebSocketContext" /> class.
		/// </summary>
		protected WebSocketContext()
		{
		}
	}
}
