using System.Collections.Generic;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Kicks a user out of the server.
	/// </summary>
	///
	/// <remarks>
	/// The current user must have administration or moderation privileges in order to be able to kick another user (see the <see cref="P:Sfs2X.Entities.User.PrivilegeId">User.PrivilegeId</see> property).
	/// The request allows sending a message to the kicked user (to make clear the reason of the following disconnection) which is delivered by means of the <see cref="F:Sfs2X.Core.SFSEvent.MODERATOR_MESSAGE" /> event.
	/// <para />
	/// Differently from the user being banned (see the <see cref="T:Sfs2X.Requests.BanUserRequest" /> request), a kicked user will be able to reconnect to the SmartFoxServer instance immediately.
	/// </remarks>
	///
	/// <example>
	/// The following example kicks the user Jack from the system:
	/// <code>
	/// User userToKick = sfs.UserManager.GetUserByName("Jack"); 
	/// sfs.Send( new KickUserRequest(userToKick.Id) );
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.MODERATOR_MESSAGE" />
	/// <seealso cref="T:Sfs2X.Requests.BanUserRequest" />
	public class KickUserRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_USER_ID = "u";

		/// <exclude />
		public static readonly string KEY_MESSAGE = "m";

		/// <exclude />
		public static readonly string KEY_DELAY = "d";

		private int userId;

		private string message;

		private int delay;

		private void Init(int userId, string message, int delaySeconds)
		{
			this.userId = userId;
			this.message = message;
			delay = delaySeconds;
			if (delay < 0)
			{
				delay = 0;
			}
		}

		/// <summary>
		/// Creates a new KickUserRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="userId">The id of the user to be kicked.</param>
		/// <param name="message">A custom message to be delivered to the user before kicking him; if <c>null</c>, the default message configured in the SmartFoxServer 2X Administration Tool is used (default = <c>null</c>).</param>
		/// <param name="delaySeconds">The number of seconds after which the user is kicked after receiving the kick message (default = <c>5</c>).</param>
		public KickUserRequest(int userId, string message, int delaySeconds)
			: base(RequestType.BanUser)
		{
			Init(userId, message, delaySeconds);
		}

		/// <summary>
		/// See <em>KickUserRequest(int, string, int)</em> constructor.
		/// </summary>
		public KickUserRequest(int userId)
			: base(RequestType.KickUser)
		{
			Init(userId, null, 5);
		}

		/// <summary>
		/// See <em>KickUserRequest(int, string, int)</em> constructor.
		/// </summary>
		public KickUserRequest(int userId, string message)
			: base(RequestType.KickUser)
		{
			Init(userId, message, 5);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (list.Count > 0)
			{
				throw new SFSValidationError("KickUser request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutInt(KEY_USER_ID, userId);
			sfso.PutInt(KEY_DELAY, delay);
			if (message != null && message.Length > 0)
			{
				sfso.PutUtfString(KEY_MESSAGE, message);
			}
		}
	}
}
