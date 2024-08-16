using Sfs2X.Entities;
using Sfs2X.Entities.Data;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Sends a public chat message.
	/// </summary>
	///
	/// <remarks>
	/// A public message is dispatched to all the users in the specified Room, including the message sender (this allows showing messages in the correct order in the application interface);
	/// the corresponding event is the <see cref="F:Sfs2X.Core.SFSEvent.PUBLIC_MESSAGE" /> event. It is also possible to send an optional object together with the message: it can contain
	/// custom parameters useful to transmit, for example, additional informations related to the message, like the text font or color, or other formatting details.
	/// <para />
	/// In case the target Room is not specified, the message is sent in the last Room joined by the sender.
	/// <para />
	/// <b>NOTE</b>: the <see cref="F:Sfs2X.Core.SFSEvent.PUBLIC_MESSAGE" /> event is dispatched if the Room is configured
	/// to allow public messaging only (see the <see cref="P:Sfs2X.Requests.RoomSettings.Permissions">RoomSettings.Permissions</see> parameter).
	/// </remarks>
	///
	/// <example>
	/// The following example sends a public message and handles the respective event:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
	///
	/// 	// Send a public message
	/// 	sfs.Send( new PublicMessageRequest("Hello everyone!") );
	/// }
	///
	/// void OnPublicMessage(BaseEvent evt) {
	/// 	// As messages are forwarded to the sender too, I have to check if I am the sender
	/// 	User sender = (User)evt.Params["sender"];
	///
	/// 	if (sender == sfs.MySelf)
	/// 	{
	/// 		Console.WriteLine("I said " + (string)evt.Params["message"]);                           // .Net / Unity
	/// 	    System.Diagnostics.Debug.WriteLine("I said " + (string)evt.Params["message"]);          // UWP
	///     }
	/// 	else
	/// 	{
	/// 		Console.WriteLine("User " + sender.Name + " said: " + (string)evt.Params["message"]);                           // .Net / Unity
	/// 	    System.Diagnostics.Debug.WriteLine("User " + sender.Name + " said: " + (string)evt.Params["message"]);          // UWP
	/// 	}
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.PUBLIC_MESSAGE" />
	/// <seealso cref="T:Sfs2X.Requests.PrivateMessageRequest" />
	public class PublicMessageRequest : GenericMessageRequest
	{
		/// <summary>
		/// Creates a new PublicMessageRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="message">The message to be sent to all the users in the target Room.</param>
		/// <param name="parameters">An instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" /> containing additional custom parameters to be sent to the message recipients (for example the color of the text, etc). Default value is <c>null</c>.</param>
		/// <param name="targetRoom">The object corresponding to the Room where the message should be dispatched; if <c>null</c>, the last Room joined by the user is used (default = <c>null</c>).</param>
		public PublicMessageRequest(string message, ISFSObject parameters, Room targetRoom)
		{
			type = 0;
			base.message = message;
			room = targetRoom;
			base.parameters = parameters;
		}

		/// <summary>
		/// See <em>PublicMessageRequest(string, ISFSObject, Room)</em> constructor.
		/// </summary>
		public PublicMessageRequest(string message, ISFSObject parameters)
			: this(message, parameters, null)
		{
		}

		/// <summary>
		/// See <em>PublicMessageRequest(string, ISFSObject, Room)</em> constructor.
		/// </summary>
		public PublicMessageRequest(string message)
			: this(message, null, null)
		{
		}
	}
}
