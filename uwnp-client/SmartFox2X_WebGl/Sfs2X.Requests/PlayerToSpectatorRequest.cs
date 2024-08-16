using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
	/// <summary>
	/// Turns the current user from player to spectator in a Game Room.
	/// </summary>
	///
	/// <remarks>
	/// If the operation is successful, all the users in the target Room are notified with the <see cref="F:Sfs2X.Core.SFSEvent.PLAYER_TO_SPECTATOR" /> event.
	/// The operation could fail if no spectator slots are available in the Game Room at the time of the request;
	/// in this case the <see cref="F:Sfs2X.Core.SFSEvent.PLAYER_TO_SPECTATOR_ERROR" /> event is dispatched to the requester's client.
	/// </remarks>
	///
	/// <example>
	/// The following example turns the current user from player to spectator in the last joined Game Room:
	/// <code>
	/// void SomeMethod() {
	/// 	sfs.AddEventListener(SFSEvent.PLAYER_TO_SPECTATOR, OnPlayerToSpectatorSwitch);
	/// 	sfs.AddEventListener(SFSEvent.PLAYER_TO_SPECTATOR_ERROR, OnPlayerToSpectatorSwitchError);
	///
	/// 	// Switch player to spectator
	/// 	sfs.Send( new PlayerToSpectatorRequest() );
	/// }
	///
	/// void OnPlayerToSpectatorSwitch(BaseEvent evt) {
	/// 	User user = (User)evt.Params["user"];
	/// 	Console.WriteLine("Player " + user.Name + " is now a spectator");                           // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Player " + user.Name + " is now a spectator");          // UWP
	/// }
	///
	/// void OnPlayerToSpectatorSwitchError(BaseEvent evt) {
	/// 	Console.WriteLine("Unable to become a spectator due to the following error: " + (string)evt.Params["errorMessage"]);                        // .Net / Unity
	/// 	System.Diagnostics.Debug.WriteLine("Unable to become a spectator due to the following error: " + (string)evt.Params["errorMessage"]);       // UWP
	/// }
	/// </code>
	/// </example>
	///
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.PLAYER_TO_SPECTATOR" />
	/// <seealso cref="F:Sfs2X.Core.SFSEvent.PLAYER_TO_SPECTATOR_ERROR" />
	/// <seealso cref="T:Sfs2X.Requests.SpectatorToPlayerRequest" />
	public class PlayerToSpectatorRequest : BaseRequest
	{
		/// <exclude />
		public static readonly string KEY_ROOM_ID = "r";

		/// <exclude />
		public static readonly string KEY_USER_ID = "u";

		private Room room;

		private void Init(Room targetRoom)
		{
			room = targetRoom;
		}

		/// <summary>
		/// Creates a new PlayerToSpectatorRequest instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed to the <see cref="M:Sfs2X.SmartFox.Send(Sfs2X.Requests.IRequest)">SmartFox.Send</see> method for the request to be performed.
		/// </remarks>
		///
		/// <param name="targetRoom">The object corresponding to the Room in which the player should be turned to spectator. If <c>null</c>, the last Room joined by the user is used (default = <c>null</c>).</param>
		public PlayerToSpectatorRequest(Room targetRoom)
			: base(RequestType.PlayerToSpectator)
		{
			Init(targetRoom);
		}

		/// <summary>
		/// See <em>PlayerToSpectatorRequest(Room)</em> constructor.
		/// </summary>
		public PlayerToSpectatorRequest()
			: base(RequestType.PlayerToSpectator)
		{
			Init(null);
		}

		/// <exclude />
		public override void Validate(SmartFox sfs)
		{
			List<string> list = new List<string>();
			if (sfs.JoinedRooms.Count < 1)
			{
				list.Add("You are not joined in any rooms");
			}
			if (list.Count > 0)
			{
				throw new SFSValidationError("LeaveRoom request error", list);
			}
		}

		/// <exclude />
		public override void Execute(SmartFox sfs)
		{
			if (room == null)
			{
				room = sfs.LastJoinedRoom;
			}
			sfso.PutInt(KEY_ROOM_ID, room.Id);
		}
	}
}
