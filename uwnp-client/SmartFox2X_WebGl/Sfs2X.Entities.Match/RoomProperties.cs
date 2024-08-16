using System;

namespace Sfs2X.Entities.Match
{
	/// <summary>
	/// The RoomProperties class contains the names of predefined properties that can be used in matching expressions to search/filter Rooms.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Entities.Match.MatchExpression" />
	/// <seealso cref="T:Sfs2X.Entities.Room" />
	public class RoomProperties
	{
		/// <summary>
		/// The Room name.
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.StringMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string NAME = "${N}";

		/// <summary>
		/// The name of the Group to which the Room belongs.
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.StringMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string GROUP_ID = "${G}";

		/// <summary>
		/// The maximum number of users allowed in the Room (players in Game Rooms).
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.NumberMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string MAX_USERS = "${MXU}";

		/// <summary>
		/// The maximum number of spectators allowed in the Room (Game Rooms only).
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.NumberMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string MAX_SPECTATORS = "${MXS}";

		/// <summary>
		/// The Room users count (players in Game Rooms).
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.NumberMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string USER_COUNT = "${UC}";

		/// <summary>
		/// The Room spectators count (Game Rooms only).
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.NumberMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string SPECTATOR_COUNT = "${SC}";

		/// <summary>
		/// The Room is a Game Room.
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.BoolMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string IS_GAME = "${ISG}";

		/// <summary>
		/// The Room is private.
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.BoolMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string IS_PRIVATE = "${ISP}";

		/// <summary>
		/// The Room has at least one free player slot.
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.BoolMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string HAS_FREE_PLAYER_SLOTS = "${HFP}";

		/// <summary>
		/// The Room is an <em>SFSGame</em> on the server-side.
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.BoolMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string IS_TYPE_SFSGAME = "${IST}";

		/// <summary>
		/// The Room is an <em>MMORom</em> on the server-side.
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.BoolMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string IS_TYPE_MMO = "${ISM}";

		/// <summary>
		/// The Room is of default type on the server-side (i.e. not an MMORoom or SFSGame).
		/// </summary>
		///
		/// <remarks>
		/// Requires a <see cref="T:Sfs2X.Entities.Match.BoolMatch" /> to be used for values comparison.
		/// </remarks>
		public static readonly string IS_TYPE_DEFAULT = "${ISD}";

		/// <exclude />
		public RoomProperties()
		{
			throw new ArgumentException("This class cannot be instantiated");
		}
	}
}
