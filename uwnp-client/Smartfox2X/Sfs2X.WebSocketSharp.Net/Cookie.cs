using System;
using System.Globalization;
using System.Text;

namespace Sfs2X.WebSocketSharp.Net
{
	/// <summary>
	/// Provides a set of methods and properties used to manage an HTTP Cookie.
	/// </summary>
	/// <remarks>
	///   <para>
	///   The Cookie class supports the following cookie formats:
	///   <see href="http://web.archive.org/web/20020803110822/http://wp.netscape.com/newsref/std/cookie_spec.html">Netscape specification</see>,
	///   <see href="http://www.ietf.org/rfc/rfc2109.txt">RFC 2109</see>, and
	///   <see href="http://www.ietf.org/rfc/rfc2965.txt">RFC 2965</see>
	///   </para>
	///   <para>
	///   The Cookie class cannot be inherited.
	///   </para>
	/// </remarks>
	[Serializable]
	public sealed class Cookie
	{
		private string _comment;

		private Uri _commentUri;

		private bool _discard;

		private string _domain;

		private DateTime _expires;

		private bool _httpOnly;

		private string _name;

		private string _path;

		private string _port;

		private int[] _ports;

		private static readonly char[] _reservedCharsForName;

		private static readonly char[] _reservedCharsForValue;

		private bool _secure;

		private DateTime _timestamp;

		private string _value;

		private int _version;

		internal bool ExactDomain { get; set; }

		internal int MaxAge
		{
			get
			{
				if (_expires == DateTime.MinValue)
				{
					return 0;
				}
				DateTime dateTime = ((_expires.Kind != DateTimeKind.Local) ? _expires.ToLocalTime() : _expires);
				TimeSpan timeSpan = dateTime - DateTime.Now;
				return (timeSpan > TimeSpan.Zero) ? ((int)timeSpan.TotalSeconds) : 0;
			}
		}

		internal int[] Ports
		{
			get
			{
				return _ports;
			}
		}

