using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Sfs2X.WebSocketSharp.Net
{
	/// <summary>
	/// Provides the access to a response to a request received by the <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListener" />.
	/// </summary>
	/// <remarks>
	/// The HttpListenerResponse class cannot be inherited.
	/// </remarks>
	public sealed class HttpListenerResponse : IDisposable
	{
		private bool _closeConnection;

		private Encoding _contentEncoding;

		private long _contentLength;

		private string _contentType;

		private HttpListenerContext _context;

		private CookieCollection _cookies;

		private bool _disposed;

		private WebHeaderCollection _headers;

		private bool _headersSent;

		private bool _keepAlive;

		private string _location;

		private ResponseStream _outputStream;

		private bool _sendChunked;

		private int _statusCode;

		private string _statusDescription;

		private Version _version;

		internal bool CloseConnection
		{
			get
			{
				return _closeConnection;
			}
			set
			{
				_closeConnection = value;
			}
		}

		internal bool HeadersSent
		{
			get
			{
				return _headersSent;
			}
			set
			{
				_headersSent = value;
			}
		}

		/// <summary>
		/// Gets or sets the encoding for the entity body data included in the response.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Text.Encoding" /> that represents the encoding for the entity body data,
		/// or <see langword="null" /> if no encoding is specified.
		/// </value>
		/// <exception cref="T:System.ObjectDisposedException">
		/// This object is closed.
		/// </exception>
		public Encoding ContentEncoding
		{
			get
			{
				return _contentEncoding;
			}
			set
			{
				checkDisposed();
				_contentEncoding = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of bytes in the entity body data included in the response.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Int64" /> that represents the value of the Content-Length entity-header.
		/// </value>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// The value specified for a set operation is less than zero.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The response has already been sent.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// This object is closed.
		/// </exception>
		public long ContentLength64
		{
			get
			{
				return _contentLength;
			}
			set
			{
				checkDisposedOrHeadersSent();
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("Less than zero.", "value");
				}
				_contentLength = value;
			}
		}

		/// <summary>
		/// Gets or sets the media type of the entity body included in the response.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the media type of the entity body,
		/// or <see langword="null" /> if no media type is specified. This value is
		/// used for the value of the Content-Type entity-header.
		/// </value>
		/// <exception cref="T:System.ArgumentException">
		/// The value specified for a set operation is empty.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// This object is closed.
		/// </exception>
		public string ContentType
		{
			get
			{
				return _contentType;
			}
			set
			{
				checkDisposed();
				if (value != null && value.Length == 0)
				{
					throw new ArgumentException("An empty string.", "value");
				}
				_contentType = value;
			}
		}

		/// <summary>
		/// Gets or sets the cookies sent with the response.
		/// </summary>
		/// <value>
		/// A <see cref="T:Sfs2X.WebSocketSharp.Net.CookieCollection" /> that contains the cookies sent with the response.
		/// </value>
		public CookieCollection Cookies
		{
			get
			{
				return _cookies ?? (_cookies = new CookieCollection());
			}
			set
			{
				_cookies = value;
			}
		}

		/// <summary>
		/// Gets or sets the HTTP headers sent to the client.
		/// </summary>
		/// <value>
		/// A <see cref="T:Sfs2X.WebSocketSharp.Net.WebHeaderCollection" /> that contains the headers sent to the client.
		/// </value>
		/// <exception cref="T:System.InvalidOperationException">
		/// The value specified for a set operation isn't valid for a response.
		/// </exception>
		public WebHeaderCollection Headers
		{
			get
			{
				return _headers ?? (_headers = new WebHeaderCollection(HttpHeaderType.Response, false));
			}
			set
			{
				if (value != null && value.State != HttpHeaderType.Response)
				{
					throw new InvalidOperationException("The specified headers aren't valid for a response.");
				}
				_headers = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the server requests a persistent connection.
		/// </summary>
		/// <value>
		/// <c>true</c> if the server requests a persistent connection; otherwise, <c>false</c>.
		/// The default value is <c>true</c>.
		/// </value>
		/// <exception cref="T:System.InvalidOperationException">
		/// The response has already been sent.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// This object is closed.
		/// </exception>
		public bool KeepAlive
		{
			get
			{
				return _keepAlive;
			}
			set
			{
				checkDisposedOrHeadersSent();
				_keepAlive = value;
			}
		}

		/// <summary>
		/// Gets a <see cref="T:System.IO.Stream" /> to use to write the entity body data.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.IO.Stream" /> to use to write the entity body data.
		/// </value>
		/// <exception cref="T:System.ObjectDisposedException">
		/// This object is closed.
		/// </exception>
		public Stream OutputStream
		{
			get
			{
				checkDisposed();
				return _outputStream ?? (_outputStream = _context.Connection.GetResponseStream());
			}
		}

		/// <summary>
		/// Gets or sets the HTTP version used in the response.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Version" /> that represents the version used in the response.
		/// </value>
		/// <exception cref="T:System.ArgumentNullException">
		/// The value specified for a set operation is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// The value specified for a set operation doesn't have its <c>Major</c> property set to 1 or
		/// doesn't have its <c>Minor</c> property set to either 0 or 1.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The response has already been sent.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// This object is closed.
		/// </exception>
		public Version ProtocolVersion
		{
			get
			{
				return _version;
			}
			set
			{
				checkDisposedOrHeadersSent();
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Major != 1 || (value.Minor != 0 && value.Minor != 1))
				{
					throw new ArgumentException("Not 1.0 or 1.1.", "value");
				}
				_version = value;
			}
		}

		/// <summary>
		/// Gets or sets the URL to which the client is redirected to locate a requested resource.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the value of the Location response-header,
		/// or <see langword="null" /> if no redirect location is specified.
		/// </value>
		/// <exception cref="T:System.ArgumentException">
		/// The value specified for a set operation isn't an absolute URL.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// This object is closed.
		/// </exception>
		public string RedirectLocation
		{
			get
			{
				return _location;
			}
			set
			{
				checkDisposed();
				if (value == null)
				{
					_location = null;
					return;
				}
				Uri result = null;
				if (!value.MaybeUri() || !Uri.TryCreate(value, UriKind.Absolute, out result))
				{
					throw new ArgumentException("Not an absolute URL.", "value");
				}
				_location = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the response uses the chunked transfer encoding.
		/// </summary>
		/// <value>
		/// <c>true</c> if the response uses the chunked transfer encoding;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.
		/// </value>
		/// <exception cref="T:System.InvalidOperationException">
		/// The response has already been sent.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// This object is closed.
		/// </exception>
		public bool SendChunked
		{
			get
			{
				return _sendChunked;
			}
			set
			{
				checkDisposedOrHeadersSent();
				_sendChunked = value;
			}
		}

		/// <summary>
		/// Gets or sets the HTTP status code returned to the client.
		/// </summary>
		/// <value>
		/// An <see cref="T:System.Int32" /> that represents the status code for the response to
		/// the request. The default value is same as <see cref="F:Sfs2X.WebSocketSharp.Net.HttpStatusCode.OK" />.
		/// </value>
		/// <exception cref="T:System.InvalidOperationException">
		/// The response has already been sent.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// This object is closed.
		/// </exception>
		/// <exception cref="T:System.Net.ProtocolViolationException">
		/// The value specified for a set operation is invalid. Valid values are
		/// between 100 and 999 inclusive.
		/// </exception>
		public int StatusCode
		{
			get
			{
				return _statusCode;
			}
			set
			{
				checkDisposedOrHeadersSent();
				if (value < 100 || value > 999)
				{
					throw new ProtocolViolationException("A value isn't between 100 and 999 inclusive.");
				}
				_statusCode = value;
				_statusDescription = value.GetStatusDescription();
			}
		}

		/// <summary>
		/// Gets or sets the description of the HTTP status code returned to the client.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the description of the status code. The default
		/// value is the <see href="http://tools.ietf.org/html/rfc2616#section-10">RFC 2616</see>
		/// description for the <see cref="P:Sfs2X.WebSocketSharp.Net.HttpListenerResponse.StatusCode" /> property value,
		/// or <see cref="F:System.String.Empty" /> if an RFC 2616 description doesn't exist.
		/// </value>
		/// <exception cref="T:System.ArgumentException">
		/// The value specified for a set operation contains invalid characters.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The response has already been sent.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// This object is closed.
		/// </exception>
		public string StatusDescription
		{
			get
			{
				return _statusDescription;
			}
			set
			{
				checkDisposedOrHeadersSent();
				if (value == null || value.Length == 0)
				{
					_statusDescription = _statusCode.GetStatusDescription();
					return;
				}
				if (!value.IsText() || value.IndexOfAny(new char[2] { '\r', '\n' }) > -1)
				{
					throw new ArgumentException("Contains invalid characters.", "value");
				}
				_statusDescription = value;
			}
		}

		internal HttpListenerResponse(HttpListenerContext context)
		{
			_context = context;
			_keepAlive = true;
			_statusCode = 200;
			_statusDescription = "OK";
			_version = HttpVersion.Version11;
		}

		private bool canAddOrUpdate(Cookie cookie)
		{
			if (_cookies == null || _cookies.Count == 0)
			{
				return true;
			}
			List<Cookie> list = findCookie(cookie).ToList();
			if (list.Count == 0)
			{
				return true;
			}
			int version = cookie.Version;
			foreach (Cookie item in list)
			{
				if (item.Version == version)
				{
					return true;
				}
			}
			return false;
		}

		private void checkDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
		}

		private void checkDisposedOrHeadersSent()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (_headersSent)
			{
				throw new InvalidOperationException("Cannot be changed after the headers are sent.");
			}
		}

		private void close(bool force)
		{
			_disposed = true;
			_context.Connection.Close(force);
		}

		private IEnumerable<Cookie> findCookie(Cookie cookie)
		{
			string name = cookie.Name;
			string domain = cookie.Domain;
			string path = cookie.Path;
			if (_cookies == null)
			{
				yield break;
			}
			foreach (Cookie c in _cookies)
			{
				if (c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase) && c.Path.Equals(path, StringComparison.Ordinal))
				{
					yield return c;
				}
			}
		}

		internal WebHeaderCollection WriteHeadersTo(MemoryStream destination)
		{
			WebHeaderCollection webHeaderCollection = new WebHeaderCollection(HttpHeaderType.Response, true);
			if (_headers != null)
			{
				webHeaderCollection.Add(_headers);
			}
			if (_contentType != null)
			{
				string value = ((_contentType.IndexOf("charset=", StringComparison.Ordinal) == -1 && _contentEncoding != null) ? string.Format("{0}; charset={1}", _contentType, _contentEncoding.WebName) : _contentType);
				webHeaderCollection.InternalSet("Content-Type", value, true);
			}
			if (webHeaderCollection["Server"] == null)
			{
				webHeaderCollection.InternalSet("Server", "websocket-sharp/1.0", true);
			}
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			if (webHeaderCollection["Date"] == null)
			{
				webHeaderCollection.InternalSet("Date", DateTime.UtcNow.ToString("r", invariantCulture), true);
			}
			if (!_sendChunked)
			{
				webHeaderCollection.InternalSet("Content-Length", _contentLength.ToString(invariantCulture), true);
			}
			else
			{
				webHeaderCollection.InternalSet("Transfer-Encoding", "chunked", true);
			}
			bool flag = !_context.Request.KeepAlive || !_keepAlive || _statusCode == 400 || _statusCode == 408 || _statusCode == 411 || _statusCode == 413 || _statusCode == 414 || _statusCode == 500 || _statusCode == 503;
			int reuses = _context.Connection.Reuses;
			if (flag || reuses >= 100)
			{
				webHeaderCollection.InternalSet("Connection", "close", true);
			}
			else
			{
				webHeaderCollection.InternalSet("Keep-Alive", string.Format("timeout=15,max={0}", 100 - reuses), true);
				if (_context.Request.ProtocolVersion < HttpVersion.Version11)
				{
					webHeaderCollection.InternalSet("Connection", "keep-alive", true);
				}
			}
			if (_location != null)
			{
				webHeaderCollection.InternalSet("Location", _location, true);
			}
			if (_cookies != null)
			{
				foreach (Cookie cookie in _cookies)
				{
					webHeaderCollection.InternalSet("Set-Cookie", cookie.ToResponseString(), true);
				}
			}
			Encoding encoding = _contentEncoding ?? Encoding.Default;
			StreamWriter streamWriter = new StreamWriter(destination, encoding, 256);
			streamWriter.Write("HTTP/{0} {1} {2}\r\n", _version, _statusCode, _statusDescription);
			streamWriter.Write(webHeaderCollection.ToStringMultiValue(true));
			streamWriter.Flush();
			destination.Position = encoding.GetPreamble().Length;
			return webHeaderCollection;
		}

		/// <summary>
		/// Closes the connection to the client without returning a response.
		/// </summary>
		public void Abort()
		{
			if (!_disposed)
			{
				close(true);
			}
		}

		/// <summary>
		/// Adds an HTTP header with the specified <paramref name="name" /> and
		/// <paramref name="value" /> to the headers for the response.
		/// </summary>
		/// <param name="name">
		/// A <see cref="T:System.String" /> that represents the name of the header to add.
		/// </param>
		/// <param name="value">
		/// A <see cref="T:System.String" /> that represents the value of the header to add.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" /> or empty.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="name" /> or <paramref name="value" /> contains invalid characters.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="name" /> is a restricted header name.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// The length of <paramref name="value" /> is greater than 65,535 characters.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The header cannot be allowed to add to the current headers.
		/// </exception>
		public void AddHeader(string name, string value)
		{
			Headers.Set(name, value);
		}

		/// <summary>
		/// Appends the specified <paramref name="cookie" /> to the cookies sent with the response.
		/// </summary>
		/// <param name="cookie">
		/// A <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" /> to append.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="cookie" /> is <see langword="null" />.
		/// </exception>
		public void AppendCookie(Cookie cookie)
		{
			Cookies.Add(cookie);
		}

		/// <summary>
		/// Appends a <paramref name="value" /> to the specified HTTP header sent with the response.
		/// </summary>
		/// <param name="name">
		/// A <see cref="T:System.String" /> that represents the name of the header to append
		/// <paramref name="value" /> to.
		/// </param>
		/// <param name="value">
		/// A <see cref="T:System.String" /> that represents the value to append to the header.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" /> or empty.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <para>
		///   <paramref name="name" /> or <paramref name="value" /> contains invalid characters.
		///   </para>
		///   <para>
		///   -or-
		///   </para>
		///   <para>
		///   <paramref name="name" /> is a restricted header name.
		///   </para>
		/// </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// The length of <paramref name="value" /> is greater than 65,535 characters.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The current headers cannot allow the header to append a value.
		/// </exception>
		public void AppendHeader(string name, string value)
		{
			Headers.Add(name, value);
		}

		/// <summary>
		/// Returns the response to the client and releases the resources used by
		/// this <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerResponse" /> instance.
		/// </summary>
		public void Close()
		{
			if (!_disposed)
			{
				close(false);
			}
		}

		/// <summary>
		/// Returns the response with the specified array of <see cref="T:System.Byte" /> to the client and
		/// releases the resources used by this <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerResponse" /> instance.
		/// </summary>
		/// <param name="responseEntity">
		/// An array of <see cref="T:System.Byte" /> that contains the response entity body data.
		/// </param>
		/// <param name="willBlock">
		/// <c>true</c> if this method blocks execution while flushing the stream to the client;
		/// otherwise, <c>false</c>.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="responseEntity" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// This object is closed.
		/// </exception>
		public void Close(byte[] responseEntity, bool willBlock)
		{
			checkDisposed();
			if (responseEntity == null)
			{
				throw new ArgumentNullException("responseEntity");
			}
			int count = responseEntity.Length;
			Stream output = OutputStream;
			if (willBlock)
			{
				output.Write(responseEntity, 0, count);
				close(false);
				return;
			}
			output.BeginWrite(responseEntity, 0, count, delegate(IAsyncResult ar)
			{
				output.EndWrite(ar);
				close(false);
			}, null);
		}

		/// <summary>
		/// Copies some properties from the specified <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerResponse" /> to
		/// this response.
		/// </summary>
		/// <param name="templateResponse">
		/// A <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerResponse" /> to copy.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="templateResponse" /> is <see langword="null" />.
		/// </exception>
		public void CopyFrom(HttpListenerResponse templateResponse)
		{
			if (templateResponse == null)
			{
				throw new ArgumentNullException("templateResponse");
			}
			if (templateResponse._headers != null)
			{
				if (_headers != null)
				{
					_headers.Clear();
				}
				Headers.Add(templateResponse._headers);
			}
			else if (_headers != null)
			{
				_headers = null;
			}
			_contentLength = templateResponse._contentLength;
			_statusCode = templateResponse._statusCode;
			_statusDescription = templateResponse._statusDescription;
			_keepAlive = templateResponse._keepAlive;
			_version = templateResponse._version;
		}

		/// <summary>
		/// Configures the response to redirect the client's request to
		/// the specified <paramref name="url" />.
		/// </summary>
		/// <remarks>
		/// This method sets the <see cref="P:Sfs2X.WebSocketSharp.Net.HttpListenerResponse.RedirectLocation" /> property to
		/// <paramref name="url" />, the <see cref="P:Sfs2X.WebSocketSharp.Net.HttpListenerResponse.StatusCode" /> property to
		/// <c>302</c>, and the <see cref="P:Sfs2X.WebSocketSharp.Net.HttpListenerResponse.StatusDescription" /> property to
		/// <c>"Found"</c>.
		/// </remarks>
		/// <param name="url">
		/// A <see cref="T:System.String" /> that represents the URL to redirect the client's request to.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="url" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="url" /> isn't an absolute URL.
		/// </exception>
		/// <exception cref="T:System.InvalidOperationException">
		/// The response has already been sent.
		/// </exception>
		/// <exception cref="T:System.ObjectDisposedException">
		/// This object is closed.
		/// </exception>
		public void Redirect(string url)
		{
			checkDisposedOrHeadersSent();
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			Uri result = null;
			if (!url.MaybeUri() || !Uri.TryCreate(url, UriKind.Absolute, out result))
			{
				throw new ArgumentException("Not an absolute URL.", "url");
			}
			_location = url;
			_statusCode = 302;
			_statusDescription = "Found";
		}

		/// <summary>
		/// Adds or updates a <paramref name="cookie" /> in the cookies sent with the response.
		/// </summary>
		/// <param name="cookie">
		/// A <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" /> to set.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="cookie" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="cookie" /> already exists in the cookies and couldn't be replaced.
		/// </exception>
		public void SetCookie(Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			if (!canAddOrUpdate(cookie))
			{
				throw new ArgumentException("Cannot be replaced.", "cookie");
			}
			Cookies.Add(cookie);
		}

		/// <summary>
		/// Releases all resources used by the <see cref="T:Sfs2X.WebSocketSharp.Net.HttpListenerResponse" />.
		/// </summary>
		void IDisposable.Dispose()
		{
			if (!_disposed)
			{
				close(true);
			}
		}
	}
}
