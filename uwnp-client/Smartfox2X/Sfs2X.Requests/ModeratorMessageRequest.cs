using System;
using Sfs2X.Entities.Data;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Sends a moderator message to a specific user or a group of users.
	/// </summary>
	///
	/// <remarks>
	/// The current user must have moderation privileges to be able to send the message (see the <see cref="P:Sfs2X.Entities.User.PrivilegeId">User.PrivilegeId</see> property).
	/// <para />
	/// The <em>recipientMode</em> parameter in the class constructor is used to determine the message recipients: a single user or all the users in a Room,
	/// a Group or the entire Zone. Upon message delivery, the clients of the recipient users dispatch the <see cref="F:Sfs2X.Core.SFSEvent.MODERATOR_MESSAGE" /> event.
	/// </remarks>
	///
	/// <example>
	/// The following example sends a moderator message to all the users in the last joned Room; it also shows how to handle the related event:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.MODERATOR_MESSAGE, OnModeratorMessage);
	///
	/// 	// Set the message recipients: all users in the current Room
	/// 	MessageRecipientMode recipMode = new MessageRecipientMode(MessageRecipientMode.TO_ROOM, sfs.LastJoinedRoom);
	///
	/// 	// Send the moderator message
	/// 	sfs.Send( new ModeratorMessageRequest("Hello everybody, I'm the Moderator!", recipMode) );
	/// }
	///
	/// void OnModeratorMessage(BaseEvent evt) {
	/// 	Console.WriteLine("The moderator sent the following message: " + (string)evt.Params["message"]);                        // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("The moderator sent the following message: " + (string)evt.Params["message"]);       // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="T:Sfs2X.Requests.MessageRecipientMode" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.MODERATOR_MESSAGE" />
	/// <seealso cref="T:Sfs2X.Requests.AdminMessageRequest" />
	public class ModeratorMessageRequest : GenericMessageRequest
	{
		/// <summary>
		/// Creates a new ModeratorMessageRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="message">The message of the moderator to be sent to the target user/s defined by the <em>recipientMode</em> parameter.</param>
		/// <param name="recipientMode">An instance of MessageRecipientMode containing the target to which the message should be delivered.</param>
		/// <param name="parameters">An instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" /> containing custom parameters to be sent to the recipient user/s (default = <c>null</c>).</param>
		public ModeratorMessageRequest(string message, MessageRecipientMode recipientMode, ISFSObject parameters)
		{
			if (recipientMode == null)
			{
				throw new ArgumentException("RecipientMode cannot be null!");
			}
			type = 2;
			base.message = message;
			base.parameters = parameters;
			recipient = recipientMode.Target;
			sendMode = recipientMode.Mode;
		}

		/// <summary>
		/// See <em>ModeratorMessageRequest(string, MessageRecipientMode, ISFSObject)</em> constructor.
		/// </summary>
		public ModeratorMessageRequest(string message, MessageRecipientMode recipientMode)
			: this(message, recipientMode, null)
		{
		}
	}
}
