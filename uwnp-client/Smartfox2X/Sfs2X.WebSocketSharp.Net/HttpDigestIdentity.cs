using System.Collections.Specialized;
using System.Security.Principal;

namespace Sfs2X.WebSocketSharp.Net
{
	/// <summary>
	/// Holds the username and other parameters from
	/// an HTTP Digest authentication attempt.
	/// </summary>
	public class HttpDigestIdentity : GenericIdentity
	{
		private NameValueCollection _parameters;

		/// <summary>
		/// Gets the algorithm parameter from a digest authentication attempt.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the algorithm parameter.
		/// </value>
		public string Algorithm
		{
			get
			{
				return _parameters["algorithm"];
			}
		}

		/// <summary>
		/// Gets the cnonce parameter from a digest authentication attempt.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the cnonce parameter.
		/// </value>
		public string Cnonce
		{
			get
			{
				return _parameters["cnonce"];
			}
		}

		/// <summary>
		/// Gets the nc parameter from a digest authentication attempt.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the nc parameter.
		/// </value>
		public string Nc
		{
			get
			{
				return _parameters["nc"];
			}
		}

		/// <summary>
		/// Gets the nonce parameter from a digest authentication attempt.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the nonce parameter.
		/// </value>
		public string Nonce
		{
			get
			{
				return _parameters["nonce"];
			}
		}

		/// <summary>
		/// Gets the opaque parameter from a digest authentication attempt.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the opaque parameter.
		/// </value>
		public string Opaque
		{
			get
			{
				return _parameters["opaque"];
			}
		}

		/// <summary>
		/// Gets the qop parameter from a digest authentication attempt.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the qop parameter.
		/// </value>
		public string Qop
		{
			get
			{
				return _parameters["qop"];
			}
		}

		/// <summary>
		/// Gets the realm parameter from a digest authentication attempt.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the realm parameter.
		/// </value>
		public string Realm
		{
			get
			{
				return _parameters["realm"];
			}
		}

		/// <summary>
		/// Gets the response parameter from a digest authentication attempt.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the response parameter.
		/// </value>
		public string Response
		{
			get
			{
				return _parameters["response"];
			}
		}

		/// <summary>
		/// Gets the uri parameter from a digest authentication attempt.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the uri parameter.
		/// </value>
		public string Uri
		{
			get
			{
				return _parameters["uri"];
			}
		}

		internal HttpDigestIdentity(NameValueCollection parameters)
			: base(parameters["username"], "Digest")
		{
			_parameters = parameters;
		}

		internal bool IsValid(string password, string realm, string method, string entity)
		{
			NameValueCollection nameValueCollection = new NameValueCollection(_parameters);
			nameValueCollection["password"] = password;
			nameValueCollection["realm"] = realm;
			nameValueCollection["method"] = method;
			nameValueCollection["entity"] = entity;
			string text = AuthenticationResponse.CreateRequestDigest(nameValueCollection);
			return _parameters["response"] == text;
		}
	}
}
