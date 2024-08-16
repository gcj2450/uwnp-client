using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Leaves one of the Rooms joined by the current user.
	/// </summary>
	///
	/// <remarks>
	/// Depending on the Room configuration defined upon its creation (see the <see cref="P:Sfs2X.Requests.RoomSettings.Events">RoomSettings.Events</see> setting), when the current user leaves it,
	/// the following events might be fired: <see cref="F:Sfs2X.Core.SFSEvent.USER_EXIT_ROOM" />, dispatched to all the users inside the Room (including the current user then)
	/// to warn them that a user has gone away; <see cref="F:Sfs2X.Core.SFSEvent.USER_COUNT_CHANGE" />, dispatched to all clients which subscribed the Group to which the Room belongs,
	/// to update the count of users inside the Room.
	/// </remarks>
	///
	/// <example>
	/// The following example makes the user leave the currently joined Room and handles the respective event:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
	///
	/// 	// Leave the last joined Room
	/// 	sfs.Send( new LeaveRoomRequest() );
	/// }
	///
	/// void OnUserExitRoom(BaseEvent evt) {
	/// 	User user = (User)evt.Params["user"];
	/// 	Room room = (Room)evt.Params["room"];
	///
	/// 	Console.WriteLine(""User " + user.Name + " just left Room " + room.Name);                       // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine(""User " + user.Name + " just left Room " + room.Name);      // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.USER_EXIT_ROOM" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.USER_COUNT_CHANGE" />
	/// <seealso cref="P:Sfs2X.Requests.RoomSettings.Events" />
	/// <seealso cref="T:Sfs2X.Requests.JoinRoomRequest" />
	public class LeaveRoomRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ROOM_ID = "r";

		private Room room;

		private void Init(Room room)
		{
			this.room = room;
		}

		/// <summary>
		/// Creates a new LeaveRoomRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="room">The Room object corresponding to the Room that the current user must leave. If <c>null</c>, the last Room joined by the user is left (default = <c>null</c>).</param>
		public LeaveRoomRequest(Room room)
			: base(RequestType.LeaveRoom)
		{
			Init(room);
		}

		/// <summary>
		/// See <em>LeaveRoomRequest(Room)</em> constructor.
		/// </summary>
		public LeaveRoomRequest()
			: base(RequestType.LeaveRoom)
		{
			Init(null);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			if (sfs.JoinedRooms.Count < 1)
			{
				throw new SFSValidationError("LeaveRoom request error", new string[1] { "You are not joined in any rooms" });
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			if (room != null)
			{
				sfso.PutInt(KEY_ROOM_ID, room.Id);
			}
		}
	}
}
