namespace Sfs2X.Util
{
	/// <summary>
	/// The ClientDisconnectionReason class contains the costants describing the possible reasons why a disconnection from the server occurred.
	/// </summary>
	public static class ClientDisconnectionReason
	{
		/// <summary>
		/// Client was disconnected because it was idle for too long.
		/// </summary>
		///
		/// <remarks>
		/// The connection timeout depends on the server settings.
		/// </remarks>
		public static readonly string IDLE = "idle";

		/// <summary>
		/// Client was kicked out of the server.
		/// </summary>
		///
		/// <remarks>
		/// Kicking can occur automatically (i.e. for swearing, if the words filter is active) or due to the intervention of a user with enough privileges (i.e. an administrator or a moderator).
		/// </remarks>
		public static readonly string KICK = "kick";

		/// <summary>
		/// Client was banned from the server.
		/// </summary>
		///
		/// <remarks>
		/// Banning can occur automatically (i.e. for flooding, if the flood filter is active) or due to the intervention of a user with enough privileges (i.e. an administrator or a moderator).
		/// </remarks>
		public static readonly string BAN = "ban";

		/// <summary>
		/// The client manually disconnected from the server.
		/// </summary>
		///
		/// <remarks>
		/// The <see cref="M:Sfs2X.SmartFox.Disconnect" /> method on the <em>SmartFox</em> class was called.
		/// </remarks>
		public static readonly string MANUAL = "manual";

		/// <summary>
		/// A generic network error occurred, and the client is unable to determine the cause of the disconnection.
		/// </summary>
		///
		/// <remarks>
		/// The server-side log should be checked for possible error messages or warnings.
		/// </remarks>
		public static readonly string UNKNOWN = "unknown";

		private static string[] reasons = new string[3] { "idle", "kick", "ban" };

		/// <exclude />
		public static string GetReason(int reasonId)
		{
			return reasons[reasonId];
		}
	}
}
