using System.Security.Principal;

namespace Sfs2X.WebSocketSharp.Net
{
	/// <summary>
	/// Holds the username and password from an HTTP Basic authentication attempt.
	/// </summary>
	public class HttpBasicIdentity : GenericIdentity
	{
		private string _password;

		/// <summary>
		/// Gets the password from a basic authentication attempt.
		/// </summary>
		/// <value>
		/// A <see cref="T:System.String" /> that represents the password.
		/// </value>
		public virtual string Password
		{
			get
			{
				return _password;
			}
		}

		internal HttpBasicIdentity(string username, string password)
			: base(username, "Basic")
		{
			_password = password;
		}
	}
}
