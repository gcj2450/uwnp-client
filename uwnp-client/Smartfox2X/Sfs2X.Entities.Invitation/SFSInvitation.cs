using Sfs2X.Entities.Data;

namespace Sfs2X.Entities.Invitation
{
	/// <summary>
	/// The SFSInvitation object contains all the informations about an invitation received by the current user.
	/// </summary>
	///
	/// <remarks>
	/// An invitation is sent through the <see cref="T:Sfs2X.Requests.Game.InviteUsersRequest" /> request and it is received as an invitation event.
	/// Clients can reply to an invitation using the <see cref="T:Sfs2X.Requests.Game.InvitationReplyRequest" /> request.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Requests.Game.InviteUsersRequest" />
	/// <seealso cref="T:Sfs2X.Requests.Game.InvitationReplyRequest" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.INVITATION" />
	public class SFSInvitation : Invitation
	{
		/// <exclude />
		/// The id is only used when the Invitation is built from a Server Side Invitation
		protected int id;

		/// <exclude />
		protected User inviter;

		/// <exclude />
		protected User invitee;

		/// <exclude />
		protected int secondsForAnswer;

		/// <exclude />
		protected ISFSObject parameters;

		/// <inheritdoc />
		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		/// <inheritdoc />
		public User Inviter
		{
			get
			{
				return inviter;
			}
		}

		/// <inheritdoc />
		public User Invitee
		{
			get
			{
				return invitee;
			}
		}

		/// <inheritdoc />
		public int SecondsForAnswer
		{
			get
			{
				return secondsForAnswer;
			}
		}

		/// <inheritdoc />
		public ISFSObject Params
		{
			get
			{
				return parameters;
			}
		}

		private void Init(User inviter, User invitee, int secondsForAnswer, ISFSObject parameters)
		{
			this.inviter = inviter;
			this.invitee = invitee;
			this.secondsForAnswer = secondsForAnswer;
			this.parameters = parameters;
		}

		/// <exclude />
		public SFSInvitation(User inviter, User invitee)
		{
			Init(inviter, invitee, 15, null);
		}

		/// <exclude />
		public SFSInvitation(User inviter, User invitee, int secondsForAnswer)
		{
			Init(inviter, invitee, secondsForAnswer, null);
		}

		/// <exclude />
		public SFSInvitation(User inviter, User invitee, int secondsForAnswer, ISFSObject parameters)
		{
			Init(inviter, invitee, secondsForAnswer, parameters);
		}
	}
}
