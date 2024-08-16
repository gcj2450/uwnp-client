using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Changes the password of a Room.
	/// </summary>
	///
	/// <remarks>
	/// This request not only changes the password of a Room, but also its "password state", which indicates if the Room is password protected or not.
	/// <para />
	/// If the operation is successful, the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_PASSWORD_STATE_CHANGE" /> event is dispatched to all the users who subscribed the Group
	/// to which the target Room belongs, including the requester user himself. If the user is not the creator (owner) of the Room, the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_PASSWORD_STATE_CHANGE_ERROR" /> event is fired.
	/// An administrator or moderator can override this constrain (he is not requested to be the Room's owner).<br />
	/// If the Room was configured so that password change is not allowed (see the <see cref="P:Sfs2X.Requests.RoomSettings.Permissions">RoomSettings.Permissions</see> parameter), the request is ignored and no error is fired.
	/// </remarks>
	///
	/// <example>
	/// The following example changes the password of an existing Room:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ROOM_PASSWORD_STATE_CHANGE, OnRoomPasswordStateChange);
	/// 	sfs.AddEventListener(SFSEvent.ROOM_PASSWORD_STATE_CHANGE_ERROR, OnRoomPasswordStateChangeError);
	///
	/// 	Room theRoom = sfs.GetRoomByName("Gonzo's Room");
	/// 	sfs.Send( new ChangeRoomPasswordStateRequest(theRoom, "mammamia") );
	/// }
	///
	/// void OnRoomPasswordStateChange(BaseEvent evt) {
	/// 	Room theRoom = (Room)evt.Params["room"];
	/// 	Console.WriteLine("The password of Room " + theRoom.Name + " was changed successfully");                        // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("The password of Room " + theRoom.Name + " was changed successfully");       // UWP
	/// }
	///
	/// void OnRoomPasswordStateChangeError(BaseEvent evt) {
	/// 	Console.WriteLine("Room password change failed: " + (string)evt.Params["errorMessage"]);                        // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Room password change failed: " + (string)evt.Params["errorMessage"]);       // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_PASSWORD_STATE_CHANGE" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_PASSWORD_STATE_CHANGE_ERROR" />
	/// <seealso cref="P:Sfs2X.Requests.RoomSettings.Permissions" />
	/// <seealso cref="T:Sfs2X.Requests.ChangeRoomNameRequest" />
	/// <seealso cref="T:Sfs2X.Requests.ChangeRoomPasswordStateRequest" />
	public class ChangeRoomPasswordStateRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ROOM = "r";

		/// <exclude />
		public static readonly string KEY_PASS = "p";

		private Room room;

		private string newPass;

		/// <summary>
		/// Creates a new ChangeRoomPasswordStateRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="room">The object corresponding to the Room whose password should be changed.</param>
		/// <param name="newPass">The new password to be assigned to the Room; an empty string or a <c>null</c> value can be passed to remove the Room's password.</param>
		public ChangeRoomPasswordStateRequest(Room room, string newPass)
			: base(RequestType.ChangeRoomPassword)
		{
			this.room = room;
			this.newPass = newPass;
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (room == null)
			{
				list.Add("Provided room is null");
			}
			if (newPass == null)
			{
				list.Add("Invalid new room password. It must be a non-null string.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("ChangePassState request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutInt(KEY_ROOM, room.Id);
			sfso.PutUtfString(KEY_PASS, newPass);
		}
	}
}
