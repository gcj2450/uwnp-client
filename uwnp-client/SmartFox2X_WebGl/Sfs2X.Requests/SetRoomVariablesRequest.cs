using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Sets one or more custom Room Variables in a Room.
	/// </summary>
	///
	/// <remarks>
	/// When a Room Variable is set, the <see cref="F:Sfs2X.Core.SFSEvent.ROOM_VARIABLES_UPDATE" /> event is dispatched to all the users in the target Room, including the user who updated it.
	/// Also, if the Room Variable is global (see the <see cref="T:Sfs2X.Entities.Variables.SFSRoomVariable" /> class description), the event is dispatched to all users who subscribed the Group to which the target Room is associated.
	/// </remarks>
	///
	/// <example>
	/// The following example sets a number of Room Variables and handles the respective update event:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.ROOM_VARIABLES_UPDATE, OnRoomVarsUpdate);
	///
	/// 	// Create some Room Variables
	/// 	List&lt;RoomVariable&gt; roomVars = new List&lt;RoomVariable&gt;();
	/// 	roomVars.Add( new SFSRoomVariable("gameStarted", false) );
	/// 	roomVars.Add( new SFSRoomVariable("gameType", "Snooker") );
	/// 	roomVars.Add( new SFSRoomVariable("minRank", 10) );
	///
	/// 	sfs.Send( new SetRoomVariablesRequest(roomVars) );
	/// }
	///
	/// void OnRoomVarsUpdate(BaseEvent evt) {
	/// 	List&lt;String&gt; changedVars = (List&lt;String&gt;)evt.Params["changedVars"];
	/// 	Room room = (Room)evt.Params["room"];
	///
	/// 	// Check if the gameStarted variable was changed
	/// 	if (changedVars.Contains ("gameStarted")) {
	/// 		if (room.GetVariable("gameStarted").GetBoolValue()) {
	/// 			Console.WriteLine("Game started");                          // .Net / Unity
	/// 	        System.Diagnostics.Debug.WriteLine("Game started");         // UWP
	/// 		} else {
	/// 			Console.WriteLine("Game stopped");                          // .Net / Unity
	/// 	        System.Diagnostics.Debug.WriteLine("Game stopped");         // UWP
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.ROOM_VARIABLES_UPDATE" />
	/// <seealso cref="T:Sfs2X.Entities.Variables.SFSRoomVariable" />
	public class SetRoomVariablesRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_VAR_ROOM = "r";

		/// <exclude />
		public static readonly string KEY_VAR_LIST = "vl";

		private ICollection<RoomVariable> roomVariables;

		private Room room;

		private void Init(ICollection<RoomVariable> roomVariables, Room room)
		{
			this.roomVariables = roomVariables;
			this.room = room;
		}

		/// <summary>
		/// Creates a new SetRoomVariablesRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="roomVariables">A collection of objects representing the Room Variables to be set.</param>
		/// <param name="room">An object representing the Room where to set the Room Variables; if <c>null</c>, the last Room joined by the current user is used (default = <c>null</c>).</param>
		public SetRoomVariablesRequest(ICollection<RoomVariable> roomVariables, Room room)
			: base(RequestType.SetRoomVariables)
		{
			Init(roomVariables, room);
		}

		/// <summary>
		/// See <em>SetRoomVariablesRequest(ICollection&lt;RoomVariable&gt;, Room)</em> constructor.
		/// </summary>
		public SetRoomVariablesRequest(ICollection<RoomVariable> roomVariables)
			: base(RequestType.SetRoomVariables)
		{
			Init(roomVariables, null);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (room == null && sfs.LastJoinedRoom == null)
			{
				list.Add("A Room was not specified and you don't seem joined in any other Room");
			}
			if (roomVariables == null || roomVariables.Count == 0)
			{
				list.Add("No variables were specified");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("SetRoomVariables request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			ISFSArray iSFSArray = SFSArray.NewInstance();
			foreach (RoomVariable roomVariable in roomVariables)
			{
				iSFSArray.AddSFSArray(roomVariable.ToSFSArray());
			}
			if (room == null)
			{
				room = sfs.LastJoinedRoom;
			}
			sfso.PutSFSArray(KEY_VAR_LIST, iSFSArray);
			sfso.PutInt(KEY_VAR_ROOM, room.Id);
		}
	}
}
