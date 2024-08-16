using Sfs2X.Entities.Data;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Sends a private chat message.
	/// </summary>
	///
	/// <remarks>
	/// The private message is dispatched to a specific user, who can be in any server Room, or even in no Room at all. The message is delivered by means of the <see cref="F:Sfs2X.Core.SFSEvent.PRIVATE_MESSAGE" /> event.
	/// It is also returned to the sender: this allows showing the messages in the correct order in the application interface. It is also possible to send an optional object together with the message:
	/// it can contain custom parameters useful to transmit, for example, additional informations related to the message, like the text font or color, or other formatting details.
	/// </remarks>
	///
	/// <example>
	/// The following example sends a private message and handles the respective event:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.PRIVATE_MESSAGE, OnPrivateMessage);
	///
	/// 	// Send a private message to Jack
	/// 	User messageRecipient = sfs.UserManager.GetUserByName("Jack");
	/// 	sfs.Send( new PrivateMessageRequest("Hello Jack!", messageRecipient.Id) );
	/// }
	///
	/// void OnPrivateMessage(BaseEvent evt) {
	/// 	// As messages are forwarded to the sender too, I have to check if I am the sender
	/// 	User sender = (User)evt.Params["sender"];
	///
	/// 	if (sender != sfs.MySelf)
	/// 	{
	/// 		Console.WriteLine("User " + sender.Name + " sent me this PM: " + (string)evt.Params["message"]);                        // .Net / Unity
	/// 	    System.Diagnostics.Debug.WriteLine("User " + sender.Name + " sent me this PM: " + (string)evt.Params["message"]);       // UWP
	///     }
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.PRIVATE_MESSAGE" />
	/// <seealso cref="T:Sfs2X.Requests.PublicMessageRequest" />
	public class PrivateMessageRequest : GenericMessageRequest
	{
		/// <summary>
		/// Creates a new PrivateMessageRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="message">The message to be sent to to the recipient user.</param>
		/// <param name="recipientId">The id of the user to which the message is to be sent.</param>
		/// <param name="parameters">An instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" /> containing additional custom parameters to be sent to the message recipient (for example the color of the text, etc). Default value is <c>null</c>.</param>
		public PrivateMessageRequest(string message, int recipientId, ISFSObject parameters)
		{
			type = 1;
			base.message = message;
			recipient = recipientId;
			base.parameters = parameters;
		}

		/// <summary>
		/// See <em>PrivateMessageRequest(string, int, ISFSObject)</em> constructor.
		/// </summary>
		public PrivateMessageRequest(string message, int recipientId)
			: this(message, recipientId, null)
		{
		}
	}
}
