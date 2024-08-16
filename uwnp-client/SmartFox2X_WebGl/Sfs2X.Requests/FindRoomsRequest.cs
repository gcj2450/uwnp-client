using System.Collections.Generic;
using Sfs2X.Entities.Match;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Retrieves a list of Rooms from the server which match the specified criteria.
	/// </summary>
	///
	/// <remarks>
	/// By providing a matching expression and a search scope (a Group or the entire Zone), SmartFoxServer can find those Rooms matching the passed criteria
	/// and return them by means of the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_FIND_RESULT" /> event.
	/// </remarks>
	///
	/// <example>
	/// The following example looks for all the server Rooms whose "country" Room Variable is set to Sweden:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ROOM_FIND_RESULT, OnRoomFindResult);
	///
	/// 	// Create a matching expression to find Rooms with a "country" variable equal to "Sweden"
	/// 	MatchExpression expr = new MatchExpression('country', StringMatch.EQUALS, 'Sweden');
	///
	/// 	// Find the Rooms
	/// 	sfs.Send( new FindRoomRequest(expr) );
	/// }
	///
	/// void OnRoomFindResult(BaseEvent evt) {
	/// 	Console.WriteLine("Rooms found: " + (List&lt;Room&gt;)evt.Params["rooms"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Rooms found: " + (List&lt;Room&gt;)evt.Params["rooms"]);        // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_FIND_RESULT" />
	/// <seealso cref="T:Sfs2X.Entities.Match.MatchExpression" />
	public class FindRoomsRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_EXPRESSION = "e";

		/// <exclude />
		public static readonly string KEY_GROUP = "g";

		/// <exclude />
		public static readonly string KEY_LIMIT = "l";

		/// <exclude />
		public static readonly string KEY_FILTERED_ROOMS = "fr";

		private MatchExpression matchExpr;

		private string groupId;

		private int limit;

		private void Init(MatchExpression expr, string groupId, int limit)
		{
			matchExpr = expr;
			this.groupId = groupId;
			this.limit = limit;
		}

		/// <summary>
		/// Creates a new FindRoomsRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="expr">A matching expression that the system will use to retrieve the Rooms.</param>
		/// <param name="groupId">The name of the Group where to search for matching Rooms; if <c>null</c>, the search is performed in the whole Zone (default = <c>null</c>).</param>
		/// <param name="limit">The maximum size of the list of Rooms that will be returned by the roomFindResult event. If <c>0</c>, all the found Rooms are returned (default = <c>0</c>).</param>
		public FindRoomsRequest(MatchExpression expr, string groupId, int limit)
			: base(RequestType.FindRooms)
		{
			Init(expr, groupId, limit);
		}

		/// <summary>
		/// See <em>FindRoomsRequest(MatchExpression, string, int)</em> constructor.
		/// </summary>
		public FindRoomsRequest(MatchExpression expr)
			: base(RequestType.FindRooms)
		{
			Init(expr, null, 0);
		}

		/// <summary>
		/// See <em>FindRoomsRequest(MatchExpression, string, int)</em> constructor.
		/// </summary>
		public FindRoomsRequest(MatchExpression expr, string groupId)
			: base(RequestType.FindRooms)
		{
			Init(expr, groupId, 0);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (matchExpr == null)
			{
				list.Add("Missing Match Expression");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("FindRooms request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutSFSArray(KEY_EXPRESSION, matchExpr.ToSFSArray());
			if (groupId != null)
			{
				sfso.PutUtfString(KEY_GROUP, groupId);
			}
			if (limit > 0)
			{
				sfso.PutShort(KEY_LIMIT, (short)limit);
			}
		}
	}
}
