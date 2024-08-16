using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Cluster
{
	/// <summary>
	/// In the SmartFoxServer 2X Cluster environment, sends a generic invitation to a list of users to join a Room on a Game Node.
	/// </summary>
	///
	/// <remarks>
	/// Invited users receive the invitation as an <see cref="F:Sfs2X.Core.SFSEvent.INVITATION" /> event dispatched to their clients:
	/// they can accept or refuse it by means of the InvitationReplyRequest request, which must be sent within the specified amount of time.
	/// <para />
	/// If the invitation is accepted, and the invited user is not yet connected to the target Game Node, the <see cref="F:Sfs2X.Core.SFSClusterEvent.CONNECTION_REQUIRED" /> event
	/// is dispatched to the their client, followed by a <see cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN" /> event after the connection and login process is completed.
	/// In case the client is already connected to the target Game Node, the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN" /> event is received immediately.
	/// </remarks>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.INVITATION" />
	/// <seealso cref="T:Sfs2X.Requests.Game.InvitationReplyRequest" />
	public class ClusterInviteUsersRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_USER = "u";

		/// <exclude />
		public static readonly string KEY_USER_ID = "ui";

		/// <exclude />
		public static readonly string KEY_INVITATION_ID = "ii";

		/// <exclude />
		public static readonly string KEY_TIME = "t";

		/// <exclude />
		public static readonly string KEY_PARAMS = "p";

		/// <exclude />
		public static readonly string KEY_INVITEE_ID = "ee";

		/// <exclude />
		public static readonly string KEY_INVITED_USERS = "iu";

		/// <exclude />
		public static readonly string KEY_SERVER_ID = "ss";

		/// <exclude />
		public static readonly string KEY_ROOM_ID = "rr";

		/// <exclude />
		public static readonly string KEY_REPLY_ID = "ri";

		/// <exclude />
		public static readonly int MAX_INVITATIONS_FROM_CLIENT_SIDE = 8;

		/// <exclude />
		public static readonly int MIN_EXPIRY_TIME = 5;

		/// <exclude />
		public static readonly int MAX_EXPIRY_TIME = 300;

		private List<object> invitedUsers;

		private int secondsForReply;

		private ISFSObject parameters;

		private ClusterTarget target;

		/// <summary>
		/// Creates a new ClusterInviteUsersRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="target">An instance of <see cref="T:Sfs2X.Requests.Cluster.ClusterTarget" /> containing the identifiers of the Game Node and Room the users are invited to join.</param>
		/// <param name="invitedUsers">A list of objects representing the users to send the invitation to.</param>
		/// <param name="secondsForReply">The number of seconds available to each invited user to reply to the invitation (recommended range: 15 to 40 seconds).</param>
		/// <param name="parameters">An instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" /> containing custom parameters which specify the invitation details.</param>
		public ClusterInviteUsersRequest(ClusterTarget target, List<object> invitedUsers, int secondsForReply, ISFSObject parameters)
			: base(RequestType.ClusterInviteUsers)
		{
			this.target = target;
			this.invitedUsers = invitedUsers;
			this.secondsForReply = secondsForReply;
			this.parameters = parameters;
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (invitedUsers == null || invitedUsers.Count < 1)
			{
				list.Add("No invitation(s) to send");
			}
			if (invitedUsers.Count > MAX_INVITATIONS_FROM_CLIENT_SIDE)
			{
				list.Add("Too many invitations. Max allowed from client side is: " + MAX_INVITATIONS_FROM_CLIENT_SIDE);
			}
			if (secondsForReply < MIN_EXPIRY_TIME || secondsForReply > MAX_EXPIRY_TIME)
			{
				list.Add("SecondsForReply value is out of range (" + MIN_EXPIRY_TIME + "-" + MAX_EXPIRY_TIME + ")");
			}
			if (target == null)
			{
				list.Add("Missing cluster target (server id and room id)");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("ClusterInviteUsers request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			List<int> list = new List<int>();
			foreach (object invitedUser in invitedUsers)
			{
				if (invitedUser is User)
				{
					if (invitedUser as User != sfs.MySelf)
					{
						list.Add((invitedUser as User).Id);
					}
				}
				else if (invitedUser is Buddy)
				{
					list.Add((invitedUser as Buddy).Id);
				}
			}
			sfso.PutUtfString(KEY_SERVER_ID, target.ServerId);
			sfso.PutInt(KEY_ROOM_ID, target.RoomId);
			sfso.PutIntArray(KEY_INVITED_USERS, list.ToArray());
			sfso.PutShort(KEY_TIME, (short)secondsForReply);
			if (parameters != null)
			{
				sfso.PutSFSObject(KEY_PARAMS, parameters);
			}
		}
	}
}
