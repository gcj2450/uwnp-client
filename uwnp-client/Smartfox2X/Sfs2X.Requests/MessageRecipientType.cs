namespace Sfs2X.Requests
{
	/// <summary>
	/// The possible message recipient modes for ModeratorMessageRequest and AdminMessageRequest requests.
	/// </summary>
	public enum MessageRecipientType
	{
		/// <summary>
		/// The moderator/administrator message will be sent to a specific user.
		/// </summary>
		///
		/// <remarks>
		/// A User instance must be passed as <em>target</em> parameter to the <see cref="T:Sfs2X.Requests.MessageRecipientMode" /> class constructor.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Entities.User" />
		TO_USER,
		/// <summary>
		/// The moderator/administrator message will be sent to all the users in a specific Room.
		/// </summary>
		///
		/// <remarks>
		/// A Room instance must be passed as <em>target</em> parameter to the <see cref="T:Sfs2X.Requests.MessageRecipientMode" /> class constructor.
		/// </remarks>
		///
		/// <seealso cref="T:Sfs2X.Entities.Room" />
		TO_ROOM,
		/// <summary>
		/// The moderator/administrator message will be sent to all the clients who subscribed a specific Room Group.
		/// </summary>
		///
		/// <remarks>
		/// A Group id must be passed as <em>target</em> parameter to the <see cref="T:Sfs2X.Requests.MessageRecipientMode" /> class constructor.
		/// </remarks>
		///
		/// <seealso cref="P:Sfs2X.Entities.Room.GroupId" />
		TO_GROUP,
		/// <summary>
		/// The moderator/administrator message will be sent to all the users in the Zone.
		/// </summary>
		///
		/// <remarks>
		/// <c>null</c> can be passed as <em>target</em> parameter to the <see cref="T:Sfs2X.Requests.MessageRecipientMode" /> class, in fact it will be ignored.
		/// </remarks>
		TO_ZONE
	}
}
