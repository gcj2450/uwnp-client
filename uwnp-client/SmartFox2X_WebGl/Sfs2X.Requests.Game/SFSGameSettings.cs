using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Match;

namespace Sfs2X.Requests.Game
{
	/// <summary>
	/// The SFSGameSettings class is a container for the settings required to create a Game Room using the CreateSFSGameRequest request.
	/// </summary>
	///
	/// <remarks>
	/// On the server-side, a Game Room is represented by the <em>SFSGame</em> Java class which extends the <see cref="T:Sfs2X.Entities.Room" /> class providing new advanced features such as player matching,
	/// game invitations, public and private games, quick game joining, etc. On the client side Game Rooms are regular Rooms with their <see cref="P:Sfs2X.Entities.Room.IsGame">Room.IsGame</see> property set to true.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Requests.Game.CreateSFSGameRequest" />
	public class SFSGameSettings : RoomSettings
	{
		private bool isPublic;

		private int minPlayersToStartGame;

		private List<object> invitedPlayers;

		private List<string> searchableRooms;

		private MatchExpression playerMatchExpression;

		private MatchExpression spectatorMatchExpression;

		private int invitationExpiryTime;

		private bool leaveJoinedLastRoom;

		private bool notifyGameStarted;

		private ISFSObject invitationParams;

		/// <summary>
		/// Indicates whether the game is public or private.
		/// </summary>
		///
		/// <remarks>
		/// A public game can be joined by any player whose User Variables match the <see cref="P:Sfs2X.Requests.Game.SFSGameSettings.PlayerMatchExpression" /> assigned to the Game Room.
		/// A private game can be joined by users invited by the game creator by means of <see cref="P:Sfs2X.Requests.Game.SFSGameSettings.InvitedPlayers" /> list.
		/// <para />
		/// The default value is <c>true</c>.
		/// </remarks>
		public bool IsPublic
		{
			get
			{
				return isPublic;
			}
			set
			{
				isPublic = value;
			}
		}

		/// <summary>
		/// Defines the minimum number of players required to start the game.
		/// </summary>
		///
		/// <remarks>
		/// If the <see cref="P:Sfs2X.Requests.Game.SFSGameSettings.NotifyGameStarted" /> property is set to <c>true</c>, when this number is reached, the game start is notified.
		/// <para />
		/// The default value is <c>2</c>.
		/// </remarks>
		public int MinPlayersToStartGame
		{
			get
			{
				return minPlayersToStartGame;
			}
			set
			{
				minPlayersToStartGame = value;
			}
		}

		/// <summary>
		/// In private games, defines a list of User objects or Buddy objects representing players to be invited to join the game.
		/// </summary>
		///
		/// <remarks>
		/// If the invitations are less than the minimum number of players required to start the game (see the <see cref="P:Sfs2X.Requests.Game.SFSGameSettings.MinPlayersToStartGame" /> property),
		/// the server will send additional invitations automatically, searching users in the Room Groups specified in the <see cref="P:Sfs2X.Requests.Game.SFSGameSettings.SearchableRooms" /> list
		/// and filtering them by means of the object passed to the <see cref="P:Sfs2X.Requests.Game.SFSGameSettings.PlayerMatchExpression" /> property.
		/// <para />
		/// The game matching criteria contained in the <see cref="P:Sfs2X.Requests.Game.SFSGameSettings.PlayerMatchExpression" /> property do not apply to the users specified in this list.
		/// <para />
		/// The default value is an empty list.
		/// </remarks>
		public List<object> InvitedPlayers
		{
			get
			{
				return invitedPlayers;
			}
			set
			{
				invitedPlayers = value;
			}
		}

		/// <summary>
		/// In private games, defines a list of Groups names where to search players to invite.
		/// </summary>
		///
		/// <remarks>
		/// If the users invited to join the game (specified through the <see cref="P:Sfs2X.Requests.Game.SFSGameSettings.InvitedPlayers" /> property) are less than the minimum number of players
		/// required to start the game (see the <see cref="P:Sfs2X.Requests.Game.SFSGameSettings.MinPlayersToStartGame" /> property), the server will invite others automatically,
		/// searching them in Rooms belonging to the Groups specified in this list and filtering them by means of the object passed to the <see cref="P:Sfs2X.Requests.Game.SFSGameSettings.PlayerMatchExpression" /> property.
		/// <para />
		/// The default value is <c>null</c>.
		/// </remarks>
		public List<string> SearchableRooms
		{
			get
			{
				return searchableRooms;
			}
			set
			{
				searchableRooms = value;
			}
		}

