using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Game
{
	/// <summary>
	/// Creates a new public or private game, including player matching criteria, invitations settings and more.
	/// </summary>
	///
	/// <remarks>
	/// A game is created through the istantiation of a <em>SFSGame</em> on the server-side, a specialized Room type that provides advanced features during the creation phase of a game.
	/// Specific game-configuration settings are passed by means of the <see cref="T:Sfs2X.Requests.Game.SFSGameSettings" /> class.
	/// <para />
	/// If the creation is successful, a <see cref="F:Sfs2X.Core.SFSEvent.ROOM_ADD" /> event is dispatched to all the users who subscribed the Group to which the Room is associated,
	/// including the game creator. Otherwise, a <see cref="F:Sfs2X.Core.SFSEvent.ROOM_CREATION_ERROR" /> event is returned to the creator's client.
	/// <para />
	/// Also, if the settings passed in the <see cref="T:Sfs2X.Requests.Game.SFSGameSettings" /> object cause invitations to join the game to be sent, an invitation event is dispatched to all the recipient clients.
	/// <para />
	/// Check the SmartFoxServer 2X documentation for a more in-depth overview of the GAME API.
	/// </remarks>
	///
	/// <example>
	/// The following example creates a new game:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ROOM_ADD, OnRoomCreated);
	/// 	sfs.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomError);
	///
	/// 	// Prepare the settings for a public game
	/// 	SFSGameSettings settings = new SFSGameSettings("DartsGame");
	/// 	settings.MaxUsers = 2;
	/// 	settings.MaxSpectators = 8;
	/// 	settings.IsPublic = true;
	/// 	settings.MinPlayersToStartGame = 2;
	/// 	settings.NotifyGameStarted = true;
	///
	/// 	// Set the matching expression to filter users joining the Room
	/// 	settings.PlayerMatchExpression = new MatchExpression("bestScore", NumberMatch.GREATER_THAN, 100);
	///
	/// 	// Set a Room Variable containing the description of the game
	/// 	List&lt;RoomVariable&gt; roomVars = new List&lt;RoomVariable&gt;();
	/// 	roomVars.Add(new SFSRoomVariable("desc", "Darts game, public, bestScore &gt; 100"));
	/// 	settings.variables = roomVars;
	///
	/// 	// Create the game
	/// 	smartFox.Send( new CreateSFSGameRequest(settings) );
	/// }
	///
	/// void OnRoomCreated(BaseEvent evt) {
	/// 	Console.WriteLine("Room created: " + (Room)evt.Params["room"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Room created: " + (Room)evt.Params["room"]);        // UWP
	/// }
	///
	/// void OnRoomError(BaseEvent evt) {
	/// 	Console.WriteLine("Room creation failed: " + (string)evt.Params["errorMessage"]);                           // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Room creation failed: " + (string)evt.Params["errorMessage"]);          // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_ADD" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_CREATION_ERROR" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.INVITATION" />
	public class CreateSFSGameRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_IS_PUBLIC = "gip";

		/// <exclude />
		public static readonly string KEY_MIN_PLAYERS = "gmp";

		/// <exclude />
		public static readonly string KEY_INVITED_PLAYERS = "ginp";

		/// <exclude />
		public static readonly string KEY_SEARCHABLE_ROOMS = "gsr";

		/// <exclude />
		public static readonly string KEY_PLAYER_MATCH_EXP = "gpme";

		/// <exclude />
		public static readonly string KEY_SPECTATOR_MATCH_EXP = "gsme";

		/// <exclude />
		public static readonly string KEY_INVITATION_EXPIRY = "gie";

		/// <exclude />
		public static readonly string KEY_LEAVE_ROOM = "glr";

		/// <exclude />
		public static readonly string KEY_NOTIFY_GAME_STARTED = "gns";

		/// <exclude />
		public static readonly string KEY_INVITATION_PARAMS = "ip";

		private CreateRoomRequest createRoomRequest;

		private SFSGameSettings settings;

		/// <summary>
		/// Creates a new CreateSFSGameRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="settings">An object containing the SFSGame configuration settings.</param>
		///
		/// <seealso cref="T:Sfs2X.Requests.Game.SFSGameSettings" />
		public CreateSFSGameRequest(SFSGameSettings settings)
			: base(RequestType.CreateSFSGame)
		{
			this.settings = settings;
			createRoomRequest = new CreateRoomRequest(settings, false, null);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			try
			{
				createRoomRequest.Validate(sfs);
			}
			catch (SFSValidationError sFSValidationError)
			{
				list = sFSValidationError.Errors;
			}
			if (settings.MinPlayersToStartGame > settings.MaxUsers)
			{
				list.Add("minPlayersToStartGame cannot be greater than maxUsers");
			}
			if (settings.InvitationExpiryTime < InviteUsersRequest.MIN_EXPIRY_TIME || settings.InvitationExpiryTime > InviteUsersRequest.MAX_EXPIRY_TIME)
			{
				list.Add("Expiry time value is out of range (" + InviteUsersRequest.MIN_EXPIRY_TIME + "-" + InviteUsersRequest.MAX_EXPIRY_TIME + ")");
			}
			if (settings.InvitedPlayers != null && settings.InvitedPlayers.Count > InviteUsersRequest.MAX_INVITATIONS_FROM_CLIENT_SIDE)
			{
				list.Add("Cannot invite more than " + InviteUsersRequest.MAX_INVITATIONS_FROM_CLIENT_SIDE + " players from client side");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("CreateSFSGame request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			createRoomRequest.Execute(sfs);
			sfso = createRoomRequest.Message.Content;
			sfso.PutBool(KEY_IS_PUBLIC, settings.IsPublic);
			sfso.PutShort(KEY_MIN_PLAYERS, (short)settings.MinPlayersToStartGame);
			sfso.PutShort(KEY_INVITATION_EXPIRY, (short)settings.InvitationExpiryTime);
			sfso.PutBool(KEY_LEAVE_ROOM, settings.LeaveLastJoinedRoom);
			sfso.PutBool(KEY_NOTIFY_GAME_STARTED, settings.NotifyGameStarted);
			if (settings.PlayerMatchExpression != null)
			{
				sfso.PutSFSArray(KEY_PLAYER_MATCH_EXP, settings.PlayerMatchExpression.ToSFSArray());
			}
			if (settings.SpectatorMatchExpression != null)
			{
				sfso.PutSFSArray(KEY_SPECTATOR_MATCH_EXP, settings.SpectatorMatchExpression.ToSFSArray());
			}
			if (settings.InvitedPlayers != null)
			{
				List<int> list = new List<int>();
				foreach (object invitedPlayer in settings.InvitedPlayers)
				{
					if (invitedPlayer is User)
					{
						list.Add((invitedPlayer as User).Id);
					}
					else if (invitedPlayer is Buddy)
					{
						list.Add((invitedPlayer as Buddy).Id);
					}
				}
				sfso.PutIntArray(KEY_INVITED_PLAYERS, list.ToArray());
			}
			if (settings.SearchableRooms != null)
			{
				sfso.PutUtfStringArray(KEY_SEARCHABLE_ROOMS, settings.SearchableRooms.ToArray());
			}
			if (settings.InvitationParams != null)
			{
				sfso.PutSFSObject(KEY_INVITATION_PARAMS, settings.InvitationParams);
			}
		}
	}
}
