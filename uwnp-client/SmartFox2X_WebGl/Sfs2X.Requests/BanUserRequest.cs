using System.Collections.Generic;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Banishes a user from the server.
	/// </summary>
	///
	/// <remarks>
	/// The current user must have administration or moderation privileges in order to be able to ban another user (see the <see cref="P:Sfs2X.Entities.User.PrivilegeId">User.PrivilegeId</see> property).
	/// The user can be banned by name or by IP address (see the <see cref="T:Sfs2X.Requests.BanMode" /> class). Also, the request allows sending a message to the banned user
	/// (to make clear the reason of the following disconnection) which is delivered by means of the <see cref="F:Sfs2X.Core.SFSEvent.MODERATOR_MESSAGE" /> event.
	/// <para />
	/// Differently from the user being kicked (see the <see cref="T:Sfs2X.Requests.KickUserRequest" /> request), a banned user won't be able to connect to the SmartFoxServer instance
	/// until the banishment expires (after 24 hours for client-side banning) or an administrator removes his name/IP address from the list of banned users by means
	/// of the SmartFoxServer 2X Administration Tool.
	/// </remarks>
	///
	/// <example>
	/// The following example bans the user Jack from the system:
	/// <code>
	/// User userToBan = sfs.UserManager.GetUserByName("Jack"); 
	/// sfs.Send( new BanUserRequest(userToBan.Id) );
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.MODERATOR_MESSAGE" />
	/// <seealso cref="T:Sfs2X.Requests.KickUserRequest" />
	/// <seealso cref="T:Sfs2X.Requests.BanMode" />
	public class BanUserRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_USER_ID = "u";

		/// <exclude />
		public static readonly string KEY_MESSAGE = "m";

		/// <exclude />
		public static readonly string KEY_DELAY = "d";

		/// <exclude />
		public static readonly string KEY_BAN_MODE = "b";

		/// <exclude />
		public static readonly string KEY_BAN_DURATION_HOURS = "dh";

		private int userId;

		private string message;

		private int delay;

		private BanMode banMode;

		private int durationHours;

		private void Init(int userId, string message, BanMode banMode, int delaySeconds, int durationHours)
		{
			this.userId = userId;
			this.message = message;
			this.banMode = banMode;
			delay = delaySeconds;
			this.durationHours = durationHours;
		}

		/// <summary>
		/// Creates a new BanUserRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="userId">The id of the user to be banned.</param>
		/// <param name="message">A custom message to be delivered to the user before banning him; if <c>null</c>, the default message configured in the SmartFoxServer 2X Administration Tool is used (default = <c>null</c>).</param>
		/// <param name="banMode">One of the ban modes defined in the <see cref="T:Sfs2X.Requests.BanMode" /> class (default = <c>BanMode.BY_NAME</c>).</param>
		/// <param name="delaySeconds">The number of seconds after which the user is banned after receiving the ban message (default = <c>5</c>).</param>
		/// <param name="durationHours">The duration of the banishment, expressed in hours (default = <c>24</c>).</param>
		///
		/// <seealso cref="T:Sfs2X.Requests.BanMode" />
		public BanUserRequest(int userId, string message, BanMode banMode, int delaySeconds, int durationHours)
			: base(RequestType.BanUser)
		{
			Init(userId, message, banMode, delaySeconds, durationHours);
		}

		/// <summary>
		/// See <em>BanUserRequest(int, string, BanMode, int, int)</em> constructor.
		/// </summary>
		public BanUserRequest(int userId)
			: base(RequestType.BanUser)
		{
			Init(userId, null, BanMode.BY_NAME, 5, 0);
		}

		/// <summary>
		/// See <em>BanUserRequest(int, string, BanMode, int, int)</em> constructor.
		/// </summary>
		public BanUserRequest(int userId, string message)
			: base(RequestType.BanUser)
		{
			Init(userId, message, BanMode.BY_NAME, 5, 0);
		}

		/// <summary>
		/// See <em>BanUserRequest(int, string, BanMode, int, int)</em> constructor.
		/// </summary>
		public BanUserRequest(int userId, string message, BanMode banMode)
			: base(RequestType.BanUser)
		{
			Init(userId, message, banMode, 5, 0);
		}

		/// <summary>
		/// See <em>BanUserRequest(int, string, BanMode, int, int)</em> constructor.
		/// </summary>
		public BanUserRequest(int userId, string message, BanMode banMode, int delaySeconds)
			: base(RequestType.BanUser)
		{
			Init(userId, message, banMode, delaySeconds, 0);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (list.Count > 0)
			{
				throw new SFSValidationError("BanUser request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutInt(KEY_USER_ID, userId);
			sfso.PutInt(KEY_DELAY, delay);
			sfso.PutInt(KEY_BAN_MODE, (int)banMode);
			sfso.PutInt(KEY_BAN_DURATION_HOURS, durationHours);
			if (message != null && message.Length > 0)
			{
				sfso.PutUtfString(KEY_MESSAGE, message);
			}
		}
	}
}
