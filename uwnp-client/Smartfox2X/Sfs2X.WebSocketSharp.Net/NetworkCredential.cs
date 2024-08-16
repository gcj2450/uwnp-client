using System;

namespace Sfs2X.WebSocketSharp.Net
{
	/// <summary>
	/// Provides the credentials for the password-based authentication.
	/// </summary>
	public class NetworkCredential
	{
		private string _domain;

		private static readonly string[] _noRoles;

		private string _password;

		private string[] _roles;

		private string _username;

		/// <summary>
		/// Gets the domain associated with the credentials.
		/// </summary>
		/// <remarks>
		/// This property returns an empty string if the domain was
		/// initialized with <see langword="null" />.
		/// </remarks>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the domain name
		/// to which the username belongs.
		/// </value>
		public string Domain
		{
			get
			{
				return _domain ?? string.Empty;
			}
			internal set
			{
				_domain = value;
			}
		}

		/// <summary>
		/// Gets the password for the username associated with the credentials.
		/// </summary>
		/// <remarks>
		/// This property returns an empty string if the password was
		/// initialized with <see langword="null" />.
		/// </remarks>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the password.
		/// </value>
		public string Password
		{
			get
			{
				return _password ?? string.Empty;
			}
			internal set
			{
				_password = value;
			}
		}

		/// <summary>
		/// Gets the roles associated with the credentials.
		/// </summary>
		/// <remarks>
		/// This property returns an empty array if the roles were
		/// initialized with <see langword="null" />.
		/// </remarks>
		/// <value>
		/// An array of <see cref="T:System.String" /> that represents the role names
		/// to which the username belongs.
		/// </value>
		public string[] Roles
		{
			get
			{
				return _roles ?? _noRoles;
			}
			internal set
			{
				_roles = value;
			}
		}

		/// <summary>
		/// Gets the username associated with the credentials.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the username.
		/// </value>
		public string Username
		{
			get
			{
				return _username;
			}
			internal set
			{
				_username = value;
			}
		}

		static NetworkCredential()
		{
			_noRoles = new string[0];
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.NetworkCredential" /> class with
		/// the specified <paramref name="username" /> and <paramref name="password" />.
		/// </summary>
		/// <param name="username">
		/// A <see cref="T:System.String" /> that represents the username associated with
		/// the credentials.
		/// </param>
		/// <param name="password">
		/// A <see cref="T:System.String" /> that represents the password for the username
		/// associated with the credentials.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="username" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="username" /> is empty.
		/// </exception>
		public NetworkCredential(string username, string password)
			: this(username, password, (string)null, (string[])null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Sfs2X.WebSocketSharp.Net.NetworkCredential" /> class with
		/// the specified <paramref name="username" />, <paramref name="password" />,
		/// <paramref name="domain" /> and <paramref name="roles" />.
		/// </summary>
		/// <param name="username">
		/// A <see cref="T:System.String" /> that represents the username associated with
		/// the credentials.
		/// </param>
		/// <param name="password">
		/// A <see cref="T:System.String" /> that represents the password for the username
		/// associated with the credentials.
		/// </param>
		/// <param name="domain">
		/// A <see cref="T:System.String" /> that represents the domain associated with
		/// the credentials.
		/// </param>
		/// <param name="roles">
		/// An array of <see cref="T:System.String" /> that represents the roles
		/// associated with the credentials if any.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="username" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="username" /> is empty.
		/// </exception>
		public NetworkCredential(string username, string password, string domain, params string[] roles)
		{
			if (username == null)
			{
				throw new ArgumentNullException("username");
			}
			if (username.Length == 0)
			{
				throw new ArgumentException("An empty string.", "username");
			}
			_username = username;
			_password = password;
			_domain = domain;
			_roles = roles;
		}
	}
}
