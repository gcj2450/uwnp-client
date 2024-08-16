using Sfs2X.Entities;
using Sfs2X.Entities.Data;

namespace Sfs2X.Requests.Buddylist
{
	/// <summary>
	/// Sends a message to a buddy in the current user's buddies list.
	/// </summary>
	///
	/// <remarks>
	/// Messages sent to buddies using the <see cref="T:Sfs2X.Requests.Buddylist.BuddyMessageRequest" /> request are similar to the standard private messages (see the <see cref="T:Sfs2X.Requests.PrivateMessageRequest" /> request)
	/// but are specifically designed for the Buddy List system: they don't require any Room parameter, nor they require that users joined a Room.
	/// Additionally, buddy messages are subject to specific validation, such as making sure that the recipient is in the sender's buddies list and the sender is not blocked by the recipient.
	/// <para />
	/// If the operation is successful, a <see cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_MESSAGE" /> event is dispatched in both the sender and recipient clients.
	/// <para />
	/// This request can be sent if the Buddy List system was previously initialized only (see the <see cref="T:Sfs2X.Requests.Buddylist.InitBuddyListRequest" /> request description).
	/// </remarks>
	///
	/// <example>
	/// The following example sends a message to a buddy and handles the related event:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSBuddyEvent.BUDDY_MESSAGE, OnBuddyMessage);
	///
	/// 	// Obtain the recipient of the message
	/// 	Buddy kermitTheFrog = sfs.BuddyListManager.GetBuddyByName("KermitTheFrog");
	///
	/// 	// Block a buddy in the current buddy list
	/// 	sfs.Send(new BuddyMessageRequest("Hello Kermit!", kermitTheFrog));
	/// }
	///
	/// void OnBuddyMessage(BaseEvent evt) {
	/// 	bool isItMe = (bool)evt.Params["isItMe"];
	/// 	Buddy buddy = (Buddy)evt.Params["buddy"];
	///
	/// 	if (isItMe)
	/// 	{
	/// 		Console.WriteLine("I said: " + (string)evt.Params["message"]);                      // .Net / Unity
	/// 	    System.Diagnostics.Debug.WriteLine("I said: " + (string)evt.Params["message"]);     // UWP
	/// 	}
	/// 	else
	/// 	{
	/// 		Console.WriteLine(buddy.Name + " said: " + (string)evt.Params["message"]);                      // .Net / Unity
	/// 	    System.Diagnostics.Debug.WriteLine(buddy.Name + " said: " + (string)evt.Params["message"]);     // UWP
	/// 	}
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSBuddyEvent.BUDDY_MESSAGE" />
	public class BuddyMessageRequest : GenericMessageRequest
	{
		/// <summary>
		/// Creates a new BuddyMessageRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="message">The message to be sent to a buddy.</param>
		/// <param name="targetBuddy">The Buddy object corresponding to the message recipient.</param>
		/// <param name="parameters">An instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" /> containing additional custom parameters (e.g. the message color, an emoticon id, etc). Default is <c>null</c>.</param>
		public BuddyMessageRequest(string message, Buddy targetBuddy, ISFSObject parameters)
		{
			type = 5;
			base.message = message;
			recipient = ((targetBuddy != null) ? targetBuddy.Id : (-1));
			base.parameters = parameters;
		}

		/// <summary>
		/// See <em>BuddyMessageRequest(String, Buddy, ISFSObject)</em> constructor.
		/// </summary>
		public BuddyMessageRequest(string message, Buddy targetBuddy)
			: this(message, targetBuddy, null)
		{
		}
	}
}
