using System;

namespace Sfs2X.Entities.Match
{
	/// <summary>
	/// The UserProperties class contains the names of predefined properties that can be used in matching expressions to search/filter users.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Entities.Match.MatchExpression" />
	/// <seealso cref="T:Sfs2X.Entities.User" />
	public class UserProperties
	{
		/// <summary>
		/// The user name.
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.StringMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string NAME = "${N}";

		/// <summary>
		/// The user is a player in a Game Room.
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.BoolMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string IS_PLAYER = "${ISP}";

		/// <summary>
		/// The user is a spectator in a Game Room.
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.BoolMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string IS_SPECTATOR = "${ISS}";

		/// <summary>
		/// The user is a Non-Player Character (NPC).
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.BoolMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string IS_NPC = "${ISN}";

		/// <summary>
		/// The user privilege id.
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.NumberMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string PRIVILEGE_ID = "${PRID}";

		/// <summary>
		/// The user joined at least one Room.
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.BoolMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string IS_IN_ANY_ROOM = "${IAR}";

		/// <exclude />
		public UserProperties()
		{
			throw new ArgumentException("This class cannot be instantiated");
		}
	}
}
