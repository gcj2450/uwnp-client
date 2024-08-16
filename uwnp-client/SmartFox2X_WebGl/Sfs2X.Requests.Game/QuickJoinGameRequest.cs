using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Match;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Game
{
	/// <summary>
	/// Quickly joins the current user in a public game.
	/// </summary>
	///
	/// <remarks>
	/// By providing a matching expression and a list of Rooms or Groups, SmartFoxServer can search for a matching public Game Room and immediately join the user into that Room as a player.
	/// <para />
	/// If a game could be found and joined, the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN" /> event is dispatched to the requester's client.
	/// </remarks>
	///
	/// <example>
	/// The following example makes the user quickly join a public game:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ROOM_JOIN, onRoomJoin);
	///
	/// 	// Create a matching expression to find a Darts game with a "maxBet" variable less than 100
	/// 	MatchExpression exp = new MatchExpression("type", StringMatch.EQUALS, "Darts").And("maxBet", NumberMatch.LESS_THAN, 100);
	///
	/// 	// Search and join a public game within the "games" Group, leaving the last joined Room
	/// 	sfs.Send( new QuickJoinGameRequest(exp, new List&lt;string&gt;(){"games"}, sfs.LastJoinedRoom) );
	/// }
	///
	/// void OnRoomJoin(BaseEvent evt) {	
	/// 	Console.WriteLine("Successfully joined Room: " + (Room)evt.Params["room"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Successfully joined Room: " + (Room)evt.Params["room"]);        // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN" />
	public class QuickJoinGameRequest : BaseRequest
	{
		private static readonly int MAX_ROOMS = 32;

		/// <exclude />
		public static readonly string KEY_ROOM_LIST = "rl";

		/// <exclude />
		public static readonly string KEY_GROUP_LIST = "gl";

		/// <exclude />
		public static readonly string KEY_ROOM_TO_LEAVE = "tl";

		/// <exclude />
		public static readonly string KEY_MATCH_EXPRESSION = "me";

		private List<Room> whereToSearchRoom;

		private List<string> whereToSearchString;

		private MatchExpression matchExpression;

		private Room roomToLeave;

		private bool isSearchListString = false;

		private bool isSearchListRoom = false;

		/// <summary>
		/// Creates a new QuickJoinGameRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="matchExpression">A matching expression that the system will use to search a Game Room where to join the current user.</param>
		/// <param name="whereToSearch">A list of Group names to which the matching expression should be applied. The maximum number of elements in this list is 32.</param>
		/// <param name="roomToLeave">An object representing the Room that the user should leave when joining the game. Default is <c>null</c>.</param>
		///
		/// <seealso cref="T:Sfs2X.Entities.Match.MatchExpression" />
		public QuickJoinGameRequest(MatchExpression matchExpression, List<string> whereToSearch, Room roomToLeave)
			: base(RequestType.QuickJoinGame)
		{
			Init(matchExpression, whereToSearch, roomToLeave);
		}

		/// <summary>
		/// See <em>QuickJoinGameRequest(MatchExpression, List&lt;string&gt;, Room)</em> constructor.
		/// </summary>
		public QuickJoinGameRequest(MatchExpression matchExpression, List<string> whereToSearch)
			: base(RequestType.QuickJoinGame)
		{
			Init(matchExpression, whereToSearch, null);
		}

		private void Init(MatchExpression matchExpression, List<string> whereToSearch, Room roomToLeave)
		{
			this.matchExpression = matchExpression;
			whereToSearchString = whereToSearch;
			this.roomToLeave = roomToLeave;
			isSearchListString = true;
		}

		/// <summary>
		/// Creates a new QuickJoinGameRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="matchExpression">A matching expression that the system will use to search a Game Room where to join the current user.</param>
		/// <param name="whereToSearch">A list of Room objects to which the matching expression should be applied. The maximum number of elements in this list is 32.</param>
		/// <param name="roomToLeave">An object representing the Room that the user should leave when joining the game. Default is <c>null</c>.</param>
		public QuickJoinGameRequest(MatchExpression matchExpression, List<Room> whereToSearch, Room roomToLeave)
			: base(RequestType.QuickJoinGame)
		{
			Init(matchExpression, whereToSearch, roomToLeave);
		}

		/// <summary>
		/// See <em>QuickJoinGameRequest(MatchExpression, List&lt;Room&gt;, Room)</em> constructor.
		/// </summary>
		public QuickJoinGameRequest(MatchExpression matchExpression, List<Room> whereToSearch)
			: base(RequestType.QuickJoinGame)
		{
			Init(matchExpression, whereToSearch, null);
		}

		private void Init(MatchExpression matchExpression, List<Room> whereToSearch, Room roomToLeave)
		{
			this.matchExpression = matchExpression;
			whereToSearchRoom = whereToSearch;
			this.roomToLeave = roomToLeave;
			isSearchListRoom = true;
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (isSearchListRoom)
			{
				if (whereToSearchRoom == null || whereToSearchRoom.Count < 1)
				{
					list.Add("Missing whereToSearch parameter");
				}
				else if (whereToSearchRoom.Count > MAX_ROOMS)
				{
					list.Add("Too many Rooms specified in the whereToSearch parameter. Client limit is: " + MAX_ROOMS);
				}
			}
			if (isSearchListString)
			{
				if (whereToSearchString == null || whereToSearchString.Count < 1)
				{
					list.Add("Missing whereToSearch parameter");
				}
				else if (whereToSearchString.Count > MAX_ROOMS)
				{
					list.Add("Too many Rooms specified in the whereToSearch parameter. Client limit is: " + MAX_ROOMS);
				}
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("QuickJoinGame request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			if (isSearchListString)
			{
				sfso.PutUtfStringArray(KEY_GROUP_LIST, whereToSearchString.ToArray());
			}
			else if (isSearchListRoom)
			{
				List<int> list = new List<int>();
				foreach (Room item in whereToSearchRoom)
				{
					list.Add(item.Id);
				}
				sfso.PutIntArray(KEY_ROOM_LIST, list.ToArray());
			}
			if (roomToLeave != null)
			{
				sfso.PutInt(KEY_ROOM_TO_LEAVE, roomToLeave.Id);
			}
			if (matchExpression != null)
			{
				sfso.PutSFSArray(KEY_MATCH_EXPRESSION, matchExpression.ToSFSArray());
			}
		}
	}
}
