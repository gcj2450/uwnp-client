using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Match;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Quickly joins the current user in a public Room, or creates a new Room if none is found.
	/// </summary>
	///
	/// <remarks>
	/// SmartFoxServer searches for a public Room that meets the criteria expressed by the passed matching expression in the passed Room Groups.
	/// If no suitable Room can be found, a new Room is created, based on the passed settings.
	/// <para />
	/// If a Room is created, the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_ADD" /> event is dispatched to all the users who subscribed the Group to which the Room is associated,
	/// including the Room creator. In any case, if a Room to join could be found or was created, the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN" /> event is then dispatched.
	/// <para />
	/// Error conditions (Room creation error, Room join error) should always be checked adding the appropriate listeners.
	/// </remarks>
	///
	/// <example>
	/// The following example makes the user quickly join a Room:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdd);
	/// 	sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
	///
	///             		// Create a matching expression to find a Darts game with a "maxBet" variable less than 100
	/// 	MatchExpression exp = new MatchExpression("type", StringMatch.EQUALS, "Darts").And("maxBet", NumberMatch.LESS_THAN, 100);
	///
	///             		// Set the Room settings to create a new Room if a matching one is not found
	///             		RoomSettings settings = new RoomSettings("NewRoom__" + new System.Random().Next());
	///             		settings.GroupId = "games";
	///             		settings.IsPublic = true;
	///             		settings.IsGame = true;
	///             		settings.MaxUsers = 10;
	///             	    settings.MinPlayersToStartGame = 2;
	///
	///             		// Set requirements to allow users find the Room (see match expression above) in Room Variables
	///             		List&lt;RoomVariable&gt; roomVars = new List&lt;RoomVariable&gt;();
	///             		roomVars.Add(new SFSRoomVariable("type", "Darts"));
	///             		roomVars.Add(new SFSRoomVariable("maxBet", 50));
	///             		settings.Variables = roomVars;
	///
	///             		// Search (or create) and join a public Room within the "games" Group
	///             		sfs.Send(new QuickJoinOrCreateRoomRequest(exp, new List&lt;string&gt;(){"games"}, settings));
	/// }
	///
	/// void OnRoomAdd(BaseEvent evt) {	
	/// 	Console.WriteLine("Room created: " + (Room)evt.Params["room"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Room created: " + (Room)evt.Params["room"]);        // UWP
	/// }
	///
	/// void OnRoomJoin(BaseEvent evt) {	
	/// 	Console.WriteLine("Successfully joined Room: " + (Room)evt.Params["room"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Successfully joined Room: " + (Room)evt.Params["room"]);        // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_ADD" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_CREATION_ERROR" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN_ERROR" />
	public class QuickJoinOrCreateRoomRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_MATCH_EXPRESSION = "me";

		/// <exclude />
		public static readonly string KEY_GROUP_LIST = "gl";

		/// <exclude />
		public static readonly string KEY_ROOM_SETTINGS = "rs";

		/// <exclude />
		public static readonly string KEY_ROOM_TO_LEAVE = "tl";

		private MatchExpression matchExpression;

		private List<string> groupList;

		private RoomSettings settings;

		private Room roomToLeave;

		private CreateRoomRequest createRoomRequest;

		/// <summary>
		/// Creates a new QuickJoinOrCreateRoomRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="matchExpression">A matching expression that the system will use to search a Room where to join the current user.</param>
		/// <param name="groupList">A list of group names to further filter the search; if null, all groups will be searched.</param>
		/// <param name="settings">If no Rooms are found through the matching expression, a new Room with the passed settings will be created and the user will auto-join it.</param>
		/// <param name="roomToLeave">An object representing the Room that the user should leave when joining the game. Default is <c>null</c>.</param>
		public QuickJoinOrCreateRoomRequest(MatchExpression matchExpression, List<string> groupList, RoomSettings settings, Room roomToLeave)
			: base(RequestType.QuickJoinOrCreateRoom)
		{
			Init(matchExpression, groupList, settings, roomToLeave);
		}

		/// <summary>
		/// See <em>QuickJoinOrCreateRoomRequest(MatchExpression, List&lt;string&gt;, RoomSettings, Room)</em> constructor.
		/// </summary>
		public QuickJoinOrCreateRoomRequest(MatchExpression matchExpression, List<string> groupList, RoomSettings settings)
			: base(RequestType.QuickJoinOrCreateRoom)
		{
			Init(matchExpression, groupList, settings, null);
		}

		private void Init(MatchExpression matchExpression, List<string> groupList, RoomSettings settings, Room roomToLeave)
		{
			this.matchExpression = matchExpression;
			this.groupList = groupList;
			this.settings = settings;
			this.roomToLeave = roomToLeave;
			createRoomRequest = new CreateRoomRequest(settings, false, null);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (matchExpression == null)
			{
				list.Add("Missing match expression");
			}
			if (groupList == null || groupList.Count == 0)
			{
				list.Add("List of groups to search is null or empty");
			}
			if (settings == null)
			{
				list.Add("No Room settings provided");
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
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("QuickJoinOrCreateRoom request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			createRoomRequest.Execute(sfs);
			ISFSObject content = createRoomRequest.Message.Content;
			sfso.PutSFSArray(KEY_MATCH_EXPRESSION, matchExpression.ToSFSArray());
			sfso.PutUtfStringArray(KEY_GROUP_LIST, groupList.ToArray());
			sfso.PutSFSObject(KEY_ROOM_SETTINGS, content);
			if (roomToLeave != null)
			{
				sfso.PutInt(KEY_ROOM_TO_LEAVE, roomToLeave.Id);
			}
		}
	}
}
