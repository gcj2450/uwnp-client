using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Match;
using Sfs2X.Exceptions;
using Sfs2X.Requests.Game;
using Sfs2X.Requests.MMO;

namespace Sfs2X.Requests.Cluster
{
	/// <summary>
	/// In SmartFoxServer 2X Cluster environment, quickly joins the current user in a public game on a Game Node, or creates a new game if none is found.
	/// </summary>
	///
	/// <remarks>
	/// The Lobby Node searches for a public Game Room that meets the criteria expressed by the passed matching expression in the passed Room Groups.
	/// If no suitable Game Room can be found or a null matching expression is passed, and if the <i>settings</i> parameter is set, a new Game Room
	/// is created on a Game Node selected by the cluster's load balancing system.
	/// <para />
	/// In any case, if a game to join can be found or is created, the <see cref="F:Sfs2X.Core.SFSClusterEvent.CONNECTION_REQUIRED" /> event is dispatched to the requester's client,
	/// followed by a <see cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN" /> event after the connection and login process is completed.
	/// In case the client is already connected to the target Game Node, the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN" /> event is received immediately.
	/// </remarks>
	///
	/// <example>
	/// The following example makes the user quickly join a public game:
	/// <code>
	/// void SomeMethod() {
	/// 	sfsLobby.AddEventListener(SFSClusterEvent.CONNECTION_REQUIRED, OnGameNodeConnectionRequired);
	///
	///             		// Create a matching expression to find a Darts game with a "maxBet" variable less than 100
	/// 	MatchExpression exp = new MatchExpression("type", StringMatch.EQUALS, "Darts").And("maxBet", NumberMatch.LESS_THAN, 100);
	///
	///             		// Set the Room settings to create a new game Room if one is not found
	///             		ClusterRoomSettings settings = new ClusterRoomSettings("NewRoom__" + new System.Random().Next());
	///             		settings.GroupId = "games";
	///             		settings.IsPublic = true;
	///             		settings.IsGame = true;
	///             		settings.MaxUsers = 10;
	///             	    settings.MinPlayersToStartGame = 2;
	///
	///             		// Search (or create) and join a public game within the "games" Group
	///             		sfsLobby.Send(new ClusterJoinOrCreateRequest(exp, new List&lt;string&gt;(){"games"}, settings));
	/// }
	///
	/// void OnGameNodeConnectionRequired(BaseEvent evt)
	/// {
	///     // Retrieve connection settings
	///     ConfigData cfg = (ConfigData)evt.Params["configData"];
	///
	///     // Retrieve and save login details
	///     gameUsername = (string)evt.Params["userName"];
	///     gamePassword = (string)evt.Params["password"];
	///
	///     // Initialize SmartFox client used to connect to the cluster game node
	/// 	sfsGame = new SmartFox();
	///
	///     // Add event listeners
	///     sfsGame.AddEventListener(SFSEvent.CONNECTION, OnGameNodeConnection);
	///     sfsGame.AddEventListener(SFSEvent.LOGIN, OnGameNodeLogin);
	///     sfsGame.AddEventListener(SFSEvent.ROOM_JOIN, OnGameRoomJoin);
	///
	///     // Establish a connection to the game node; a game room will be joined automatically after login
	///     sfsGame.Connect(cfg);
	/// }
	///
	/// void OnGameNodeConnection(BaseEvent evt)
	/// {
	///     if ((bool)evt.Params["success"])
	///     {
	///         // Login
	///         sfsGame.Send(new LoginRequest(gameUsername, gamePassword));
	///     }
	/// }
	///
	/// void OnGameNodeLogin(BaseEvent evt)
	/// {
	///     // Nothing to do; a game Room-autojoin is triggered by the server
	/// }
	///
	/// void OnGameRoomJoin(BaseEvent evt) {	
	/// 	Console.WriteLine("Successfully joined Room: " + (Room)evt.Params["room"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Successfully joined Room: " + (Room)evt.Params["room"]);        // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSClusterEvent.CONNECTION_REQUIRED" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN" />
	public class ClusterJoinOrCreateRequest : BaseRequest
	{
		/// <exclude />
		private static readonly string KEY_GROUP_LIST = "gl";

		/// <exclude />
		public static readonly string KEY_ROOM_SETTINGS = "rs";

		/// <exclude />
		public static readonly string KEY_MATCH_EXPRESSION = "me";

		/// <exclude />
		public static readonly string KEY_IS_PUBLIC = "gip";

		/// <exclude />
		public static readonly string KEY_MIN_PLAYERS = "gmp";

		/// <exclude />
		public static readonly string KEY_INVITED_PLAYERS = "ginp";

		/// <exclude />
		public static readonly string KEY_INVITATION_EXPIRY = "gie";

		/// <exclude />
		public static readonly string KEY_NOTIFY_GAME_STARTED = "gns";

		/// <exclude />
		public static readonly string KEY_INVITATION_PARAMS = "ip";

		private MatchExpression matchExpression;

		private List<string> groupNames;

		private RoomSettings settings;

		private CreateRoomRequest createRoomRequest;

		/// <summary>
		/// Creates a new ClusterJoinOrCreateRequest instance.
		/// </summary>
		///
		/// <param name="matchExpression">A matching expression that the system will use to search a Game Room where to join the current user; if <c>null</c>, a new Game Room will be created.</param>
		/// <param name="groupNames">A list of group names to further filter the search; if null, all groups will be searched.</param>
		/// <param name="settings">If no Rooms are found through the matching expression, a new Room with the passed settings will be created and the user will auto-join it.</param>
		///
		/// <seealso cref="T:Sfs2X.Entities.Match.MatchExpression" />
		/// <seealso cref="T:Sfs2X.Requests.Cluster.ClusterRoomSettings" />
		public ClusterJoinOrCreateRequest(MatchExpression matchExpression, List<string> groupNames, RoomSettings settings)
			: base(RequestType.ClusterJoinOrCreateRequest)
		{
			Init(matchExpression, groupNames, settings);
		}

