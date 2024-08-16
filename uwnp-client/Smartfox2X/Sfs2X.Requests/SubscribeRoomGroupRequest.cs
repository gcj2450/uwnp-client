using System.Collections.Generic;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Subscribes the current user to Room-related events occurring in the specified Group.
	/// </summary>
	///
	/// <remarks>
	/// This allows the user to be notified of specific Room events even if he didn't join the Room from which the events originated, provided the Room belongs to the subscribed Group.
	/// <para />
	/// If the subscription operation is successful, the current user receives a <see cref="F:Sfs2X.Core.SFSEvent.ROOM_GROUP_SUBSCRIBE" /> event; otherwise the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_GROUP_SUBSCRIBE_ERROR" /> event is fired.
	/// </remarks>
	///
	/// <example>
	/// The following example makes the current user subscribe a Group:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE, OnSubscribeRoomGroup);
	/// 	sfs.AddEventListener(SFSEvent.ROOM_GROUP_SUBSCRIBE_ERROR, OnSubscribeRoomGroupError);
	///
	/// 	// Subscribe the "card_games" group
	/// 	sfs.Send( new SubscribeRoomGroupRequest("card_games") );
	/// }
	///
	/// void OnSubscribeRoomGroup(BaseEvent evt) {
	/// 	Console.WriteLine("Group subscribed. The following rooms are now accessible: " + (List&lt;Room&gt;)evt.Params["newRooms"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Group subscribed. The following rooms are now accessible: " + (List&lt;Room&gt;)evt.Params["newRooms"]);        // UWP
	/// }
	///
	/// void OnSubscribeRoomGroupError(BaseEvent evt) {
	/// 	Console.WriteLine("Group subscription failed: " + (string)evt.Params["errorMessage"]);                          // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Group subscription failed: " + (string)evt.Params["errorMessage"]);         // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_GROUP_SUBSCRIBE" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_GROUP_SUBSCRIBE_ERROR" />
	/// <seealso cref="T:Sfs2X.Requests.UnsubscribeRoomGroupRequest" />
	public class SubscribeRoomGroupRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_GROUP_ID = "g";

		/// <exclude />
		public static readonly string KEY_ROOM_LIST = "rl";

		private string groupId;

		/// <summary>
		/// Creates a new SubscribeRoomGroupRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="groupId">The name of the Room Group to subscribe.</param>
		public SubscribeRoomGroupRequest(string groupId)
			: base(RequestType.SubscribeRoomGroup)
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
				throw new SFSValidationError("SubscribeGroup request Error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutUtfString(KEY_GROUP_ID, groupId);
		}
	}
}
