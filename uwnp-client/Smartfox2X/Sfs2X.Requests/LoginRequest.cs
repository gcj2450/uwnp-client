using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;
using Sfs2X.Util;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Logs the current user in one of the server Zones.
	/// </summary>
	///
	/// <remarks>
	/// Each Zone represent an indipendent multiuser application governed by SmartFoxServer. In order to join a Zone, a user name and password are usually required.
	/// In order to validate the user credentials, a custom login process should be implemented in the Zone's server-side Extension.
	/// <para />
	/// Read the SmartFoxServer 2X documentation about the login process for more informations.
	/// <para />
	/// If the login operation is successful, the current user receives a <see cref="F:Sfs2X.Core.SFSEvent.LOGIN" /> event; otherwise the <see cref="F:Sfs2X.Core.SFSEvent.LOGIN_ERROR" /> event is fired.
	/// </remarks>
	///
	/// <example>
	/// The following example performs a login in the "SimpleChat" Zone:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
	/// 	sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
	///
	/// 	// Login
	/// 	sfs.Send( new LoginRequest("FozzieTheBear", "", "SimpleChat") );
	/// }
	///
	/// void OnLogin(BaseEvent evt) {
	/// 	Console.WriteLine("Login successful!");                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Login successful!");        // UWP
	/// }
	///
	/// void OnLoginError(BaseEvent evt) {
	/// 	Console.WriteLine("Login failure: " + (string)evt.Params["errorMessage"]);                      // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Login failure: " + (string)evt.Params["errorMessage"]);     // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.LOGIN" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.LOGIN_ERROR" />
	/// <seealso cref="T:Sfs2X.Requests.LogoutRequest" />
	public class LoginRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ZONE_NAME = "zn";

		/// <exclude />
		public static readonly string KEY_USER_NAME = "un";

		/// <exclude />
		public static readonly string KEY_PASSWORD = "pw";

		/// <exclude />
		public static readonly string KEY_PARAMS = "p";

		/// <exclude />
		public static readonly string KEY_PRIVILEGE_ID = "pi";

		/// <exclude />
		public static readonly string KEY_ID = "id";

		/// <exclude />
		public static readonly string KEY_ROOMLIST = "rl";

		/// <exclude />
		public static readonly string KEY_RECONNECTION_SECONDS = "rs";

		private string zoneName;

		private string userName;

		private string password;

		private ISFSObject parameters;

		private void Init(string userName, string password, string zoneName, ISFSObject parameters)
		{
			this.userName = userName;
			this.password = ((password == null) ? "" : password);
			this.zoneName = zoneName;
			this.parameters = parameters;
		}

		/// <summary>
		/// Creates a new LoginRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="userName">The name to be assigned to the user. If an empty string is passed and the Zone allows guest users, the name is generated automatically by the server.</param>
		/// <param name="password">The user password to access the system. SmartFoxServer doesn't offer a default authentication system, so the password must be validated implementing a custom login system in the Zone's server-side Extension.</param>
		/// <param name="zoneName">The name (case-sensitive) of the server Zone to login to; if a Zone name is not specified, the client will use the setting loaded via <see cref="M:Sfs2X.SmartFox.LoadConfig" /> method.</param>
		/// <param name="parameters">An instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" /> containing custom parameters to be passed to the Zone Extension (requires a custom login system to be in place). Default value is <c>null</c>.</param>
		public LoginRequest(string userName, string password, string zoneName, ISFSObject parameters)
			: base(RequestType.Login)
		{
			Init(userName, password, zoneName, parameters);
		}

		/// <summary>
		/// See <em>LoginRequest(string, string, string, ISFSObject)</em> constructor.
		/// </summary>
		public LoginRequest(string userName, string password, string zoneName)
			: base(RequestType.Login)
		{
			Init(userName, password, zoneName, null);
		}

		/// <summary>
		/// See <em>LoginRequest(string, string, string, ISFSObject)</em> constructor.
		/// </summary>
		public LoginRequest(string userName, string password)
			: base(RequestType.Login)
		{
			Init(userName, password, null, null);
		}

		/// <summary>
		/// See <em>LoginRequest(string, string, string, ISFSObject)</em> constructor.
		/// </summary>
		public LoginRequest(string userName)
			: base(RequestType.Login)
		{
			Init(userName, null, null, null);
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_ZONE_NAME, zoneName);
			sfso.PutUtfString(KEY_USER_NAME, userName);
			if (password.Length > 0)
			{
				password = PasswordUtil.MD5Password(sfs.SessionToken + password);
			}
			sfso.PutUtfString(KEY_PASSWORD, password);
			if (parameters != null)
			{
				sfso.PutSFSObject(KEY_PARAMS, parameters);
			}
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			if (sfs.MySelf != null)
			{
				throw new SFSValidationError("LoginRequest Error", new string[1] { "You are already logged in. Logout first" });
			}
			if ((zoneName == null || zoneName.Length == 0) && sfs.Config != null)
			{
				zoneName = sfs.Config.Zone;
			}
			if (zoneName == null || zoneName.Length == 0)
			{
				throw new SFSValidationError("LoginRequest Error", new string[1] { "Missing Zone name" });
			}
		}
	}
}
