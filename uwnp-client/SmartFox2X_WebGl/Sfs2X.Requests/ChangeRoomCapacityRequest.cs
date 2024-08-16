using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Changes the maximum number of users and/or spectators who can join a Room.
	/// </summary>
	///
	/// <remarks>
	/// If the operation is successful, the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_CAPACITY_CHANGE" /> event is dispatched to all the users who subscribed the Group to which the target Room belongs,
	/// including the requester user himself. If the user is not the creator (owner) of the Room, the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_CAPACITY_CHANGE_ERROR" /> event is fired.
	/// An administrator or moderator can override this constrain (he is not requested to be the Room's owner).
	/// <para />
	/// Please note that some limitations are applied to the passed values (i.e. a client can't set the max users to more than 200, or the max spectators to more than 32).<br />
	/// Alos, if the Room was configured so that resizing is not allowed (see the <see cref="P:Sfs2X.Requests.RoomSettings.Permissions">RoomSettings.Permissions</see> parameter), the request is ignored and no error is fired.
	/// <para />
	/// In case the Room's capacity is reduced to a value less than the current number of users/spectators inside the Room, exceeding users are NOT disconnected.
	/// </remarks>
	///
	/// <example>
	/// The following example changes the capacity of an existing Room:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ROOM_CAPACITY_CHANGE, OnRoomCapacityChange);
	/// 	sfs.AddEventListener(SFSEvent.ROOM_CAPACITY_CHANGE_ERROR, OnRoomCapacityChangeError);
	///
	/// 	Room theRoom = sfs.GetRoomByName("Gonzo's Room");
	///
	/// 	// Resize the Room so that it allows a maximum of 100 users and zero spectators
	/// 	sfs.Send( new ChangeRoomCapacityRequest(theRoom, 100, 0) );
	/// }
	///
	/// void OnRoomCapacityChange(BaseEvent evt) {
	/// 	Room room = (Room)evt.Params["room"];
	/// 	Console.WriteLine("The capacity of Room " + room.Name + " was changed successfully");                       // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("The capacity of Room " + room.Name + " was changed successfully");      // UWP
	/// }
	///
	/// void OnRoomCapacityChangeError(BaseEvent evt) {
	/// 	Console.WriteLine("Room capacity change failed: " + (string)evt.Params["errorMessage"]);                        // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Room capacity change failed: " + (string)evt.Params["errorMessage"]);       // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_CAPACITY_CHANGE" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_CAPACITY_CHANGE_ERROR" />
	/// <seealso cref="P:Sfs2X.Requests.RoomSettings.Permissions" />
	/// <seealso cref="T:Sfs2X.Requests.ChangeRoomNameRequest" />
	/// <seealso cref="T:Sfs2X.Requests.ChangeRoomPasswordStateRequest" />
	public class ChangeRoomCapacityRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ROOM = "r";

		/// <exclude />
		public static readonly string KEY_USER_SIZE = "u";

		/// <exclude />
		public static readonly string KEY_SPEC_SIZE = "s";

		private Room room;

		private int newMaxUsers;

		private int newMaxSpect;

		/// <summary>
		/// Creates a new ChangeRoomCapacityRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="room">The object corresponding to the Room whose capacity should be changed.</param>
		/// <param name="newMaxUsers">The new maximum number of users/players who can join the Room.</param>
		/// <param name="newMaxSpect">The new maximum number of spectators who can join the Room (for Game Rooms only).</param>
		public ChangeRoomCapacityRequest(Room room, int newMaxUsers, int newMaxSpect)
			: base(RequestType.ChangeRoomCapacity)
		{
			this.room = room;
			this.newMaxUsers = newMaxUsers;
			this.newMaxSpect = newMaxSpect;
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (room == null)
			{
				list.Add("Provided room is null");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("ChangeRoomCapacity request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			sfso.PutInt(KEY_ROOM, room.Id);
			sfso.PutInt(KEY_USER_SIZE, newMaxUsers);
			sfso.PutInt(KEY_SPEC_SIZE, newMaxSpect);
		}
	}
}