		/// <summary>
		/// Gets or sets the value of the Comment attribute of the cookie.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the comment to document intended use of the cookie.
		/// </value>
		public string Comment
		{
			get
			{
				return _comment;
			}
			set
			{
				_comment = value ?? string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the value of the CommentURL attribute of the cookie.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.Uri" /> that represents the URI that provides the comment to document intended
		/// use of the cookie.
		/// </value>
		public Uri CommentUri
		{
			get
			{
				return _commentUri;
			}
			set
			{
				_commentUri = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the client discards the cookie unconditionally
		/// when the client terminates.
		/// </summary>
		/// <value>
		/// <c>true</c> if the client discards the cookie unconditionally when the client terminates;
		/// otherwise, <c>false</c>. The default value is <c>false</c>.
		/// </value>
		public bool Discard
		{
			get
			{
				return _discard;
			}
			set
			{
				_discard = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of the Domain attribute of the cookie.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the URI for which the cookie is valid.
		/// </value>
		public string Domain
		{
			get
			{
				return _domain;
			}
			set
			{
				if (value.IsNullOrEmpty())
				{
					_domain = string.Empty;
					ExactDomain = true;
				}
				else
				{
					_domain = value;
					ExactDomain = value[0] != '.';
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the cookie has expired.
		/// </summary>
		/// <value>
		/// <c>true</c> if the cookie has expired; otherwise, <c>false</c>.
		/// The default value is <c>false</c>.
		/// </value>
		public bool Expired
		{
			get
			{
				return _expires != DateTime.MinValue && _expires <= DateTime.Now;
			}
			set
			{
				_expires = (value ? DateTime.Now : DateTime.MinValue);
			}
		}

		/// <summary>
		/// Gets or sets the value of the Expires attribute of the cookie.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.DateTime" /> that represents the date and time at which the cookie expires.
		/// The default value is <see cref="F:System.DateTime.MinValue" />.
		/// </value>
		public DateTime Expires
		{
			get
			{
				return _expires;
			}
			set
			{
				_expires = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether non-HTTP APIs can access the cookie.
		/// </summary>
		/// <value>
		/// <c>true</c> if non-HTTP APIs cannot access the cookie; otherwise, <c>false</c>.
		/// The default value is <c>false</c>.
		/// </value>
		public bool HttpOnly
		{
			get
			{
				return _httpOnly;
			}
			set
			{
				_httpOnly = value;
			}
		}

		/// <summary>
		/// Gets or sets the Name of the cookie.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the Name of the cookie.
		/// </value>
		/// <exception cref="T:Sfs2X.WebSocketSharp.Net.CookieException">
		///   <para>
		///   The value specified for a set operation is <see langword="null" /> or empty.
		///   </para>
		///   <para>
		///   - or -
		///   </para>
		///   <para>
		///   The value specified for a set operation contains an invalid character.
		///   </para>
		/// </exception>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				string message;
				if (!canSetName(value, out message))
				{
					throw new CookieException(message);
				}
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of the Path attribute of the cookie.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the subset of URI on the origin server
		/// to which the cookie applies.
		/// </value>
		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value ?? string.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the value of the Port attribute of the cookie.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the list of TCP ports to which the cookie applies.
		/// </value>
		/// <exception cref="T:Sfs2X.WebSocketSharp.Net.CookieException">
		/// The value specified for a set operation isn't enclosed in double quotes or
		/// couldn't be parsed.
		/// </exception>
		public string Port
		{
			get
			{
				return _port;
			}
			set
			{
				if (value.IsNullOrEmpty())
				{
					_port = string.Empty;
					_ports = new int[0];
					return;
				}
				if (!value.IsEnclosedIn('"'))
				{
					throw new CookieException("The value specified for the Port attribute isn't enclosed in double quotes.");
				}
				string parseError;
				if (!tryCreatePorts(value, out _ports, out parseError))
				{
					throw new CookieException(string.Format("The value specified for the Port attribute contains an invalid value: {0}", parseError));
				}
				_port = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the security level of the cookie is secure.
		/// </summary>
		/// <remarks>
		/// When this property is <c>true</c>, the cookie may be included in the HTTP request
		/// only if the request is transmitted over the HTTPS.
		/// </remarks>
		/// <value>
		/// <c>true</c> if the security level of the cookie is secure; otherwise, <c>false</c>.
		/// The default value is <c>false</c>.
		/// </value>
		public bool Secure
		{
			get
			{
				return _secure;
			}
			set
			{
				_secure = value;
			}
		}

		/// <summary>
		/// Gets the time when the cookie was issued.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.DateTime" /> that represents the time when the cookie was issued.
		/// </value>
		public DateTime TimeStamp
		{
			get
			{
				return _timestamp;
			}
		}

		/// <summary>
		/// Gets or sets the Value of the cookie.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the Value of the cookie.
		/// </value>
		/// <exception cref="T:Sfs2X.WebSocketSharp.Net.CookieException">
		///   <para>
		///   The value specified for a set operation is <see langword="null" />.
		///   </para>
		///   <para>
		///   - or -
		///   </para>
		///   <para>
		///   The value specified for a set operation contains a string not enclosed in double quotes
		///   that contains an invalid character.
		///   </para>
		/// </exception>
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				string message;
				if (!canSetValue(value, out message))
				{
					throw new CookieException(message);
				}
				_value = ((value.Length > 0) ? value : "\"\"");
			}
		}

		/// <summary>
		/// Gets or sets the value of the Version attribute of the cookie.
		/// </summary>
		/// <value>
		/// An <see cref="T:System.Int32" /> that represents the version of the HTTP state management
		/// to which the cookie conforms.
		/// </value>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// The value specified for a set operation isn't 0 or 1.
		/// </exception>
		public int Version
		{
			get
			{
				return _version;
			}
			set
			{
				if (value < 0 || value > 1)
				{
					throw new ArgumentOutOfRangeException("value", "Not 0 or 1.");
				}
				_version = value;
			}
		}

		static Cookie()
		{
			_reservedCharsForName = new char[7] { ' ', '=', ';', ',', '\n', '\r', '\t' };
			_reservedCharsForValue = new char[2] { ';', ',' };
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" /> class.
		/// </summary>
		public Cookie()
		{
			_comment = string.Empty;
			_domain = string.Empty;
			_expires = DateTime.MinValue;
			_name = string.Empty;
			_path = string.Empty;
			_port = string.Empty;
			_ports = new int[0];
			_timestamp = DateTime.Now;
			_value = string.Empty;
			_version = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" /> class with the specified
		/// <paramref name="name" /> and <paramref name="value" />.
		/// </summary>
		/// <param name="name">
		/// A <see cref="T:System.String" /> that represents the Name of the cookie.
		/// </param>
		/// <param name="value">
		/// A <see cref="T:System.String" /> that represents the Value of the cookie.
		/// </param>
		/// <exception cref="T:Sfs2X.WebSocketSharp.Net.CookieException">
		///   <para>
		///   <paramref name="name" /> is <see langword="null" /> or empty.
		///   </para>
		///   <para>
		///   - or -
		///   </para>
		///   <para>
		///   <paramref name="name" /> contains an invalid character.
		///   </para>
		///   <para>
		///   - or -
		///   </para>
		///   <para>
		///   <paramref name="value" /> is <see langword="null" />.
		///   </para>
		///   <para>
		///   - or -
		///   </para>
		///   <para>
		///   <paramref name="value" /> contains a string not enclosed in double quotes
		///   that contains an invalid character.
		///   </para>
		/// </exception>
		public Cookie(string name, string value)
			: this()
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" /> class with the specified
		/// <paramref name="name" />, <paramref name="value" />, and <paramref name="path" />.
		/// </summary>
		/// <param name="name">
		/// A <see cref="T:System.String" /> that represents the Name of the cookie.
		/// </param>
		/// <param name="value">
		/// A <see cref="T:System.String" /> that represents the Value of the cookie.
		/// </param>
		/// <param name="path">
		/// A <see cref="T:System.String" /> that represents the value of the Path attribute of the cookie.
		/// </param>
		/// <exception cref="T:Sfs2X.WebSocketSharp.Net.CookieException">
		///   <para>
		///   <paramref name="name" /> is <see langword="null" /> or empty.
		///   </para>
		///   <para>
		///   - or -
		///   </para>
		///   <para>
		///   <paramref name="name" /> contains an invalid character.
		///   </para>
		///   <para>
		///   - or -
		///   </para>
		///   <para>
		///   <paramref name="value" /> is <see langword="null" />.
		///   </para>
		///   <para>
		///   - or -
		///   </para>
		///   <para>
		///   <paramref name="value" /> contains a string not enclosed in double quotes
		///   that contains an invalid character.
		///   </para>
		/// </exception>
		public Cookie(string name, string value, string path)
			: this(name, value)
		{
			Path = path;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" /> class with the specified
		/// <paramref name="name" />, <paramref name="value" />, <paramref name="path" />, and
		/// <paramref name="domain" />.
		/// </summary>
		/// <param name="name">
		/// A <see cref="T:System.String" /> that represents the Name of the cookie.
		/// </param>
		/// <param name="value">
		/// A <see cref="T:System.String" /> that represents the Value of the cookie.
		/// </param>
		/// <param name="path">
		/// A <see cref="T:System.String" /> that represents the value of the Path attribute of the cookie.
		/// </param>
		/// <param name="domain">
		/// A <see cref="T:System.String" /> that represents the value of the Domain attribute of the cookie.
		/// </param>
		/// <exception cref="T:Sfs2X.WebSocketSharp.Net.CookieException">
		///   <para>
		///   <paramref name="name" /> is <see langword="null" /> or empty.
		///   </para>
		///   <para>
		///   - or -
		///   </para>
		///   <para>
		///   <paramref name="name" /> contains an invalid character.
		///   </para>
		///   <para>
		///   - or -
		///   </para>
		///   <para>
		///   <paramref name="value" /> is <see langword="null" />.
		///   </para>
		///   <para>
		///   - or -
		///   </para>
		///   <para>
		///   <paramref name="value" /> contains a string not enclosed in double quotes
		///   that contains an invalid character.
		///   </para>
		/// </exception>
		public Cookie(string name, string value, string path, string domain)
			: this(name, value, path)
		{
			Domain = domain;
		}

		private static bool canSetName(string name, out string message)
		{
			if (name.IsNullOrEmpty())
			{
				message = "The value specified for the Name is null or empty.";
				return false;
			}
			if (name[0] == '$' || name.Contains(_reservedCharsForName))
			{
				message = "The value specified for the Name contains an invalid character.";
				return false;
			}
			message = string.Empty;
			return true;
		}

		private static bool canSetValue(string value, out string message)
		{
			if (value == null)
			{
				message = "The value specified for the Value is null.";
				return false;
			}
			if (value.Contains(_reservedCharsForValue) && !value.IsEnclosedIn('"'))
			{
				message = "The value specified for the Value contains an invalid character.";
				return false;
			}
			message = string.Empty;
			return true;
		}

		private static int hash(int i, int j, int k, int l, int m)
		{
			return i ^ ((j << 13) | (j >> 19)) ^ ((k << 26) | (k >> 6)) ^ ((l << 7) | (l >> 25)) ^ ((m << 20) | (m >> 12));
		}

		private string toResponseStringVersion0()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0}={1}", _name, _value);
			if (_expires != DateTime.MinValue)
			{
				stringBuilder.AppendFormat("; Expires={0}", _expires.ToUniversalTime().ToString("ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'", CultureInfo.CreateSpecificCulture("en-US")));
			}
			if (!_path.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Path={0}", _path);
			}
			if (!_domain.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Domain={0}", _domain);
			}
			if (_secure)
			{
				stringBuilder.Append("; Secure");
			}
			if (_httpOnly)
			{
				stringBuilder.Append("; HttpOnly");
			}
			return stringBuilder.ToString();
		}

		private string toResponseStringVersion1()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0}={1}; Version={2}", _name, _value, _version);
			if (_expires != DateTime.MinValue)
			{
				stringBuilder.AppendFormat("; Max-Age={0}", MaxAge);
			}
			if (!_path.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Path={0}", _path);
			}
			if (!_domain.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Domain={0}", _domain);
			}
			if (!_port.IsNullOrEmpty())
			{
				if (_port == "\"\"")
				{
					stringBuilder.Append("; Port");
				}
				else
				{
					stringBuilder.AppendFormat("; Port={0}", _port);
				}
			}
			if (!_comment.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; Comment={0}", _comment.UrlEncode());
			}
			if (_commentUri != null)
			{
				string originalString = _commentUri.OriginalString;
				stringBuilder.AppendFormat("; CommentURL={0}", originalString.IsToken() ? originalString : originalString.Quote());
			}
			if (_discard)
			{
				stringBuilder.Append("; Discard");
			}
			if (_secure)
			{
				stringBuilder.Append("; Secure");
			}
			return stringBuilder.ToString();
		}

		private static bool tryCreatePorts(string value, out int[] result, out string parseError)
		{
			string[] array = value.Trim('"').Split(',');
			int num = array.Length;
			int[] array2 = new int[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = int.MinValue;
				string text = array[i].Trim();
				if (text.Length != 0 && !int.TryParse(text, out array2[i]))
				{
					result = new int[0];
					parseError = text;
					return false;
				}
			}
			result = array2;
			parseError = string.Empty;
			return true;
		}

		internal string ToRequestString(Uri uri)
		{
			if (_name.Length == 0)
			{
				return string.Empty;
			}
			if (_version == 0)
			{
				return string.Format("{0}={1}", _name, _value);
			}
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("$Version={0}; {1}={2}", _version, _name, _value);
			if (!_path.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; $Path={0}", _path);
			}
			else if (uri != null)
			{
				stringBuilder.AppendFormat("; $Path={0}", uri.GetAbsolutePath());
			}
			else
			{
				stringBuilder.Append("; $Path=/");
			}
			if ((uri == null || uri.Host != _domain) && !_domain.IsNullOrEmpty())
			{
				stringBuilder.AppendFormat("; $Domain={0}", _domain);
			}
			if (!_port.IsNullOrEmpty())
			{
				if (_port == "\"\"")
				{
					stringBuilder.Append("; $Port");
				}
				else
				{
					stringBuilder.AppendFormat("; $Port={0}", _port);
				}
			}
			return stringBuilder.ToString();
		}

		internal string ToResponseString()
		{
			return (_name.Length <= 0) ? string.Empty : ((_version == 0) ? toResponseStringVersion0() : toResponseStringVersion1());
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object" /> is equal to the current
		/// <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" />.
		/// </summary>
		/// <param name="comparand">
		/// An <see cref="T:System.Object" /> to compare with the current <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" />.
		/// </param>
		/// <returns>
		/// <c>true</c> if <paramref name="comparand" /> is equal to the current <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" />;
		/// otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object comparand)
		{
			Cookie cookie = comparand as Cookie;
			return cookie != null && _name.Equals(cookie.Name, StringComparison.InvariantCultureIgnoreCase) && _value.Equals(cookie.Value, StringComparison.InvariantCulture) && _path.Equals(cookie.Path, StringComparison.InvariantCulture) && _domain.Equals(cookie.Domain, StringComparison.InvariantCultureIgnoreCase) && _version == cookie.Version;
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" /> object.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Int32" /> that represents the hash code for the current <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" />.
		/// </returns>
		public override int GetHashCode()
		{
			return hash(StringComparer.InvariantCultureIgnoreCase.GetHashCode(_name), _value.GetHashCode(), _path.GetHashCode(), StringComparer.InvariantCultureIgnoreCase.GetHashCode(_domain), _version);
		}

		/// <summary>
		/// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" />.
		/// </summary>
		/// <remarks>
		/// This method returns a <see cref="T:System.String" /> to use to send an HTTP Cookie to
		/// an origin server.
		/// </remarks>
		/// <returns>
		/// A <see cref="T:System.String" /> that represents the current <see cref="T:Sfs2X.WebSocketSharp.Net.Cookie" />.
		/// </returns>
		public override string ToString()
		{
			return ToRequestString(null);
		}
	}
}
