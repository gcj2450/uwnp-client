using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Invitation;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests.Game
{
	/// <summary>
	/// Replies to an invitation received by the current user.
	/// </summary>
	///
	/// <remarks>
	/// Users who receive an invitation sent by means of the <see cref="T:Sfs2X.Requests.Game.InviteUsersRequest" /> request can either accept or refuse it using this request.
	/// The reply causes an <see cref="F:Sfs2X.Core.SFSEvent.INVITATION_REPLY" /> event to be dispatched to the inviter; if a reply is not sent, or it is sent after the invitation expiration,
	/// the system will react as if the invitation was refused.
	/// <para />
	/// If an error occurs while the reply is delivered to the inviter user (for example the invitation is already expired), an <see cref="F:Sfs2X.Core.SFSEvent.INVITATION_REPLY_ERROR" /> event is returned to the current user.
	/// </remarks>
	///
	/// <example>
	/// The following example receives an invitation and accepts it automatically; in a real case scenario, the application interface usually allows the user choosing to accept or refuse the invitation, or even ignore it:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.INVITATION, OnInvitationReceived);
	/// 	sfs.AddEventListener(SFSEvent.INVITATION_REPLY_ERROR, OnInvitationReplyError);
	/// }
	///
	/// void OnInvitationReceived(BaseEvent evt) {
	/// 	// Let's accept this invitation
	/// 	sfs.Send( new InvitationReplyRequest((Invitation)evt.Params["invitation"], InvitationReply.ACCEPT) );
	/// }
	///
	/// void OnInvitationReplyError(BaseEvent evt) {
	/// 	Console.WriteLine("Failed to reply to invitation due to the following problem: " + (string)evt.Params["errorMessage"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Failed to reply to invitation due to the following problem: " + (string)evt.Params["errorMessage"]);        // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.INVITATION_REPLY" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.INVITATION_REPLY_ERROR" />
	/// <seealso cref="T:Sfs2X.Requests.Game.InviteUsersRequest" />
	public class InvitationReplyRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_INVITATION_ID = "i";

		/// <exclude />
		public static readonly string KEY_INVITATION_REPLY = "r";

		/// <exclude />
		public static readonly string KEY_INVITATION_PARAMS = "p";

		private Invitation invitation;

		private InvitationReply reply;

		private ISFSObject parameters;

		private void Init(Invitation invitation, InvitationReply reply, ISFSObject parameters)
		{
			this.invitation = invitation;
			this.reply = reply;
			this.parameters = parameters;
		}

		/// <summary>
		/// Creates a new InvitationReplyRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="invitation">An instance of the Invitation class containing the invitation details (inviter, custom parameters, etc).</param>
		/// <param name="reply">The answer to be sent to the inviter, among those available as constants in the <see cref="T:Sfs2X.Entities.Invitation.InvitationReply" /> class.</param>
		/// <param name="parameters">An instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" /> containing custom parameters to be returned to the inviter together with the reply (for example a message describing the reason of refusal). Default is <c>null</c>.</param>
		///
		/// <seealso cref="T:Sfs2X.Entities.Invitation.InvitationReply" />
		public InvitationReplyRequest(Invitation invitation, InvitationReply reply, ISFSObject parameters)
			: base(RequestType.InvitationReply)
		{
			Init(invitation, reply, parameters);
		}

		/// <summary>
		/// See <em>InvitationReplyRequest(Invitation, InvitationReply, ISFSObject)</em> constructor.
		/// </summary>
		public InvitationReplyRequest(Invitation invitation, InvitationReply reply)
			: base(RequestType.InvitationReply)
		{
			Init(invitation, reply, null);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (invitation == null)
			{
				list.Add("Missing invitation object");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("InvitationReply request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutInt(KEY_INVITATION_ID, invitation.Id);
			sfso.PutByte(KEY_INVITATION_REPLY, (byte)reply);
			if (parameters != null)
			{
				sfso.PutSFSObject(KEY_INVITATION_PARAMS, parameters);
			}
		}
	}
}
