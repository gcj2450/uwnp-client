using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Game
{
	/// <summary>
	/// Sends an invitation to other users/players to join a specific Room.
	/// </summary>
	///
	/// <remarks>
	/// Invited users receive the invitation as an <see cref="F:Sfs2X.Core.SFSEvent.INVITATION" /> event dispatched to their clients: they can accept or refuse it
	/// by means of the InvitationReplyRequest request, which must be sent within the specified amount of time.
	/// <para />
	/// Depending on the Room's settings this invitation can be sent by the Room's owner only or by any other user.
	/// This behavior can be set via the RoomSettings.AllowOwnerOnlyInvitation parameter.
	/// <para />
	/// <b>NOTE:</b> spectators in a Game Room are not allowed to invite other users; only players are.
	/// <para />
	/// An invitation can also specify the amount of time given to each invitee to reply. Default is 30 seconds.
	/// A positive answer will attempt to join the user in the designated Room. For Game Rooms the <em>asSpectator</em> flag can be toggled to join the invitee as player or spectator (default = player).
	/// <para />
	/// There aren't any specific notifications sent back to the inviter after the invitee's response. Users that have accepted the invitation will join the Room while those who didn't reply or turned down the invitation won't generate any event.
	/// In order to send specific messages (e.g. chat), just send a private message back to the inviter.
	/// </remarks>
	///
	/// <example>
	/// The following example invites two more users in the current game:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, onUserJoin);
	///
	/// 	List&lt;string&gt; invitedUsers = new List&lt;string&gt;(){"Fozzie", "Piggy"};
	///             		Room room = sfs.GetRoomByName("The Garden");
	///
	/// 	// Add message to be shown to the invited users
	/// 	ISFSObject params = SFSObject.NewInstance();
	/// 	params.PutUtfString("msg", "You are invited in this Room: " + room.Name);
	///
	/// 	// Send the request
	/// 	sfs.Send( new JoinRoomInvitationRequest(room, invitedUsers, params) );
	/// }
	///
	/// void onUserJoin(BaseEvent evt) {
	/// 	User user = (User)evt.Params["user"];
	///
	/// 	Console.WriteLine("Room joined by: " + user.Name);						// .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Room joined by: " + user.Name);		// UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="T:Sfs2X.Requests.RoomSettings" />
	public class JoinRoomInvitationRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ROOM_ID = "r";

		/// <exclude />
		public static readonly string KEY_EXPIRY_SECONDS = "es";

		/// <exclude />
		public static readonly string KEY_INVITED_NAMES = "in";

		/// <exclude />
		public static readonly string KEY_AS_SPECT = "as";

		/// <exclude />
		public static readonly string KEY_OPTIONAL_PARAMS = "op";

		private Room targetRoom;

		private List<string> invitedUserNames;

		private int expirySeconds;

		private bool asSpectator;

		private ISFSObject parameters;

		/// <summary>
		/// Creates a new JoinRoomInvitationRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="targetRoom">The Room to join (must have free user/player slots).</param>
		/// <param name="invitedUserNames">A list of user names to invite.</param>
		/// <param name="parameters">An instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" /> containing any relevant parameter or message to be sent to the invited users (for example an invitation message). Default is <c>null</c>.</param>
		/// <param name="expirySeconds">The time given to the invitee to reply to the invitation. Default is 30.</param>
		/// <param name="asSpectator">In Game Rooms only, indicates if the invited user(s) should join as spectator(s) instead of player(s). Default is <c>false</c>.</param>
		public JoinRoomInvitationRequest(Room targetRoom, List<string> invitedUserNames, ISFSObject parameters, int expirySeconds, bool asSpectator)
			: base(RequestType.JoinRoomInvite)
		{
			Init(targetRoom, invitedUserNames, parameters, expirySeconds, asSpectator);
		}

		/// <summary>
		/// See <em>JoinRoomInvitationRequest(Room, List&lt;string&gt;, ISFSObject, int, bool)</em> constructor.
		/// </summary>
		public JoinRoomInvitationRequest(Room targetRoom, List<string> invitedUserNames, ISFSObject parameters, int expirySeconds)
			: base(RequestType.JoinRoomInvite)
		{
			Init(targetRoom, invitedUserNames, parameters, expirySeconds, false);
		}

		/// <summary>
		/// See <em>JoinRoomInvitationRequest(Room, List&lt;string&gt;, ISFSObject, int, bool)</em> constructor.
		/// </summary>
		public JoinRoomInvitationRequest(Room targetRoom, List<string> invitedUserNames, ISFSObject parameters)
			: base(RequestType.JoinRoomInvite)
		{
			Init(targetRoom, invitedUserNames, parameters, 30, false);
		}

		/// <summary>
		/// See <em>JoinRoomInvitationRequest(Room, List&lt;string&gt;, ISFSObject, int, bool)</em> constructor.
		/// </summary>
		public JoinRoomInvitationRequest(Room targetRoom, List<string> invitedUserNames)
			: base(RequestType.JoinRoomInvite)
		{
			Init(targetRoom, invitedUserNames, null, 30, false);
		}

		private void Init(Room targetRoom, List<string> invitedUserNames, ISFSObject parameters, int expirySeconds, bool asSpectator)
		{
			this.targetRoom = targetRoom;
			this.invitedUserNames = invitedUserNames;
			this.expirySeconds = expirySeconds;
			this.asSpectator = asSpectator;
			ISFSObject iSFSObject2;
			if (parameters == null)
			{
				ISFSObject iSFSObject = new SFSObject();
				iSFSObject2 = iSFSObject;
			}
			else
			{
				iSFSObject2 = parameters;
			}
			this.parameters = iSFSObject2;
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (targetRoom == null)
			{
				list.Add("Missing target room");
			}
			else if (invitedUserNames == null || invitedUserNames.Count < 1)
			{
				list.Add("No invitees provided");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("JoinRoomInvitationRequest request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutInt(KEY_ROOM_ID, targetRoom.Id);
			sfso.PutUtfStringArray(KEY_INVITED_NAMES, invitedUserNames.ToArray());
			sfso.PutSFSObject(KEY_OPTIONAL_PARAMS, parameters);
			sfso.PutInt(KEY_EXPIRY_SECONDS, expirySeconds);
			sfso.PutBool(KEY_AS_SPECT, asSpectator);
		}
	}
}
