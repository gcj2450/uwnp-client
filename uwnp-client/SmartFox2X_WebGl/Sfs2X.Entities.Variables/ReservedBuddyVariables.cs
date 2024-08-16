namespace Sfs2X.Entities.Variables
{
	/// <summary>
	/// The ReservedBuddyVariables class contains the costants describing the SmartFoxServer API reserved Buddy Variable names.
	/// </summary>
	///
	/// <remarks>
	/// Reserved Buddy Variables are used to store specific buddy-related informations.
	/// </remarks>
	public class ReservedBuddyVariables
	{
		/// <summary>
		/// The Buddy Variable with this name keeps track of the online/offline state of the user in a buddy list.
		/// </summary>
		///
		/// <remarks>
		/// This variable is persistent, which means that the online/offline state is preserved upon disconnection.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.IsOnline" />
		/// <seealso cref="P:Sfs2X.Entities.Managers.IBuddyManager.MyOnlineState" />
		public static readonly string BV_ONLINE = "$__BV_ONLINE__";

		/// <summary>
		/// The Buddy Variable with this name stores the custom state of the user in a buddy list.
		/// </summary>
		///
		/// <remarks>
		/// This variable is persistent, which means that the custom state is preserved upon disconnection.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.State" />
		/// <seealso cref="P:Sfs2X.Entities.Managers.IBuddyManager.MyState" />
		public static readonly string BV_STATE = "$__BV_STATE__";

		/// <summary>
		/// The Buddy Variable with this name stores the optional nickname of the user in a buddy list.
		/// </summary>
		///
		/// <remarks>
		/// This variable is persistent, which means that the nickname is preserved upon disconnection.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.Buddy.NickName" />
		/// <seealso cref="P:Sfs2X.Entities.Managers.IBuddyManager.MyNickName" />
		public static readonly string BV_NICKNAME = "$__BV_NICKNAME__";
	}
}
