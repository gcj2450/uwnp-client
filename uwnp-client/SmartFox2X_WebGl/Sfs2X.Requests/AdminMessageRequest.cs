using System;
using Sfs2X.Entities.Data;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Sends an administrator message to a specific user or a group of users.
	/// </summary>
	///
	/// <remarks>
	/// The current user must have administration privileges to be able to send the message (see the <see cref="P:Sfs2X.Entities.User.PrivilegeId">User.PrivilegeId</see> property).
	/// <para />
	/// The <em>recipientMode</em> parameter in the class constructor is used to determine the message recipients: a single user or all the users in a Room,
	/// a Group or the entire Zone. Upon message delivery, the clients of the recipient users dispatch the <see cref="F:Sfs2X.Core.SFSEvent.ADMIN_MESSAGE" /> event.
	/// </remarks>
	///
	/// <example>
	/// The following example sends an administration message to all the users in the Zone; it also shows how to handle the related event:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ADMIN_MESSAGE, OnAdminMessage);
	///
	/// 	// Set the message recipients: all users in the Zone
	/// 	MessageRecipientMode recipMode = new MessageRecipientMode(MessageRecipientMode.TO_ZONE, null);
	///
	/// 	// Send the administrator message
	/// 	sfs.Send( new AdminMessageRequest("Hello to everybody from the Administrator!", recipMode) );
	/// }
	///
	/// void OnAdminMessage(BaseEvent evt) {
	/// 	Console.WriteLine("The administrator sent the following message: " + (string)evt.Params["message"]);                        // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("The administrator sent the following message: " + (string)evt.Params["message"]);       // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="T:Sfs2X.Requests.MessageRecipientMode" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ADMIN_MESSAGE" />
	/// <seealso cref="T:Sfs2X.Requests.ModeratorMessageRequest" />
	public class AdminMessageRequest : GenericMessageRequest
	{
		/// <summary>
		/// Creates a new AdminMessageRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="message">The message of the administrator to be sent to the target user/s defined by the <em>recipientMode</em> parameter.</param>
		/// <param name="recipientMode">An instance of MessageRecipientMode containing the target to which the message should be delivered.</param>
		/// <param name="parameters">An instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" /> containing custom parameters to be sent to the recipient user/s (default = <c>null</c>).</param>
		public AdminMessageRequest(string message, MessageRecipientMode recipientMode, ISFSObject parameters)
		{
			if (recipientMode == null)
			{
				throw new ArgumentException("RecipientMode cannot be null!");
			}
			type = 3;
			base.message = message;
			base.parameters = parameters;
			recipient = recipientMode.Target;
			sendMode = recipientMode.Mode;
		}

		/// <summary>
		/// See <em>AdminMessageRequest(string, MessageRecipientMode, ISFSObject)</em> constructor.
		/// </summary>
		public AdminMessageRequest(string message, MessageRecipientMode recipientMode)
			: this(message, recipientMode, null)
		{
		}
	}
}
