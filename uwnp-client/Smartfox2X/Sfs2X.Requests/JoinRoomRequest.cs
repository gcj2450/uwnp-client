using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Joins the current user in a Room.
	/// </summary>
	///
	/// <remarks>
	/// If the operation is successful, the current user receives a <see cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN" /> event; otherwise the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN_ERROR" /> event is fired.
	/// This usually happens when the Room is full, or the password is wrong in case of password protected Rooms.
	/// <para />
	/// Depending on the Room configuration defined upon its creation (see the <see cref="P:Sfs2X.Requests.RoomSettings.Events">RoomSettings.Events</see> setting), when the current user joins it,
	/// the following events might be fired: <see cref="F:Sfs2X.Core.SFSEvent.USER_ENTER_ROOM" />, dispatched to the other users inside the Room to warn them that a new user has arrived;
	/// <see cref="F:Sfs2X.Core.SFSEvent.USER_COUNT_CHANGE" />, dispatched to all clients which subscribed the Group to which the Room belongs, to update the count of users inside the Room.
	/// </remarks>
	///
	/// <example>
	/// The following example makes the user join an existing Room:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
	/// 	sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnJoinRoomError);
	///
	/// 	// Join a Room called "Lobby"
	/// 	sfs.Send( new JoinRoomRequest("Lobby") );
	/// }
	///
	/// void OnJoinRoom(BaseEvent evt) {
	/// 	Console.WriteLine("Room joined successfully: " + (Room)evt.Params["room"]);                         // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Room joined successfully: " + (Room)evt.Params["room"]);        // UWP
	/// }
	///
	/// void OnJoinRoomError(BaseEvent evt) {
	/// 	Console.WriteLine("Room joining failed: " + (string)evt.Params["errorMessage"]);                        // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Room joining failed: " + (string)evt.Params["errorMessage"]);       // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_JOIN_ERROR" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.USER_ENTER_ROOM" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.USER_COUNT_CHANGE" />
	/// <seealso cref="P:Sfs2X.Requests.RoomSettings.Events" />
	/// <seealso cref="T:Sfs2X.Requests.LeaveRoomRequest" />
	public class JoinRoomRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ROOM = "r";

		/// <exclude />
		public static readonly string KEY_USER_LIST = "ul";

		/// <exclude />
		public static readonly string KEY_ROOM_NAME = "n";

		/// <exclude />
		public static readonly string KEY_ROOM_ID = "i";

		/// <exclude />
		public static readonly string KEY_PASS = "p";

		/// <exclude />
		public static readonly string KEY_ROOM_TO_LEAVE = "rl";

		/// <exclude />
		public static readonly string KEY_AS_SPECTATOR = "sp";

		private int id = -1;

		private string name;

		private string pass;

		private int? roomIdToLeave;

		private bool asSpectator;

		private void Init(object id, string pass, int? roomIdToLeave, bool asSpectator)
		{
			if (id is string)
			{
				name = id as string;
			}
			else if (id is int)
			{
				this.id = (int)id;
			}
			else if (id is Room)
			{
				this.id = (id as Room).Id;
			}
			this.pass = pass;
			this.roomIdToLeave = roomIdToLeave;
			this.asSpectator = asSpectator;
		}

		/// <summary>
		/// Creates a new JoinRoomRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="id">The id or the name of the Room to be joined.</param>
		/// <param name="pass">The password of the Room, in case it is password protected (default = <c>null</c>).</param>
		/// <param name="roomIdToLeave">The id of a previously joined Room that the user should leave when joining the new Room. By default, the last joined Room is left; if a negative number is passed, no previous Room is left (default = <c>null</c>).</param>
		/// <param name="asSpectator"><c>true</c> to join the Room as a spectator (in Game Rooms only) (default = <c>false</c>).</param>
		public JoinRoomRequest(object id, string pass, int? roomIdToLeave, bool asSpectator)
			: base(RequestType.JoinRoom)
		{
			Init(id, pass, roomIdToLeave, asSpectator);
		}

		/// <summary>
		/// See <em>JoinRoomRequest(object, string, int?, bool)</em> constructor.
		/// </summary>
		public JoinRoomRequest(object id, string pass, int? roomIdToLeave)
			: base(RequestType.JoinRoom)
		{
			Init(id, pass, roomIdToLeave, false);
		}

		/// <summary>
		/// See <em>JoinRoomRequest(object, string, int?, bool)</em> constructor.
		/// </summary>
		public JoinRoomRequest(object id, string pass)
			: base(RequestType.JoinRoom)
		{
			Init(id, pass, null, false);
		}

		/// <summary>
		/// See <em>JoinRoomRequest(object, string, int?, bool)</em> constructor.
		/// </summary>
		public JoinRoomRequest(object id)
			: base(RequestType.JoinRoom)
		{
			Init(id, null, null, false);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			if (id < 0 && name == null)
			{
				throw new SFSValidationError("JoinRoomRequest Error", new string[1] { "Missing Room id or name, you should provide at least one" });
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			if (id > -1)
			{
				sfso.PutInt(KEY_ROOM_ID, id);
			}
			else if (name != null)
			{
				sfso.PutUtfString(KEY_ROOM_NAME, name);
			}
			if (pass != null)
			{
				sfso.PutUtfString(KEY_PASS, pass);
			}
			if (roomIdToLeave.HasValue)
			{
				sfso.PutInt(KEY_ROOM_TO_LEAVE, roomIdToLeave.Value);
			}
			if (asSpectator)
			{
				sfso.PutBool(KEY_AS_SPECTATOR, asSpectator);
			}
		}
	}
}
