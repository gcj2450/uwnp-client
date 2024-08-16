using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Changes the name of a Room.
	/// </summary>
	///
	/// <remarks>
	/// If the renaming operation is successful, the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_NAME_CHANGE" /> event is dispatched to all the users who subscribed the Group to which the target Room belongs,
	/// including the user who renamed it. If the user is not the creator (owner) of the Room, or if the new name doesn't match the related criteria in Zone configuration, the
	/// <see cref="F:Sfs2X.Core.SFSEvent.ROOM_NAME_CHANGE_ERROR" /> event is fired. An administrator or moderator can override this constrain (he is not requested to be the Room's owner).<br />
	/// If the Room was configured so that renaming is not allowed (see the <see cref="P:Sfs2X.Requests.RoomSettings.Permissions">RoomSettings.Permissions</see> parameter), the request is ignored and no error is fired.
	/// </remarks>
	///
	/// <example>
	/// The following example renames an existing Room:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ROOM_NAME_CHANGE, OnRoomNameChange);
	/// 	sfs.AddEventListener(SFSEvent.ROOM_NAME_CHANGE_ERROR, OnRoomNameChangeError);
	///
	/// 	Room theRoom = sfs.GetRoomByName("Gonzo's Room");
	/// 	sfs.Send( new ChangeRoomNameRequest(theRoom, "Gonzo The Great's Room") );
	/// }
	///
	/// void OnRoomNameChange(BaseEvent evt) {
	/// 	Room theRoom = (Room)evt.Params["room"];
	/// 	Console.WriteLine("Room " + (string)evt.Params["oldName"] + " was successfully renamed to " + theRoom.Name);                        // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Room " + (string)evt.Params["oldName"] + " was successfully renamed to " + theRoom.Name);       // UWP
	/// }
	///
	/// void OnRoomNameChangeError(BaseEvent evt) {
	/// 	Console.WriteLine("Room name change failed: " + (string)evt.Params["errorMessage"]);                        // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Room name change failed: " + (string)evt.Params["errorMessage"]);       // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_NAME_CHANGE" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_NAME_CHANGE_ERROR" />
	/// <seealso cref="P:Sfs2X.Requests.RoomSettings.Permissions" />
	/// <seealso cref="T:Sfs2X.Requests.ChangeRoomCapacityRequest" />
	/// <seealso cref="T:Sfs2X.Requests.ChangeRoomPasswordStateRequest" />
	public class ChangeRoomNameRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ROOM = "r";

		/// <exclude />
		public static readonly string KEY_NAME = "n";

		private Room room;

		private string newName;

		/// <summary>
		/// Creates a new ChangeRoomNameRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="room">The object corresponding to the Room whose name should be changed.</param>
		/// <param name="newName">The new name to be assigned to the Room.</param>
		public ChangeRoomNameRequest(Room room, string newName)
			: base(RequestType.ChangeRoomName)
		{
			this.room = room;
			this.newName = newName;
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (room == null)
			{
				list.Add("Provided room is null");
			}
			if (newName == null || newName.Length == 0)
			{
				list.Add("Invalid new room name. It must be a non-null and non-empty string.");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("ChangeRoomName request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutInt(KEY_ROOM, room.Id);
			sfso.PutUtfString(KEY_NAME, newName);
		}
	}
}
