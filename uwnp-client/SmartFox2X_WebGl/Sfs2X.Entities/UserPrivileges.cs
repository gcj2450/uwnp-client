namespace Sfs2X.Entities
{
	/// <summary>
	/// This class contains the costants describing the default user types known by SmartFoxServer.
	/// </summary>
	///
	/// <remarks>
	/// The server assigns one of these values or a custom-defined one to the <see cref="P:Sfs2X.Entities.User.PrivilegeId">User.PrivilegeId</see> property whenever a user logs in.
	/// <para />
	/// Read the SmartFoxServer 2X documentation for more informations about privilege profiles and their permissions.
	/// </remarks>
	///
	/// <seealso cref="P:Sfs2X.Entities.User.PrivilegeId" />
	public enum UserPrivileges
	{
		/// <summary>
		/// The Guest user is usually the lowest level in the privilege profiles scale.
		/// </summary>
		GUEST,
		/// <summary>
		/// The standard user is usually registered in the application custom login system; uses a unique name and password to login.
		/// </summary>
		STANDARD,
		/// <summary>
		/// The moderator user can send dedicated "moderator messages", kick and ban users.
		/// </summary>
		///
		/// <seealso cref="T:Sfs2X.Requests.ModeratorMessageRequest" />
		/// <seealso cref="T:Sfs2X.Requests.KickUserRequest" />
		/// <seealso cref="T:Sfs2X.Requests.BanUserRequest" />
		MODERATOR,
		/// <summary>
		/// The administrator user can send dedicated "administrator messages", kick and ban users.
		/// </summary>
		///
		/// <seealso cref="T:Sfs2X.Requests.AdminMessageRequest" />
		/// <seealso cref="T:Sfs2X.Requests.KickUserRequest" />
		/// <seealso cref="T:Sfs2X.Requests.BanUserRequest" />
		ADMINISTRATOR
	}
}