		/// <summary>
		/// In private games, defines the number of seconds that the users invited to join the game have to reply to the invitation.
		/// </summary>
		///
		/// <remarks>
		/// The suggested range is 10 to 40 seconds. Default value is <c>15</c>.
		/// </remarks>
		public int InvitationExpiryTime
		{
			get
			{
				return invitationExpiryTime;
			}
			set
			{
				invitationExpiryTime = value;
			}
		}

		/// <summary>
		/// In private games, indicates whether the players must leave the previous Room when joining the game or not.
		/// </summary>
		///
		/// <remarks>
		/// This setting applies to private games only because users join the Game Room automatically when they accept the invitation to play,
		/// while public games require a <see cref="T:Sfs2X.Requests.JoinRoomRequest" /> request to be sent, where this behavior can be determined manually.
		/// <para />
		/// The default value is <c>true</c>.
		/// </remarks>
		public bool LeaveLastJoinedRoom
		{
			get
			{
				return leaveJoinedLastRoom;
			}
			set
			{
				leaveJoinedLastRoom = value;
			}
		}

		/// <summary>
		/// Indicates if a game state change must be notified when the minimum number of players is reached.
		/// </summary>
		///
		/// <remarks>
		/// If this setting is true, the game state (started or stopped) is handled by means of the reserved Room Variable represented by the <see cref="F:Sfs2X.Entities.Variables.ReservedRoomVariables.RV_GAME_STARTED">ReservedRoomVariables.RV_GAME_STARTED</see> constant.
		/// Listening to the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_VARIABLES_UPDATE" /> event for this variable allows clients to be notified when the game can start due to minimum number of players being reached.
		/// <para />
		/// As the used Room Variable is created as <em>global</em> (see the <see cref="T:Sfs2X.Entities.Variables.SFSRoomVariable" /> class description), its update is broadcast outside the Room too:
		/// this can be used on the client-side, for example, to show the game state in a list of available games.
		/// <para />
		/// The default value is <c>false</c>.
		/// </remarks>
		public bool NotifyGameStarted
		{
			get
			{
				return notifyGameStarted;
			}
			set
			{
				notifyGameStarted = value;
			}
		}

		/// <summary>
		/// Defines the game matching expression to be used to filters players.
		/// </summary>
		///
		/// <remarks>
		/// Filtering is applied when:
		/// <ol>
		/// 	<li>users try to join a public Game Room as players (their User Variables must match the matching criteria);</li>
		/// 	<li>the server selects additional users to be invited to join a private game (see the <see cref="P:Sfs2X.Requests.Game.SFSGameSettings.SearchableRooms" /> property).</li>
		/// </ol>
		/// Filtering is not applied to users invited by the creator to join a private game (see the <see cref="P:Sfs2X.Requests.Game.SFSGameSettings.InvitedPlayers" /> property).
		/// <para />
		/// The default value is <c>null</c>.
		/// </remarks>
		/// <seealso cref="P:Sfs2X.Requests.Game.SFSGameSettings.SpectatorMatchExpression" />
		public MatchExpression PlayerMatchExpression
		{
			get
			{
				return playerMatchExpression;
			}
			set
			{
				playerMatchExpression = value;
			}
		}

		/// <summary>
		/// Defines the game matching expression to be used to filters spectators.
		/// </summary>
		///
		/// <remarks>
		/// Filtering is applied when users try to join a public Game Room as spectators (their User Variables must match the matching criteria).
		/// <para />
		/// The default value is <c>null</c>.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Requests.Game.SFSGameSettings.PlayerMatchExpression" />
		public MatchExpression SpectatorMatchExpression
		{
			get
			{
				return spectatorMatchExpression;
			}
			set
			{
				spectatorMatchExpression = value;
			}
		}

		/// <summary>
		/// In private games, defines an optional object containing additional custom parameters to be sent together with the invitation.
		/// </summary>
		///
		/// <remarks>
		/// This object must be an instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" />. Possible custom parameters to be transferred to the invitees are a message
		/// for the recipient, the game details (title, type...), the inviter details, etc.
		/// <para />
		/// The default value is <c>null</c>.
		/// </remarks>
		public ISFSObject InvitationParams
		{
			get
			{
				return invitationParams;
			}
			set
			{
				invitationParams = value;
			}
		}

		/// <summary>
		/// Creates a new SFSGameSettings instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="T:Sfs2X.Requests.Game.CreateSFSGameRequest" /> class constructor.
		/// </remarks>
		///
		/// <param name="name">The name of the Game Room to be created.</param>
		///
		/// <seealso cref="T:Sfs2X.Requests.Game.CreateSFSGameRequest" />
		public SFSGameSettings(string name)
			: base(name)
		{
			isPublic = true;
			minPlayersToStartGame = 2;
			invitationExpiryTime = 15;
			leaveJoinedLastRoom = true;
			invitedPlayers = new List<object>();
			searchableRooms = new List<string>();
		}
	}
}
