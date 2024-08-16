namespace Sfs2X.Entities.Invitation
{
	/// <summary>
	/// The InvitationReply enumeration contains the costants describing the possible replies to an invitation.
	/// </summary>
	///
	/// <seealso cref="T:Sfs2X.Requests.Game.InvitationReplyRequest" />
	public enum InvitationReply
	{
		/// <summary>
		/// Invitation is accepted.
		/// </summary>
		ACCEPT = 0,
		/// <summary>
		/// Invitation is refused.
		/// </summary>
		REFUSE = 1,
		/// <summary>
		/// Invitation expired.
		/// </summary>
		EXPIRED = 255
	}
}
