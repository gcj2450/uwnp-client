using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Game
{
	/// <summary>
	/// Sends a generic invitation to a list of users.
	/// </summary>
	///
	/// <remarks>
	/// Invitations can be used for different purposes, such as requesting users to join a game or visit a specific Room, asking the permission to add them as buddies, etc.
	/// Invited users receive the invitation as an <see cref="F:Sfs2X.Core.SFSEvent.INVITATION" /> event dispatched to their clients: they can accept or refuse it
	/// by means of the InvitationReplyRequest request, which must be sent within the specified amount of time.
	/// </remarks>
	///
	/// <example>
	/// The following example sends an invitation to join the current user in his private Room; the invitation contains a custom message and the Room name and password, so that the recipient clients can join the Room if the users accept the invitation:
	/// <code>
	/// void SomeMethod() {
	/// 	// Add a listener to the invitation reply
	/// 	sfs.AddEventListener(SFSEvent.INVITATION_REPLY, OnInvitationReply);
	///
	/// 	// Choose the invitation recipients
	/// 	User friend1 = sfs.UserManager.GetUserByName("Piggy");
	/// 	User friend2 = sfs.UserManager.GetUserByName("Gonzo");
	///
	/// 	List&lt;object&gt; invitedUsers = new List&lt;object&gt;();
	/// 	invitedUsers.Add(friend1);
	/// 	invitedUsers.Add(friend2);
	///
	/// 	// Set the custom invitation details
	/// 	ISFSObject parameters = new SFSObject();
	/// 	parameters.PutUtfString("msg", "Would you like to join me in my private room?");
	/// 	parameters.PutUtfString("roomName", "Kermit's room");
	/// 	parameters.PutUtfString("roomPwd", "drowssap");
	///
	/// 	// Send the invitation; recipients have 20 seconds to reply before the invitation expires
	/// 	sfs.Send( new InviteUsersRequest(invitedUsers, 20, parameters) );
	/// }
	///
	/// void OnInvitationReply(BaseEvent evt) {	
	/// 	// If at least one recipient accepted the invitation, make me join my private Room to meet him there
	/// 	if ((InvitationReply)evt.Params["reply"] == InvitationReply.ACCEPT) {
	/// 		Room currentRoom = sfs.LastJoinedRoom;
	/// 		if (currentRoom.name != "Kermit's room")
	/// 			sfs.Send(new JoinRoomRequest("Kermit's room"));
	/// 	}
	/// 	else ((InvitationReply)evt.Params["reply"] == InvitationReply.REFUSE) {
	/// 		Console.WriteLine((User)evt.Params["invitee"] + " refused the invitation");                         // .Net / Unity
	/// 	    System.Diagnostics.Debug.WriteLine((User)evt.Params["invitee"] + " refused the invitation");        // UWP
	/// 	}
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.INVITATION" />
	/// <seealso cref="T:Sfs2X.Requests.Game.InvitationReplyRequest" />
	public class InviteUsersRequest : BaseRequest
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

		/// <summary>
		/// Creates a new InviteUsersRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="invitedUsers">A list of objects representing the users to send the invitation to.</param>
		/// <param name="secondsForReply">The number of seconds available to each invited user to reply to the invitation (recommended range: 15 to 40 seconds).</param>
		/// <param name="parameters">An instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" /> containing custom parameters which specify the invitation details.</param>
		public InviteUsersRequest(List<object> invitedUsers, int secondsForReply, ISFSObject parameters)
			: base(RequestType.InviteUser)
		{
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
			if (list.Count > 0)
			{
				throw new SFSValidationError("InviteUsers request error", list);
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
			sfso.PutIntArray(KEY_INVITED_USERS, list.ToArray());
			sfso.PutShort(KEY_TIME, (short)secondsForReply);
			if (parameters != null)
			{
				sfso.PutSFSObject(KEY_PARAMS, parameters);
			}
		}
	}
}
