using System.Collections.Generic;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Unsubscribes the current user to Room-related events occurring in the specified Group. 
	/// </summary>
	///
	/// <remarks>
	/// This allows the user to stop being notified of specific Room events occurring in Rooms belonging to the unsubscribed Group.
	/// <para />
	/// If the operation is successful, the current user receives a <see cref="F:Sfs2X.Core.SFSEvent.ROOM_GROUP_UNSUBSCRIBE" /> event; otherwise the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_GROUP_UNSUBSCRIBE_ERROR" /> event is fired.
	/// </remarks>
	///
	/// <example>
	/// The following example makes the current user unsubscribe a Group:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ROOM_GROUP_UNSUBSCRIBE, OnUnsubscribeRoomGroup);
	/// 	sfs.AddEventListener(SFSEvent.ROOM_GROUP_UNSUBSCRIBE_ERROR, OnUnsubscribeRoomGroupError);
	///
	/// 	// Unsubscribe the "card_games" group
	/// 	sfs.Send( new UnsubscribeRoomGroupRequest("card_games") );
	/// }
	///
	/// void OnUnsubscribeRoomGroup(BaseEvent evt) {
	/// 	Console.WriteLine("Group unsubscribed: " + (string)evt.Params["groupId"]);                          // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Group unsubscribed: " + (string)evt.Params["groupId"]);         // UWP
	/// }
	///
	/// void OnUnsubscribeRoomGroupError(BaseEvent evt) {
	/// 	Console.WriteLine("Group unsubscribing failed: " + (string)evt.Params["errorMessage"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Group unsubscribing failed: " + (string)evt.Params["errorMessage"]);        // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_GROUP_UNSUBSCRIBE" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_GROUP_UNSUBSCRIBE_ERROR" />
	/// <seealso cref="T:Sfs2X.Requests.SubscribeRoomGroupRequest" />
	public class UnsubscribeRoomGroupRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_GROUP_ID = "g";

		private string groupId;

		/// <summary>
		/// Creates a new UnsubscribeRoomGroupRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="groupId">The name of the Room Group to unsubscribe.</param>
		public UnsubscribeRoomGroupRequest(string groupId)
			: base(RequestType.UnsubscribeRoomGroup)
		{
			this.groupId = groupId;
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (groupId == null || groupId.Length == 0)
			{
				list.Add("Invalid groupId. Must be a string with at least 1 character.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("UnsubscribeGroup request Error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_GROUP_ID, groupId);
		}
	}
}