		/// <summary>
		/// See <em>ClusterJoinOrCreateRequest(MatchExpression, List&lt;string&gt;, RoomSettings)</em> constructor.
		/// </summary>
		public ClusterJoinOrCreateRequest(MatchExpression matchExpression, List<string> groupNames)
			: base(RequestType.ClusterJoinOrCreateRequest)
		{
			Init(matchExpression, groupNames, null);
		}

		/// <summary>
		/// See <em>ClusterJoinOrCreateRequest(MatchExpression, List&lt;string&gt;, RoomSettings)</em> constructor.
		/// </summary>
		public ClusterJoinOrCreateRequest(MatchExpression matchExpression)
			: base(RequestType.ClusterJoinOrCreateRequest)
		{
			Init(matchExpression, null, null);
		}

		/// <summary>
		/// See <em>ClusterJoinOrCreateRequest(MatchExpression, List&lt;string&gt;, RoomSettings)</em> constructor.
		/// </summary>
		public ClusterJoinOrCreateRequest(RoomSettings settings)
			: base(RequestType.ClusterJoinOrCreateRequest)
		{
			Init(null, null, settings);
		}

		private void Init(MatchExpression matchExpression, List<string> groupNames, RoomSettings settings)
		{
			this.matchExpression = matchExpression;
			this.groupNames = groupNames;
			this.settings = settings;
			if (settings != null)
			{
				createRoomRequest = new CreateRoomRequest(settings, false, null);
			}
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (settings != null)
			{
				bool flag = settings is ClusterRoomSettings;
				bool flag2 = settings is MMORoomSettings;
				if (!flag && !flag2)
				{
					list.Add("Unsupported RoomSetting type: " + settings.GetType().Name + ". Accepted types are: ClusterRoomSettings and MMORoomSettings");
				}
				else
				{
					try
					{
						createRoomRequest.Validate(sfs);
					}
					catch (SFSValidationError sFSValidationError)
					{
						list.AddRange(sFSValidationError.Errors);
					}
					if (flag)
					{
						ValidateClusterRoom((ClusterRoomSettings)settings, list);
					}
				}
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("ClusterJoinOrCreateRequest request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			if (matchExpression != null)
			{
				sfso.PutSFSArray(KEY_MATCH_EXPRESSION, matchExpression.ToSFSArray());
			}
			if (groupNames != null && groupNames.Count > 0)
			{
				sfso.PutUtfStringArray(KEY_GROUP_LIST, groupNames.ToArray());
			}
			if (settings == null)
			{
				return;
			}
			createRoomRequest.Execute(sfs);
			ISFSObject content = createRoomRequest.Message.Content;
			if (settings is ClusterRoomSettings)
			{
				ClusterRoomSettings clusterRoomSettings = (ClusterRoomSettings)settings;
				content.PutBool(KEY_IS_PUBLIC, clusterRoomSettings.IsPublic);
				content.PutShort(KEY_MIN_PLAYERS, (short)clusterRoomSettings.MinPlayersToStartGame);
				content.PutShort(KEY_INVITATION_EXPIRY, (short)clusterRoomSettings.InvitationExpiryTime);
				content.PutBool(KEY_NOTIFY_GAME_STARTED, clusterRoomSettings.NotifyGameStarted);
				List<object> invitedPlayers = clusterRoomSettings.InvitedPlayers;
				if (invitedPlayers != null)
				{
					List<int> list = new List<int>(invitedPlayers.Count);
					foreach (object item in invitedPlayers)
					{
						if (item is User)
						{
							list.Add(((User)item).Id);
						}
						else if (item is Buddy)
						{
							list.Add(((Buddy)item).Id);
						}
					}
					content.PutIntArray(KEY_INVITED_PLAYERS, list.ToArray());
				}
				if (clusterRoomSettings.InvitationParams != null)
				{
					content.PutSFSObject(KEY_INVITATION_PARAMS, clusterRoomSettings.InvitationParams);
				}
			}
			sfso.PutSFSObject(KEY_ROOM_SETTINGS, content);
		}

		private void ValidateClusterRoom(ClusterRoomSettings settings, List<string> errors)
		{
			if (settings.MinPlayersToStartGame > settings.MaxUsers)
			{
				errors.Add("MinPlayersToStartGame cannot be greater than MaxUsers");
			}
			if (settings.InvitationExpiryTime < InviteUsersRequest.MIN_EXPIRY_TIME || settings.InvitationExpiryTime > InviteUsersRequest.MAX_EXPIRY_TIME)
			{
				errors.Add("Expiry time value is out of range (" + InviteUsersRequest.MIN_EXPIRY_TIME + "-" + InviteUsersRequest.MAX_EXPIRY_TIME + ")");
			}
			if (settings.InvitedPlayers != null && settings.InvitedPlayers.Count > InviteUsersRequest.MAX_INVITATIONS_FROM_CLIENT_SIDE)
			{
				errors.Add("Cannot invite more than " + InviteUsersRequest.MAX_INVITATIONS_FROM_CLIENT_SIDE + " players from client side");
			}
		}
	}
}
