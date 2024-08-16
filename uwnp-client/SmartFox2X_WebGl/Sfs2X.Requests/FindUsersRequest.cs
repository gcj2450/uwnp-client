using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Match;
using Sfs2X.Exceptions;
using Sfs2X.Logging;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Retrieves a list of users from the server which match the specified criteria.
	/// </summary>
	///
	/// <remarks>
	/// By providing a matching expression and a search scope (a Room, a Group or the entire Zone), SmartFoxServer can find
	/// those users matching the passed criteria and return them by means of the <see cref="F:Sfs2X.Core.SFSEvent.USER_FIND_RESULT" /> event.
	/// </remarks>
	///
	/// <example>
	/// The following example looks for all the users whose "age" User Variable is greater than 29:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.USER_FIND_RESULT, OnUserFindResult);
	///
	/// 	// Create a matching expression to find users with an "age" variable greater than 29:
	/// 	MatchExpression expr = new MatchExpression("age", NumberMatch.GREATER_THAN, 29);
	///
	/// 	// Find the users
	/// 	sfs.Send( new FindUsersRequest(expr) );
	/// }
	///
	/// void OnUserFindResult(BaseEvent evt) {
	/// 	Console.WriteLine("Users found: " + (List&lt;User&gt;)evt.Params["users"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Users found: " + (List&lt;User&gt;)evt.Params["users"]);        // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.USER_FIND_RESULT" />
	/// <seealso cref="T:Sfs2X.Entities.Match.MatchExpression" />
	public class FindUsersRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_EXPRESSION = "e";

		/// <exclude />
		public static readonly string KEY_GROUP = "g";

		/// <exclude />
		public static readonly string KEY_ROOM = "r";

		/// <exclude />
		public static readonly string KEY_LIMIT = "l";

		/// <exclude />
		public static readonly string KEY_FILTERED_USERS = "fu";

		private MatchExpression matchExpr;

		private object target;

		private int limit;

		private void Init(MatchExpression expr, object target, int limit)
		{
			matchExpr = expr;
			this.target = target;
			this.limit = limit;
		}

		/// <summary>
		/// Creates a new FindUsersRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="expr">A matching expression that the system will use to retrieve the users.</param>
		/// <param name="target">The name of a Group or a single Room object where to search for matching users; if <c>null</c>, the search is performed in the whole Zone (default = <c>null</c>).</param>
		/// <param name="limit">The maximum size of the list of users that will be returned by the userFindResult event. If <c>0</c>, all the found users are returned (default = <c>0</c>).</param>
		public FindUsersRequest(MatchExpression expr, string target, int limit)
			: base(RequestType.FindUsers)
		{
			Init(expr, target, limit);
		}

		/// <summary>
		/// See <em>FindUsersRequest(MatchExpression, string, int)</em> constructor.
		/// </summary>
		public FindUsersRequest(MatchExpression expr)
			: base(RequestType.FindUsers)
		{
			Init(expr, null, 0);
		}

		/// <summary>
		/// See <em>FindUsersRequest(MatchExpression, string, int)</em> constructor.
		/// </summary>
		public FindUsersRequest(MatchExpression expr, Room target)
			: base(RequestType.FindUsers)
		{
			Init(expr, target, 0);
		}

		/// <summary>
		/// See <em>FindUsersRequest(MatchExpression, string, int)</em> constructor.
		/// </summary>
		public FindUsersRequest(MatchExpression expr, Room target, int limit)
			: base(RequestType.FindUsers)
		{
			Init(expr, target, limit);
		}

		/// <summary>
		/// See <em>FindUsersRequest(MatchExpression, string, int)</em> constructor.
		/// </summary>
		public FindUsersRequest(MatchExpression expr, string target)
			: base(RequestType.FindUsers)
		{
			Init(expr, target, 0);
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
				throw new SFSValidationError("FindUsers request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutSFSArray(KEY_EXPRESSION, matchExpr.ToSFSArray());
			if (target != null)
			{
				if (target is Room)
				{
					sfso.PutInt(KEY_ROOM, (target as Room).Id);
				}
				else if (target is string)
				{
					sfso.PutUtfString(KEY_GROUP, target as string);
				}
				else
				{
					Logger log = sfs.Log;
					string[] array = new string[1];
					object obj = target;
					array[0] = "Unsupport target type for FindUsersRequest: " + ((obj != null) ? obj.ToString() : null);
					log.Warn(array);
				}
			}
			if (limit > 0)
			{
				sfso.PutShort(KEY_LIMIT, (short)limit);
			}
		}
	}
}
