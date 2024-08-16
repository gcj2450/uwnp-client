using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Sfs2X.Requests.Cluster
{
	/// <summary>
	/// The ClusterRoomSettings class is a container for the settings required to create a Game Room on a Game Node in a SmartFoxServer 2X Cluster.
	/// </summary>
	///
	/// <remarks>
	/// On the server-side, a Game Room is represented on the Game Node by the <em>SFSGame</em> Java class which extends the <see cref="T:Sfs2X.Entities.Room" /> class providing advanced features such as player matching,
	/// game invitations, public and private games, quick game joining, etc. On the client side Game Rooms are regular Rooms with their <see cref="P:Sfs2X.Entities.Room.IsGame">Room.IsGame</see> property set to true.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Requests.Cluster.ClusterJoinOrCreateRequest" />
	public class ClusterRoomSettings : RoomSettings
	{
		private bool isPublic;

		private int minPlayersToStartGame;

		private List<object> invitedPlayers;

		private int invitationExpiryTime;

		private bool notifyGameStarted;

		private ISFSObject invitationParams;

		/// <summary>
		/// Indicates whether the game is public or private.
		/// </summary>
		///
		/// <remarks>
		/// A public game can be joined by any player.
		/// A private game can only be joined by users invited by the game creator by means of <see cref="P:Sfs2X.Requests.Cluster.ClusterRoomSettings.InvitedPlayers" /> list.
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
		/// If the <see cref="P:Sfs2X.Requests.Cluster.ClusterRoomSettings.NotifyGameStarted" /> property is set to <c>true</c>, when this number is reached, the game start is notified.
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
		/// In private games, defines a list of Buddy or User objects representing players to be invited to join the game.
		/// </summary>
		///
		/// <remarks>
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
		/// In private games, defines the number of seconds that the users invited to join the game have to reply to the invitation.
		/// </summary>
		///
		/// <remarks>
		/// The suggested range is 30 to 60 seconds. Default value is <c>30</c>.
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
		/// Indicates if a game state change must be notified when the minimum number of players is reached.
		/// </summary>
		///
		/// <remarks>
		/// If this setting is true, the game state (started or stopped) is handled by means of the reserved Room Variable represented by the <see cref="F:Sfs2X.Entities.Variables.ReservedRoomVariables.RV_GAME_STARTED">ReservedRoomVariables.RV_GAME_STARTED</see> constant.
		/// Listening to the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_VARIABLES_UPDATE" /> event for this variable allows clients to be notified when the game can start due to minimum number of players being reached.
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
		/// Creates a new ClusterRoomSettings instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="T:Sfs2X.Requests.Cluster.ClusterJoinOrCreateRequest" /> class constructor.
		/// </remarks>
		///
		/// <param name="name">The name of the Game Room to be created.</param>
		///
		/// <seealso cref="T:Sfs2X.Requests.Cluster.ClusterJoinOrCreateRequest" />
		public ClusterRoomSettings(string name)
			: base(name)
		{
			base.IsGame = true;
			isPublic = true;
			minPlayersToStartGame = 2;
			invitationExpiryTime = 30;
			invitedPlayers = new List<object>();
		}
	}
}
