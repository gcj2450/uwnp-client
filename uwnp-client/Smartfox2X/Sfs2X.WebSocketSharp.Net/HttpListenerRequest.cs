using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Sfs2X.WebSocketSharp.Net
{
	/// <summary>
	/// Represents an incoming request to a <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListener" /> instance.
	/// </summary>
	/// <remarks>
	/// This class cannot be inherited.
	/// </remarks>
	public sealed class HttpListenerRequest
	{
		private static readonly byte[] _100continue;

		private string[] _acceptTypes;

		private bool _chunked;

		private HttpConnection _connection;

		private Encoding _contentEncoding;

		private long _contentLength;

		private HttpListenerContext _context;

		private CookieCollection _cookies;

		private WebHeaderCollection _headers;

		private string _httpMethod;

		private Stream _inputStream;

		private Version _protocolVersion;

		private NameValueCollection _queryString;

		private string _rawUrl;

		private Guid _requestTraceIdentifier;

		private Uri _url;

		private Uri _urlReferrer;

		private bool _urlSet;

		private string _userHostName;

		private string[] _userLanguages;

		/// <summary>
		/// Gets the media types that are acceptable for the client.
		/// </summary>
		/// <value>
		///   <para>
		///   An array of <see cref="T:System.String" /> that contains the names of the media
		///   types specified in the value of the Accept header.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the header is not present.
		///   </para>
		/// </value>
		public string[] AcceptTypes
		{
			get
			{
				string text = _headers["Accept"];
				if (text == null)
				{
					return null;
				}
				if (_acceptTypes == null)
				{
					_acceptTypes = text.SplitHeaderValue(',').Trim().ToList()
						.ToArray();
				}
				return _acceptTypes;
			}
		}

		/// <summary>
		/// Gets an error code that identifies a problem with the certificate
		/// provided by the client.
		/// </summary>
		/// <value>
		/// An <see cref="T:System.Int32" /> that represents an error code.
		/// </value>
		/// <exception cref="T:System.NotSupportedException">
		/// This property is not supported.
		/// </exception>
		public int ClientCertificateError
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Gets the encoding for the entity body data included in the request.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.Text.Encoding" /> converted from the charset value of the
		///   Content-Type header.
		///   </para>
		///   <para>
		///   <see cref="P:System.Text.Encoding.UTF8" /> if the charset value is not available.
		///   </para>
		/// </value>
		public Encoding ContentEncoding
		{
			get
			{
				if (_contentEncoding == null)
				{
					_contentEncoding = getContentEncoding() ?? Encoding.UTF8;
				}
				return _contentEncoding;
			}
		}

		/// <summary>
		/// Gets the length in bytes of the entity body data included in the
		/// request.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.Int64" /> converted from the value of the Content-Length
		///   header.
		///   </para>
		///   <para>
		///   -1 if the header is not present.
		///   </para>
		/// </value>
		public long ContentLength64
		{
			get
			{
				return _contentLength;
			}
		}

		/// <summary>
		/// Gets the media type of the entity body data included in the request.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.String" /> that represents the value of the Content-Type
		///   header.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the header is not present.
		///   </para>
		/// </value>
		public string ContentType
		{
			get
			{
				return _headers["Content-Type"];
			}
		}

		/// <summary>
		/// Gets the cookies included in the request.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:Sfs2X.WebSocketSharp.Net.CookieCollection" /> that contains the cookies.
		///   </para>
		///   <para>
		///   An empty collection if not included.
		///   </para>
		/// </value>
		public CookieCollection Cookies
		{
			get
			{
				if (_cookies == null)
				{
					_cookies = _headers.GetCookies(false);
				}
				return _cookies;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the request has the entity body data.
		/// </summary>
		/// <value>
		/// <c>true</c> if the request has the entity body data; otherwise,
		/// <c>false</c>.
		/// </value>
		public bool HasEntityBody
		{
			get
			{
				return _contentLength > 0 || _chunked;
			}
		}

		/// <summary>
		/// Gets the headers included in the request.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Collections.Specialized.NameValueCollection" /> that contains the headers.
		/// </value>
		public NameValueCollection Headers
		{
			get
			{
				return _headers;
			}
		}

		/// <summary>
		/// Gets the HTTP method specified by the client.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the HTTP method specified in
		/// the request line.
		/// </value>
		public string HttpMethod
		{
			get
			{
				return _httpMethod;
			}
		}

		/// <summary>
		/// Gets a stream that contains the entity body data included in
		/// the request.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.IO.Stream" /> that contains the entity body data.
		///   </para>
		///   <para>
		///   <see cref="F:System.IO.Stream.Null" /> if the entity body data is not available.
		///   </para>
		/// </value>
		public Stream InputStream
		{
			get
			{
				if (_inputStream == null)
				{
					_inputStream = getInputStream() ?? Stream.Null;
				}
				return _inputStream;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the client is authenticated.
		/// </summary>
		/// <value>
		/// <c>true</c> if the client is authenticated; otherwise, <c>false</c>.
		/// </value>
		public bool IsAuthenticated
		{
			get
			{
				return _context.User != null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the request is sent from the local
		/// computer.
		/// </summary>
		/// <value>
		/// <c>true</c> if the request is sent from the same computer as the server;
		/// otherwise, <c>false</c>.
		/// </value>
		public bool IsLocal
		{
			get
			{
				return _connection.IsLocal;
			}
		}

		/// <summary>
		/// Gets a value indicating whether a secure connection is used to send
		/// the request.
		/// </summary>
		/// <value>
		/// <c>true</c> if the connection is secure; otherwise, <c>false</c>.
		/// </value>
		public bool IsSecureConnection
		{
			get
			{
				return _connection.IsSecure;
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
		public bool IsWebSocketRequest
		{
			get
			{
				return _httpMethod == "GET" && _protocolVersion > HttpVersion.Version10 && _headers.Upgrades("websocket");
			}
		}

		/// <summary>
		/// Gets a value indicating whether a persistent connection is requested.
		/// </summary>
		/// <value>
		/// <c>true</c> if the request specifies that the connection is kept open;
		/// otherwise, <c>false</c>.
		/// </value>
		public bool KeepAlive
		{
			get
			{
				return _headers.KeepsAlive(_protocolVersion);
			}
		}

		/// <summary>
		/// Gets the endpoint to which the request is sent.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Net.IPEndPoint" /> that represents the server IP
		/// address and port number.
		/// </value>
		public IPEndPoint LocalEndPoint
		{
			get
			{
				return _connection.LocalEndPoint;
			}
		}

		/// <summary>
		/// Gets the HTTP version specified by the client.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Version" /> that represents the HTTP version specified in
		/// the request line.
		/// </value>
		public Version ProtocolVersion
		{
			get
			{
				return _protocolVersion;
			}
		}

		/// <summary>
		/// Gets the query string included in the request.
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
		public NameValueCollection QueryString
		{
			get
			{
				if (_queryString == null)
				{
					Uri url = Url;
					_queryString = QueryStringCollection.Parse((url != null) ? url.Query : null, Encoding.UTF8);
				}
				return _queryString;
			}
		}

		/// <summary>
		/// Gets the raw URL specified by the client.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the request target specified in
		/// the request line.
		/// </value>
		public string RawUrl
		{
			get
			{
				return _rawUrl;
			}
		}

		/// <summary>
		/// Gets the endpoint from which the request is sent.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Net.IPEndPoint" /> that represents the client IP
		/// address and port number.
		/// </value>
		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return _connection.RemoteEndPoint;
			}
		}

		/// <summary>
		/// Gets the trace identifier of the request.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Guid" /> that represents the trace identifier.
		/// </value>
		public Guid RequestTraceIdentifier
		{
			get
			{
				return _requestTraceIdentifier;
			}
		}

		/// <summary>
		/// Gets the URL requested by the client.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.Uri" /> that represents the URL parsed from the request.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the URL cannot be parsed.
		///   </para>
		/// </value>
		public Uri Url
		{
			get
			{
				if (!_urlSet)
				{
					_url = HttpUtility.CreateRequestUrl(_rawUrl, _userHostName ?? UserHostAddress, IsWebSocketRequest, IsSecureConnection);
					_urlSet = true;
				}
				return _url;
			}
		}

		/// <summary>
		/// Gets the URI of the resource from which the requested URL was obtained.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.Uri" /> converted from the value of the Referer header.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the header value is not available.
		///   </para>
		/// </value>
		public Uri UrlReferrer
		{
			get
			{
				string text = _headers["Referer"];
				if (text == null)
				{
					return null;
				}
				if (_urlReferrer == null)
				{
					_urlReferrer = text.ToUri();
				}
				return _urlReferrer;
			}
		}

		/// <summary>
		/// Gets the user agent from which the request is originated.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.String" /> that represents the value of the User-Agent
		///   header.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the header is not present.
		///   </para>
		/// </value>
		public string UserAgent
		{
			get
			{
				return _headers["User-Agent"];
			}
		}

		/// <summary>
		/// Gets the IP address and port number to which the request is sent.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the server IP address and port
		/// number.
		/// </value>
		public string UserHostAddress
		{
			get
			{
				return _connection.LocalEndPoint.ToString();
			}
		}

		/// <summary>
		/// Gets the server host name requested by the client.
		/// </summary>
		/// <value>
		///   <para>
		///   A <see cref="T:System.String" /> that represents the value of the Host header.
		///   </para>
		///   <para>
		///   It includes the port number if provided.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the header is not present.
		///   </para>
		/// </value>
		public string UserHostName
		{
			get
			{
				return _userHostName;
			}
		}

		/// <summary>
		/// Gets the natural languages that are acceptable for the client.
		/// </summary>
		/// <value>
		///   <para>
		///   An array of <see cref="T:System.String" /> that contains the names of the
		///   natural languages specified in the value of the Accept-Language
		///   header.
		///   </para>
		///   <para>
		///   <see langword="null" /> if the header is not present.
		///   </para>
		/// </value>
		public string[] UserLanguages
		{
			get
			{
				string text = _headers["Accept-Language"];
				if (text == null)
				{
					return null;
				}
				if (_userLanguages == null)
				{
					_userLanguages = text.Split(',').Trim().ToList()
						.ToArray();
				}
				return _userLanguages;
			}
		}

		static HttpListenerRequest()
		{
			_100continue = Encoding.ASCII.GetBytes("HTTP/1.1 100 Continue\r\n\r\n");
		}

		internal HttpListenerRequest(HttpListenerContext context)
		{
			_context = context;
			_connection = context.Connection;
			_contentLength = -1L;
			_headers = new WebHeaderCollection();
			_requestTraceIdentifier = Guid.NewGuid();
		}

		private void finishInitialization10()
		{
			string text = _headers["Transfer-Encoding"];
			if (text != null)
			{
				_context.ErrorMessage = "Invalid Transfer-Encoding header";
			}
			else if (_httpMethod == "POST")
			{
				if (_contentLength == -1)
				{
					_context.ErrorMessage = "Content-Length header required";
				}
				else if (_contentLength == 0)
				{
					_context.ErrorMessage = "Invalid Content-Length header";
				}
			}
		}

		private Encoding getContentEncoding()
		{
			string text = _headers["Content-Type"];
			if (text == null)
			{
				return null;
			}
			Encoding result;
			HttpUtility.TryGetEncoding(text, out result);
			return result;
		}

		private RequestStream getInputStream()
		{
			return (_contentLength > 0 || _chunked) ? _connection.GetRequestStream(_contentLength, _chunked) : null;
		}

		internal void AddHeader(string headerField)
		{
			char c = headerField[0];
			if (c == ' ' || c == '\t')
			{
				_context.ErrorMessage = "Invalid header field";
				return;
			}
			int num = headerField.IndexOf(':');
			if (num < 1)
			{
				_context.ErrorMessage = "Invalid header field";
				return;
			}
			string text = headerField.Substring(0, num).Trim();
			if (text.Length == 0 || !text.IsToken())
			{
				_context.ErrorMessage = "Invalid header name";
				return;
			}
			string text2 = ((num < headerField.Length - 1) ? headerField.Substring(num + 1).Trim() : string.Empty);
			_headers.InternalSet(text, text2, false);
			string text3 = text.ToLower(CultureInfo.InvariantCulture);
			if (text3 == "host")
			{
				if (_userHostName != null)
				{
					_context.ErrorMessage = "Invalid Host header";
				}
				else if (text2.Length == 0)
				{
					_context.ErrorMessage = "Invalid Host header";
				}
				else
				{
					_userHostName = text2;
				}
			}
			else if (text3 == "content-length")
			{
				long result;
				if (_contentLength > -1)
				{
					_context.ErrorMessage = "Invalid Content-Length header";
				}
				else if (!long.TryParse(text2, out result))
				{
					_context.ErrorMessage = "Invalid Content-Length header";
				}
				else if (result < 0)
				{
					_context.ErrorMessage = "Invalid Content-Length header";
				}
				else
				{
					_contentLength = result;
				}
			}
		}

		internal void FinishInitialization()
		{
			if (_protocolVersion == HttpVersion.Version10)
			{
				finishInitialization10();
				return;
			}
			if (_userHostName == null)
			{
				_context.ErrorMessage = "Host header required";
				return;
			}
			string text = _headers["Transfer-Encoding"];
			if (text != null)
			{
				StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;
				if (!text.Equals("chunked", comparisonType))
				{
					_context.ErrorMessage = string.Empty;
					_context.ErrorStatus = 501;
					return;
				}
				_chunked = true;
			}
			if ((_httpMethod == "POST" || _httpMethod == "PUT") && _contentLength <= 0 && !_chunked)
			{
				_context.ErrorMessage = string.Empty;
				_context.ErrorStatus = 411;
				return;
			}
			string text2 = _headers["Expect"];
			if (text2 != null)
			{
				StringComparison comparisonType2 = StringComparison.OrdinalIgnoreCase;
				if (!text2.Equals("100-continue", comparisonType2))
				{
					_context.ErrorMessage = "Invalid Expect header";
					return;
				}
				ResponseStream responseStream = _connection.GetResponseStream();
				responseStream.InternalWrite(_100continue, 0, _100continue.Length);
			}
		}

		internal bool FlushInput()
		{
			Stream inputStream = InputStream;
			if (inputStream == Stream.Null)
			{
				return true;
			}
			int num = 2048;
			if (_contentLength > 0 && _contentLength < num)
			{
				num = (int)_contentLength;
			}
			byte[] buffer = new byte[num];
			while (true)
			{
				try
				{
					IAsyncResult asyncResult = inputStream.BeginRead(buffer, 0, num, null, null);
					if (!asyncResult.IsCompleted)
					{
						int millisecondsTimeout = 100;
						if (!asyncResult.AsyncWaitHandle.WaitOne(millisecondsTimeout))
						{
							return false;
						}
					}
					if (inputStream.EndRead(asyncResult) <= 0)
					{
						return true;
					}
				}
				catch
				{
					return false;
				}
			}
		}

		internal bool IsUpgradeRequest(string protocol)
		{
			return _headers.Upgrades(protocol);
		}

		internal void SetRequestLine(string requestLine)
		{
			string[] array = requestLine.Split(new char[1] { ' ' }, 3);
			if (array.Length < 3)
			{
				_context.ErrorMessage = "Invalid request line (parts)";
				return;
			}
			string text = array[0];
			if (text.Length == 0)
			{
				_context.ErrorMessage = "Invalid request line (method)";
				return;
			}
			string text2 = array[1];
			if (text2.Length == 0)
			{
				_context.ErrorMessage = "Invalid request line (target)";
				return;
			}
			string text3 = array[2];
			if (text3.Length != 8)
			{
				_context.ErrorMessage = "Invalid request line (version)";
				return;
			}
			if (text3.IndexOf("HTTP/") != 0)
			{
				_context.ErrorMessage = "Invalid request line (version)";
				return;
			}
			Version result;
			if (!text3.Substring(5).TryCreateVersion(out result))
			{
				_context.ErrorMessage = "Invalid request line (version)";
				return;
			}
			if (result.Major < 1)
			{
				_context.ErrorMessage = "Invalid request line (version)";
				return;
			}
			if (!text.IsHttpMethod(result))
			{
				_context.ErrorMessage = "Invalid request line (method)";
				return;
			}
			_httpMethod = text;
			_rawUrl = text2;
			_protocolVersion = result;
		}

		/// <summary>
		/// Begins getting the certificate provided by the client asynchronously.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.IAsyncResult" /> instance that indicates the status of the
		/// operation.
		/// </returns>
		/// <param name="requestCallback">
		/// An <see cref="T:System.AsyncCallback" /> delegate that invokes the method called
		/// when the operation is complete.
		/// </param>
		/// <param name="state">
		/// An <see cref="T:System.Object" /> that represents a user defined object to pass to
		/// the callback delegate.
		/// </param>
		/// <exception cref="T:System.NotSupportedException">
		/// This method is not supported.
		/// </exception>
		public IAsyncResult BeginGetClientCertificate(AsyncCallback requestCallback, object state)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Ends an asynchronous operation to get the certificate provided by the
		/// client.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that represents an X.509 certificate
		/// provided by the client.
		/// </returns>
		/// <param name="asyncResult">
		/// An <see cref="T:System.IAsyncResult" /> instance returned when the operation
		/// started.
		/// </param>
		/// <exception cref="T:System.NotSupportedException">
		/// This method is not supported.
		/// </exception>
		public X509Certificate2 EndGetClientCertificate(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the certificate provided by the client.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> that represents an X.509 certificate
		/// provided by the client.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">
		/// This method is not supported.
		/// </exception>
		public X509Certificate2 GetClientCertificate()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Returns a string that represents the current instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> that contains the request line and headers
		/// included in the request.
		/// </returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0} {1} HTTP/{2}\r\n", _httpMethod, _rawUrl, _protocolVersion).Append(_headers.ToString());
			return stringBuilder.ToString();
		}
	}
}
