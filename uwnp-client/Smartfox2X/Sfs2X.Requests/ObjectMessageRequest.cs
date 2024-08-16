using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Sends an object containing custom data to all users in a Room, or a subset of them.
	/// </summary>
	///
	/// <remarks>
	/// The data object is delivered to the selected users (or all users excluding the sender) inside the target Room by means of the <see cref="F:Sfs2X.Core.SFSEvent.OBJECT_MESSAGE" /> event.
	/// It can be useful to send game data, like for example the target coordinates of the user's avatar in a virtual world.
	/// </remarks>
	///
	/// <example>
	/// The following example sends the player's character movement coordinates and handles the respective event:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.OBJECT_MESSAGE, OnObjectMessage);
	///
	/// 	// Send a game move to all players
	/// 	ISFSObject dataObj = new SFSObject();
	/// 	dataObj.PutInt("x", myAvatar.x);
	/// 	dataObj.PutInt("y", myAvatar.y);
	/// 	sfs.Send( new ObjectMessageRequest(dataObj) );
	/// }
	///
	/// void OnObjectMessage(BaseEvent evt) {
	/// 	ISFSObject dataObj = (SFSObject)evt.Params["message"];
	/// 	int x = dataObj.GetInt("x");
	/// 	int y = dataObj.GetInt("y");
	///
	/// 	// etc...
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.OBJECT_MESSAGE" />
	public class ObjectMessageRequest : GenericMessageRequest
	{
		/// <summary>
		/// Creates a new ObjectMessageRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="obj">An instance of <see cref="T:Sfs2X.Entities.Data.SFSObject" /> containing custom parameters to be sent to the message recipients.</param>
		/// <param name="targetRoom">The Room object corresponding to the Room where the message should be dispatched; if null, the last Room joined by the user is used (default = <c>null</c>).</param>
		/// <param name="recipients">A collection of User objects corresponding to the message recipients (default = <c>null</c>); if <c>null</c>, the message is sent to all users in the target Room (except the sender himself).</param>
		public ObjectMessageRequest(ISFSObject obj, Room targetRoom, ICollection<User> recipients)
		{
			type = 4;
			parameters = obj;
			room = targetRoom;
			recipient = recipients;
		}

		/// <summary>
		/// See <em>ObjectMessageRequest(ISFSObject, Room, ICollection&lt;User&gt;)</em> constructor.
		/// </summary>
		public ObjectMessageRequest(ISFSObject obj, Room targetRoom)
			: this(obj, targetRoom, null)
		{
		}

		/// <summary>
		/// See <em>ObjectMessageRequest(ISFSObject, Room, ICollection&lt;User&gt;)</em> constructor.
		/// </summary>
		public ObjectMessageRequest(ISFSObject obj)
			: this(obj, null, null)
		{
		}
	}
}
